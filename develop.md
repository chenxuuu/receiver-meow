# 综述

软件会启动多个虚拟机处理数据，一旦相应的虚拟机新建完毕，除非手动重载，不会再重启改虚拟机：

|虚拟机名称|启动条件|会处理的事件|
|--|--|--|
|main|软件启动、虚拟机重载后|处理启动事件、好友请求、群邀请等无法归属的事件|
|private|私聊|处理所有私聊消息|
|各个群号|群消息等|处理所有与群相关的消息、事件|

所有lua虚拟机皆由LuaTask框架进行管理，各自均为单线程运行。分为多个虚拟机是为了提高效率，防止群数量太多，互相阻塞。

每个虚拟机生成后，都会直接运行`main.lua`。脚本需要在其中注册好需要的事件、定时器任务。

# 触发事件

触发接口见[LuaTask-csharp](https://github.com/chenxuuu/LuaTask-csharp)

基本代码如下：

```lua
--lua里注册触发应对事件
sys.tiggerRegister('事件',function(data)
    print('触发',data.xxxxx)
end)
```

```csharp
//C#里触发事件
LuaEnv.LuaStates.Run("虚拟机名称", "事件", 触发数据);
```

所有事件均可在[Ws.cs的Events函数](https://github.com/chenxuuu/receiver-meow/blob/master/ReceiverMeow/ReceiverMeow/GoHttp/Ws.cs#L90)找到，并且可以轻松地看出传入lua的数据内容

http的原始接口文档可在[onebot-事件](https://github.com/howmanybots/onebot/blob/master/v11/specs/event/README.md)查看

http返回的`原视json数据`将**原封不动**地存放在传给lua数据的`raw`键下，方便按需调用

# 主动接口

该部分直接由lua脚本实现

lua脚本构造请求数据，直接通过[Http.cs](https://github.com/chenxuuu/receiver-meow/blob/master/ReceiverMeow/ReceiverMeow/GoHttp/Http.cs)来与http接口进行交互。

http接口文档见[onebot-公开API](https://github.com/howmanybots/onebot/blob/master/v11/specs/api/public.md)

该部分可翻阅lua脚本，全部逻辑均由lua实现。

# CQ码

见[onebot-消息段类型](https://github.com/howmanybots/onebot/blob/master/v11/specs/message/segment.md)

# 调用C#接口

请参考[Nlua](https://github.com/NLua/NLua/)关于`import`函数的使用说明

上面描述的`主动接口`基本都是调用C#接口进行发送。如果还看不懂调用方法，请参考现有Lua脚本依葫芦画瓢。
