local _SKYNET = require("src.skynet")
local _SOCKET = require("skynet.socket")

local _fd
local _ip
local _CMD = {}

function _CMD.Start(fd, ip)
    _fd = fd
    _ip = ip

    local package = string.pack(">s2", "gge")
    _SOCKET.write(_fd, package)
end

function _CMD.Exit()
    _SKYNET.exit()
end

local function _Start()
    _SKYNET.DispatchCommand(_CMD)
end

_SKYNET.start(_Start)
