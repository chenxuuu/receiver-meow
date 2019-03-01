--提前运行的脚本
--用于提前声明某些要用到的函数

--加强随机数随机性
math.randomseed(tostring(os.time()):reverse():sub(1, 6))

--防止跑死循环，超时15秒自动结束
local start = os.time()
function trace (event, line)
    if os.time() - start >=15 then
        error("运行超时")
    end
end
debug.sethook(trace, "l")
