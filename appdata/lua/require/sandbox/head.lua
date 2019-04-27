--加上需要require的路径
package.path = package.path..
";./data/app/com.papapoi.ReceiverMeow/lua/require/sandbox/?.lua"

JSONLIB = require("JSON")
utils = require("utils")
struct = require("struct")
BIT = require("bit")
nvm = require("nvm")

--加强随机数随机性
math.randomseed(tostring(os.time()):reverse():sub(1, 6))

local lineCount = 0
--重写print函数
function print(...)
    local maxLine = 10
    if lineCount >= maxLine then return end
    if lua_run_result_var ~= "" then
        lua_run_result_var = lua_run_result_var.."\r\n"
    end
    for i=1,select('#', ...) do
        lua_run_result_var = lua_run_result_var..tostring(select(i, ...))
        if i ~= select('#', ...) then
            lua_run_result_var = lua_run_result_var.."\t"
        end
    end
    lineCount = lineCount + 1
    if lineCount == maxLine then
        lua_run_result_var = lua_run_result_var.."\r\n...\r\n余下输出过多，自动省略"
    end
end

json = {
    decode = function (s)--安全的，带解析结果返回的json解析函数
        local result, info = pcall(function(t) return JSONLIB:decode(t) end, s)
        if result then
            return info, true
        else
            return {}, false, info
        end
    end,
    encode = function (t)
        return JSONLIB:encode(t)
    end
}

local runCount = 0
local start = os.time()
function trace (event, line)
    runCount = runCount + 1
    if runCount > 100000 then
        error("运行代码量超过阈值")
    end
    if os.time() - start >= 5 then
        error("代码运行超时")
    end
end
debug.sethook(trace, "l")

loadstring = load

pack = {
    pack = struct.pack,
    unpack = struct.unpack,
}

bit = BIT.bit32
bit.bit = function(b) return bit.lshift(1,b) end
bit.isset = function(v,p) return bit.rshift(v,p) % 2 == 1 end
bit.isclear = function(v,p) return not bit.isset(v,p) end

--安全的函数
local safeFunctions = {
    assert = true,
    setmetatable = true,
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
    JSONLIB = true,
    json = true,
    loadstring = true,
    pack = true,
    lockbox = true,
    crypto = true,
    bit = true,
    nvm = true,
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

