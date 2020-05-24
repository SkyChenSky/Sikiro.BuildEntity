# 实体生成插件
基于visual studio sdk开发的方便、直观的实体生成工具。

## 项目博客地址
http://www.cnblogs.com/skychen1218/p/6848128.html

## 更新历史
|时间|内容|
| ------- | ------| 
|2019.11.6|修复批量更新与添加、更新后项目自动重新加载文件、mysql数据库的映射|
|2020.1.8|通过配置文件区分mssql和mysql|
|2020.5.24|修复引用问题、全选的问题、2019兼容问题|

## 支持版本
visual studio 2013、2015、2017、2019


## 怎么使用

### 配置结构

```xml
<AutoEntity>
  <ConnString>
    <![CDATA[
   Server=im.gshichina.com;Port=5002;Database=person_platform;Uid=ge;Pwd=shi2019
    ]]>
  </ConnString>
  <Type>
    mysql
  </Type>
  <Template>
    <![CDATA[
/*
 * 本文件由根据实体插件自动生成，请勿更改
 * =========================== */

using System;
using Chloe.Annotations;
namespace $entity.ProjectName
{
    /// <summary>
    /// $entity.TableComment
    /// </summary>
    [Table("$entity.TableName")]
    public class $entity.ClassName
    {
#foreach($c in $entity.Columns)
        
        /// <summary>
#if($c.Remark != "")
        /// $c.Remark
#else
        /// $c.Name
#end
        /// </summary>
#if($c.IsIdentity)
        [AutoIncrement]
#elseif($c.CSharpType == "int")
        [NonAutoIncrementAttribute]
#end    
        [Column("$c.Name")]
        public $c.CSharpType  $c.PropertyName{ get; set; }
#end
    }
}
]]>
  </Template>
</AutoEntity>
```

|数据库名称|文件内使用|
| ------- | ------| 
|sql server|mssql|
|mysql|mysql|

### 效果图
![img](https://img2020.cnblogs.com/blog/488722/202005/488722-20200524221612186-697876586.gif "效果图")

### 安装

下载代码编译完成后，到bin目录下找到AutoBuildEntity.vsix双击安装

### 模板配置

配置文件命名约定为__entity.xml，可以参考源码目录下的文件《__entity.xml》。结构主要区分为数据库链接配置与实体模板配置，因为引入组件[NVelocity](https://github.com/castleproject/NVelocity/blob/master/docs/nvelocity.md)，如果需要对模板扩展可以查看具体文档。

### 实际使用
先把上面的配置放到项目-选中项目-右键-点击‘自动生成实体工具’-选择新增（更新、删除）数据源-确认。很简单、很直观


## 可能遇到的问题

### 查找VS的界面guid与cmdid
在VS的【扩展与更新】搜索并安装Extensibility Tools，然后在vs【视图】-【 Enable VSIP Logging】点击并重启后，就可以用ctrl+shirt+右键点击需要查的界面，就可以弹出需要的信息，我测试过vs2017可用。

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

### visual studio sdk包下载（如果你无法编译通过）
- 小于2015版本
   - https://marketplace.visualstudio.com/items?itemName=VisualStudioProductTeam.MicrosoftVisualStudio2013SDK
- 大于2015版本
   - 工具-获取工具和功能-勾选-Visual Studio 扩展开发


### 微软开发文档
https://docs.microsoft.com/zh-cn/dotnet/api/envdte.dte?redirectedfrom=MSDN&view=visualstudiosdk-2017


## 标签
visual studio sdk vsix 插件 工具
