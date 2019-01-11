local function getResult(s)
    local str = httpGet("https://ai-backend.binwang.me/chat/couplet/"..string.urlEncode(s))
    if not str or str:len() == 0 then return "加载失败" end
    local t,r = jsonDecode(str)
    if r then
        return t.output
    else
        return "加载失败"
    end
end

local l = message:find("对联")
if l <= 4 and message:len() > 6 then
    print(at(fromqq))
    print("上联："..message:sub(l+6))
    print("下联："..getResult(message:sub(l+6)))
end
