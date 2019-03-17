--提前运行的脚本
--用于提前声明某些要用到的函数

--加强随机数随机性
math.randomseed(tostring(os.time()):reverse():sub(1, 6))

--防止跑死循环，超时15秒自动结束
local start = os.time()
function trace (event, line)
    if os.time() - start >=15 then
        error("代码运行超时，超过15秒")
    end
end
debug.sethook(trace, "l")

--加上需要require的路径
package.path = package.path..
";./data/app/com.papapoi.ReceiverMeow/lua/require/?.lua"


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

--修正http接口可选参数
local oldapiHttpGet = apiHttpGet
apiHttpGet = function (url,para,timeout,cookie)
    return oldapiHttpGet(url,para or "",timeout or 5000,cookie or "")
end
local oldapiHttpPost = apiHttpPost
apiHttpPost = function (url,para,timeout,cookie)
    return oldapiHttpPost(url,para or "",timeout or 5000,cookie or "")
end

--加载字符串工具包
require("strings")

function getRandomString(len)
    local str = "1234567890abcdefhijklmnopqrstuvwxyz"
    local ret = ""
    for i = 1, len do
        local rchr = math.random(1, string.len(str))
        ret = ret .. string.sub(str, rchr, rchr)
    end
    return ret
end

function image(url)
    local file = getRandomString(25)..".luatemp"
    apiHttpDownload(url,"data/image/"..file,5000)
    return "[CQ:image,file="..file.."]"
end

