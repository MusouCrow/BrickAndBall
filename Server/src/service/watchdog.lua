local _SKYNET = require("src.skynet")

local _gate
local _agents = {}
local _CMD = {}

function _CMD.Start(conf)
    _SKYNET.Call(_gate, "open", conf)
end

function _CMD.NewAgent(fd, ip)
    print("new client from: " .. ip)
    _agents[fd] = _SKYNET.newservice("agent")
    _SKYNET.Call(_gate, "OpenClient", fd)
    _SKYNET.Send(_agents[fd], "Start", fd, ip)
end

function _CMD.DelAgent(fd)
    local a = _agents[fd]
    _agents[fd] = nil

    if (a) then
        _SKYNET.Call(_gate, "CloseClient", fd)
        _SKYNET.Send(a, "Exit")
    end
end

local function _Start()
    _gate = _SKYNET.newservice("gate")
    _SKYNET.DispatchCommand(_CMD)
end

_SKYNET.start(_Start)
