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

​	安装freerdp包的指令：sudo apt install freerdp2-dev

### 网络配置

​	这部分主要是因为访问 github 经常会出问题，如果你的电脑经常需要梯子才能访问 github，那么你的虚拟机同样需要借用主机的梯子才能使用。

​	具体方法如下：

- 查看主机 ip 地址，打开即可找到主机的 ip 地址，你可能有多个 ip 地址，推荐在下面使用描述和名称里有 vmware 字样的 ipv4地址![001](linux环境安装_image\001.jpg)

- 在主机上记录下 clash for windows 里的端口号以及打开允许局域网![002](linux环境安装_image\002.jpg)

- 在 linux 虚拟机上打开网络设置中的代理，在进行一些不需要梯子的操作时，比如安装一些软件访问国内百度等网站，记得关闭这个代理以及 clash![003](linux环境安装_image\003.jpg)

