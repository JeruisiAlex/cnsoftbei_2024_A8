Windows开发

**1.显示文件信息**

- 文件路径：string
- 文件名：string
- 文件大小：long
- 创建时间：DateTime
- 上次访问时间：DateTime
- 上次写入时间：DateTime
- 图标：Icon（转换为Bitmap类型）

> VB中的实现

创建远程app的实现

> createRemoteApp：判断新加的程序是否已存在，若存在，则展示错误信息，若不存在，则调用DoBrowsePath

> DoBrowsePath：读取exe文件的文件路径；
>
> 定义title=GetEXETitle(FilePath)，我认为此时的shortnametext和fullnametext均为空，则执行以下两步，定义remoteApp的全名和别名

![image-20240711105047915](C:\Users\周琳萍\AppData\Roaming\Typora\typora-user-images\image-20240711105047915.png)

CreateRemoteApp->DoBrowsePath
