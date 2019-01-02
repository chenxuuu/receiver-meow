function randomStr(str, num)
    local ret = ""
    for i = 1, num do
        local rchr = math.random(1, string.len(str))
        ret = ret .. string.sub(str, rchr, rchr)
    end
    return ret
end
print("magnet:?xt=urn:btih:"..randomStr("1234567890ABCDEF", 40))
