#include <math.h>

#include "../../include/ui.h"

double windowWidth;
double windowHeight;
gint minWidth = 900;
gint minHeight = 600;
GtkWidget *window; // 主窗口

// 创建UI
void CreateUI(int argc,char *argv[]) {

    GtkWidget *contentStack;

    // 初始化GTK
    gtk_init(&argc, &argv);

    // 获取屏幕大小
    GdkScreen *screen = gdk_screen_get_default();
    int screenHeight = gdk_screen_get_height(screen);

    // 创建主窗口
    windowHeight = screenHeight * 0.5;
    if(windowHeight < minHeight) {
        windowHeight = minHeight;
    }
    windowWidth = windowHeight * 1.5;
    window = CreateWindow(windowWidth,windowHeight);

    // 禁止窗口拉伸
    gtk_window_set_resizable(GTK_WINDOW(window), FALSE);

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
gboolean OnWindowDestroy(GtkWidget *widget, GdkEvent *event, gpointer data) {

    pthread_mutex_lock(&isConnectMutex);
    if(isConnect) {
        ErrDialog("请断开连接后关闭应用！");
        pthread_mutex_unlock(&isConnectMutex);
        return TRUE;
    }
    pthread_mutex_unlock(&isConnectMutex);
    gtk_main_quit();
    return FALSE;
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
GtkWidget * CreateWindow(int width,int height) {
    GtkWidget * window = gtk_window_new(GTK_WINDOW_TOPLEVEL);
    gtk_window_set_default_size(GTK_WINDOW(window), width, height);

    // 移除最大化按钮
    gtk_window_set_deletable(GTK_WINDOW(window), TRUE);
    gtk_window_set_resizable(GTK_WINDOW(window), FALSE);

    g_signal_connect(window, "delete-event", G_CALLBACK(OnWindowDestroy), NULL);
    return window;
}

// 创建标题栏
void CreateTitle(GtkWidget* window) {
    GtkWidget * headerBar = gtk_header_bar_new();
    char title[50] = "SKRO  ";
    strcat(title,VERSION);
    gtk_header_bar_set_title(GTK_HEADER_BAR(headerBar), title);
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




