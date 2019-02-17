--请用自己的token
local token = "737aa093c7d9c16b7c6fdc1b70af2fb02bf01e11"
local function getInfo(id)
    local html = httpGet("http://api.waqi.info/feed/@"..tostring(id).."/","token="..token)
    local d,r,e = jsonDecode(html)
    if not r then return "加载失败" end
    return d.data.city.name.."的空气质量如下："..
    (d.data.aqi and "\r\n空气质量指数："..d.data.aqi or "")..
    (d.data.iaqi.pm25 and "\r\npm2.5："..d.data.iaqi.pm25.v or "")..
    (d.data.iaqi.pm10 and "\r\npm2.5："..d.data.iaqi.pm10.v or "")..
    (d.data.iaqi.co and "\r\npm2.5："..d.data.iaqi.co.v or "")..
    (d.data.iaqi.no2 and "\r\npm2.5："..d.data.iaqi.no2.v or "")..
    (d.data.iaqi.o3 and "\r\npm2.5："..d.data.iaqi.o3.v or "")..
    (d.data.iaqi.so2 and "\r\npm2.5："..d.data.iaqi.so2.v or "")..
    (d.data.attributions[1].name and "\r\n数据来源："..d.data.attributions[1].name or "")..
    (d.data.time.s and "\r\n数据更新时间："..d.data.time.s or "")
end

local function search(name)
    for i=1,name:len() do
        if name:byte(i) > 127 then return "城市名称不能用中文！" end
    end
    local html = httpGet("http://api.waqi.info/search/","keyword="..name.."&token="..token)
    local d,r,e = jsonDecode(html)
    if not r then return "加载失败" end
    local result = {}
    for i=1,#d.data do
        table.insert(result, d.data[i].uid.."："..d.data[i].station.name)
    end
    return "共找到"..tostring(#result).."个监测站："..
    "\r\n"..table.concat(result,"\r\n")..
    "\r\n使用指令“空气质量”加监测站编号查看数据"
end

if message == "空气质量" then
    print(at(qq))
    print("使用帮助：\r\n发送空气质量加城市英文(拼音)，即可查询\r\n如：空气质量harbin")
elseif message:find("空气质量") == 1 then
    message = message:gsub("空气质量 *","")
    if tonumber(message) then
        print(at(qq))
        print(getInfo(message))
    else
        print(at(qq))
        print(search(message))
    end
end
