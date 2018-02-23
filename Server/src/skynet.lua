local _ORIGIN_SKYNET = require("skynet")
local _SKYNET = {}
setmetatable(_SKYNET, {__index = _ORIGIN_SKYNET})

local _protoName = "lua"

function _SKYNET.Dispatch(func)
    return _ORIGIN_SKYNET.dispatch(_protoName, func)
end

function _SKYNET.Send(addr, ...)
    return _ORIGIN_SKYNET.send(addr, _protoName, ...)
end

function _SKYNET.Call(addr, ...)
    return _ORIGIN_SKYNET.call(addr, _protoName, ...)
end

function _SKYNET.Redirect(addr, source, ...)
    return _ORIGIN_SKYNET.redirect(addr, source, _protoName, ...)
end

function _SKYNET.DispatchCommand(CMD)
    local MsgEvent = function(session, source, cmd, ...)
        _SKYNET.ret(_SKYNET.pack(CMD[cmd](...)))
    end

    _SKYNET.Dispatch(MsgEvent)
end

function _SKYNET.Getenv(key, isNum)
    local v = _ORIGIN_SKYNET.getenv(key)

    if (isNum) then
        v = tonumber(v)
    end

    return v
end

return _SKYNET