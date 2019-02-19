function getPic(s,ban)
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
    local url = urls[math.random(1,#urls)]
    return "[CQ:music,type=custom,url="..url..",audio=11,title=你要的图片,content=点击查看,image="..url.."]"
end

print(getPic("https://konachan.com/post?page="..tostring(math.random(1,170)).."&tags=order%3Ascore+rating%3Asafe+panties",true))
