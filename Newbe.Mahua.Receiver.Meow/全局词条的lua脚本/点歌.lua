message = "点歌种太阳"
local songID
if message:find("点歌") == 1 then
    if message:gsub("点歌",""):gsub("%d","") == "" then
        songID = message:gsub("点歌","")
    else
        local html = httpGet("http://s.music.163.com/search/get/", "type=1&s="..message:gsub("点歌",""):urlEncode())
        local jo = JSON:decode(html)
        if jo and jo.result then
            songID = jo.result.songs[1].id
        end
    end
    if songID then
        print("[CQ:music,type=163,id="..songID.."]")
    else
        print(at(fromqq))
        print("机器人爆炸了，原因：根本没这首歌")
    end
end
