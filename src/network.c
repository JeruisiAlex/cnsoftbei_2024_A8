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
    int sock = 0;
    char *hello = "Hello from C client";
    char buffer[1024] = {0};

    struct sockaddr_in serv_addr;

    // 创建套接字
    if ((sock = socket(AF_INET, SOCK_STREAM, 0)) < 0) {
        printf("\n Socket creation error \n");
        return -1;
    }

    serv_addr.sin_family = AF_INET;
    serv_addr.sin_port = htons(PORT);

    // 将服务器地址转换为二进制形式
    if(inet_pton(AF_INET, "192.168.134.129", &serv_addr.sin_addr) <= 0) {
        printf("\nInvalid address/ Address not supported \n");
        return -1;
    }

    // 连接到服务器
    if (connect(sock, (struct sockaddr *)&serv_addr, sizeof(serv_addr)) < 0) {
        printf("\nConnection Failed \n");
        return -1;
    }

    // 发送数据到服务器
    send(sock, hello, strlen(hello), 0);
    printf("Hello message sent\n");

    // 读取来自服务器的数据
    int valread = read(sock, buffer, 1024);
    printf("Received: %s\n", buffer);

    // 关闭套接字
    close(sock);
}

void Heartbeat() {

}

int DisconnectToServer() {

}
