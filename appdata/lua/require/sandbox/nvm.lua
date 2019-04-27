local data = {}

function set(a,b)
    data[a] = b
end

function get(a)
    return data[a]
end

function sett(a,b,c)
    if not data[a] then data[a] = {} end
    data[a][b] = c
end

function gett(a,b,c)
    if not data[a] then data[a] = {} end
    return data[a][b]
end

