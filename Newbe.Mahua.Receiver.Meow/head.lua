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
function httpGet(url,para,timeout,cookie)
    if not para then para = "" end
    if not timeout then timeout = 5000 end
    if not cookie then cookie = "" end
    local result = httpGet_row(url,para,timeout)
    if result ~= "" then return result end
end

--httpPost获取
function httpPost(url,para,timeout,cookie)
    if not para then para = "" end
    if not timeout then timeout = 5000 end
    if not cookie then cookie = "" end
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

--发送语音
function music(url)
    str = "1234567890ABCDEFHIJKLMNOPQRSTUVWXYZ"
    local ret = ""
    for i = 1, 20 do
        local rchr = math.random(1, string.len(str))
        ret = ret .. string.sub(str, rchr, rchr)
    end
    if httpDownload(url,"record/"..ret,5000) then
        return "[CQ:record,file="..ret.."]"
    end
end

--图片对象
img = {width = 0, height = 0, imageData = nil}
--新建图片对象
function img:new (width,height)
    o = {}
    setmetatable(o, self)
    self.__index = self
    self.width = width or 0
    self.height = height or 0
    self.imageData = getImg(self.width,self.height)
    return o
end
--摆放文字
function img:setText(x,y,text,type1,size,r,g,b)
    self.imageData = setImgText(self.imageData,x-1,y-1,text,
    type1 or "微软雅黑", size or 9, r or 0, g or 0, b or 0)
end
--摆放矩形
function img:setBlock(x,y,xx,yy,r,g,b)
    self.imageData = putImgBlock(self.imageData,x-1,y-1,
    xx-1>self.width-1 and self.width-1 or xx-1,yy-1>self.height-1 and self.height-1 or yy-1,
    r,g,b)
end
--摆放其他图片
function img:setImg(x,y,path,xx,yy)
    self.imageData = setImgImage(self.imageData,x-1,y-1,path,xx and xx-1 or 0, yy and yy-1 or 0)
end
--获取图片结果
function img:get()
    return "[CQ:image,file="..getImgDir(self.imageData).."]"
end


local runCount = 0
local start = os.time()
function trace (event, line)
    runCount = runCount + 1
    if runCount > 100000 then
        error("运行代码量超过阈值")
    elseif os.time() - start >=15 then
        error("运行超时")
    end
end
debug.sethook(trace, "l")

--安全的函数
local safeFunctions = {
    setmetatable = true,
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
    img = true,
    getImg = true,
    setImgText = true,
    putImgBlock = true,
    setImgImage = true,
    getImgDir = true,
    getPath = true,
    sendGroupMessage = true,
    sendPrivateMessage = true,
    getImageUrl = true,
    httpDownload = true,
    music = true,
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

