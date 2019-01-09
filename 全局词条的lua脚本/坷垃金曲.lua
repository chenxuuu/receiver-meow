function getSong(song)
    local id = tonumber(song)
    if not id then return at(fromqq).."请给正确编号哦" end
    if id < 1 or id > 71 then return at(fromqq).."编号不对哦，编号只能是1-71" end
    return "[CQ:record,file=CoolQ 语音时代！\\坷垃金曲\\"..string.format("%03d",id)..".mp3]"
end

if message:find("坷垃金曲") == 1 then
    print(getSong(message:gsub("坷垃金曲","")))
end
