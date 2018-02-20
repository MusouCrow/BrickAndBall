
local lutil = require "lutil"

local M = {}
local DelayPacket = {}
local LatencySM = {} 

local function getms()
    return math.floor(lutil.gettimeofday())
end

local function rand()
    return math.random(100)
end

--delay packet
function DelayPacket.new(data)
    local self = {}
    self.data = data
    self.ts = 0
    setmetatable(self, {__index = DelayPacket})
    return self
end

function DelayPacket:getdata()
    return self.data
end

function DelayPacket:getts()
    return self.ts
end

function DelayPacket:setts(ts)
    self.ts = ts
end

--latency simulator
function LatencySM.new(lostrate, rttmin, rttmax)
    local self = {}
    lostrate = lostrate or 10
    rttmin = rttmin or 60
    rttmax = rttmin or 125

    self.lostrate = lostrate/2
    self.rttmin = rttmin/2
    self.rttmax = rttmax/2

    self.tunnel01 = {}
    self.tunnel10 = {}

    setmetatable(self, {__index = LatencySM})
    return self
end

function LatencySM:send(peer, data)
    local ra = rand()
    if rand() < self.lostrate then
        return
    end
    local pkt = DelayPacket.new(data)
    local nowt = getms()
    local delay = self.rttmin
    
    if self.rttmax > self.rttmin then
        delay = delay + rand() % (self.rttmax - self.rttmin)
    end
    pkt:setts(nowt + delay)
    if peer == 0 then
        table.insert(self.tunnel01, pkt)
    else
        table.insert(self.tunnel10, pkt)
    end
end

function LatencySM:recv(peer)
    local tunnel
    if peer == 0 then
        tunnel = self.tunnel10
    else
        tunnel = self.tunnel01
    end
    if #tunnel == 0 then
        return -1
    end
    local pkt = tunnel[1]
    local nowt = getms()
    if nowt < pkt:getts() then
        return -2
    end
    table.remove(tunnel, 1)
    local ret = pkt:getdata()
    return #ret, ret
end

M.LatencySM = LatencySM

return M

