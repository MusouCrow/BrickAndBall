
package.cpath=package.cpath..";./lualib/?.so"
package.path=package.path..";./src/?.lua"


local LatencySM = require "latencysm"
local LKcp = require "lkcp"
local LUtil = require "lutil"

--创建模拟网络：丢包率10%，RTT 60-125ms
local lsm = LatencySM.LatencySM.new(10, 60, 125)


local function getms()
    return math.floor(LUtil.gettimeofday())
end

local function udp_output(buf, info)
    print(info.id, info.a, info.b, info.c)
    if info.b then
        info.c(info.a)
    end
    lsm:send(info.id, buf)
end

local function test(mode)
    --此处info用于output接口回调数据
    local session = 0x11223344
    local info = {
        id = 0,
        a = 'aaa',
        b = false,
    }
    local kcp1 = LKcp.lkcp_create(session, function (buf)
        udp_output(buf, info)
    end)
    local info2 = {
        id = 1,
        a = 'aaaaaaaaaaaaa',
        b = true,
        c = function (a)
            print 'hahahah!!!'
        end,
    }
    local kcp2 = LKcp.lkcp_create(session, function (buf)
        udp_output(buf, info2)
    end)

    local current = getms()
    local slap = current + 20
    local index = 0
    local inext = 0
    local sumrtt = 0

    local count = 0
    local maxrtt = 0

	--配置窗口大小：平均延迟200ms，每20ms发送一个包，
	--而考虑到丢包重发，设置最大收发窗口为128
	kcp1:lkcp_wndsize(128, 128)
	kcp2:lkcp_wndsize(128, 128)

	--判断测试用例的模式
    if mode == 0 then
		--默认模式
        kcp1:lkcp_nodelay(0, 10, 0, 0)
        kcp2:lkcp_nodelay(0, 10, 0, 0)
    elseif mode == 1 then
		--普通模式，关闭流控等
        kcp1:lkcp_nodelay(0, 10, 0, 1)
        kcp2:lkcp_nodelay(0, 10, 0, 1)
    else
		--启动快速模式
		--第二个参数 nodelay-启用以后若干常规加速将启动
		--第三个参数 interval为内部处理时钟，默认设置为 10ms
		--第四个参数 resend为快速重传指标，设置为2
		--第五个参数 为是否禁用常规流控，这里禁止
        kcp1:lkcp_nodelay(1, 10, 2, 1)
        kcp2:lkcp_nodelay(1, 10, 2, 1)
    end

    local buffer = ""
    local hrlen = 0
    local hr = ""
    local ts1 = getms()

    while 1 do
        current = getms()

        local nextt1 = kcp1:lkcp_check(current) 
        local nextt2 = kcp2:lkcp_check(current)
        local nextt = math.min(nextt1, nextt2)
        local diff = nextt - current
        if diff > 0 then
            LUtil.isleep(diff)
            current = getms()
        end
        
        kcp1:lkcp_update(current)
        kcp2:lkcp_update(current)
        
		--每隔 20ms，kcp1发送数据
        while current >= slap do
            local s1 = LUtil.uint322netbytes(index)
            local s2 = LUtil.uint322netbytes(current)
            kcp1:lkcp_send(s1..s2)
            --kcp1:lkcp_flush()
            slap = slap + 20
            index = index + 1
        end

		--处理虚拟网络：检测是否有udp包从p1->p2
		while 1 do
			hrlen, hr = lsm:recv(1)
			if hrlen < 0 then
                break
            end
			--如果 p2收到udp，则作为下层协议输入到kcp2
			kcp2:lkcp_input(hr)
        end

		--处理虚拟网络：检测是否有udp包从p2->p1
		while 1 do
			hrlen, hr = lsm:recv(0)
			if hrlen < 0 then
                break
            end
			--如果 p1收到udp，则作为下层协议输入到kcp1
			kcp1:lkcp_input(hr)
        end

        --kcp2接收到任何包都返回回去
        while 1 do
            hrlen, hr = kcp2:lkcp_recv()
            if hrlen <= 0 then
                break
            end
            kcp2:lkcp_send(hr)
            --kcp2:lkcp_flush()
        end

		--kcp1收到kcp2的回射数据
		while 1 do
		    hrlen, hr = kcp1:lkcp_recv()
			--没有收到包就退出
			if hrlen <= 0 then
                break
            end

            local hr1 = string.sub(hr, 1, 4)
            local hr2 = string.sub(hr, 5, 8)
            local sn = LUtil.netbytes2uint32(hr1)
            local ts = LUtil.netbytes2uint32(hr2)
            local rtt = current - ts
			
			if sn ~= inext then
				--如果收到的包不连续
				print(string.format("ERROR sn %d<->%d\n", count, inext))
				return
            end

			inext = inext + 1
			sumrtt = sumrtt + rtt
			count = count + 1
			if rtt > maxrtt then
                maxrtt = rtt
            end

			print(string.format("[RECV] mode=%d sn=%d rtt=%d\n", mode, sn, rtt))
        end
		if inext > 10 then
            break
        end
    end

    ts1 = getms() - ts1
    names = {"default", "normal", "fast"}
    print(string.format("%s mode result (%dms):", names[mode+1], ts1))
    print(string.format("avgrtt=%d maxrtt=%d", math.floor(sumrtt/count), maxrtt))
    print("press enter to next ...")
    io.read()
end

--测试
test(0) --默认模式，类似 TCP：正常模式，无快速重传，常规流控
test(1) --普通模式，关闭流控等
test(2) --快速模式，所有开关都打开，且关闭流控

