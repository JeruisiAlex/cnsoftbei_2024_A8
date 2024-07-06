#include "../include/ui.h"


// 主窗口关闭时的回调函数
void on_window_destroy(GtkWidget *widget, gpointer data) {
    gtk_main_quit();
}

// 加载CSS
void load_css() {
    // 创建并加载CSS提供者
    GtkCssProvider *provider = gtk_css_provider_new();
    gtk_css_provider_load_from_data(GTK_CSS_PROVIDER(provider), CSS, -1, NULL);

    // 获取屏幕和显示
    GdkDisplay *display = gdk_display_get_default();
    GdkScreen *screen = gdk_display_get_default_screen(display);

    // 添加CSS样式到屏幕
    gtk_style_context_add_provider_for_screen(screen, GTK_STYLE_PROVIDER(provider), GTK_STYLE_PROVIDER_PRIORITY_USER);
}