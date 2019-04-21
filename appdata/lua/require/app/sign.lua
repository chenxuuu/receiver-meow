--签到

return function (qq)
    local day = math.floor(os.time()/(3600*24))--取整，今天
    local signData = apiXmlGet("sign",tostring(qq))
    local data = signData == "" and
    {
        last = 0,--上次签到时间戳
        count = 0,--连签计数
    } or jsonDecode(signData)

    if data.last == day then
        return "你已经签过到啦"
    end

    if data.last == day - 1 then
        data.count = data.count + 1
    else
        data.count = 1
    end
    data.last = day
    local j = jsonEncode(data)
    apiXmlSet("sign",tostring(qq),j)

    local cards = apiXmlGet("banCard",tostring(qq))
    cards = cards == "" and 0 or tonumber(cards) or 0
    local banCard = math.random(1,10)
    cards = cards + banCard
    apiXmlSet("banCard",tostring(qq),tostring(cards+data.count-1))
    return "签到成功\r\n"..
    "抽中了"..tostring(banCard).."张禁言卡\r\n"..
    "附赠"..tostring(data.count-1).."张连签奖励\r\n"..
    "当前禁言卡数量："..tostring(cards)
end



