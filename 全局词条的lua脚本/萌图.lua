function getPic()
    local html = httpGet("https://danbooru.donmai.us/posts?ms=1&page="..tostring(math.random(1,999)).."&tags=blush&utf8=%E2%9C%93")
    if not html or html:len() == 0 then return "加载失败" end
    local b,begin = html:find([[large%-file%-url=".-" ]],begin)
    local urls = {}
    while true do
        b,begin = html:find([[large%-file%-url=".-" ]],begin)
        if not b then break end
        local url = html:sub(b + string.len([[large-file-url="]]),begin-string.len([[" ]]))
        if url:sub(1,2) == "//" then url = "https:"..url end
        table.insert(urls, url)
    end
    if #urls == 0 then return "未找到匹配图片" end
    return image(urls[math.random(1,#urls)])
end

if message:len() > 6 then
    print(at(fromqq))
    print(getPic())
end
