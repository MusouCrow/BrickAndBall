local _SKYNET = require("src.skynet")
local _SOCKET = require("src.socket")
local _ID = require("src.id")

local _gate
local _roomMap = {}
local _leftFdMap = {}
local _rightFdMap = {}
local _FUNC = {}
local _CMD = {}

local _marks = {
    [_ID.input] = "ReceiveInput",
    [_ID.comparison] = "ReceiveComparison"
}

function _FUNC.GetRoom(fd)
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
        return _roomMap[leftFd .. rightFd], leftFd, rightFd
    end
end

function _CMD.NewRoom(leftFd, rightFd)
    if (not _SKYNET.Call(_gate, "CheckAgent", {leftFd, rightFd})) then
        return
    end

    _leftFdMap[leftFd] = rightFd
    _rightFdMap[rightFd] = leftFd
    _roomMap[leftFd .. rightFd] = _SKYNET.newservice("room")
    _SKYNET.Send(_roomMap[leftFd .. rightFd], "Start", leftFd, rightFd)

    print("start room", _SOCKET.ToAddress(leftFd), _SOCKET.ToAddress(rightFd))
end

function _CMD.OnDisconnect(id, fd)
    local room, leftFd, rightFd = _FUNC.GetRoom(fd)

    if (room) then
        _leftFdMap[leftFd] = nil
        _rightFdMap[rightFd] = nil
        _SKYNET.Send(room, "Exit")
        _roomMap[leftFd .. rightFd] = nil
        _SKYNET.Send(_gate, "Kick", {leftFd, rightFd})

        print("exit room", _SOCKET.ToAddress(leftFd), _SOCKET.ToAddress(rightFd))
    end
end

function _CMD.OnReceive(id, fd, obj)
    local room, leftFd, rightFd = _FUNC.GetRoom(fd)

    if (room) then
        _SKYNET.Send(room, _marks[id], fd, obj)
    end
end

local function _Start()
    _gate = _SKYNET.queryservice("gate")
    _SKYNET.DispatchCommand(_CMD)
    _SKYNET.Send(_gate, "Register", _ID.disconnect, _SKYNET.self(), "OnDisconnect")

    for k in pairs(_marks) do
        _SKYNET.Send(_gate, "Register", k, _SKYNET.self(), "OnReceive")
    end
end

_SKYNET.start(_Start)
