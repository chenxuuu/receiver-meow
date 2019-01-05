local say = httpGet("http://v1.hitokoto.cn")

local types = {
a = "Anime - 动画",
b = "Comic – 漫画",
c = "Game – 游戏",
d = "Novel – 小说",
e = "Myself – 原创",
f = "Internet – 来自网络",
g = "Other – 其他",
}

function getText(s)
    if not s then return end

    local data = JSON:decode(s)
    if not s then return end

    local hitokoto,saytype,from
    if not pcall(function ()
        hitokoto,saytype,from = data.hitokoto, data.type, data.from
    end) then
        return
    end

    return hitokoto.."\r\n--"..from.."\r\n"..types[saytype]
end

print(getText(say) or at(fromqq).."\r\n加载失败啦")
