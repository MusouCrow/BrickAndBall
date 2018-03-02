local _KCP = require("lkcp")
local _JSON = require("cjson")

local _Agent = require("src.class")()

function _Agent:Ctor(conv, fd, SendWrap)
    self._kcp = _KCP.lkcp_create(conv, SendWrap)
    self._kcp:lkcp_nodelay(1, 10, 2, 1)
    self._kcp:lkcp_wndsize(128, 128)

    self._fd = fd
    self.heartbeat = true
end

function _Agent:Update(clock)
    self._kcp:lkcp_update(clock)
end

function _Agent:Input(data)
    self._kcp:lkcp_input(data)
    self.heartbeat = true
end

function _Agent:Send(id, obj)
    local buffer = string.pack("b", id)

    if (obj) then
        local data

        if (type(obj) == "table") then
            data = _JSON.encode(obj)
        else
            data = obj
        end

        buffer = buffer .. data
    end

    self._kcp:lkcp_send(buffer)
end

function _Agent:Recv()
    local len, buffer = self._kcp:lkcp_recv()

    if (len > 0) then
        local obj
        local data = string.sub(buffer, 2)

        if (#data > 0) then
            local isDone, ret = pcall(_JSON.decode, data)

            if (isDone) then
                obj = ret
            end
        end

        return string.unpack("b", buffer), obj
    end
end

function _Agent:GetFd()
    return self._fd
end

return _Agent