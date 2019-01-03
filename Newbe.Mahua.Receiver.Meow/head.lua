JSON = require("JSON")
utils = require("utils")
--重写print函数
function print(...)
    if lua_run_result_var ~= "" then
        lua_run_result_var = lua_run_result_var.."\r\n"
    end
    for i=1,select('#', ...) do
        lua_run_result_var = lua_run_result_var..tostring(select(i, ...))
        if i ~= select('#', ...) then
            lua_run_result_var = lua_run_result_var.."\t"
        end
    end
end

--返回at某人的字符串
function at(qq)
    return "[CQ:at,qq="..tostring(qq).."]"
end

--httpGet获取
function httpGet(url,para,timeout)
    if not para then para = "" end
    if not timeout then timeout = 5000 end
    return httpGet_row(url,para,timeout):fromHex()
end

--httpPost获取
function httpPost(url,para,timeout)
    if not para then para = "" end
    if not timeout then timeout = 5000 end
    return httpPost_row(url,para,timeout):fromHex()
end

--url编码
function string.urlEncode(s)
    local s = s:toHex()
    return urlEncode_row(s)
end

--安全的函数
local safeFunctions = {
    assert = true,
    error = true,
    ipairs = true,
    next = true,
    pairs = true,
    pcall = true,
    select = true,
    tonumber = true,
    tostring = true,
    type = true,
    unpack = true,
    _VERSION = true,
    xpcall = true,
    coroutine = true,
    string = true,
    table = true,
    math = true,
    print = true,
    _G = true,
    lua_run_result_var = true,
    os = true,
    at = true,
    httpGet_row = true,
    httpGet = true,
    httpPost_row = true,
    httpPost = true,
    JSON = true,
    encodeChange = true,
    urlEncode_row = true,
}

--安全的os函数
local safeOsFunctions = {
    clock = true,
    difftime = true,
    time = true,
    date = true,
}
--去除所有不安全函数
for fnc in pairs(os) do
    if not safeOsFunctions[fnc] then
        os[fnc] = nil
    end
end
for fnc in pairs(_G) do
    if not safeFunctions[fnc] then
        _G[fnc] = nil
    end
end

--加强随机数随机性
math.randomseed(tostring(os.time()):reverse():sub(1, 6))
