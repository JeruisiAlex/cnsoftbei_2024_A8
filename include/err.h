// 用来标识各种错误

#ifndef ERR_H
#define ERR_H

#define SUCCESS 0
#define NAME_NF 1 // 没有找到主机名
#define USER_INFO_ERR 2 // 用户名或密码不正确
#define CONNECT_UP_TO_LIMIT 3 // 连接数量到达上限
#define HISTORY_NF 6 // 没有找到存放历史连接的文件
#define MALLOC_ERR 7 // malloc失败
#define IMAGE_NF 8 // 没有找到对应图片

#define PATH_FILE_LOST_STRING "资源文件path缺失，无法保存共享文件夹路径，请重新安装！"
#define SHARE_PATH_LOST_STRING "共享文件夹路径不存在，请重新选择共享文件夹，否则部分功能无法使用！"
#define FREE_RDP_LOST_STRING "资源文件xfreerdp或open缺失，请重新安装应用！"
#define NO_SOCKET_STRING "文件描述数量不足或权限不足。"
#define PORT_USED_STRING "端口5678或6789被占用，请确保这两个端口可用性再启动本软件。"
#define NETWORK_ERR_STRING "网络连接失败"
#define USER_INFO_ERR_STRING "用户名或密码错误！"
#define CONNECT_UP_TO_LIMIT_STRING "服务器连接数量已经到达上限"

extern int errorNumber;

#endif //ERR_H
