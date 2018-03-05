local _SKYNET = require("src.skynet")
local _SOCKET = require("src.socket")
local _JSON = require("cjson")
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
local _version = _SKYNET.Getenv("version", true)
local _FUNC = {}
local _CMD = {}

function _FUNC.SendEvent(id, fd, obj)
    if (_eventHandler[id]) then
        for n=1, #_eventHandler[id] do
            _SKYNET.Send(_eventHandler[id][n].service, _eventHandler[id][n].name, id, fd, obj)
        end
    end
end

function _FUNC.OnReceive(data, from)
    if (not _agentMap[from] and string.unpack("b", data, #data) == _ID.connect) then
        local addr = _SOCKET.ToAddress(from)
        _SKYNET.Log("connect", addr)
        _agentMap[from] = _Agent.New(1, from, function (_data)
            _SOCKET.sendto(_udp, from, _data)
        end)

        _clientCount = _clientCount + 1
        _agentMap[from]:Send(_ID.connect, {addr = addr, version = _version, isFull = _clientCount > _maxClient})
    elseif (_agentMap[from]) then
        _agentMap[from]:Input(data)
    end
end

function _FUNC.Update()
    for k, v in pairs(_agentMap) do
        v:Update(_timer)
    end

    _timer = _timer + _updateInterval * 10
end

function _FUNC.Recv()
    for k, v in pairs(_agentMap) do
        local id, obj = v:Recv()

        if (id) then
            _FUNC.SendEvent(id, k, obj)
        end
    end
end

function _FUNC.Kick(fd)
    if (not _agentMap[fd]) then
        return
    end

    _agentMap[fd] = nil
    _clientCount = _clientCount - 1
    _FUNC.SendEvent(_ID.disconnect, fd)
    _SKYNET.Log("disconnect", _SOCKET.ToAddress(fd))
end

function _FUNC.Heartbeat()
    for k, v in pairs(_agentMap) do
        if (not v.heartbeat) then
            _FUNC.Kick(k)
        else
            v.heartbeat = false
        end
    end
end

function _CMD.Register(id, service, name)
    if (not _eventHandler[id]) then
        _eventHandler[id] = {}
    end

    table.insert(_eventHandler[id], {service = service, name = name})
end

function _CMD.Heartbeat(id, fd)
    _agentMap[fd]:Send(_ID.heartbeat)
end

function _CMD.OnHandshake(id, fd, obj)
    _agentMap[fd].deviceModel = obj.deviceModel
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

function _CMD.GetAgentValue(fd, name)
    if (type(fd) == "table") then
        local ret = {}

        for n=1, #fd do
            if (_agentMap[fd[n]]) then
                ret[n] = _agentMap[fd[n]][name]
            else
                _SKYNET.Log("no existed", fd[n])
            end
        end

        return ret
    else
        if (_agentMap[fd]) then
            return _agentMap[fd][name]
        else
            _SKYNET.Log("no existed", fd)
        end
    end
end

function _CMD.Send(fd, id, obj)
    if (type(fd) == "table") then
        local data

        if (obj) then
            data = _JSON.encode(obj)
        end

        for n=1, #fd do
            if (_agentMap[fd[n]]) then
                _agentMap[fd[n]]:Send(id, data)
            else
                _SKYNET.Log("no existed", fd[n])
            end
        end
    else
        if (_agentMap[fd]) then
            _agentMap[fd]:Send(id, obj)
        else
            _SKYNET.Log("no existed", fd)
        end
    end
end

function _CMD.Kick(fd)
    if (type(fd) == "table") then
        for n=1, #fd do
            _FUNC.Kick(fd[n])
        end
    else
        _FUNC.Kick(fd)
    end
end

local function _Start()
    _udp = _SOCKET.udp(_FUNC.OnReceive, _SKYNET.Getenv("udp_address"), _SKYNET.Getenv("udp_port", true))
    _SKYNET.Loop(_FUNC.Update, _updateInterval)
    _SKYNET.Loop(_FUNC.Recv, 0)
    _SKYNET.Loop(_FUNC.Heartbeat, _heartbeatInterval)
    _SKYNET.DispatchCommand(_CMD)
    _SKYNET.Send(_SKYNET.self(), "Register", _ID.heartbeat, _SKYNET.self(), "Heartbeat")
    _SKYNET.Send(_SKYNET.self(), "Register", _ID.handshake, _SKYNET.self(), "OnHandshake")
end

_SKYNET.start(_Start)
