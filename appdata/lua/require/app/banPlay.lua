--抽奖

return function (msg,qq,g)
    if not g then
        return "私聊抽你[CQ:emoji,id=128052]的奖呢"
    end
    local cards = apiXmlGet("banCard",tostring(qq))
    cards = cards == "" and 0 or tonumber(cards) or 0
    if msg == "抽奖" then
        -- if cqGetMemberInfo(g,cqGetLoginQQ()).PermitType == 1 or
        --     (cqGetMemberInfo(g,qq).PermitType ~= 1) then
        --     return cqCode_At(qq).."权限不足，抽奖功能无效"
        -- end
        if math.random() > 0.9 then
            local banTime = math.random(1,60)
            cqSetGroupBanSpeak(g,qq,banTime*60)
            return cqCode_At(qq).."恭喜你抽中了禁言"..tostring(banTime).."分钟"
        else
            local banCard = math.random(-5,6)
            cards = cards + banCard
            apiXmlSet("banCard",tostring(qq),tostring(cards))
            return cqCode_At(qq).."恭喜你抽中了"..tostring(banCard).."张禁言卡\r\n"..
                    "当前禁言卡数量："..tostring(cards)
        end
    elseif msg == "禁言卡" then

        return cqCode_At(qq).."当前禁言卡数量："..tostring(cards)
    elseif msg:find("%d+") then
        if cards <= 0 then
            return cqCode_At(qq).."你只有"..tostring(cards).."张禁言卡，无法操作"
        end
        apiXmlSet("banCard",tostring(qq),tostring(cards-1))
        local v = tonumber(msg:match("(%d+)"))
        local banTime = math.random(1,60)
        cqSetGroupBanSpeak(g,v,banTime*60)
        return cqCode_At(qq).."已将"..tostring(v).."禁言"..tostring(banTime).."分钟"
    else
        return "未匹配到任何命令"
    end
end

