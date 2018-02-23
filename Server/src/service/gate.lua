local _SKYNET = require("src.skynet")
local _SOCKET = require("skynet.socket")
local _ID = require("src.id")

local _Agent = require("src.agent")

local _udp
local _agentMap = {}
local _eventHandler = {}
local _timer = 0
local _maxClient = _SKYNET.Getenv("max_client", true)
local _clientCount = 0
local _updateInterval = _SKYNET.Getenv("update_interval", true)
local _heartbeatInterval = _SKYNET.Getenv("heartbeat_interval", true)
local _CMD = {}

function _CMD.Register(id, service, name)
    if (not _eventHandler[id]) then
        _eventHandler[id] = {}
    end

    table.insert(_eventHandler[id], {service = service, name = name})
end

function _CMD.Heartbeat(id, fd)
    _agentMap[fd]:Send(_ID.heartbeat)
end

function _CMD.CheckAgent(fd)
    if (type(fd) == "table") then
        for n=1, #fd do
            if (not _agentMap[fd[n]]) then
                return false
            end
        end

        return true
    else
        return _agentMap[fd] ~= nil
    end
end

function _CMD.Send(fd, id, obj)
    if (type(fd) == "table") then
        for n=1, #fd do
            if (_agentMap[fd[n]]) then
                _agentMap[fd[n]]:Send(id, obj)
            else
                print("no existed", fd[n])
            end
        end
    else
        if (_agentMap[fd]) then
            _agentMap[fd]:Send(id, obj)
        else
            print("no existed", fd)
        end
    end
end

local function _SendEvent(id, fd, obj)
    if (_eventHandler[id]) then
        for n=1, #_eventHandler[id] do
            _SKYNET.Send(_eventHandler[id][n].service, _eventHandler[id][n].name, id, fd, obj)
        end
    end
end

local function _OnReceive(data, from)
    if (_clientCount < _maxClient and not _agentMap[from] and string.unpack("b", data, #data) == _ID.connect) then
        print("connect", _SOCKET.udp_address(from))
        _agentMap[from] = _Agent.New(1, from, function (_data)
            _SOCKET.sendto(_udp, from, _data)
        end)

        _clientCount = _clientCount + 1
        _agentMap[from]:Send(_ID.connect, {fd = from})
    elseif (_agentMap[from]) then
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
            local id, obj = v:Recv()

            if (id) then
                _SendEvent(id, k, obj)
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
                _clientCount = _clientCount - 1
                _SendEvent(_ID.disconnect, k)
                print("disconnect", _SOCKET.udp_address(k))
            else
                v.heartbeat = false
            end
        end

        _SKYNET.sleep(_heartbeatInterval)
    end
end

local function _Start()
    _udp = _SOCKET.udp(_OnReceive, _SKYNET.Getenv("udp_address"), _SKYNET.Getenv("udp_port", true))
    _SKYNET.fork(_Update)
    _SKYNET.fork(_Recv)
    _SKYNET.fork(_Heartbeat)
    _SKYNET.DispatchCommand(_CMD)
    _SKYNET.Send(_SKYNET.self(), "Register", _ID.heartbeat, _SKYNET.self(), "Heartbeat")
end

_SKYNET.start(_Start)
