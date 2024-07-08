//
// Created by Jeruisi on 24-7-8.
//
#include "../include/kernel.h"
#include "../include/err.h"
char hostName[256];

int GetHostName() {
    if(gethostname(hostName, sizeof(hostName)) == -1) return NAME_NF;
    return 0;
}
