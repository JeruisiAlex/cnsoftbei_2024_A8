//
// Created by Jeruisi on 24-7-6.
//
#include "../include/xrdp.h"
#include "../include/rdp.h"
#include <stdlib.h>
#include <string.h>
#include <stdio.h>

int openXRdp() {
    char param[1024]="../assets/xfreerdp /v:";
    strcat(param, set.address);
    strcat(param, strdup(" /u:"));
    strcat(param, set.username);
    strcat(param, strdup(" /p:"));
    strcat(param, set.password);
    strcat(param, strdup(" /sound /microphone"));


    system(param);
}