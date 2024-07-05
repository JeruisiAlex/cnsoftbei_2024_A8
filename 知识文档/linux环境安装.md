### 初始化

​	更新安装器版本指令：sudo apt update



### 编译器

#### gcc

​	安装指令：sudo apt install gcc

​	版本查看指令：gcc --version

#### g++（用于编译运行 c++）

​	安装指令：sudo apt install g++

​	版本查看指令：g++ --version

### 软件安装

#### git

​	安装指令：sudo apt-get install git

​	版本查看指令：git --version

#### freerdp

​	sudo apt install freerdp2-dev

​	sudo apt install libx11-dev libssl-dev

#### gtk

​	sudo apt install build-essential			// debian包构建工具。
​	sudo apt install cmake						// cmake编译工具
​	sudo apt install libgtk-3-dev				// gtk开发依赖工具
​	sudo apt install libgtkmm-3.0-dev 

#### clion

- 在虚拟机中进入这个网址 https://www.jetbrains.com/clion/，直接点击 **Download**

- 进入下一个页面再点 **Download**，然后保留就好

- 打开压缩包所在文件夹，打开终端，输入 **tar -xzvf CLion-2** 然后按 TAB 补全，再回车解压

- 执行 sudo mv **刚刚解压出来的文件夹名称** /opt

- 执行 cd /opt/clion- **记得按 TAB 补全**

- 执行 cd bin

- 执行 ./clion.sh 如果遇见弹窗全部点允许之类的

- 后面就是与windows程序类似的安装了，一路同意就行，可以选择30天试用，后面如果还需要我们再破解就好了，别付费不值得，除非你有钱。**记得删除安装包**

- 创建 clion 的快捷方式：

  - 终端执行 sudo apt install -y vim

  - 执行 cd /usr/share/applications

  - 执行 sudo vim CLion.desktop

  - 粘贴下面内容

    ```
    [Desktop Entry]
    Version=1.0
    Name=CLion
    Comment=This is my application
    Exec=/opt/clion-2024.1.2/bin/clion.sh // 这里的clion-2024.1.2需要换成你复制到 opt 里面的文件夹名称，下面也是
    Icon=/opt/clion-2024.1.2/bin/clion.png
    Terminal=false
    Type=Application
    Categories=Development;
    ```

  - 输入  :wq 保存退出

  - 然后这个位置就有 clion 了，右键点击即可发送快捷方式到桌面

    ![004](linux环境安装_image\004.jpg)

### 网络配置

​	这部分主要是因为访问 github 经常会出问题，如果你的电脑经常需要梯子才能访问 github，那么你的虚拟机同样需要借用主机的梯子才能使用。

​	具体方法如下：

- 查看主机 ip 地址，打开即可找到主机的 ip 地址，你可能有多个 ip 地址，推荐在下面使用描述和名称里有 vmware 字样的 ipv4地址![001](linux环境安装_image\001.jpg)

- 在主机上记录下 clash for windows 里的端口号以及打开允许局域网![002](linux环境安装_image\002.jpg)

- 在 linux 虚拟机上打开网络设置中的代理，在进行一些不需要梯子的操作时，比如安装一些软件访问国内百度等网站，记得关闭这个代理以及 clash![003](linux环境安装_image\003.jpg)

### git配置

* 获取个人账户的 token：

![](linux环境安装_image\005.png)

![](linux环境安装_image\006.png)

![](linux环境安装_image\007.png)

> [!NOTE]
>
> 注意保存好密钥。可以创建一个 .txt 文件把密钥保存好

* 在终端输入：`git config --global credential.helper cache`

> 上面的命令会保证密钥在 15min 不用重复输入
>
> 如果 15min 后密钥失效，则再输入一次上面的命令

* 再 `git push`
* 在弹出的框内输入个人信息。username 就输github的用户名。password 就输入之前保存的 token

