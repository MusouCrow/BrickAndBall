local _SKYNET = require("src.skynet")
local _ID = require("src.id")

local _gate
local _leftFd
local _rightFd
local _CMD = {}

function _CMD.Exit()
    print("exit room")
    _SKYNET.exit()
end

function _CMD.Start(leftFd, rightFd)
    _leftFd = leftFd
    _rightFd = rightFd

    _SKYNET.Send(_gate, "Send", {_leftFd, _rightFd}, _ID.start, {seed = os.time(), leftFd = leftFd, rightFd = rightFd})
end

local function _Start()
    print("start room")
    _gate = _SKYNET.queryservice("gate")
    _SKYNET.DispatchCommand(_CMD)
end

_SKYNET.start(_Start)
