local _SKYNET = require("src.skynet")
local _ID = require("src.id")

local _gate
local _lobby
local _readyFd
local _CMD = {}

function _CMD.OnHandshake(id, fd)
    if (not _SKYNET.Call(_gate, "CheckAgent", fd)) then
        return
    end

    if (not _readyFd) then
        _readyFd = fd
    else
        _SKYNET.Send(_lobby, "NewRoom", _readyFd, fd)
        _readyFd = nil
    end
end

function _CMD.OnDisconnect(id, fd)
    if (_readyFd == fd) then
        _readyFd = nil
    end
end

local function _Start()
    _gate = _SKYNET.queryservice("gate")
    _lobby = _SKYNET.queryservice("lobby")

    _SKYNET.DispatchCommand(_CMD)
    _SKYNET.Send(_gate, "Register", _ID.handshake, _SKYNET.self(), "OnHandshake")
    _SKYNET.Send(_gate, "Register", _ID.disconnect, _SKYNET.self(), "OnDisconnect")
end

_SKYNET.start(_Start)
