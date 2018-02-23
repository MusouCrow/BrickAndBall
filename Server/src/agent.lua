local _KCP = require("lkcp")

local _Agent = require("src.class")()

function _Agent:Ctor(conv, tag, SendWrap)
    self._kcp = _KCP.lkcp_create(conv, SendWrap)
    self._tag = tag
    self.heartbeat = true
end

function _Agent:Update(clock)
    self._kcp:lkcp_update(clock)
end

function _Agent:Input(data)
    self._kcp:lkcp_input(data)
    self.heartbeat = true
end

function _Agent:Send(id)
    local data = string.pack("b", id)
    self._kcp:lkcp_send(data)
end

function _Agent:Recv()
    local len, data = self._kcp:lkcp_recv()

    if (len > 0) then
        return string.unpack("b", data), string.sub(data, 2)
    end
end

function _Agent:GetTag()
    return self._tag
end

return _Agent