#include "../../include/ui.h"

int CreateUI(int argc,char *argv[]) {

    GtkWidget *window;
    GtkWidget *contentStack;

    // 初始化GTK
    gtk_init(&argc, &argv);

    // 获取屏幕大小
    GdkScreen *screen = gdk_screen_get_default();
    screenWidth = gdk_screen_get_width(screen);
    screenHeight = gdk_screen_get_height(screen);

    // 创建主窗口
    window = CreateWindow();

    // 创建自定义标题栏
    CreateTitle(window);

    // 创建box的框架
    contentStack = CreateBoxFrame(window);

    // 搭建右侧的内容栈
    CreateContent(window,contentStack);

    // 显示所有组件
    gtk_widget_show_all(window);
}


// 主窗口关闭时的回调函数
void OnWindowDestroy(GtkWidget *widget, gpointer data) {
    gtk_main_quit();
}

// 加载CSS
void LoadCss() {
    // 创建并加载CSS提供者
    GtkCssProvider *provider = gtk_css_provider_new();
    gtk_css_provider_load_from_data(GTK_CSS_PROVIDER(provider), css, -1, NULL);

    // 获取屏幕和显示
    GdkDisplay *display = gdk_display_get_default();
    GdkScreen *screen = gdk_display_get_default_screen(display);

    // 添加CSS样式到屏幕
    gtk_style_context_add_provider_for_screen(screen, GTK_STYLE_PROVIDER(provider), GTK_STYLE_PROVIDER_PRIORITY_USER);
}

// 创建主窗口
GtkWidget * CreateWindow() {
    GtkWidget * window = gtk_window_new(GTK_WINDOW_TOPLEVEL);
    gtk_window_set_default_size(GTK_WINDOW(window), (gint)((double)screenWidth * wdPercen), (gint)((double)screenHeight * htPercen));
    g_signal_connect(window, "destroy", G_CALLBACK(OnWindowDestroy), NULL);
    return window;
}

// 创建标题栏
void CreateTitle(GtkWidget* window) {
    GtkWidget * headerBar = gtk_header_bar_new();
    gtk_header_bar_set_title(GTK_HEADER_BAR(headerBar), "SKRO");
    gtk_header_bar_set_show_close_button(GTK_HEADER_BAR(headerBar), TRUE);
    gtk_widget_set_name(headerBar, "headerbar");
    gtk_window_set_titlebar(GTK_WINDOW(window), headerBar);
}

// 搭建框架。包括：创建主盒子、侧边盒子、内容栈、内容栈中的滚动窗口。返回内容栈
GtkWidget * CreateBoxFrame(GtkWidget *window) {
    // 创建主盒子
    GtkWidget *mainBox = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 0);
    gtk_container_add(GTK_CONTAINER(window), mainBox);
    return CreateBar(mainBox);
}

