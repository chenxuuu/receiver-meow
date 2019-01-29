function search(expressNo,c)
    local comCode = c
    if not comCode then
        if expressNo:len() == 0 then
            expressNo = getData(fromqq,"search_for_kydi")
        else
            setData(fromqq,"search_for_kydi",expressNo)
        end
        if expressNo:len() == 0 then
            return at(fromqq).."你没有历史查询记录哦"
        end
        local html = httpGet("https://www.kuaidi100.com/autonumber/autoComNum", "text="..expressNo)
        if not html then
            return at(fromqq).."数据加载失败"
        end
        local jsonC = JSON:decode(html)
        comCode = jsonC.auto[1].comCode
    end
    if not comCode then
        return at(fromqq).."没找到这个快递"
    end
    result_msg = comCode.."\r\n"
    html = httpGet("https://www.kuaidi100.com/query", "type="..comCode.."&postid="..expressNo)
    local jo = JSON:decode(html)
    for i in pairs(jo.data) do
        result_msg = result_msg..jo.data[i].time.." "..jo.data[i].context.."\r\n"
    end
    if result_msg == comCode.."\r\n" then
        return at(fromqq).."快递加载失败啦",comCode
    else
        return result_msg..at(fromqq).."\r\n下次查询该快递可直接发送“查快递”"
    end
end

if message:find("查快递") == 1 then
    local data = message:gsub("查快递 *","")
    local id = data:split(" ")
    print(search(id[1],id[2]))
end
