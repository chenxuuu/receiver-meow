--抽奖

return function (qq,g)
    if not g then
        return "私聊抽你[CQ:emoji,id=128052]的奖呢"
    end
    local banTime = math.random(1,60*24)
    cqSetGroupBanSpeak(g,qq,banTime*60)
    return cqCode_At(qq).."恭喜你抽中了禁言"..tostring(banTime).."分钟"
end

