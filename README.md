# receiver-meow

[![All Contributors](https://img.shields.io/badge/all_contributors-3-orange.svg?style=flat-square)](#contributors)
[![Build status](https://ci.appveyor.com/api/projects/status/46tmg2sh60l7kekf?svg=true)](https://ci.appveyor.com/project/chenxuuu/receiver-meow)
[![MIT](https://img.shields.io/static/v1.svg?label=license&message=MIT&color=green)](https://github.com/chenxuuu/receiver-meow/blob/master/LICENSE)
[![NLua](https://img.shields.io/badge/dependencies-NLua-green.svg)](https://github.com/NLua/NLua/)

能运行`lua`脚本的接待喵qq机器人，欢迎加入交流群`931546484`

## 功能

- 对接[go-cqhttp](https://github.com/Mrs4s/go-cqhttp)的websocket通讯协议
- 使用 .Net Core 5 开发，可跨平台
- 消息处理逻辑，完全由lua实现
- lua代码动态加载，重载虚拟机后，立即生效
- Lua层可直接调用C#层接口
- 自带了http(s) post/get、2D图片处理、数据存储(xml)等接口

## 下载

正式版：[GitHub Releases](https://github.com/chenxuuu/receiver-meow/releases)

快照版：[appveyor](https://ci.appveyor.com/project/chenxuuu/receiver-meow/build/artifacts)

## 默认脚本

自从插件的`V2.0.0`版本开始，默认脚本仓库与主仓库分离，Lua代码可在此仓库查看：[receiver-meow-lua](https://github.com/chenxuuu/receiver-meow-lua)

## lua接口

调用C#接口，请参考[Nlua](https://github.com/NLua/NLua/)关于`import`函数的使用说明

## Task架构介绍

主虚拟机由Task框架调度，具体的任务、定时器用法请见[LuaTask项目的Readme](https://github.com/chenxuuu/LuaTask-csharp)

每次收到新的消息上报，便会加到对应名称的Lua虚拟机中来处理，具体分配代码见[Events.cs](https://github.com/chenxuuu/receiver-meow/blob/Native.Csharp.Frame-4.0/ReceiverMeow/ReceiverMeow/App/Events.cs)

整个LuaTask管理，由[LuaStates.cs](https://github.com/chenxuuu/receiver-meow/blob/Native.Csharp.Frame-4.0/ReceiverMeow/ReceiverMeow/App/LuaEnv/LuaStates.cs)控制：

```log
                  LuaStates.cs文件的代码逻辑

           +-----------+                +--------------------+
New message|           | Name not exist |                    |
>>>>>>>>>>>+ lua pool  +--------------->+create new lua state|
           |           |                |                    |
           +----+------+                +-------+----------+-+
                |                               |          |
                |Name Exist                     |          |
                v                               |          |
       +--------+-------------------+           |          |
       | add new task to this state +<----------+          |
       +-------------+--------------+                      |
                     |                                     |
                     |                                     |
       +-------------+-------------+                       |
       |                           |   start run new state |
       |   task framework running  +<----------------------+
       |                           |
       +---------------------------+
```

## 食用

请等待工程重写完毕再使用。

~~向机器人发送`帮助`或`help`加上页数，可以查看指令说明~~

脚本的功能解释请见[Lua脚本项目的Readme](https://github.com/chenxuuu/receiver-meow-lua)

## 结尾

本项目基于MIT协议

## Contributors ✨

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><a href="https://github.com/NAGATOYUKl"><img src="https://avatars3.githubusercontent.com/u/42117627?v=4" width="100px;" alt=""/><br /><sub><b>一般通过吃瓜群众</b></sub></a><br /><a href="#maintenance-NAGATOYUKl" title="Maintenance">🚧</a> <a href="https://github.com/chenxuuu/receiver-meow/commits?author=NAGATOYUKl" title="Code">💻</a> <a href="#ideas-NAGATOYUKl" title="Ideas, Planning, & Feedback">🤔</a></td>
    <td align="center"><a href="https://github.com/littlecxm"><img src="https://avatars0.githubusercontent.com/u/16154023?v=4" width="100px;" alt=""/><br /><sub><b>CXM</b></sub></a><br /><a href="https://github.com/chenxuuu/receiver-meow/commits?author=littlecxm" title="Code">💻</a> <a href="https://github.com/chenxuuu/receiver-meow/issues?q=author%3Alittlecxm" title="Bug reports">🐛</a></td>
    <td align="center"><a href="https://github.com/morinoyuki"><img src="https://avatars1.githubusercontent.com/u/37149715?v=4" width="100px;" alt=""/><br /><sub><b>morinoyuki</b></sub></a><br /><a href="https://github.com/chenxuuu/receiver-meow/commits?author=morinoyuki" title="Code">💻</a> <a href="https://github.com/chenxuuu/receiver-meow/issues?q=author%3Amorinoyuki" title="Bug reports">🐛</a></td>
    <td align="center"><a href="https://github.com/gy39830"><img src="https://avatars1.githubusercontent.com/u/60922309?v=4" width="100px;" alt=""/><br /><sub><b>gy39830</b></sub></a><br /><a href="https://github.com/chenxuuu/receiver-meow/commits?author=gy39830" title="Code">💻</a> <a href="https://github.com/chenxuuu/receiver-meow/issues?q=author%3Agy39830" title="Bug reports">🐛</a></td>
  </tr>
</table>

<!-- markdownlint-enable -->
<!-- prettier-ignore-end -->
<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!
