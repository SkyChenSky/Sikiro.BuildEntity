# 实体生成插件
基于visual studio sdk开发的方便、直观的实体生成工具。

## 更新历史
|时间|内容
|2019.11.6|修复批量更新与添加、更新后项目自动重新加载文件、mysql数据库的映射

## 支持版本
visual studio 2013、2015、2017


## 怎么使用

### 效果图
![img](https://github.com/SkyChenSky/AutoBuildEntity/blob/master/AutoBuildEntity/Resources/entity.gif "效果图")

### 安装

下载代码编译完成后，到bin目录下找到AutoBuildEntity.vsix双击安装

### 模板配置

配置文件命名约定为__entity.xml，可以参考源码目录下的文件《__entity.xml》。结构主要区分为数据库链接配置与实体模板配置，因为引入组件[NVelocity](https://github.com/castleproject/NVelocity/blob/master/docs/nvelocity.md)，如果需要对模板扩展可以查看具体文档。

### 实体生成
选中项目-右键-点击‘自动生成实体工具’-选择新增（更新、删除）数据源-确认。很简单、很直观


## 可能遇到的问题

### 无法调试
选中项目-右键-属性-调试

- 启动配置外部程序(按照你的VS版本选择)
```
C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe
```

- 命令行参数
```
/rootsuffix Exp
```

### 制作icon
http://iconfont.cn/search/index

http://www.easyicon.net/covert/

### visual studio sdk包下载
http://www.microsoft.com/en-us/download/details.aspx?id=40758fa43d42b-25b5-4a42-fe9b-1634f450f5ee=True

### 微软开发文档
https://docs.microsoft.com/zh-cn/dotnet/api/envdte.dte?redirectedfrom=MSDN&view=visualstudiosdk-2017

### 项目博客地址
http://www.cnblogs.com/skychen1218/p/6848128.html


## 标签
visual studio sdk vsix 插件 工具
