function geturl()
    local html = httpPost("https://biubiubiubiubiubiubiubiubiubiubiubiubiubiubiubiubiubiubiubiubiu.com/str.php","content=qq.com")
    if not html then return "加载失败" end
    if html:find("网址过长，生成失败！") then return "网址过长，生成失败！" end
    local url = html:match("biubiubiubiubiu.com/(.-)\"")
    return "26个biu加上点com斜杠"..url
end

if message:find("长链接") == 1 and message:len() > 6 then
    print(at(fromqq).."生成结果如下~")
    print(geturl(message:sub(7):gsub("&","%26")))
end
