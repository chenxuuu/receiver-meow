# receiver-meow

[![MIT](https://img.shields.io/static/v1.svg?label=license&message=MIT&color=green)](https://github.com/chenxuuu/receiver-meow/blob/master/LICENSE)
[![Native.SDK](https://img.shields.io/badge/dependencies-Native.SDK-blueviolet.svg)](https://github.com/Jie2GG/Native.Csharp.Frame)
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

## lua接口列表

请见[api.md](api.md)

## 自行编写lua功能的教程（供初学者使用）

[接待喵lua插件教程](https://www.chenxublog.com/?s=%E6%8E%A5%E5%BE%85%E5%96%B5lua%E6%8F%92%E4%BB%B6%E6%95%99%E7%A8%8B&submit=%E6%90%9C%E7%B4%A2)

## 说明

lua代码全部在`appdata`文件夹中

所有lua接口可以参考`com.papapoi.ReceiverMeow\Native.Csharp\App\LuaEnv\LuaEnv.cs`文件中的函数定义

如想自己编译，请注意需要将[package压缩包](https://github.com/chenxuuu/receiver-meow/releases/download/v0.0/packages.7z)，下载并解压到vs项目的文件夹处

## 食用

实际使用时，只需要将cpk文件放入酷q的app文件夹，cpk文件可以去[这里下载](https://github.com/chenxuuu/receiver-meow/releases/latest)，或者自行编译（自行编译注意打开开发者模式，删掉生成后的lua53.dll）

然后，请**务必**将`appdata`文件夹内的所有文件夹与文件(目前只有`lua`与`xml`两个文件夹)，复制到酷q的`酷Q Air\data\app\com.papapoi.ReceiverMeow\`文件夹下，可以[点我下载](https://minhaskamal.github.io/DownGit/#/home?url=https://github.com/chenxuuu/receiver-meow/tree/master/appdata)

> 注意，如果提示找不到xxx.lua，请先检查文件夹是否正确，如果正确，请尝试保证所有路径都不包含中文和空格。

## 结尾

插件基于[Native.SDK](https://github.com/Jie2GG/Native.Csharp.Frame)
