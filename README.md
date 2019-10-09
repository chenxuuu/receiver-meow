# receiver-meow
[![All Contributors](https://img.shields.io/badge/all_contributors-2-orange.svg?style=flat-square)](#contributors)

[![appveyor](https://ci.appveyor.com/api/projects/status/46tmg2sh60l7kekf?svg=true)](https://ci.appveyor.com/project/chenxuuu/receiver-meow)
[![MIT](https://img.shields.io/static/v1.svg?label=license&message=MIT&color=green)](https://github.com/chenxuuu/receiver-meow/blob/master/LICENSE)
[![Native.SDK](https://img.shields.io/badge/dependencies-Native.SDK-blueviolet.svg)](https://github.com/Jie2GG/Native.Csharp.Frame)
[![NLua](https://img.shields.io/badge/dependencies-NLua-green.svg)](https://github.com/NLua/NLua/)
[![code-size](https://img.shields.io/github/languages/code-size/chenxuuu/receiver-meow.svg)](https://github.com/chenxuuu/receiver-meow/archive/master.zip)

能运行`lua`脚本的接待喵qq机器人，欢迎加入交流群`931546484`

## 功能特点

- 消息处理逻辑，完全由lua实现
- lua代码动态加载，更改完后即时生效
- 酷Q接口基本全部提供给了lua层，接口丰富
- 自带了http(s) post/get、2D图片处理、数据存储(xml)等接口
- 底层使用C#开发，.net framework 4.5版本

## 脚本已有功能

- 关键词回复及设置
- 今日运势
- 查快递
- 空气质量
- 点赞
- qq音乐点歌
- 根据截图查动画
- 根据图片查p站id
- 下象棋
- 抽奖禁言
- minecraft消息联通功能（需配合相应插件使用）
- 签到奖励禁言卡
- 图灵机器人接口功能

## lua接口列表

请见[api.md](api.md)

## 自行编写lua功能的教程（供初学者使用）

[接待喵lua插件教程](https://www.chenxublog.com/?s=%E6%8E%A5%E5%BE%85%E5%96%B5lua%E6%8F%92%E4%BB%B6%E6%95%99%E7%A8%8B&submit=%E6%90%9C%E7%B4%A2)

## 说明

lua代码全部在`appdata`文件夹中

所有lua接口可以参考`com.papapoi.ReceiverMeow\Native.Csharp\App\LuaEnv\LuaEnv.cs`文件中的函数定义

如想自己编译，请注意需要将[package压缩包](https://github.com/chenxuuu/receiver-meow/releases/download/v0.0/packages.zip)，下载并解压到vs项目的文件夹处

## 食用

实际使用时，只需要将cpk文件放入酷q的app文件夹，cpk文件可以去[这里下载](https://github.com/chenxuuu/receiver-meow/releases/latest)，或者自行编译（自行编译注意打开开发者模式，删掉生成后的lua53.dll）

然后，等待插件提示，自动下载必要的lua脚本文件，等待提示脚本下载完成，进行下一步

打开`酷Q Air\data\app\com.papapoi.ReceiverMeow\xml\settings.xml`更改设置，尤其是管理员qq号

打开酷q，启用即可

向机器人发送`帮助`或`help`加上页数，可以查看指令说明

## 结尾

插件基于[Native.SDK](https://github.com/Jie2GG/Native.Csharp.Frame)

## Contributors ✨

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore -->
<table>
  <tr>
    <td align="center"><a href="https://github.com/NAGATOYUKl"><img src="https://avatars3.githubusercontent.com/u/42117627?v=4" width="100px;" alt="一般通过吃瓜群众"/><br /><sub><b>一般通过吃瓜群众</b></sub></a><br /><a href="#maintenance-NAGATOYUKl" title="Maintenance">🚧</a> <a href="https://github.com/chenxuuu/receiver-meow/commits?author=NAGATOYUKl" title="Code">💻</a> <a href="#ideas-NAGATOYUKl" title="Ideas, Planning, & Feedback">🤔</a></td>
    <td align="center"><a href="https://github.com/morinoyuki"><img src="https://avatars1.githubusercontent.com/u/37149715?v=4" width="100px;" alt="morinoyuki"/><br /><sub><b>morinoyuki</b></sub></a><br /><a href="https://github.com/chenxuuu/receiver-meow/commits?author=morinoyuki" title="Code">💻</a> <a href="https://github.com/chenxuuu/receiver-meow/issues?q=author%3Amorinoyuki" title="Bug reports">🐛</a></td>
  </tr>
</table>

<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!