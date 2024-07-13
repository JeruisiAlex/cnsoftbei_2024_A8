#include "./include/ui.h"
#include "include/err.h"
#include "include/kernel.h"
#include "include/network.h"

#include <stdio.h>
#include <stdlib.h>

int main(int argc, char *argv[]) {
    if(KernelInit() == 1) {
        KernelClose();
        return 0;
    }
    CreateUI(argc,argv);
    if(KernetCheck() == 1) {
        KernelClose();
        return 0;
    }
    if(NetworkInit() == 1) {
        KernelClose();
        NetworkClose();
        return 0;
    }
    // 进入GTK主循环
    gtk_main();

    KernelClose();
    NetworkClose();

    return 0;
}
