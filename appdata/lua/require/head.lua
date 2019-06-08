--提前运行的脚本
--用于提前声明某些要用到的函数

--管理员账号，从xml中读取
--只会在启动第一次执行的时候读一次，后面都不读
admin = tonumber(apiGetVar("adminqq")) or -1
if admin == -1 then
    admin = apiXmlGet("settings","adminqq")
    apiSetVar("adminqq",admin)
    admin = tonumber(admin) or -1
end
if admin == -1 then
    cqAddLoger(20, "lua插件警告", "请去"..apiGetPath().."data/app/com.papapoi.ReceiverMeow/lua/xml/settings.xml文件，设置管理员qq！")
end

--加强随机数随机性
math.randomseed(tostring(os.time()):reverse():sub(1, 6))

--防止跑死循环，超时设置秒数自动结束，-1表示禁用
local maxSeconds = -1
local start = os.time()
function trace (event, line)
    if os.time() - start >=maxSeconds then
        error("代码运行超时")
    end
end
if maxSeconds > 0 then
    debug.sethook(trace, "l")
end

--加上需要require的路径
local rootPath = apiGetAsciiHex(apiGetPath())
rootPath = rootPath:gsub("[%s%p]", ""):upper()
rootPath = rootPath:gsub("%x%x", function(c)
                                    return string.char(tonumber(c, 16))
                                end)
package.path = package.path..
";"..rootPath.."/data/app/com.papapoi.ReceiverMeow/lua/require/?.lua"

--加载字符串工具包
require("strings")

--重载几个可能影响中文目录的函数
local oldrequire = require
require = function (s)
    local s = apiGetAsciiHex(s):fromHex()
    return oldrequire(s)
end
local oldloadfile = loadfile
loadfile = function (s)
    local s = apiGetAsciiHex(s):fromHex()
    return oldloadfile(s)
end

JSON = require("JSON")
--安全的，带解析结果返回的json解析函数
--返回值：数据,是否成功,错误信息
function jsonDecode(s)
    local result, info = pcall(function(t) return JSON:decode(t) end, s)
    if result then
        return info, true
    else
        return {}, false, info
    end
end
function jsonEncode(t)
    local result, info = pcall(function(t) return JSON:encode(t) end, t)
    if result then
        return info, true
    else
        return "", false, info
    end
end

--修正http接口可选参数
local oldapiHttpGet = apiHttpGet
apiHttpGet = function (url,para,timeout,cookie)
    return oldapiHttpGet(url,para or "",timeout or 5000,cookie or "")
end
local oldapiHttpPost = apiHttpPost
apiHttpPost = function (url,para,timeout,cookie,contentType)
    return oldapiHttpPost(url,para or "",timeout or 5000,cookie or "",contentType or "application/x-www-form-urlencoded")
end

--获取随机字符串
function getRandomString(len)
    local str = "1234567890abcdefhijklmnopqrstuvwxyz"
    local ret = ""
    for i = 1, len do
        local rchr = math.random(1, string.len(str))
        ret = ret .. string.sub(str, rchr, rchr)
    end
    return ret
end

--根据url显示图片
function image(url)
    local file = getRandomString(25)..".luatemp"
    apiHttpDownload(url,"data/image/"..file,5000)
    return "[CQ:image,file="..file.."]"
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
    self.imageData = apiGetBitmap(self.width,self.height)
    return o
end
--摆放文字
function img:setText(x,y,text,type1,size,r,g,b)
    self.imageData = apiPutText(self.imageData,x-1,y-1,text,
    type1 or "微软雅黑", size or 9, r or 0, g or 0, b or 0)
end
--摆放矩形
function img:setBlock(x,y,xx,yy,r,g,b)
    self.imageData = apiPutBlock(self.imageData,x-1,y-1,
    xx-1>self.width-1 and self.width-1 or xx-1,yy-1>self.height-1 and self.height-1 or yy-1,
    r,g,b)
end
--摆放其他图片
function img:setImg(x,y,path,xx,yy)
    self.imageData = apiSetImage(self.imageData,x-1,y-1,path,xx and xx-1 or 0, yy and yy-1 or 0)
end
--获取图片结果
function img:get()
    return "[CQ:image,file="..apiGetDir(self.imageData).."]"
end

--获取群成员信息
local oldcqGetMemberInfo = cqGetMemberInfo
cqGetMemberInfo = function (g,q,a)
    local r = {}
    if not a then a = false end
    return oldcqGetMemberInfo(r,g,q,a)
end
