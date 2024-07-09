//
// Created by Jeruisi on 24-7-8.
// 软件网络层，用于接受和发送网络消息给服务端
//

#ifndef NETWORK_H
#define NETWORK_H

#define PORT 6789

struct NWInfo {
    char address[20];
    char username[30];
    char password[256];
};

extern struct NWInfo nwInfo;

int SetNWInfo(const char *address, const char *username, const char *password);

int ConnectToServer();

void Heartbeat();

int DisconnectToServer();

#endif //NETWORK_H
