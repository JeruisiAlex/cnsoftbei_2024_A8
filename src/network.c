//
// Created by Jeruisi on 24-7-8.
// 网络子系统，负责处理与客户端信息的收发
//
#include "../include/network.h"

#include <errno.h>

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
struct RDPInfo rdpInfo;
char *serverName;
int isConnect;
pthread_mutex_t isConnectMutex;
int isShare;
struct PIDList *head;

pthread_mutex_t mutex;
pthread_cond_t cond;
pthread_t threadForServer;
pthread_t threadForRemoteApp;
int port;
int isRun;
pthread_mutex_t isRunMutex;


int NetworkInit() {
    pthread_mutex_init(&isConnectMutex, NULL);
    pthread_mutex_init(&mutex, NULL);
    pthread_cond_init(&cond, NULL);
    pthread_mutex_init(&isRunMutex, NULL);
    isRun = 1;
    isConnect = 0;
    if(CheckPort() == 1) {
        return 1;
    }
    return 0;
}

void NetworkClose() {
    pthread_mutex_destroy(&mutex);
    pthread_cond_destroy(&cond);
    pthread_mutex_destroy(&isConnectMutex);
    pthread_mutex_destroy(&isRunMutex);
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
    int code = -1;
    if(recv(sock, &code, sizeof(int), 0) <= 0) return 1;
    printf("%d\n", code);
    if(code == USER_INFO_ERR) {
        ErrDialog(USER_INFO_ERR_STRING);
        return 1;
    }
    if(code  == 1) {
        printf("连接已经到达上限\n");
        return 1;
    }
    if(code < 0){
        printf("连接失败\n");
        return 1;
    }

    return 0;
}

int tryPort(int port) {
    int sock = socket(AF_INET, SOCK_STREAM, 0);
        if(sock < 0) {
            return 1;
        }
    struct sockaddr_in addr;
    addr.sin_family = AF_INET;
    addr.sin_port = htonl(port);
    addr.sin_addr.s_addr = htonl(INADDR_ANY);
    if (bind(sock, (struct sockaddr *)&addr, sizeof(addr)) < 0) {
        return 1;
    }
    close(sock);
    return 0;
}

int ChoosePort(int sock) {
    int code = 1;
    port = SERVER_PORT;
    while(code != 0) {
        port++;
        while(tryPort(port)) port++;
        if(send(sock, &port, sizeof(int), 0) < 0) return 1;
        if(recv(sock, &code, sizeof(int), 0) <= 0) return 1;
    }
    printf("%d\n", port);
    return 0;
}

int SendHostName(int sock) {
    int length = strlen(hostName);
    if(send(sock, &length, sizeof(int), 0) < 0) return 1;
    if(send(sock, hostName, length, 0) < 0) return 1;
    if(recv(sock, &length, sizeof(int), 0) <= 0) return 1;
    serverName = malloc(length+1);
    if(recv(sock, serverName, length, 0) <= 0) return 1;
    printf("%s\n", serverName);
    return 0;
}

int ConnectToServer() {
    ConnectingHome(networkInfo.address);
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
    if(connect(*newsock, (struct sockaddr *)&addr, sizeof(struct sockaddr_in)) < 0) return 1;
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
    if(recv(sockId, &length, sizeof(int), 0) <=0 ) return NULL;
    rdpInfo.name = malloc(length + 1);
    if(recv(sockId, rdpInfo.name, length, 0) <=0 ) return NULL;
    // 打开断开连接的按钮

    if(pthread_create(&threadForRemoteApp, NULL, ConnectToRemoteApp, NULL) != 0) {
        // 打开重连应用端的按钮

    }

    pthread_mutex_lock(&mutex);
    pthread_cond_wait(&cond, &mutex);
    pthread_mutex_unlock(&mutex);

    int code = 0;
    send(sockId, &code, sizeof(int), 0);
    pthread_mutex_lock(&isConnectMutex);
    UnconnectHome();
    isConnect = 0;
    pthread_mutex_unlock(&isConnectMutex);

    return NULL;
}

int DisconnectToServer() {

    pthread_mutex_lock(&mutex);
    pthread_cond_signal(&cond);
    pthread_mutex_unlock(&mutex);

    pthread_mutex_lock(&isRunMutex);
    isRun = 0;
    pthread_mutex_unlock(&isRunMutex);
    for(struct PIDList *p = head; p; p = p -> next) {
        kill(p->pid, SIGTERM);
        removePid(p);
    }
    return 0;
}

void ReConnectToRemoteApp() {
    if(pthread_create(&threadForRemoteApp, NULL, ConnectToRemoteApp, NULL) != 0) {
        // 打开重连应用端的按钮

    }
}

void* ConnectToRemoteApp(void *info) {
    // int pid = OpenRemoteApp(rdpInfo.name);
    int pid = 1;
    if(pid < 0) {
        // 打开重连应用端按钮

    } else {
        sleep(2);
        if(kill(pid, 0) != 0) {
            struct sockaddr_in addr;
            int sock = CreateClient(&addr, REMOTEAPP_PORT);
            if(connect(sock, (struct sockaddr *)&addr, sizeof(struct sockaddr)) < 0) {
                // 打开重连应用端按钮

            } else {
                printf("连接成功\n");
                pthread_mutex_lock(&isRunMutex);
                while(isRun) {
                    pthread_mutex_unlock(&isRunMutex);
                    int code = 0;
                    int length = 0;
                    char *name;
                    if(recv(sock, &code, sizeof(int), 0) > 0) {
                        if(code > 0) {
                            if(recv(sock, &length, sizeof(int), 0) > 0) {
                                name = malloc(length);
                                if(recv(sock, name, length, 0) > 0) {
                                    OpenRemoteApp(name);
                                }
                                free(name);
                            }
                        }
                    }
                    pthread_mutex_lock(&isRunMutex);
                }
                pthread_mutex_unlock(&isRunMutex);
            }
        } else {
            // 打开重连应用端按钮

        }
    }

    pthread_mutex_lock(&mutex);
    pthread_cond_signal(&cond);
    pthread_mutex_unlock(&mutex);

    return NULL;
}

int OpenRemoteApp(const char *name) {
    pid_t pid;
    if(isShare == 0) {
        char *argv[6];
        argv[0] = malloc(10);
        strcpy(argv[0], "xfreerdp");
        argv[1] = malloc(strlen(networkInfo.address));
        strcpy(argv[1], networkInfo.address);
        argv[2] = malloc(strlen(networkInfo.username));
        strcpy(argv[2], networkInfo.username);
        argv[3] = malloc(strlen(networkInfo.password));
        strcpy(argv[3], networkInfo.password);
        argv[4] = malloc(strlen(name));
        strcpy(argv[4], name);
        argv[5] = NULL;
        if (posix_spawnp(&pid, OPEN_FREE_RDP_PATH, NULL, NULL, argv,  environ)) {
            return -1;
        }
    } else {
        char *argv[7];
        argv[0] = malloc(10);
        strcpy(argv[0], "xfreerdp");
        argv[1] = malloc(strlen(networkInfo.address));
        strcpy(argv[1], networkInfo.address);
        argv[2] = malloc(strlen(networkInfo.username));
        strcpy(argv[2], networkInfo.username);
        argv[3] = malloc(strlen(networkInfo.password));
        strcpy(argv[3], networkInfo.password);
        argv[4] = malloc(strlen(name));
        strcpy(argv[4], name);
        argv[5] = malloc(strlen(sharePath));
        strcpy(argv[5], sharePath);
        argv[6] = NULL;
        if (posix_spawnp(&pid, OPEN_FREE_RDP_PATH, NULL, NULL, argv,  environ)) {
            return -1;
        }
    }

    addPid(pid);
    return pid;
}
