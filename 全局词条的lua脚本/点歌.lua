--message = "点歌种太阳"
local songID
if message:find("点歌") == 1 then
    if message:gsub("点歌",""):gsub("%d","") == "" then
        songID = message:gsub("点歌","")
    else
        local html = httpGet("https://c.y.qq.com/soso/fcgi-bin/client_search_cp", "?ct=24&qqmusic_ver=1298&new_json=1&remoteplace=txt.yqq.song&searchid=&t=0&aggr=1&cr=1&catZhida=1&lossless=0&flag_qc=0&p=1&n=20&w="..message:gsub("点歌",""):urlEncode())
        local str = html:match("callback%((.+)%)")
        local j,r,e = jsonDecode(str)
        if r then
            songID = j.data.song.list[1].id
        else
            print("机器人爆炸了，原因："..e)
        end
    end
    if songID then
        print("[CQ:music,type=qq,id="..songID.."]")
    else
        print(at(fromqq))
        print("机器人爆炸了，原因：根本没这首歌")
    end
end
