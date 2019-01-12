function getPic()
    local html = httpGet("https://konachan.com/post?page="..tostring(math.random(1,999)).."&tags=game_cg")
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

if message:len() > 6 then
    if fromgroup == "115872123" then
        print(at(fromqq))
        print(getPic())
    else
        print(at(fromqq).."该群（"..fromgroup.."）没有权限使用本指令")
    end
end
