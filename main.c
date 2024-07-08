#include "./include/ui.h"
#include <stdio.h>
#include <stdlib.h>

char hostName[256];

int main(int argc, char *argv[]) {

    CreateUI(argc,argv);

    // 进入GTK主循环
    gtk_main();

    return 0;
}
