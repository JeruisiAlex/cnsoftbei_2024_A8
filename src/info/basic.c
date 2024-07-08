#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "../../include/info.h"
#include "../../include/err.h"

int GetName(){

    // 获取主机名
    if (gethostname(hostName, sizeof(hostName)) == -1) {
        printf("HOST NAME NOT FOUND：没有找到主机名！");
        return NAME_NF;
    }

    return 0;
}