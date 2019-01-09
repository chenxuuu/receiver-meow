function getPicture()
    local html = httpGet("https://www.meishichina.com/")
    if not html or html:len() == 0 then return "" end
    local b,begin = html:find([[data%-src=".-x%-oss%-process=style/c320]],begin)
    local urls = {}
    while true do
        b,begin = html:find([[data%-src=".-x%-oss%-process=style/c320]],begin)
        if not b then break end
        local url = html:sub(b + string.len([[data-src="]]),begin)
        if url:sub(1,2) == "//" then url = "https:"..url end
        table.insert(urls, url)
    end
    if #urls == 0 then return "" end
    return at(fromqq).."晚安~\r\n"..image(urls[math.random(1,#urls)])
end

print(getPicture())
