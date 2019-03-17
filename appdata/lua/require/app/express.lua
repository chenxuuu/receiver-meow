--查快递
function search(expressNo,c,fromqq)
    local comCode = c
    if not comCode then
        if expressNo:len() == 0 then
            expressNo = apiXmlGet(tostring(fromqq),"express")
        else
            apiXmlSet(tostring(fromqq),"express",expressNo)
        end
        if expressNo:len() == 0 then
            return cqCode_At(fromqq).."你没有历史查询记录哦"
        end
        local html = apiHttpGet("https://www.kuaidi100.com/autonumber/autoComNum", "text="..expressNo)
        local jsonC,r = jsonDecode(html)
        if not r then return cqCode_At(fromqq).."数据加载失败" end
        comCode = jsonC.auto[1].comCode
    end
    if not comCode then
        return cqCode_At(fromqq).."没找到这个快递"
    end
    local result_msg = comCode.."\r\n"
    html = apiHttpGet("https://www.kuaidi100.com/query", "type="..comCode.."&postid="..expressNo)
    local jo,r = jsonDecode(html)
    if not r then return cqCode_At(fromqq).."数据加载失败" end
    for i in pairs(jo.data) do
        result_msg = result_msg..jo.data[i].time.." "..jo.data[i].context.."\r\n"
    end
    if result_msg == comCode.."\r\n" then
        return cqCode_At(fromqq).."快递加载失败啦",comCode
    else
        return result_msg..cqCode_At(fromqq).."\r\n下次查询该快递可直接发送“查快递”"
    end
end

return function (qq,message)
    local data = message:gsub("查快递 *","")
    local id = data:split(" ")
    return search(id[1],id[2],qq)
end
