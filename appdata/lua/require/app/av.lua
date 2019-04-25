--解析b站av号
function getInfo(av)
    av = av:match("av(%d+)")
    local html = apiHttpGet("http://api.bilibili.com/x/web-interface/view?aid="..av)
    if not html then return "查找失败" end
    local j,r,e = jsonDecode(html)
    if j.code ~= 0 then return "数据解析失败啦" end
    image = j.data.pic and image(j.data.pic)
    return (image and image.."\r\n" or "")..
    "av"..av..",标题："..j.data.title..
    "\r\n"..j.data.desc:gsub("<br/>","\r\n")..
    "\r\n分区："..(j.data.tname and j.data.tname or "")
end

return getInfo
