JSON = require("JSON")
utils = require("utils")

--加强随机数随机性
math.randomseed(tostring(os.time()):reverse():sub(1, 6))

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
    local result = httpGet_row(url,para,timeout)
    if result ~= "" then return result end
end

--httpPost获取
function httpPost(url,para,timeout)
    if not para then para = "" end
    if not timeout then timeout = 5000 end
    local result = httpPost_row(url,para,timeout)
    if result ~= "" then return result end
end

--存储数据
function setData(qq,name,str)
    assert(type(qq) == "string", "setData invalid first partment("..type(qq)..") must be string")
    assert(type(name) == "string", "setData invalid second partment("..type(name)..") must be string")
    assert(type(str) == "string", "setData invalid third partment("..type(str)..") must be string")
    assert(name:len() >= 10, "setData second partment too short("..tostring(name:len())..") must more then 10 byte")
    setData_row(qq,name,str)
end

--读取数据
function getData(qq,name,str)
    assert(type(qq) == "string", "getData invalid first partment("..type(qq)..") must be string")
    assert(type(name) == "string", "getData invalid second partment("..type(name)..") must be string")
    local result = getData_row(qq,name)
    return result
end

--安全的，带解析结果返回的json解析函数
function jsonDecode(s)
    local result, info = pcall(function(t) return JSON:decode(t) end, s)
    if result then
        return info, true
    else
        return {}, false, info
    end
end

--显示某张图片
function image(url,ban)
    str = "1234567890ABCDEFHIJKLMNOPQRSTUVWXYZ"
    local ret = ""
    for i = 1, 20 do
        local rchr = math.random(1, string.len(str))
        ret = ret .. string.sub(str, rchr, rchr)
    end
    local b = false
    if ban then b = true end
    if fileDownload(url,ret,5000,b) then
        return "[CQ:image,file=download\\"..ret.."]"
    end
end

local runCount = 0
local start = os.time()
function trace (event, line)
    runCount = runCount + 1
    if runCount > 100000 then
        error("运行代码量超过阈值")
    elseif os.time() - start >=10 then
        error("运行超时")
    end
end
debug.sethook(trace, "l")

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
    jsonDecode = true,
    fileDownload = true,
    image = true,
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

