//
// Created by Jeruisi on 24-7-8.
// 软件核心层，用来处理与 RemoteApp 相关的操作
//

#ifndef KERNEL_H
#define KERNEL_H

#define LOCK_FILE "/tmp/SKRO_lockfile.lock"
#define PATH_FILE "../data/path"
#define SHARE_PATH "../share"
#define FREE_RDP_PATH "../assets/xfreerdp"
#define OPEN_FREE_RDP_PATH "../assets/open"

struct PIDList {
    int pid;
    struct PIDList *next;
    struct PIDList *last;
};

extern char hostName[256];
extern char *sharePath;
extern int isShare;
extern int fd; // 需要关闭

extern struct PIDList *head;

int KernelInit();

void KernelClose();

int KernetCheck();

int LockFile();

int GetHostName();

void ReadSharePath();

int CheckSharePath();

int CreateFolder(char *folderPath);

int CreateFile(char *filePath);

int CheckFreeRDP();

int SaveSharePath();

void addPid(int pid);

void removePid(struct PIDList *pid);

#endif //KERNEL_H
