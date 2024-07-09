#include "./include/ui.h"
#include "include/err.h"
#include "include/kernel.h"
#include "include/network.h"

#include <stdio.h>
#include <stdlib.h>

int main(int argc, char *argv[]) {

    GetHostName();

    ConnectToServer();

    CreateUI(argc,argv);

    // 进入GTK主循环
    gtk_main();

    return 0;
}
