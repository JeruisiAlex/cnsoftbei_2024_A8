//
// Created by Jeruisi on 24-7-8.
//
#include "../include/kernel.h"
#include "../include/err.h"
#include "../include/ui.h"

#include <stdio.h>
#include <unistd.h>
#include <string.h>
#include <stdlib.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <errno.h>
#include <fcntl.h>
#include <libgen.h>
#include <linux/limits.h>
#include <dirent.h>

int errorNumber;
char hostName[256];
char *sharePath;
int isShare;
int fd;

int KernelInit() {
    errorNumber = SUCCESS;
    if(LockFile() == 1) return 1;
    GetHostName();
    ReadSharePath();
    return 0;
}

void KernelClose() {
    close(fd);
}

int KernetCheck() {
    if(errorNumber != SUCCESS) {
        ErrDialog(strdup(PATH_FILE_LOST_STRING));
    }
    if(CheckSharePath() != 0) {
        if(CreateFolder(sharePath) != 0 ) {
            ErrDialog(strdup(SHARE_PATH_LOST_STRING));
            isShare = 0;
        } else {
            isShare = 1;
        }
    }
    if(CheckFreeRDP() == 1) {
        ErrDialog(strdup(FREE_RDP_LOST_STRING));
        return 1;
    }
    return 0;
}

int LockFile() {
    fd = open(strdup(LOCK_FILE), O_RDWR | O_CREAT, 0666);
    if(fd == -1) return 1;
    if(lockf(fd, F_TLOCK, 0) == -1) return 1;
    return 0;
}

int GetHostName() {
    if(gethostname(hostName, sizeof(hostName)) == -1) return NAME_NF;
    return 0;
}

void ReadSharePath() {
    char *pathFile = strdup(PATH_FILE);
    FILE *file = fopen(pathFile, "rb");
    if(file != NULL) {
        sharePath = malloc(PATH_MAX);
        int length = fread(sharePath, sizeof(char), PATH_MAX, file);
        if(length == 0 || sharePath[0] == '\0') sharePath = strdup(SHARE_PATH);
        else sharePath[length] = '\0';
        fclose(file);
    } else {
        sharePath = strdup(SHARE_PATH);
        if( CreateFile(pathFile) == 1) {
            errorNumber = 10;
        }
    }
}

int CheckSharePath() {
    if(opendir(sharePath) == NULL && errno == ENOENT) {
        return 1;
    }
    return 0;
}

int CreateFolder(char *folderPath) {
    char *p = NULL;
    int len = strlen(folderPath);
    if (folderPath[len - 1] == '/') {
        folderPath[len - 1] = '\0';
    }
    for (p = folderPath + 1 ; *p; p++) {
        if (*p == '/') {
            *p = '\0';
            if (mkdir(folderPath, S_IRWXU) != 0 && errno != EEXIST) {
                return 1;
            }
            *p = '/';
        }
    }
    if (mkdir(folderPath, S_IRWXU) != 0 && errno != EEXIST) {
        return 1;
    }
    return 0;
}

int CreateFile(char *filePath) {
    char *folderPath = malloc(strlen(filePath));
    strcpy(folderPath, filePath);
    folderPath = dirname(folderPath);
    if(folderPath[0] != '.' || strlen(folderPath) > 1) {
        if(CreateFolder(folderPath) == 1) return 1;
    }
    FILE *file = fopen(filePath, "w");
    if (file == NULL) {
        return 1;
    }
    fclose(file);
    free(folderPath);
    return 0;
}

int CheckFreeRDP() {
    FILE *file = fopen(strdup(FREE_RDP_PATH), "r");
    if(file == NULL) return 1;
    fclose(file);
    file = fopen(strdup(OPEN_FREE_RDP_PATH), "r");
    if(file == NULL) return 1;
    fclose(file);
    return 0;
}

int SaveSharePath() {
    FILE *file = fopen(strdup(PATH_FILE), "wb");
    if(file == NULL) {
        ErrDialog(strdup(PATH_FILE_LOST_STRING));
        return 1;
    } else {
        fwrite(sharePath, sizeof(char), strlen(sharePath), file);
        fclose(file);
    }
    return 0;
}
