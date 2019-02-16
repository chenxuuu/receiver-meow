--api key请用自己的
local key = ""

local function getPixivId(pic)
    local html = httpGet("https://saucenao.com/search.php?"..(key and "api_key="..key.."&" or "")..
        "db=999&output_type=2&numres=16&url="..pic:urlEncode())
    local t,r,_ = jsonDecode(html)
    if not r then return "查找失败" end
    if not t.results or #t.results==0 then return "未找到结果" end
    local result = ""
    for i=1,#t.results do
        if t.results[i].header.index_id == 5 and tonumber(t.results[i].header.similarity) > 90 then
            return (t.results[i].header.thumbnail and image(t.results[i].header.thumbnail) or "").."\r\n"..
            (t.results[i].data.title and t.results[i].data.title or "").."\r\n"..
            (t.results[i].data.pixiv_id and "p站id："..t.results[i].data.pixiv_id or "").."\r\n"..
            (t.results[i].data.member_name and "画师："..t.results[i].data.member_name or "").."\r\n"..
            (t.results[i].data.ext_urls[1] and t.results[i].data.ext_urls[1] or "")
        end
    end
    return "未找到结果"
end


local pic = getImageUrl(message)
if pic ~= "" then
    print(at(fromqq))
    print(getPixivId(pic))
end
