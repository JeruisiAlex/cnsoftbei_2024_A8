//
// Created by Jeruisi on 24-7-8.
// 网络子系统，负责处理与客户端信息的收发
//
#include "../include/network.h"

#include <string.h>
#include <threads.h>
#include <stdio.h>
#include <unistd.h>
#include <arpa/inet.h>

struct NWInfo nwInfo;

int SetNWInfo(const char *address, const char *username, const char *password) {
    if(strcpy(nwInfo.address, address) == NULL) return 1;
    if(strcpy(nwInfo.username, username) == NULL) return 1;
    if(strcpy(nwInfo.password, password) == NULL) return 1;
    return 0;
}

int ConnectToServer() {
    int sock;
}

void Heartbeat() {

}

int DisconnectToServer() {

}
