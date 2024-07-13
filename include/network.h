//
// Created by Jeruisi on 24-7-8.
// 软件网络层，用于接受和发送网络消息给服务端
//

#ifndef NETWORK_H
#define NETWORK_H

#include <pthread.h>

#define PORT "6789"
#define SERVER_PORT 6789
#define REMOTEAPP_PORT 5678

struct NetworkInfo {
    char address[20];
    char username[30];
    char password[256];
};

struct RDPInfo {
    char *name;
    int pid;
};

extern struct NetworkInfo networkInfo;
extern struct RDPInfo rdpInfo;
extern char *serverName;
extern int isConnect;
extern pthread_mutex_t isConnectMutex;

int NetworkInit();

void NetworkClose();

int CheckPort();

int SetNetworkInfo(const char *address, const char *username, const char *password);

int ConnectToServer();

void *ReveiveServer(void *sock);

int DisconnectToServer();

void ReConnectToRemoteApp();

void* ConnectToRemoteApp(void *info);

int OpenRemoteApp(const char *name);


#endif //NETWORK_H
