// 用于获取各种信息

#ifndef INFO_H
#define INFO_H

#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <ifaddrs.h>
#include <netinet/in.h>
#include <arpa/inet.h>

extern char hostName[256];
#define PORT "6789"

int GetName();

#endif //INFO_H
