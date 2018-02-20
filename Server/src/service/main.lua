local _SKYNET = require("src.skynet")
local _SOCKET = require("skynet.socket")

local _Agent = require("src.agent")

local _udp
local _agentMap = {}
local _timer = 0
local _updateInterval = _SKYNET.getenv("update_interval")
local _heartbeatInterval = _SKYNET.getenv("heartbeat_interval")

local function _OnReceive(data, from)
    if (not _agentMap[from] and string.find(data, "connect")) then
        print("connect", _SOCKET.udp_address(from))
        _agentMap[from] = _Agent.New(1, from, function (_data)
            _SOCKET.sendto(_udp, from, _data)
        end)

        _agentMap[from]:Send("welcome")
    end

    if (_agentMap[from]) then
        _agentMap[from]:Input(data)
    end
end

local function _Update()
    while true do
        for k, v in pairs(_agentMap) do
            v:Update(_timer)
        end

        _timer = _timer + _updateInterval * 10
        _SKYNET.sleep(_updateInterval)
    end
end

local function _Recv()
    while true do
        for k, v in pairs(_agentMap) do
            local len, data = v:Recv()

            if (data == "h") then
                v:Send("h")
            end
        end

        _SKYNET.yield()
    end
end

local function _Heartbeat()
    while true do
        for k, v in pairs(_agentMap) do
            if (not v.heartbeat) then
                _agentMap[k] = nil
                print("disconnect", _SOCKET.udp_address(k))
            else
                v.heartbeat = false
            end
        end

        _SKYNET.sleep(_heartbeatInterval)
    end
end

local function _Start()
    print("start")

    _udp = _SOCKET.udp(_OnReceive, _SKYNET.getenv("udp_address"), _SKYNET.getenv("udp_port"))
    _SKYNET.fork(_Update)
    _SKYNET.fork(_Recv)
    _SKYNET.fork(_Heartbeat)
end

_SKYNET.start(_Start)
