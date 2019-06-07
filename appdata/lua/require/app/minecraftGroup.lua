local oldapiTcpSend = apiTcpSend
apiTcpSend = function (msg,cmd)
    if cmd then oldapiTcpSend("cmd"..msg) return end
    msg = msg:gsub("%[CQ:.-%]","[特殊]"):gsub("\r","")
    oldapiTcpSend(msg)
end

return function (msg,qq,group)
    if msg:find("命令") == 1 and qq == 961726194 then
        local cmd = msg:sub(("命令"):len()+1)
        apiTcpSend(cmd,true)
        cqSendGroupMessage(group,cqCode_At(qq).."命令"..cmd.."已执行")
        return true
    elseif group == 241464054 then --玩家群
        local player = apiXmlGet("bindQq",tostring(qq))
        local step = apiXmlGet("bindStep",tostring(qq))
        apiTcpSend("[群消息]["..(player == "" and "无名氏"..tostring(qq) or player).."]"..msg)
        local oldcqSendGroupMessage = cqSendGroupMessage
        cqSendGroupMessage = function (g,m)
            if g==241464054 then
                oldcqSendGroupMessage(g,m)
                apiTcpSend("[群消息][接待喵]"..m)
            else
                oldcqSendGroupMessage(g,m)
            end
        end
        if msg:find("绑定") == 1 and player == "" then--绑定命令
            local player = msg:match("([a-zA-Z0-9_]+)")
            player = apiXmlRow("bindQq",player) ~= "" and "" or player
            if player == "" then
                cqSendGroupMessage(241464054,cqCode_At(qq).."id重复，换个吧")
            elseif player then
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
        elseif msg:find("查询.+") == 1 or msg == "查询" then--查询某玩家在线信息
            local p = msg == "查询" and player or msg:match("查询(%w+)")
            local onlineData = apiXmlGet("minecraftData",p)
            if onlineData == "" then
                local tempqq = msg:match("%d+")
                if tempqq then
                    p = apiXmlGet("bindQq",tostring(tempqq))
                    onlineData = apiXmlGet("minecraftData",p)
                end
                if onlineData == "" then
                    cqSendGroupMessage(241464054,cqCode_At(qq).."未查询到该玩家信息")
                    return true
                end
            end
            local data = jsonDecode(onlineData)
            if data.last == "online" then
                data.time = data.time + os.time() - data.ltime
            end
            cqSendGroupMessage(241464054,cqCode_At(qq)..
                p.."\r\n"..
                "当前状态："..(data.last == "online" and "在线" or "离线").."\r\n"..
                "累计在线："..string.format("%d小时%d分钟", math.floor(data.time/(60*60)), math.floor(data.time/60)%60)..
                (data.last == "online" and "" or "\r\n上次在线时间："..os.date("%Y年%m月%d日",data.ltime)))
            return true
        elseif msg == "在线" then
            local onlineData = apiXmlGet("minecraftData","[online]")
            local online = {}--存储在线所有人id
            if onlineData ~= "" then
                online = onlineData:split(",")
            end
            cqSendGroupMessage(241464054,cqCode_At(qq).."当前在线人数"..tostring(#online).."人："..
                                (onlineData=="" and "" or "\r\n"..onlineData))
            return true
        elseif msg == "激活" then--激活
            if step == "pass" or step == "done" then
                local onlineData = apiXmlGet("minecraftData",player)
                local data = onlineData == "" and
                {
                    time = 0,
                    last = "offline",
                    ltime = os.time(),
                } or jsonDecode(onlineData)
                if data.last == "offline" then
                    cqSendGroupMessage(241464054,cqCode_At(qq).."请上线后再操作")
                else
                    cqSendGroupMessage(241464054,cqCode_At(qq).."已给予玩家"..player.."权限")
                    apiTcpSend("lp user "..player.." permission set group.whitelist",true)
                    apiTcpSend("lp user "..player.." permission unset group.default",true)
                    if step == "pass" then
                        apiXmlSet("bindStep",tostring(qq),"done")
                    end
                end
            elseif step == "waiting" then
                cqSendGroupMessage(241464054,cqCode_At(qq).."你还没通过审核呢")
            end
            return true
        elseif msg == "催促审核" and step == "waiting" then--催促审核
            cqSendGroupMessage(567145439, "接待喵糖拌管理：\r\n玩家"..player.."\r\n仅行了催促操作"..
                                "\r\n请及时检查该玩家是否已经提交白名单申请https://wj.qq.com/mine.html"..
                                "\r\n如果符合要求，请回复“通过"..tostring(qq).."”来给予白名单"..
                                "\r\n如果不符合要求，请回复“不通过"..tostring(qq).."原因”来给打回去重填")
            cqSendGroupMessage(241464054,cqCode_At(qq).."催促成功")
            return true
        elseif msg:find("重置密码") == 1 and (step == "pass" or step == "done") then
            local password = getRandomString(6)
            apiTcpSend("flexiblelogin resetpw "..player.." "..password,true)
            cqSendGroupMessage(241464054,cqCode_At(qq).."已重置，请看私聊")
            cqSendPrivateMessage(qq,"密码重置成功，初始密码为："..password.."\r\n"..
            "请在登陆后使用命令/changepassword [密码] [确认密码]来修改密码")
        end
    elseif group == 567145439 then --管理群
        if msg:find("删除 *%d+") == 1 then
            local qq = msg:match("删除 *(%d+)")
            local player = apiXmlGet("bindQq",tostring(qq))
            if player ~= "" then
                apiXmlDelete("bindStep",tostring(qq))
                apiXmlDelete("bindQq",tostring(qq))
                apiTcpSend("lp user "..player.." permission set group.default",true)
                apiTcpSend("lp user "..player.." permission unset group.whitelist",true)
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
                cqSendGroupMessage(241464054,cqCode_At(tonumber(qq)).."你的白名单申请已经通过了哟~\r\n"..
                            "游戏上线后，在群里发送“激活”即可获取权限~\r\n"..
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
                cqSendGroupMessage(241464054,cqCode_At(tonumber(qq)).."你的白名单申请并没有通过。\r\n"..
                            "原因："..reason.."\r\n"..
                            "请按照原因重新填写白名单：https://wj.qq.com/s/1308067/143c\r\n"..
                            "你的id："..player)
            end
            return true
        elseif msg == "清空在线" then
            apiXmlSet("minecraftData","[online]","")
            cqSendGroupMessage(567145439,cqCode_At(qq).."已清空所有在线信息")
            return true
        end
    end
end
