local _ORIGIN_SOCKET = require("skynet.socket")
local _SOCKET = {}
setmetatable(_SOCKET, {__index = _ORIGIN_SOCKET})

function _SOCKET.ToAddress(fd)
    local addr, port = _ORIGIN_SOCKET.udp_address(fd)
    return addr .. ":" .. port
end

return _SOCKET