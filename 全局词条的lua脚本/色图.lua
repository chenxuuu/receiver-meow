function getPic(s,max)
    local html = httpGet(s)
    if not html or html:len() == 0 then return "加载失败" end
    local b,begin = html:find([["directlink largeimg" href=".-">]],begin)
    local urls = {}
    while true do
        b,begin = html:find([["directlink largeimg" href=".-">]],begin)
        if not b then break end
        local url = html:sub(b + string.len([["directlink largeimg" href="]]),begin-string.len([[">]]))
        if url:sub(1,2) == "//" then url = "https:"..url end
        table.insert(urls, url)
    end
    if #urls == 0 then return "未找到匹配图片" end
    return image(urls[math.random(1,#urls)])
end


if fromgroup == "115872123" then
    print(at(fromqq))
    print(getPic("https://konachan.com/post?page="..tostring(math.random(1,200)).."&tags=order%3Ascore+rating%3Aexplicit"))
else
    print(at(fromqq).."本群仅显示全年龄图：")
    print(getPic("https://konachan.com/post?page="..tostring(math.random(1,200)).."&tags=order%3Ascore+rating%3Asafe"))
end
