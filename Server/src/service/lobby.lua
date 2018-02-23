local _SKYNET = require("src.skynet")
local _ID = require("src.id")

local _gate
local _roomMap = {}
local _leftFdMap = {}
local _rightFdMap = {}
local _CMD = {}

function _CMD.NewRoom(leftFd, rightFd)
    if (not _SKYNET.Call(_gate, "CheckAgent", {leftFd, rightFd})) then
        return
    end

    _leftFdMap[leftFd] = rightFd
    _rightFdMap[rightFd] = leftFd
    _roomMap[leftFd .. rightFd] = _SKYNET.newservice("room")
    _SKYNET.Send(_roomMap[leftFd .. rightFd], "Start", leftFd, rightFd)
end

function _CMD.OnDisconnect(id, fd)
    local leftFd
    local rightFd

    if (_leftFdMap[fd]) then
        leftFd = fd
        rightFd = _leftFdMap[fd]
    elseif (_rightFdMap[fd]) then
        leftFd = _rightFdMap[fd]
        rightFd = fd
    end

    if (leftFd and rightFd) then
        _leftFdMap[leftFd] = nil
        _rightFdMap[rightFd] = nil
        _SKYNET.Send(_roomMap[leftFd .. rightFd], "Exit")
        _roomMap[leftFd .. rightFd] = nil
    end
end

local function _Start()
    _gate = _SKYNET.queryservice("gate")
    _SKYNET.DispatchCommand(_CMD)
    _SKYNET.Send(_gate, "Register", _ID.disconnect, _SKYNET.self(), "OnDisconnect")
end

_SKYNET.start(_Start)
