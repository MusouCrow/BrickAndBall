local _SKYNET = require("src.skynet")
local _NETPACK = require("skynet.netpack")
local _GATESERVER = require("snax.gateserver")

local _watchdog
local _connections = {}
local _HANDLER = {}
local _CMD = {}

function _CMD.OpenClient(fd)
    _GATESERVER.openclient(fd)
end

function _CMD.CloseClient(fd)
    _GATESERVER.closeclient(fd)
end

function _HANDLER.open(source, conf)
    _watchdog = source
end

function _HANDLER.connect(fd, ip)
    _connections[fd] = {fd = fd, ip = ip}
    _SKYNET.Send(_watchdog, "NewAgent", fd, ip)
end

function _HANDLER.disconnect(fd)
    _connections[fd] = nil
    _SKYNET.Send(_watchdog, "DelAgent", fd)
    print("disconnect", fd)
end

function _HANDLER.error(fd, msg)
    _connections[fd] = nil
    print("error", fd, msg)
end

function _HANDLER.message(fd, msg, sz)
    print(fd, msg, sz)
    print(_NETPACK.tostring(msg, sz))
end

function _HANDLER.command(cmd, source, ...)
    return _CMD[cmd](...)
end

function _HANDLER.warning(fd, size)

end

_GATESERVER.start(_HANDLER)
