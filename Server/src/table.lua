local _TABLE = {
    empty = {}
}

function _TABLE.Clear(tab)
    for k in pairs (tab) do
        tab [k] = nil
    end
end

function _TABLE.Count(tab)
    local count = 0

    for k in pairs (tab) do
        count = count + 1
    end

    return count
end

function _TABLE.Resolve(tab)
    local n = 0
    local keys = {}
    local values = {}

    for k, v in pairs(tab) do
        n = n + 1
        keys[n] = k
        values[n] = v
    end

    return keys, values
end

return _TABLE