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

function _Agent:Send(data)
    self._kcp:lkcp_send(data)
end

function _Agent:Recv()
    return self._kcp:lkcp_recv()
end

function _Agent:GetTag()
    return self._tag
end

return _Agent