local _SKYNET = require("src.skynet")

local function _Start()
    _SKYNET.Log("start")

    _SKYNET.uniqueservice("gate")
    _SKYNET.uniqueservice("lobby")
    _SKYNET.uniqueservice("queue")
    _SKYNET.exit()
end

_SKYNET.start(_Start)
