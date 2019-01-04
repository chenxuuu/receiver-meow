expressNo = message:gsub("查快递","")
if message:find("查快递") == 1 then
    local html = httpGet("https://www.kuaidi100.com/autonumber/autoComNum", "text="..expressNo)
    local jsonC = JSON:decode(html)
    local comCode = jsonC.auto[1].comCode
    if comCode then
        result_msg = comCode.."\r\n"
        html = httpGet("https://www.kuaidi100.com/query", "type="..comCode.."&postid="..expressNo)
        local jo = JSON:decode(html)
        for i in pairs(jo.data) do
            result_msg = result_msg..jo.data[i].time.." "..jo.data[i].context.."\r\n"
        end
        if result_msg == comCode.."\r\n" then
            print("快递加载失败啦",comCode) else print(at(fromqq).."\r\n"..result_msg)
        end
    else
        print("没找到这个快递")
    end
end
