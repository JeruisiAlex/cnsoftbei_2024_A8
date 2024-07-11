# cnsoftbei_2024_A8
2024年中国软件杯A8赛题



## 开发团队介绍
- 队名：SSSS.软件王
- Jeruisi：项目经理
- TieZhuGieGie：linux端开发
- Pin：windows端开发



## 分支介绍
- main：保存工程文档，包括组会文件，开发计划，需求分析，设计说明，测试文档，用户使用说明等文档
- w_dev：windows端的合并文件
- w_j：windows端开发人员Jeruisi的分支
- w_p：windows端开发人员Pin的分支
- l_dev：linux端的合并文件
- l_j：linux端开发人员Jeruisi的分支
- l_t：linux端开发人员TieZhuGieGie分支
- w_build：windows端的可执行文件
- l_build：linux端的可执行文件



## 需求分析
### 必要需求
- 使用windows应用
  - 在国产操作系统上通过局域网连接一台windows电脑来使用windows软件
  - 类似于使用电脑本身的软件而不是将整个windows电脑桌面投影的虚拟机形式



### 可选需求
- 安装windows应用
  - 表现为类似在linux上双击.exe安装包，然后在windows上安装应用
- 卸载windows应用
- 保存windows应用的数据



## 操作系统选择

- 国产操作系统选择：银河麒麟（KylinOS）桌面操作系统V10 SP1 2303版本
- windows操作系统选择：windows 10 pro（即win10专业版）



## 应用传输协议选择

- RDP
  - RDP是Windows本身就有的一个封装的完整的远程桌面传输协议
- FreeRDP
  - linux上可以使用 FreeRDP 来访问 RDP 获取 windows 上的视频，音频，传递键鼠输入
  - FreeRDP 是开源的且有丰富的 API



## 开发语言选择

- C系列
  - 因为 windows 本身很多都是用 C/C++写的
  - RDP 和 FreeRDP 也是用 C/C++实现的
  - 用C系列语言比较好调用相关的接口



## 实现方案

