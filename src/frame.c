#include "../include/ui.h"


// 主窗口关闭时的回调函数
void on_window_destroy(GtkWidget *widget, gpointer data) {
    gtk_main_quit();
}
