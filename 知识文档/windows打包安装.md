# windows 打包安装

## 前期准备

### 设置项目图标

> * 图标必须为 .icon 格式
> * 为保证图标可加载，注意把图标放在项目以内。

* 打开 Visual Studio。

* 在“解决方案资源管理器”中，右键点击项目，然后选择“属性”。

* 在项目属性窗口中，找到“应用程序”选项卡。

* 在“图标和清单”部分，你会看到“图标”选项，点击浏览按钮选择你的 `.ico` 文件。

### 下载第三方工具

https://jrsoftware.org/isinfo.php

下载最新的即可

## 如何打包

1. 打开 inno setup安装包制作工具

![](https://img-blog.csdnimg.cn/img_convert/3879631712624a55f55415762c3fb75c.png)

![](https://img-blog.csdnimg.cn/img_convert/a7dd116e3b3ec2dd81fd83e3b7a7d586.png)

2. 编写有关应用信息。

> 注意：最好用英文。因为 Inno Setup 本身不支持中文。如果用中文，之后用户安装的时候会显示乱码

![](c:\软件会在这里吗\360极速浏览器\360chrome\chrome\User Data\temp\b2b9d3d9d3359b6fdc6853a2a523d50f_5e754361e65d76d2c1282b321bba50cf.png)

> 之后安装好后会在这里显示：
>
> ![](https://img-blog.csdnimg.cn/img_convert/db04b56f687506c240b799e7cbe991b8.png)

3. 配置默认安装路径有关

![](https://img-blog.csdnimg.cn/img_convert/fcb37d09a5c90999c5c3db4606780bdf.png)

4. 选择主启动程序（我们的就是 Server）

![](https://img-blog.csdnimg.cn/img_convert/8b01aee0b0a7bbd5c46576539f725c85.png)

5. 选择程序所需要的所有文件或文件夹

![](https://img-blog.csdnimg.cn/img_convert/10b70a8204ee46ddcc3dac2ce5b9ab61.png)

6. 一些信息编写

![](https://img-blog.csdnimg.cn/img_convert/e284746562d22ecdf85b0ea6b7b46a4b.png)

7. 一些配置选项

![](https://img-blog.csdnimg.cn/img_convert/49e35942e6b4591649b35af723525e8b.png)

8. 添加版权信息文件

![](https://img-blog.csdnimg.cn/img_convert/fc31207d786ab81a5fc0f6e4936e5747.png)

9. 系统用户使用权限等

![](https://img-blog.csdnimg.cn/img_convert/3a50c293b22252996cbb7bc8213345dc.png)

10. 选择语言（English？）
11. 之后一直点 Next 直到 Finish
12. 生成脚本代码，编译脚本

![](https://img-blog.csdnimg.cn/img_convert/dc334622ba7762ea153e2499f1ba9009.png)

13. 选择保存脚本代码

![](https://img-blog.csdnimg.cn/img_convert/dd8c4f7465765cc0426db1f76cf1f730.png)

14. 保存到个人指定的位置

![](https://img-blog.csdnimg.cn/img_convert/7ccf4ca1548beaf100bd812d24095989.png)

15. 编译成功

![](https://img-blog.csdnimg.cn/img_convert/726395c7415f0efaa5582d95ca55832f.png)

16. 看到编译成功以后，生成的安装包文件。

![](https://img-blog.csdnimg.cn/img_convert/2f3f022d2908c4e6df803091ff921273.png)