--每分钟会调用一次这个文件，根据需求自行编辑~

local function sendMcCommand(cmd)
    httpGet("http://127.0.0.1:666/"..cmd:urlEncode())
end

local m = math.floor(os.time()/60)
local m5 = m%5

if m5==4 then
    sendMcCommand("function cleanalert")
elseif m5==0 then
    sendMcCommand("function clean")
end

sendMcCommand("function msg"..tostring(math.random(1,4)))
