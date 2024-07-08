#include "./include/ui.h"
#include "include/err.h"
#include "include/kernel.h"

#include <stdio.h>
#include <stdlib.h>

int main(int argc, char *argv[]) {

    GetHostName();

    CreateUI(argc,argv);

    // 进入GTK主循环
    gtk_main();

    return 0;
}
