# 插件对lua新增的接口列表

本文档尽量做到及时更新，实际请参考C#代码的实现

括号中的变量为C#代码声明时的变量，在lua中使用时只需要考虑一一对应关系即可，如`long`和`int`实际都为`number`。返回值也是如此。

---

## 工具类接口，基本均为自己写的接口，部分为head.lua中编写的接口

### apiGetPath()

- 获取程序运行目录
- 返回值：string

### 图片生成与处理接口

```lua
--img:new 新建图片对象，传入值为：宽度,高度
local pic = img:new(1000,1000)
--img:setBlock 绘制长方形，传入值为：起始x坐标,起始y坐标,结束x坐标,结束y坐标,颜色R值,颜色G值,颜色B值
pic:setBlock(1,1,1000,1000,255,0,255)
--img:setImg 附加图片，传入值为：起始x坐标,起始y坐标,图片绝对路径,[图片宽度,图片高度]
pic:setImg(1,1,[[D:\kuqpro\data\image\00EBED617380C9AE4D815333091E590E.png]])
--img:setText 附加文字，传入值为：起始x坐标,起始y坐标,文字内容,[字体名称,字体大小,颜色R值,颜色G值,颜色B值]
pic:setText(1,1,"测试",nil,25,0,255,255)
--img:get 获取图片结果
cqSendGroupMessage(group,pic:get())
```

### apiGetImageUrl(string message)

- 获取qq消息中图片的网址
- 返回值：string

### apiHttpDownload(string Url, string fileName, int timeout)

- 下载文件
- 返回值：bool下载结果

### apiHttpGet(url,para,timeout) 发起一个http get请求

- `url`网址，string
- `para`参数，string，默认空，可留空
- `timeout`网址，number，默认5000，可留空
- `cookie`cookie，string，默认空，可留空

举例：

```lua
cqSendGroupMessage(group,apiHttpGet("https://www.baidu.com"):sub(1,300))
apiHttpGet("https://www.abc.com","aa=123&bb=233&c="..string.urlEncode("中文参数"))
apiHttpGet("https://qq.com","aa=123&bb=233&c=",5000,"uid=xxxxx;aaa=vvvvv")
```

### apiHttpPost(url,para,timeout) 发起一个http post请求

请参考get，用法一样

### apiBase64File(string url)

- 获取在线文件的base64结果
- 返回值：string

### apiGetHardDiskFreeSpace()

- 获取指定驱动器的剩余空间总大小(单位为MB)
- 返回值：number

### apiTcpSend(string)

- 发送tcp广播数据

### string.urlEncode(string) 进行url编码

举例：

```lua
local zh,en = "中文","English"
cqSendGroupMessage(group,zh:urlEncode(), en:urlEncode())
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
    cqSendGroupMessage(group,t.a,t.b)
else
    cqSendGroupMessage(group,t,result,error)
end
```

### jsonEncode(table) json编码

参考上面用法

### image(url) 显示指定链接的图片

```lua
print(image("https://xxx.xxx.xxx/xxx.jpg"))
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

### apiSandBox(codeString)

在沙盒中安全地运行lua脚本

---

## 酷q接口

### cqCqCode_At(long qq)

- 获取酷Q "At某人" 代码
- 返回值：string

### cqCqCode_Emoji(int id)

- 获取酷Q "emoji表情" 代码
- 返回值：string

### cqCqCode_Face(int id)

- 获取酷Q "表情" 代码
- 返回值：string

### cqCqCode_Shake()

- 获取酷Q "窗口抖动" 代码
- 返回值：string

### cqCqCode_Trope(string str)

- 获取字符串的转义形式
- 返回值：string

### cqCqCode_UnTrope(string str)

- 获取字符串的非转义形式
- 返回值：string

### cqCqCode_ShareLink(string url, string title, string content, string imgUrl)

- 获取酷Q "链接分享" 代码
- 返回值：string

### cqCqCode_ShareCard(string cardType, long id)

- 获取酷Q "名片分享" 代码
- cardType：qq或group
- 返回值：string

### cqCqCode_ShareGPS(string site, string detail, double lat, double lon, int zoom)

- 获取酷Q "位置分享" 代码
- 返回值：string

### cqCqCode_Anonymous(bool forced)

- 获取酷Q "匿名" 代码
- 返回值：string

### cqCqCode_Image(string path)

- 获取酷Q "图片" 代码
- 返回值：string

### cqCqCode_Music(long id, string type, bool newStyle)

- 获取酷Q "音乐" 代码
- 返回值：string

### cqCqCode_MusciDIY(string url, string musicUrl, string title, string content, string imgUrl)

- 获取酷Q "音乐自定义" 代码
- 返回值：string

### cqCqCode_Record(string path)

- 获取酷Q "语音" 代码
- 返回值：string

### cqSendGroupMessage(long groupId, string message)

- 发送群消息
- 返回值：int

### cqSendPrivateMessage(long qqId, string message)

- 发送私聊消息
- 返回值：int

### cqSendDiscussMessage(long discussId, string message)

- 发送讨论组消息
- 返回值：int

### cqSendPraise(long qqId, int count)

- 发送赞
- 返回值：int

### cqRepealMessage(long id)

- 撤回消息
- 返回值：int

### cqGetLoginQQ()

- 取登录QQ
- 返回值：long

### cqGetLoginNick()

- 获取当前登录QQ的昵称
- 返回值：string

### cqGetAppDirectory()

- 取应用目录
- 返回值：string

### cqAddLoger(int level, string type, string content)

- 添加日志
- level取值：
  - 调试：Debug = 0
  - 信息：Info = 10
  - 信息_成功：Info_Success = 11
  - 信息_接收：Info_Receive = 12
  - 信息_发送：Info_Send = 13
  - 警告：Warning = 20
  - 错误：Error = 30
  - 严重错误：Fatal = 40
- 返回值：int

### cqAddFatalError(string msg)

- 添加致命错误提示
- 返回值：int

### cqSetGroupWholeBanSpeak(long groupId, bool isOpen)

- 置全群禁言
- 返回值：int

### cqSetFriendAddRequest(string tag,int respond,string msg)

- 置好友添加请求
- 返回值：int

### cqSetGroupAddRequest(string tag, int request, int respond, string msg)

- 置群添加请求
- 返回值：int

### cqSetGroupMemberNewCard(long groupId, long qqId, string newNick)

- 置群成员名片
- 返回值：int

### cqSetGroupManager(long groupId, long qqId, bool isCalcel)

- 置群管理员
- 返回值：int

### cqSetAnonymousStatus(long groupId, bool isOpen)

- 置群匿名设置
- 返回值：int

### cqSetGroupMemberRemove(long groupId, long qqId, bool notAccept)

- 置群员移除
- 返回值：int

### cqSetDiscussExit(long discussId)

- 置讨论组退出
- 返回值：int

---

## XML操作类接口，通常用于存取少量数据，不能当作大数据库使用

### apiXmlReplayGet(string group, string msg)

随机获取一条结果

### apiXmlListGet(string group, string msg)

获取所有回复的列表

### apiXmlDelete(string group, string msg)

删除所有匹配的条目

### apiXmlRemove(string group, string msg, string ans)

删除完全匹配的第一个条目

### apiXmlInsert(string group, string msg, string ans)

插入一个词条

### apiXmlSet(string group, string msg,string str)

更改某条的值

### apiXmlGet(string group, string msg)

获取某条的结果
