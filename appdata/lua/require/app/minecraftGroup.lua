local codePath = [[C:\Users\chenx\Desktop\code.txt]]

--取一条激活码
local function pickCode()
    local file = io.open(codePath, "r")
    if not file then
        return "数据读取出错啦，请联系服主"
    end
    local all = file:read("*a"):split("\n")
    file:close()
    file = io.open(codePath, "w")
    if all and #all > 0 then
        local code = table.remove(all,1)
        file:write(table.concat(all, "\n"))
        file:close()
        return code
    end
    file:close()
    return "数据读取出错啦，请联系服主"
end

return function (msg,qq,group)
    if group == 241464054 then --玩家群
        local player = apiXmlGet("bindQq",tostring(qq))
        local step = apiXmlGet("bindStep",tostring(qq))
        if msg:find("绑定") == 1 and player == "" then--绑定命令
            local player = msg:match("([a-zA-Z0-9_]+)")
            if player then
                apiXmlSet("bindQq",tostring(qq),player)
                apiXmlSet("bindStep",tostring(qq),"waiting")
                cqSendGroupMessage(241464054,cqCode_At(qq).."绑定"..player.."成功！\r\n"..
                                    "请耐心等待管理员审核白名单申请哟~\r\n"..
                                    "如未申请请打开此链接：https://wj.qq.com/s/1308067/143c\r\n"..
                                    "如果过去24小时仍未被审核，请回复“催促审核”来进行催促")
                cqSendGroupMessage(567145439, "接待喵糖拌管理：\r\n玩家"..player.."\r\n已成功绑定"..
                                    "\r\n请及时检查该玩家是否已经提交白名单申请https://wj.qq.com/mine.html"..
                                    "\r\n如果符合要求，请回复“通过"..tostring(qq).."”来给予白名单"..
                                    "\r\n如果不符合要求，请回复“不通过"..tostring(qq).."空格原因”来给打回去重填")
            else
                cqSendGroupMessage(241464054,cqCode_At(qq).."id不符合要求，仅允许数字、字母、下划线")
            end
            return true
        elseif player == "" then--没绑定id
            cqSendGroupMessage(241464054,cqCode_At(qq).."你没有绑定游戏id，请发送“绑定”加上id，来绑定自己的id")
            return true
        elseif msg == "激活" then--激活
            if step == "pass" then
                cqSendGroupMessage(241464054,cqCode_At(qq).."已私聊发送激活码")
                cqSendPrivateMessage(qq,"获取权限，请在游戏内输入命令/giftCode use "..pickCode())
                apiXmlSet("bindStep",tostring(qq),"done")
            elseif step == "waiting" then
                cqSendGroupMessage(241464054,cqCode_At(qq).."你还没通过审核呢")
            elseif step == "done" then
                cqSendGroupMessage(241464054,cqCode_At(qq).."你已经领过激活码了")
            end
            return true
        elseif msg == "催促审核" and step == "waiting" then--催促审核
            cqSendGroupMessage(567145439, "接待喵糖拌管理：\r\n玩家"..player.."\r\n仅行了催促操作"..
                                "\r\n请及时检查该玩家是否已经提交白名单申请https://wj.qq.com/mine.html"..
                                "\r\n如果符合要求，请回复“通过"..tostring(qq).."”来给予白名单"..
                                "\r\n如果不符合要求，请回复“不通过"..tostring(qq).."原因”来给打回去重填")
            cqSendGroupMessage(241464054,cqCode_At(qq).."催促成功")
            return true
        end
    elseif group == 567145439 then --管理群
        if msg:find("删除 *%d+") == 1 then
            local qq = msg:match("删除 *(%d+)")
            local player = apiXmlGet("bindQq",tostring(qq))
            if player ~= "" then
                apiXmlDelete("bindStep",tostring(qq))
                apiXmlDelete("bindQq",tostring(qq))
                cqSendGroupMessage(567145439,"已删除玩家"..player.."的绑定信息")
            else
                cqSendGroupMessage(567145439,"没找到这个玩家")
            end
            return true
        elseif msg:find("通过 *%d+") == 1 then
            local qq = msg:match("通过 *(%d+)")
            local player = apiXmlGet("bindQq",tostring(qq))
            local step = apiXmlGet("bindStep",tostring(qq))
            if player == "" then
                cqSendGroupMessage(567145439,"该qq没有进行过绑定")
            elseif step ~= "waiting" then
                cqSendGroupMessage(567145439,"玩家"..player.."不在待审核名单中")
            else
                apiXmlSet("bindStep",tostring(qq),"pass")
                cqSendGroupMessage(567145439,"已通过"..player.."的白名单申请")
                cqSendGroupMessage(241464054,cqCode_At(qq).."你的白名单申请已经通过了哟~\r\n"..
                            "在群里发送“激活”即可获取激活账号的方法哦~\r\n"..
                            "你的id："..player)
            end
            return true
        elseif msg:find("不通过 *%d+ .+") == 1 then
            local qq,reason = msg:match("不通过 *(%d+) (.+)")
            local player = apiXmlGet("bindQq",tostring(qq))
            local step = apiXmlGet("bindStep",tostring(qq))
            if player == "" then
                cqSendGroupMessage(567145439,"该qq没有进行过绑定")
            elseif step ~= "waiting" then
                cqSendGroupMessage(567145439,"玩家"..player.."不在待审核名单中")
            else
                cqSendGroupMessage(567145439,"已打回"..player.."的白名单申请，原因："..reason)
                cqSendGroupMessage(241464054,cqCode_At(qq).."你的白名单申请并没有通过。\r\n"..
                            "原因："..reason.."\r\n"..
                            "请按照原因重新填写白名单：https://wj.qq.com/s/1308067/143c\r\n"..
                            "你的id："..player)
            end
            return true
        end
    end
end
