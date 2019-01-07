# 接待喵Lua脚本接口说明

欢迎各位大佬玩耍，有需求或者有建议都可以提交issue哦~

## lua功能指令（感叹号和冒号为中文符号）：

- lua 脚本，直接测试脚本，如：`lua print(2333)`
- ！luaadd 关键词：脚本，添加关键词触发的脚本，如`！luaadd 测试：print("发送者："..at(fromqq).."，群号："..fromgroup.."，消息内容："..message)`
- ！luadel 关键词，删除关键词触发的脚本
- ！lualist，查看脚本列表
- ！luasee 关键词，查看关键词触发的脚本内容

---

## 实例代码

```lua
--message = "点歌种太阳"
--message = "点歌123456"
local songID
if message:find("点歌") == 1 then
    if message:gsub("点歌",""):gsub("%d","") == "" then
        songID = message:gsub("点歌","")
    else
        local html = httpGet("http://s.music.163.com/search/get/", "type=1&s="..message:gsub("点歌",""):urlEncode())
        local jo = JSON:decode(html)
        if jo and jo.result and jo.result.songs and jo.result.songs[1] then
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
```

---

## 词条返回时可获取的参数

- `message`用户发的消息内容
- `fromqq`发送者qq，字符串型
- `fromgroup`发送者所在群号，字符串型

---

## 相关接口

### lua自带的大部分函数

- print
- math
- 等等等等...

### at(string) 返回at某人的字符串

举例：

```lua
print(at("123456"))
```

### httpGet(url,para,timeout) 发起一个http get请求

- `url`网址，string
- `para`参数，string，默认空，可留空
- `timeout`网址，number，默认5000，可留空

举例：

```lua
print(httpGet("https://www.baidu.com"):sub(1,300))
httpGet("https://www.abc.com","aa=123&bb=233&c="..string.urlEncode("中文参数"))
```

### httpPost(url,para,timeout) 发起一个http post请求

请参考get，用法一样

### string.urlEncode(string) 进行url编码

举例：

```lua
local zh,en = "中文","English"
print(zh:urlEncode(), en:urlEncode())
```

### jsonDecode(string) json解码

举例：

```lua
local j = [[{
    "a":123,
    "b":"ccc"
}]]
local t,result,error = jsonDecode(j)
if result then
    print(t.a,t.b)
else
    print(t,result,error)
end
```

### getData(fromqq,name) 读取指定名称的数据

```lua
getData(fromqq,"search_for_kydi")
```

### setData(fromqq,name,data) 存储指定名称的数据

注意！name名称必须大于十个字节长度，防止重名的情况发生

```lua
setData(fromqq,"search_for_kydi","12345678")
```

### string.fromHex(hex)

```lua
--- 将HEX字符串转成Lua字符串，如"313233616263"转为"123abc", 函数里加入了过滤分隔符，可以过滤掉大部分分隔符（可参见正则表达式中\s和\p的范围）。
-- @string hex,16进制组成的串
-- @return charstring,字符组成的串
-- @return len,输出字符串的长度
-- @usage
string.fromHex("010203")       -->  "\1\2\3"
string.fromHex("313233616263") -->  "123abc"
```

### string.toHex(str, separator)

```lua
--- 将Lua字符串转成HEX字符串，如"123abc"转为"313233616263"
-- @string str 输入字符串
-- @string[opt=""] separator 输出的16进制字符串分隔符
-- @return hexstring 16进制组成的串
-- @return len 输入的字符串长度
-- @usage
string.toHex("\1\2\3")     --> "010203" 3
string.toHex("123abc")     --> "313233616263" 6
string.toHex("123abc"," ") --> "31 32 33 61 62 63 " 6
```

### string.split(str, delimiter)

```lua
--- 按照指定分隔符分割字符串
-- @string str 输入字符串
-- @string delimiter 分隔符
-- @return 分割后的字符串列表
-- @usage
"123,456,789":split(',') --> {'123','456','789'}
```
