//
// Created by Jeruisi on 24-7-8.
// 网络子系统，负责处理与客户端信息的收发r
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

#include "../include/uifunc.h"

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
pthread_t *threadForServer;
pthread_t *threadForRemoteApp;
int port;
int isRun;
pthread_mutex_t isRunMutex;
int isOpen;
pthread_mutex_t isOpenMutex;


int NetworkInit() {
    pthread_mutex_init(&isConnectMutex, NULL);
    pthread_mutex_init(&mutex, NULL);
    pthread_cond_init(&cond, NULL);
    pthread_mutex_init(&isRunMutex, NULL);
    pthread_mutex_init(&isOpenMutex, NULL);
    isRun = 1;
    isOpen = 0;
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
    pthread_mutex_destroy(&isOpenMutex);
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

int CreateClient(struct sockaddr_in *addr, int port, int seconds) {
    int sock = socket(AF_INET, SOCK_STREAM, 0);

    struct timeval timeout;
    timeout.tv_sec = seconds;
    timeout.tv_usec = 0;

    if(sock < 0) return -1;

    setsockopt(sock, SOL_SOCKET, SO_SNDTIMEO, &timeout, sizeof(timeout));

    addr->sin_family = AF_INET;
    addr->sin_port = htons(port);

    if(inet_pton(AF_INET, networkInfo.address, &(addr->sin_addr)) <= 0) {
        close(sock);
        return -1;
    }

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
        return USER_INFO_ERR;
    }
    if(code  == CONNECT_UP_TO_LIMIT) {
        return CONNECT_UP_TO_LIMIT;
    }
    if(code < 0){
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
    AddOneHistoryRecord(networkInfo.address, networkInfo.username, networkInfo.password);
    AddHistoryBox(networkInfo.address, networkInfo.username, networkInfo.password);
    printf("%s\n", serverName);
    return 0;
}

int TryToConnectToServer() {
    int ret;

    struct sockaddr_in addr;
    printf("获取网络标识符\n");
    int sock = CreateClient(&addr, SERVER_PORT, 2);
    if(sock < 0) return 1;
    printf("开始连接\n");
    if(connect(sock, (struct sockaddr *)&addr, sizeof(struct sockaddr_in)) < 0) return 1;
    if((ret = CheckUserInfo(sock)) != 0) return ret;
    if(ChoosePort(sock) < 0) return 1;
    if(SendHostName(sock) != 0) return 1;
    close(sock);

    printf("创建线程\n");
    int *newsock = malloc(sizeof(int));
    *newsock = CreateClient(&addr, port, 2);
    if(connect(*newsock, (struct sockaddr *)&addr, sizeof(struct sockaddr_in)) < 0) return 1;
    threadForServer = malloc(sizeof(pthread_t));
    if(pthread_create(threadForServer, NULL, ReveiveServer, newsock) !=0 ) {
        close(*newsock);
        return 1;
    }

    return 0;
}

void LogOff() {
    char sys[1024]=FREE_RDP_PATH;
    strcat(sys, " /v:");
    strcat(sys, networkInfo.address);
    strcat(sys, " /u:");
    strcat(sys, networkInfo.username);
    strcat(sys, " /p:");
    strcat(sys, networkInfo.password);
    strcat(sys, " /logoff");
    syscall(sys);
}

void ConnectToServer(char *address, char *username, char *password) {
    pthread_mutex_lock(&isConnectMutex);
    if(isConnect) {
        pthread_mutex_unlock(&isConnectMutex);
        DisconnectToServer();
        sleep(1);
    } else {
        pthread_mutex_unlock(&isConnectMutex);
    }

    pthread_mutex_lock(&isConnectMutex);
    isConnect = 1;
    pthread_mutex_unlock(&isConnectMutex);

    SetNetworkInfo(address, username, password);
    ConnectingHome(networkInfo.address);

    int ret = TryToConnectToServer();
    if(ret != 0) {
        pthread_mutex_lock(&isConnectMutex);
        UnconnectHome();
        isConnect = 0;
        pthread_mutex_unlock(&isConnectMutex);

        if(ret == 1) {
            ErrDialog(NETWORK_ERR_STRING);
        } else if(ret == USER_INFO_ERR) {
            ErrDialog(USER_INFO_ERR_STRING);
        } else if(ret == CONNECT_UP_TO_LIMIT) {
            ErrDialog(CONNECT_UP_TO_LIMIT_STRING);
        }
    } else {
        ConnectedHome(networkInfo.address, serverName);
    }

}

void *ReveiveServer(void *sock) {
    int sockId = *((int *)sock);
    int length = 0;
    if(recv(sockId, &length, sizeof(int), 0) <=0 ) return NULL;
    if(recv(sockId, rdpInfo.name, length, 0) <=0 ) return NULL;
    printf("%s\n", rdpInfo.name);
    // 打开断开连接的按钮
    ShowUnconnectButton();

    pthread_mutex_lock(&isRunMutex);
    isRun = 1;
    pthread_mutex_unlock(&isRunMutex);
    ReConnectToRemoteApp();

    pthread_mutex_lock(&mutex);
    pthread_cond_wait(&cond, &mutex);
    pthread_mutex_unlock(&mutex);

    printf("发送断开连接消息\n");
    int code = 0;
    send(sockId, &code, sizeof(int), 0);
    pthread_mutex_lock(&isConnectMutex);
    close(sockId);
    free(sock);
    UnconnectHome();
    isConnect = 0;
    pthread_mutex_unlock(&isConnectMutex);

    printf("断开连接\n");

    return NULL;
}

int DisconnectToServer() {

    pthread_mutex_lock(&isRunMutex);
    isRun = 0;
    pthread_mutex_unlock(&isRunMutex);
    // for(struct PIDList *p = head; p; p = p -> next) {
    //     kill(p->pid, SIGTERM);
    //     removePid(p);
    // }
    syscall("pkill xfreerdp");
    syscall("pkill open");
    LogOff();

    pthread_mutex_lock(&mutex);
    pthread_cond_signal(&cond);
    pthread_mutex_unlock(&mutex);

    printf("完成关闭\n");

    return 0;
}

void ReConnectToRemoteApp() {
    threadForRemoteApp = malloc(sizeof(pthread_t));
    LogOff();
    if(pthread_create(threadForRemoteApp, NULL, ConnectToRemoteApp, NULL) != 0) {
        // 打开重连应用端的按钮
        ShowReconnectButton();
    }
}

void* ConnectToRemoteApp(void *info) {
    pthread_mutex_lock(&isOpenMutex);
    isOpen = 1;
    pthread_mutex_unlock(&isOpenMutex);

    printf("%s\n", rdpInfo.name);
    int pid = OpenRemoteApp(rdpInfo.name);
    if(pid < 0) {
        // 打开重连应用端按钮
        ShowReconnectButton();
    } else {
        sleep(2);
        struct sockaddr_in addr;
        int sock = CreateClient(&addr, REMOTEAPP_PORT, 1);
        printf("here\n");
        pthread_mutex_lock(&isRunMutex);
        while(isRun && connect(sock, (struct sockaddr *)&addr, sizeof(struct sockaddr)) < 0) {
            pthread_mutex_unlock(&isRunMutex);
            pthread_mutex_lock(&isRunMutex);
        }
        pthread_mutex_unlock(&isRunMutex);

        printf("remoteApp 连接成功\n");
        pthread_mutex_lock(&isRunMutex);
        while(isRun) {
            pthread_mutex_unlock(&isRunMutex);
            int code = 0;
            int length = 0;
            char *name;
            if(isShare) {
                int value = 1;
                send(sock, &value, sizeof(int), 0);
            } else {
                int value = 0;
                send(sock, &value, sizeof(int), 0);
            }
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
                else {
                    pthread_mutex_lock(&isRunMutex);
                    break;
                }
            }
            pthread_mutex_lock(&isRunMutex);
        }
        pthread_mutex_unlock(&isRunMutex);

        LogOff();
    }

    pthread_mutex_lock(&isRunMutex);
    if(isRun) {
        ShowReconnectButton();
    }
    pthread_mutex_unlock(&isRunMutex);

    pthread_mutex_lock(&isOpenMutex);
    isOpen = 0;
    pthread_mutex_unlock(&isOpenMutex);
    printf("退出remoteapp\n");

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
            printf("启动进程失败!\n");
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
            printf("启动进程失败!\n");
            return -1;
        }
    }

    // addPid(pid);
    return pid;
}
