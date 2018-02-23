local _SKYNET = require("src.skynet")

local function _Start()
    print("start")
    _SKYNET.uniqueservice("gate")
    _SKYNET.uniqueservice("lobby")
    _SKYNET.uniqueservice("queue")
    _SKYNET.exit()
end

_SKYNET.start(_Start)
