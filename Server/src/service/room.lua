local _SKYNET = require("src.skynet")
local _SOCKET = require("src.socket")
local _ID = require("src.id")
local _TABLE = require("src.table")

local _gate
local _fds = {}
local _inputMap = {}
local _comparsionHandler = {}
local _inputSender = {addrs = {}, inputs = {}}
local _FUNC = {}
local _CMD = {}
local _playerCount = 2

function _FUNC.Send(id, obj)
    _SKYNET.Send(_gate, "Send", _fds, id, obj)
end

function _CMD.Exit()
    _SKYNET.exit()
end

function _CMD.Start(leftFd, rightFd)
    _fds = {leftFd, rightFd}
    _FUNC.Send(_ID.start, {seed = os.time(), leftAddr = _SOCKET.ToAddress(leftFd), rightAddr = _SOCKET.ToAddress(rightFd)})
end

function _CMD.ReceiveInput(fd, obj)
    _inputMap[fd] = obj
    local count = _TABLE.Count(_inputMap)

    if (count == _playerCount) then
        _TABLE.Clear(_inputSender.addrs)
        _TABLE.Clear(_inputSender.inputs)

        for k, v in pairs(_inputMap) do
            table.insert(_inputSender.addrs, _SOCKET.ToAddress(k))
            table.insert(_inputSender.inputs, v)
        end

        _FUNC.Send(_ID.input, _inputSender)
        _TABLE.Clear(_inputMap)
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
                print(v, "!=", late)
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
