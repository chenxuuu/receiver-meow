--象棋

--存储数据规则：
--<对方qq号>,<发起人1/被发起人0>,<上一步下过1/上一步没下过0>,<棋盘每一行内容>

--显示棋盘内容，返回值为最终图片
--print(show(dataNow,{x=1,y=1},{x=2,y=1}))
function show(data,last,now)
    local xtable = {8,65,122,179,236,293,350,407,464}
    local ytable = {8,65,122,179,236,293,350,407,464,521}
    local pic = img:new(544,624)
    pic:setImg(1,1,"data/app/com.papapoi.ReceiverMeow/lua/require/app/chess/bg.png",544,674)
    for i=1,#data do
        for j=1,9 do
            if data[i]:sub(j,j)~="." then
                pic:setImg(xtable[j],ytable[i],"data/app/com.papapoi.ReceiverMeow/lua/require/app/chess/"..data[i]:sub(j,j)..".png",54,54)
            end
        end
    end
    if last then pic:setImg(xtable[last.x],ytable[last.y],"data/app/com.papapoi.ReceiverMeow/lua/require/app/chess/box.png",54,54) end
    if now then pic:setImg(xtable[now.x],ytable[now.y],"data/app/com.papapoi.ReceiverMeow/lua/require/app/chess/box.png",54,54) end
    return pic:get()
end

--初始化棋盘与双方的最初数据
function init(fromqq,anotherqq)
    if fromqq == anotherqq then
        return cqCode_At(tonumber(fromqq)).."无法与自己下棋"
    end
    local dataNow = {
    "abcdedcba",
    ".........",
    ".f.....f.",
    "g.g.g.g.g",
    ".........",
    ".........",
    "7.7.7.7.7",
    ".6.....6.",
    ".........",
    "123454321",}
    apiXmlSet("chess",fromqq,anotherqq..",0,1,"..table.concat(dataNow, "/"))
    apiXmlSet("chess",anotherqq,fromqq..",1,0,"..table.concat(dataNow, "/"))
    return show(dataNow).."\r\n"..
    ("已开局，红色为"..cqCode_At(tonumber(anotherqq)).."，黑色为"..cqCode_At(tonumber(fromqq))).."\r\n"..
    ("接下来请"..cqCode_At(tonumber(anotherqq)).."走子")
end

--显示当前棋盘
function showNow(fromqq)
    local str1 = apiXmlGet("chess",fromqq)
    --获取己方已保存数据
    if not str1:find(",") then return cqCode_At(tonumber(fromqq)).."你没有正在进行的棋局" end
    local t1 = str1:split(",")
    local str2 = apiXmlGet("chess",t1[1])
    --判断对方是否在棋局中
    if not str2:find(",") then return cqCode_At(tonumber(fromqq)).."对手已退出棋局，请重新开局" end
    local t2 = str2:split(",")
    --判断对方的对手qq是否相同，棋盘数据是否相同
    if t2[1] ~= fromqq or t1[4] ~= t2[4] then return cqCode_At(tonumber(fromqq)).."对手已退出棋局，请重新开局" end

    local pieces = t1[4]:split("/")
    return (show(pieces)).."\r\n"..
    (cqCode_At(tonumber(fromqq)).."棋盘状态如上").."\r\n"..
    ("接下来该"..cqCode_At(tonumber(t1[3] == "1" and t1[1] or fromqq)).."走子")
end

--走子处理
function play(moveData,fromqq)
    local str1 = apiXmlGet("chess",fromqq)
    --获取己方已保存数据
    if not str1:find(",") then return cqCode_At(tonumber(fromqq)).."你没有正在进行的棋局" end
    local t1 = str1:split(",")
    --判断是否轮到改人走子
    if t1[3] == "1" then return cqCode_At(tonumber(fromqq)).."还没有轮到你走子哦" end
    local str2 = apiXmlGet("chess",t1[1])
    --判断对方是否在棋局中
    if not str2:find(",") then return cqCode_At(tonumber(fromqq)).."对手已退出棋局，请重新开局" end
    local t2 = str2:split(",")
    --判断对方的对手qq是否相同，棋盘数据是否相同
    if t2[1] ~= fromqq or t1[4] ~= t2[4] then return cqCode_At(tonumber(fromqq)).."对手已退出棋局，请重新开局" end

    --整理前后位置数据
    moveData = moveData:upper()
    local movet = {A=1,B=2,C=3,D=4,E=5,F=6,G=7,H=8,I=9,J=10}--坐标转换表
    local lastx,nextx = moveData:match("(%d)%u(%d)")
    local lasty,nexty = moveData:match("(%u)%d(%u)")
    lastx,nextx = tonumber(lastx),tonumber(nextx)
    lasty = movet[lasty]
    nexty = movet[nexty]
    --判断走子是否合法
    if not (lastx and nexty and lasty and nextx and
            lastx > 0 and lastx < 10 and
            lasty > 0 and lasty < 11 and
            nextx > 0 and nextx < 10 and
            nexty > 0 and nexty < 11) then
        return cqCode_At(tonumber(fromqq)).."走子位置错误，请检查命令"
    end

    --棋盘当前状态的table
    local pieces = t1[4]:split("/")
    --获取某座标的棋子
    local function getPoint(x,y)
        return pieces[y]:sub(x,x)
    end
    --移动棋子到指定坐标，返回被吃的棋子名称
    local function movePoint(x,y,xx,yy)
        local pick = pieces[yy]:sub(xx,xx)
        pieces[yy] = pieces[yy]:sub(1,xx-1)..pieces[y]:sub(x,x)..pieces[yy]:sub(xx+1)
        pieces[y] = pieces[y]:sub(1,x-1).."."..pieces[y]:sub(x+1)
        return pick
    end
    local pick = getPoint(lastx,lasty)
    --检查选择的子是否合法
    if pick == "." then return cqCode_At(tonumber(fromqq)).."所选起始位置无棋子，请检查命令" end
    if (pick:find("%d") and t1[2] == "0") or (pick:find("%l") and t1[2] == "1") then
        return cqCode_At(tonumber(fromqq))..(t1[2] == "1" and "红" or "黑").."色的棋子才是你的哦，不要动别人的棋子"
    end
    --检查目标坐标是否为己方棋子
    local dest = getPoint(nextx,nexty)
    if (dest:find("%d") and t1[2] == "1") or (dest:find("%l") and t1[2] == "0") then
        return cqCode_At(tonumber(fromqq)).."你不能吃自己的棋子"
    end

    --检查走子是否合规
    local function max(a,b) return a>b and a or b end--取最大的
    local function min(a,b) return a<b and a or b end--取最小的
    local checkRule = {
        a = function(x,y,xx,yy)--车
                if x~=xx and y ~= yy and math.abs(x-xx)+math.abs(y-yy) < 1 then return false end--必须有一个坐标不变
                if math.abs(x-xx)+math.abs(y-yy) < 2 then return true end--移动一步，不用判断中途是否有阻挡
                if x==xx then for i=min(y,yy)+1,max(y,yy)-1 do--找路上是否有子阻挡
                    if getPoint(x,i) ~= "." then return false end
                end end
                if y==yy then for i=min(x,xx)+1,max(x,xx)-1 do
                    if getPoint(i,y) ~= "." then return false end
                end end
                return true
            end,
        b = function(x,y,xx,yy)--马
                if (x+2==xx and y+1==yy and getPoint(x+1,y)==".") or
                (x+1==xx and y+2==yy and getPoint(x,y+1)==".") or
                (x-1==xx and y+2==yy and getPoint(x,y+1)==".") or
                (x-2==xx and y+1==yy and getPoint(x-1,y)==".") or
                (x-2==xx and y-1==yy and getPoint(x-1,y)==".") or
                (x-1==xx and y-2==yy and getPoint(x,y-1)==".") or
                (x+1==xx and y-2==yy and getPoint(x,y-1)==".") or
                (x+2==xx and y-1==yy and getPoint(x+1,y)==".") then
                    return true
                else
                    return false
                end
            end,
        c = function(x,y,xx,yy)--象，黑方
                if yy > 5 then return false end--不能过界
                if (x+2==xx and y+2==yy and getPoint(x+1,y+1)==".") or
                (x-2==xx and y+2==yy and getPoint(x-1,y+1)==".") or
                (x-2==xx and y-2==yy and getPoint(x-1,y-1)==".") or
                (x+2==xx and y-2==yy and getPoint(x+1,y-1)==".") then
                    return true
                else
                    return false
                end
            end,
        ["3"] = function(x,y,xx,yy)--象，红方
                    if yy < 6 then return false end--不能过界
                    if (x+2==xx and y+2==yy and getPoint(x+1,y+1)==".") or
                    (x-2==xx and y+2==yy and getPoint(x-1,y+1)==".") or
                    (x-2==xx and y-2==yy and getPoint(x-1,y-1)==".") or
                    (x+2==xx and y-2==yy and getPoint(x+1,y-1)==".") then
                        return true
                    else
                        return false
                    end
                end,
        d = function(x,y,xx,yy)--士，黑方
                if xx<4 or xx>6 or yy>3 then return false end--不能过界
                if math.abs(x-xx)==1 and math.abs(y-yy)==1 then
                    return true
                else
                    return false
                end
            end,
        ["4"] = function(x,y,xx,yy)--士，红方
                    if xx<4 or xx>6 or yy<8 then return false end--不能过界
                    if math.abs(x-xx)==1 and math.abs(y-yy)==1 then
                        return true
                    else
                        return false
                    end
                end,
        e = function(x,y,xx,yy)--将，黑方
                if xx<4 or xx>6 or yy>3 then return false end--不能过界
                if math.abs(x-xx) + math.abs(y-yy) == 1 then--xy只有一个变化加一
                    return true
                else
                    return false
                end
            end,
        ["5"] = function(x,y,xx,yy)--帅，红方
                    if xx<4 or xx>6 or yy<8 then return false end--不能过界
                    if math.abs(x-xx) + math.abs(y-yy) == 1 then--xy只有一个变化加一
                        return true
                    else
                        return false
                    end
                end,
        f = function(x,y,xx,yy)--炮
                if x~=xx and y ~= yy and math.abs(x-xx)+math.abs(y-yy) < 1 then return false end--必须有一个坐标不变
                local count = 0
                if x==xx then for i=min(y,yy),max(y,yy) do--横着走
                    if getPoint(x,i) ~= "." then count = count + 1 end
                end end
                if y==yy then for i=min(x,xx),max(x,xx) do--竖着走
                    if getPoint(i,y) ~= "." then count = count + 1 end
                end end
                if (count == 3 and getPoint(xx,yy) ~= ".") or count == 1 then--中途无子或跳着吃子
                    return true
                else
                    return false
                end
            end,
        g = function(x,y,xx,yy)--卒，黑方
                if y < 6 then--在己方内
                    if math.abs(x-xx)==0 and yy-y==1 then return true else return false end
                else
                    if math.abs(x-xx)+math.abs(y-yy)==1 and yy-y>=0 then return true else return false end
                end
            end,
        ["7"] = function(x,y,xx,yy)--兵，红方
                    if y > 5 then--在己方内
                        if math.abs(x-xx)==0 and yy-y==-1 then return true else return false end
                    else
                        if math.abs(x-xx)+math.abs(y-yy)==1 and yy-y<=0 then return true else return false end
                    end
                end,
    }
    checkRule["1"] = checkRule.a
    checkRule["2"] = checkRule.b
    checkRule["6"] = checkRule.f
    --应用规则并阻止不合规的走子
    if not checkRule[getPoint(lastx,lasty)](lastx,lasty,nextx,nexty) then
        return cqCode_At(tonumber(fromqq)).."这枚棋子不能这么走"
    end

    local eat = movePoint(lastx,lasty,nextx,nexty)
    if eat=="e" or eat=="5" then
        apiXmlSet("chess",fromqq,"")
        apiXmlSet("chess",t1[1],"")
        return (show(pieces,{x=lastx,y=lasty},{x=nextx,y=nexty})).."\r\n"..
        (cqCode_At(tonumber(fromqq)).."赢了"..cqCode_At(tonumber(t1[1]))).."\r\n"..
        ("恭喜，本局结束")
    else
        local piecesStr = table.concat(pieces, "/")
        apiXmlSet("chess",fromqq,t1[1]..","..t1[2]..",1,"..piecesStr)
        apiXmlSet("chess",t1[1],t2[1]..","..t2[2]..",0,"..piecesStr)
        return (show(pieces,{x=lastx,y=lasty},{x=nextx,y=nexty})).."\r\n"..
        (cqCode_At(tonumber(fromqq)).."已走子").."\r\n"..
        ("接下来请"..cqCode_At(tonumber(t1[1])).."走子")
    end
end

return function (fromqq,message)
    local fromqq = tostring(fromqq)
    local message = tostring(message)
    if message:find("象棋") == 1 then
        message = message:gsub(" ","")
        if message == "象棋开局" then
            apiXmlSet("chess",fromqq,"wait")
            return (cqCode_At(tonumber(fromqq))).."\r\n"..
            ("已开启新的一局").."\r\n"..
            ("其他人使用命令：\r\n象棋加入"..fromqq.."\r\n可加入棋局")
        elseif message:find("象棋加入") == 1 then
            local anotherqq = message:gsub("象棋加入","")
            if apiXmlGet("chess",anotherqq) ~= "wait" then
                return (cqCode_At(tonumber(fromqq))).."\r\n"..
                ("未找到用户"..anotherqq.."开启的棋局")
            else
                return init(fromqq,anotherqq)
            end
        elseif message == "象棋退出" then
            apiXmlSet("chess",fromqq,"")
            return (cqCode_At(tonumber(fromqq))).."\r\n"..
            ("已退出所有棋局")
        elseif message:find("象棋走") == 1 and message:len() == string.len("象棋走A5B5") then
            return play(message:sub(string.len("象棋走")),fromqq)
        elseif message == "象棋棋盘" then
            return showNow(fromqq)
        else
            return [[象棋命令帮助[CQ:emoji,id=128161]
[CQ:emoji,id=128682]象棋开局 开启新一轮棋局
[CQ:emoji,id=128273]象棋加入 加 对方qq号 加入对方棋局
[CQ:emoji,id=128228]象棋退出 退出当前棋局
[CQ:emoji,id=128195]象棋棋盘 查看当前的棋盘状态
[CQ:emoji,id=128205]象棋走 加 前后坐标 移动棋子
如：象棋走A5B5
仅限同群对战]]
        end
    end
end





