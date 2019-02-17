# receiver-meow

[![Build Status](https://travis-ci.com/chenxuuu/receiver-meow.svg?branch=master)](https://travis-ci.com/chenxuuu/receiver-meow)

能运行`lua`脚本的接待喵qq机器人

lua接口文档[点我](https://github.com/chenxuuu/receiver-meow/blob/master/lua.md)查看

## 如何运行？

项目ci在每个版本号变化后，会自动发布一份新的打包文件，文件可以在[点我去下载页面](https://github.com/chenxuuu/receiver-meow/releases/latest)下载

下载`receiver-meow.7z`后，解压

打开`kuqpro_pack\data\settings.xml`，按照说明配置管理员qq等信息即可

打开`CQP.exe`即可运行（需要自己买授权码的）

## 如何更新？

把老版本data文件夹里的东西放到新版本里直接跑就行了

---

用C#实现的功能：

- 手动点赞
- 自定义指定关键字词，并进行回复
- 抽奖禁言功能
- 自助获取词条修改权限
- 复读群消息

用lua脚本实现的功能（需要自行手动添加）：

- 点金坷垃歌曲
- 查快递
- 点网易云的歌
- 哔哩哔哩av号查询
- 必应每日壁纸
- 对联
- 晚安报复社会
- 新番查询
- 开车（随机字符假磁链）
- 一言
- 全年龄色图
- 象棋
- 查询每日运势
- 搜p站图片功能
- 查询空气质量

---

如借用本项目，请自行将所有api key改成自己的

基于[酷Q](https://cqp.cc/)与[Newbe.Mahua](https://github.com/newbe36524/Newbe.Mahua.Framework/)框架，重新编写的接待喵qq机器人
