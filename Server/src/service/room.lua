local _SKYNET = require("src.skynet")
local _SOCKET = require("src.socket")
local _ID = require("src.id")
local _TABLE = require("src.table")

local _gate
local _fds = {}
local _inputMap = {}
local _comparsionHandler = {}
local _playSender = {addrs = {}, inputs = {}}
local _FUNC = {}
local _CMD = {}
local _playerCount = 2
local _playInterval = _SKYNET.Getenv("_play_interval", true)
local _playFrame = 1
local _timer = 0
local _readyPlay = false

function _FUNC.Send(id, obj)
    _SKYNET.Send(_gate, "Send", _fds, id, obj)
end

function _FUNC.Play()
    if (not _readyPlay) then
        return
    end

    _TABLE.Clear(_playSender.addrs)
    _TABLE.Clear(_playSender.inputs)

    for k, v in pairs(_inputMap) do
        table.insert(_playSender.addrs, _SOCKET.ToAddress(k))
        table.insert(_playSender.inputs, v)
    end

    _FUNC.Send(_ID.input, _playSender)
    _TABLE.Clear(_inputMap)
    _readyPlay = false
    _timer = _SKYNET.now()
    _playFrame = _playFrame + 1
end

function _CMD.Exit()
    _SKYNET.exit()
end

function _CMD.Start(leftFd, rightFd)
    _fds = {leftFd, rightFd}
    _FUNC.Send(_ID.start, {seed = os.time(), leftAddr = _SOCKET.ToAddress(leftFd), rightAddr = _SOCKET.ToAddress(rightFd)})
end

function _CMD.ReceiveInput(fd, obj)
    _inputMap[fd] = obj.data

    if (obj.frame == _playFrame) then
        if (not _readyPlay) then
            _readyPlay = true
            local time = _playInterval - (_SKYNET.now() - _timer)
            time = time < 0 and 0 or time
            _SKYNET.timeout(time, _FUNC.Play)
        else
            _FUNC.Play()
        end
    end
end

function _CMD.ReceiveComparison(fd, obj)
    if (not _comparsionHandler[obj.playFrame]) then
        _comparsionHandler[obj.playFrame] = {}
    end

    local map = _comparsionHandler[obj.playFrame]
    map[fd] = obj.content

    if (_TABLE.Count(map) == _playerCount) then
        local late

        for k, v in pairs(map) do
            if (late and v ~= late) then
                print(obj.playFrame, v, "!=", late)
            end

            late = v
        end

        _comparsionHandler[obj.playFrame] = nil
    end
end

local function _Start()
    _gate = _SKYNET.queryservice("gate")
    _SKYNET.DispatchCommand(_CMD)
end

_SKYNET.start(_Start)
