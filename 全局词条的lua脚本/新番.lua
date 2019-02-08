--来源：从pandownload那边偷来的2333
--稍微修改了一下就能用了
--感谢pandownload作者

--初始化新番列表
function onInitAnime()
    return parse(get("http://www.dilidili.name/"))
end

--列表项点击事件
function onItemClick(item)
    local act = ACT_SHARELINK
    local arg = ""
    local _, _, li = string.find(get(item.url), "<li class=\"list_xz\">(.-)<li>")
    if li then
        _, _, arg, pwd = string.find(li, "(https?://pan.baidu.com/s/[A-Za-z0-9-_]+).->(.-)</a>")
        if arg then
            _, _, pwd = string.find(pwd, "(%w%w%w%w)")
            if pwd then
                arg = arg .. " " .. pwd
            end
        end
    end
    if arg == nil or #arg == 0 then
        act = ACT_ERROR
        arg = "获取链接失败"
    end
    return act, arg
end

function get(url)
    return httpGet(url)
end

function parse(data)
    local anime_week = {}
    local week = {"星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期日"}
    local sep = {"<!%-%- 星期一开始 %-%->", "<!%-%- 星期二开始 %-%->", "<!%-%- 星期三开始 %-%->", "<!%-%- 星期四开始 %-%->", "<!%-%- 星期五开始 %-%->", "<!%-%- 星期六开始 %-%->", "<!%-%- 星期日开始 %-%->", "<!%-%- seven end %-%->"}
    for i = 1, 7 do
        local _, _, tmp = string.find(data, sep[i] .. "(.-)" .. sep[i+1])
        local begin = 1
        local anime_day = {["title"] = week[i]}
        while tmp do
            local _, b, url, img, name = string.find(tmp, "<a href=\"(.-)\".-<img src=\"(.-)\".-<p>(.-)</p>", begin)
            if url == nil then
                break
            end
            if #url > 0 and string.byte(url) == 47 then
                url = "http://www.dilidili.wang" .. url
            end
            if #img > 0 and string.byte(img) == 47 then
                img = "http://www.dilidili.wang" .. img
            end
            table.insert(anime_day, {["url"] = url, ["name"] = name, ["image"] = img, ["icon_size"] = "55,55"})
            begin = b + 1
        end
        table.insert(anime_week, anime_day)
    end
    return anime_week
end

--列取番剧列表
function reply(s,day,d)
    local t = onInitAnime()
    for i,j in pairs(t[day]) do
        if type(j) == "table" then
            print(j.name)
            if d then
                local _,url = onItemClick(j)
                print(url)
            end
        else
            print(j.."（发送新番加星期阿拉伯数字可查询当日新番哦）")
            if not d then
                print("加上“下载”可用获取下载链接哦")
            end
        end
    end
end

--求出今天是星期几
local day_now = os.date("*t", os.time()).wday
day_now = day_now == 1 and 7 or day_now - 1

--处理消息
if message:find("新番") == 1 then
    local dl = message:find("下载") and message:find("下载") > 0--是否需要下载
    message = message:gsub("新番",""):gsub("下载","")
    local extra = tonumber(message)
    if not extra then
        reply(s,day_now,dl)--查今天的
    else
        reply(s,extra,dl)--查指定的
    end
end

