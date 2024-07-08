// 用来标识各种错误

#ifndef ERR_H
#define ERR_H

#define NAME_NF 1 // 没有找到主机名
#define CONNECT_TIME_OUT 2 // 与服务端连接超时
#define USER_INFO_ERR 3 // 用户名或密码不正确
#define GET_API_ERR 4 // 获取应用列表或已发布应用列表失败
#define CONNECT_COUNT_OUT 5 // 服务器连接数量到达上限

#endif //ERR_H
