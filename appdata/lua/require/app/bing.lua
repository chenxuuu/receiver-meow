--必应美图

function getImage()
    local s = apiHttpGet("http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=10")
    local t,result,error = jsonDecode(s)
    if result then
        if t.images and t.images[1] and t.images[1].url then
            return image("http://www.bing.com"..t.images[1].url).."\r\n"..t.images[1].copyright
        else
            return "图片加载失败啦"
        end
    else
        return "数据解析失败啦，错误原因："..error
    end
end

return getImage


