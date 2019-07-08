--加上需要require的路径
local rootPath = apiGetAsciiHex(apiGetPath())
rootPath = rootPath:gsub("[%s%p]", ""):upper()
rootPath = rootPath:gsub("%x%x", function(c)
                                    return string.char(tonumber(c, 16))
                                end)
package.path = package.path..
";"..rootPath.."/data/app/com.papapoi.ReceiverMeow/lua/require/sandbox/?.lua"

JSONLIB = require("JSON")
utils = require("utils")

--加强随机数随机性
math.randomseed(tostring(os.time()):reverse():sub(1, 6))

local less = false
local maxLine = 10
local maxLen = 500
--重写print函数
function print(...)
    if less then return end
    if lua_run_result_var ~= "" then
        lua_run_result_var = lua_run_result_var.."\r\n"
    end
    for i=1,select('#', ...) do
        lua_run_result_var = lua_run_result_var..tostring(select(i, ...))
        if i ~= select('#', ...) then
            lua_run_result_var = lua_run_result_var.."\t"
        end
    end
    local _, count = lua_run_result_var:gsub("\n", "\n")
    if count >= maxLine then
        lua_run_result_var = lua_run_result_var.."\r\n...\r\n余下输出过多，自动省略"
        less = true
    end
    if lua_run_result_var:len() >= maxLen then
        lua_run_result_var = lua_run_result_var:sub(maxLen).."\r\n...\r\n余下输出过多，自动省略"
        less = true
    end
end

json = {
    null = "\0",
    decode = function (s)--安全的，带解析结果返回的json解析函数
        local result, info = pcall(function(t) return JSONLIB:decode(t) end, s)
        if result then
            return info, true
        else
            return {}, false, info
        end
    end,
    encode = function (t)
        return JSONLIB:encode(t,nil,{null=json.null})
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

struct = require("struct")
pack = {
    pack = struct.pack,
    unpack = struct.unpack,
}

BIT = require("bit")
bit = BIT.bit32
bit.bit = function(b) return bit.lshift(1,b) end
bit.isset = function(v,p) return bit.rshift(v,p) % 2 == 1 end
bit.isclear = function(v,p) return not bit.isset(v,p) end

nvm = require("nvm")

log = {
    info = print,
    trace = print,
    debug = print,
    warn = print,
    error = print,
    fatal = print,
}

misc = require("misc")

--安全的函数
local safeFunctions = {
    assert = true,
    setmetatable = true,
    getmetatable = true,
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
    log = true,
    misc = true,
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

