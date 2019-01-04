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
    local result = httpGet_row(url,para,timeout):fromHex()
    if result ~= "" then return result end
end

--httpPost获取
function httpPost(url,para,timeout)
    if not para then para = "" end
    if not timeout then timeout = 5000 end
    local result = httpPost_row(url,para,timeout):fromHex()
    if result ~= "" then return result end
end

--url编码
function string.urlEncode(s)
    local s = s:toHex()
    return urlEncode_row(s)
end

--存储数据
function setData(qq,name,str)
    assert(type(qq) == "string", "setData invalid first partment("..type(qq)..") must be string")
    assert(type(name) == "string", "setData invalid second partment("..type(name)..") must be string")
    assert(type(str) == "string", "setData invalid third partment("..type(str)..") must be string")
    assert(name:len() >= 10, "setData second partment too short("..tostring(name:len())..") must more then 10 byte")
    name = name:toHex()
    str = str:toHex()
    setData_row(qq,name,str)
end

--读取数据
function getData(qq,name,str)
    assert(type(qq) == "string", "getData invalid first partment("..type(qq)..") must be string")
    assert(type(name) == "string", "getData invalid second partment("..type(name)..") must be string")
    name = name:toHex()
    local result = getData_row(qq,name):fromHex()
    return result
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
    setData_row = true,
    getData_row = true,
    setData = true,
    getData = true,
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
