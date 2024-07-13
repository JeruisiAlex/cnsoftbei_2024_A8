//
// Created by Jeruisi on 24-7-8.
// 网络子系统，负责处理与客户端信息的收发
//
#include "../include/network.h"
#include "../include/err.h"
#include "../include/ui.h"
#include "../include/kernel.h"

#include <string.h>
#include <threads.h>
#include <stdio.h>
#include <unistd.h>
#include <arpa/inet.h>
#include <pthread.h>
#include <spawn.h>

extern char **environ;

struct NetworkInfo networkInfo;
char *serverName;
int isConnect;
int isShare;

pthread_mutex_t mutex;
pthread_cond_t cond;
pthread_t threadForServer;
pthread_t threadForRemoteApp;
int port;


int NetworkInit() {
    pthread_mutex_init(&mutex, NULL);
    pthread_cond_init(&cond, NULL);
    isConnect = 0;
    if(CheckPort() == 1) {
        return 1;
    }
    return 0;
}

void NetworkClose() {
    pthread_mutex_destroy(&mutex);
    pthread_cond_destroy(&cond);
}

int CheckPort() {
    int sock = socket(AF_INET, SOCK_STREAM, 0);
    if(sock < 0) {
        ErrDialog(strdup(NO_SOCKET_STRING));
        return 1;
    }
    struct sockaddr_in addr;
    addr.sin_family = AF_INET;
    addr.sin_port = htonl(SERVER_PORT);
    addr.sin_addr.s_addr = htonl(INADDR_ANY);
    if (bind(sock, (struct sockaddr *)&addr, sizeof(addr)) < 0) {
        ErrDialog(strdup(PORT_USED_STRING));
        return 1;
    }
    close(sock);

    sock = socket(AF_INET, SOCK_STREAM, 0);
    if(sock < 0) {
        ErrDialog(strdup(NO_SOCKET_STRING));
        return 1;
    }
    addr.sin_port = htonl(REMOTEAPP_PORT);
    if (bind(sock, (struct sockaddr *)&addr, sizeof(addr)) < 0) {
        ErrDialog(strdup(PORT_USED_STRING));
        return 1;
    }
    close(sock);

    return 0;
}

int SetNetworkInfo(const char *address, const char *username, const char *password) {
    if(strcpy(networkInfo.address, address) == NULL) return 1;
    if(strcpy(networkInfo.username, username) == NULL) return 1;
    if(strcpy(networkInfo.password, password) == NULL) return 1;
    return 0;
}

int CreateClient(struct sockaddr_in *addr, int port) {
    int sock = socket(AF_INET, SOCK_STREAM, 0);
    if(sock < 0) return -1;
    addr->sin_family = AF_INET;
    addr->sin_port = htons(port);
    if(inet_pton(AF_INET, networkInfo.address, &(addr->sin_addr)) <= 0) return -1;;
    return sock;
}

int CheckUserInfo(int sock) {
    int length = strlen(networkInfo.username);
    if(send(sock, &length, sizeof(int), 0) < 0) return 1;
    if(send(sock, &(networkInfo.username), length, 0) < 0) return 1;
    length = strlen(networkInfo.password);
    if(send(sock, &length, sizeof(int), 0) < 0) return 1;
    if(send(sock, &(networkInfo.password), length, 0) < 0) return 1;
    int code;
    if(recv(sock, &code, sizeof(int), 0) <= 0) return 1;
    if(code == USER_INFO_ERR) {
        ErrDialog(USER_INFO_ERR_STRING);
        return 1;
    }
    return 0;
}

int ChoosePort(int sock) {
    int code = 1;
    port = SERVER_PORT;
    while(code != 0) {
        port++;
        if(send(sock, &port, sizeof(int), 0) < 0) return 1;
        if(recv(sock, &code, sizeof(int), 0) <= 0) return 1;
    }
    return 0;
}

int SendHostName(int sock) {
    int length = strlen(hostName);
    if(send(sock, &length, sizeof(int), 0) < 0) return 1;
    if(send(sock, hostName, length, 0) < 0) return 1;
    if(recv(sock, &length, sizeof(int), 0) <= 0) return 1;
    serverName = malloc(length+1);
    if(recv(sock, serverName, length, 0) <= 0) return 1;

    return 0;
}

int ConnectToServer() {
    struct sockaddr_in addr;
    int sock = CreateClient(&addr, SERVER_PORT);
    if(sock < 0) return 1;
    if(connect(sock, (struct sockaddr *)&addr, sizeof(struct sockaddr_in)) < 0) return 1;
    if(CheckUserInfo(sock) != 0) return 1;
    if(ChoosePort(sock) < 0) return 1;
    if(SendHostName(sock) != 0) return 1;
    close(sock);

    int *newsock = malloc(sizeof(int));
    *newsock = CreateClient(&addr, port);
    if(connect(*newsock, (struct sockaddr *)&addr, sizeof(struct sockaddr_in)) < 0)
    if(pthread_create(&threadForServer, NULL, ReveiveServer, newsock) !=0 ) {
        close(*newsock);
        return 1;
    }

    ConnectedHome(networkInfo.address, serverName);
    return 0;
}

void *ReveiveServer(void *sock) {
    int sockId = *((int *)sock);
    int length = 0;
    struct RDPInfo *info = malloc(sizeof(struct RDPInfo));
    if(recv(sockId, &length, sizeof(int), 0) <=0 ) {
        free(info);
        return NULL;
    }
    info->name = malloc(length + 1);
    if(recv(sockId, info->name, length, 0) <=0 ) {
        free(info);
        return NULL;
    }
    int pid = OpenRemoteApp(info->name);

    return NULL;
}

int DisconnectToServer() {

    return 0;
}

void* ConnectToRemoteApp(void *info) {

    return NULL;
}

int OpenRemoteApp(char *name) {
    pid_t pid;

    if(isShare == 0) {
        char argv[6][256] = {"freerdp", "ip", "user", "password", "appname", NULL};
        strcpy(argv[1], networkInfo.address);
        strcpy(argv[2], networkInfo.username);
        strcpy(argv[3], networkInfo.password);
        strcpy(argv[4], name);
        if (posix_spawnp(&pid, OPEN_FREE_RDP_PATH, NULL, NULL, argv,  environ)) {
            return -1;
        }
    } else {
        char argv[7][256] = {"freerdp", "ip", "user", "password", "appname", "share", NULL};
        strcpy(argv[1], networkInfo.address);
        strcpy(argv[2], networkInfo.username);
        strcpy(argv[3], networkInfo.password);
        strcpy(argv[4], name);
        strcpy(argv[5], sharePath);
        if (posix_spawnp(&pid, OPEN_FREE_RDP_PATH, NULL, NULL, argv,  environ)) {
            return -1;
        }
    }

    return pid;
}
