local function _Clone(object, table)
    local lookup_table = {}

    local function _copy(object)
        if type(object) ~= "table" then
            return object
        elseif lookup_table[object] then
            return lookup_table[object]
        end

        local new_table = table or {}
        lookup_table[object] = new_table

        for key, value in pairs(object) do
            new_table[_copy(key)] = _copy(value)
        end

        return setmetatable(new_table, getmetatable(object))
    end

    return _copy(object, table)
end

local function _Class(...) -- super list
    local cls
	local superList = {...}

    if (#superList > 0) then
		cls = _Clone(superList[1])
		
        for n=2, #superList do
			cls = _Clone(superList[n], cls)
		end
    else
        cls = {Ctor = function() end}
    end

    function cls.New(...)
        local instance = setmetatable({}, {__index = cls})
        instance.class = cls
        instance:Ctor(...)
        return instance
    end    

    return cls
end

return _Class