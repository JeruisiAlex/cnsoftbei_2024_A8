#include "../../include/ui.h"

double windowWidth;
double windowHeight;

GtkWidget *homePage;
GtkWidget *content;

/* 实现左侧导航栏功能 */

// 切换堆栈页面的回调函数
void OnSwitchPage(GtkButton *button, gpointer data) {
    GtkStack *stack = GTK_STACK(data);
    const gchar *pageName = gtk_button_get_label(button);

    // 如果现在已经连接，则不可以切换到历史连接
    int flag;
    pthread_mutex_lock(&isConnectMutex);
    flag = isConnect;
    pthread_mutex_unlock(&isConnectMutex);

    if(flag == 1 && strcmp(pageName,"历史连接") == 0) {
        return;
    }

    gtk_stack_set_visible_child_name(stack, pageName);

    // 如果有当前激活的按钮，恢复其背景颜色
    if (activeButton) {
        gtk_widget_set_name(activeButton, "inactive-button");
    }

    // 将当前按钮设置为激活状态
    gtk_widget_set_name(GTK_WIDGET(button), "active-button");
    activeButton = GTK_WIDGET(button);
}

// 增加左侧导航栏按钮。返回创建的按钮
GtkWidget *AddBarButton(GtkWidget *contentStack, GtkWidget *sidebarBox, char *content) {
    GtkWidget *button;
    button = gtk_button_new_with_label(content);
    g_signal_connect(button, "clicked", G_CALLBACK(OnSwitchPage), contentStack);
    gtk_widget_set_margin_top(button, 0);
    gtk_widget_set_margin_bottom(button, 0);
    gtk_widget_set_margin_start(button, 0);
    gtk_widget_set_margin_end(button, 0);
    gtk_widget_set_size_request(button, (gint)(windowWidth/6.0), 25); // 设置按钮大小
    gtk_widget_set_name(button, "inactive-button"); // 设置初始样式
    gtk_box_pack_start(GTK_BOX(sidebarBox), button, FALSE, FALSE, 0);

    if(activeButton == NULL) {
        activeButton = GTK_WIDGET(button);
        gtk_widget_set_name(GTK_WIDGET(button), "active-button");
    }

    return button;
}

// 在box中增加分隔符，让按钮之间可以分开
void AddSeparator(GtkWidget* box) {
    // 创建一个空白的标签作为小间隔
    GtkWidget* spacer = gtk_label_new(NULL); // 没有文本的标签
    gtk_widget_set_size_request(spacer, -1, 10); // 设置高度为5像素
    gtk_box_pack_start(GTK_BOX(box), spacer, FALSE, FALSE, 0);
}

// 创建侧边导航栏，并为导航栏添加内容。返回内容栈。
GtkWidget * CreateBar(GtkWidget *mainBox) {

    // 创建侧边栏盒子
    GtkWidget *sidebarBox = gtk_box_new(GTK_ORIENTATION_VERTICAL, 0);
    gtk_box_set_homogeneous(GTK_BOX(sidebarBox), FALSE); // 不均匀分布
    gtk_widget_set_name(sidebarBox, "sidebar");
    gtk_box_pack_start(GTK_BOX(mainBox), sidebarBox, FALSE, FALSE, 0);

    // 创建滚动窗口
    GtkWidget *scrolled_window = gtk_scrolled_window_new(NULL, NULL);
    gtk_scrolled_window_set_policy(GTK_SCROLLED_WINDOW(scrolled_window), GTK_POLICY_AUTOMATIC, GTK_POLICY_AUTOMATIC);
    gtk_box_pack_start(GTK_BOX(mainBox), scrolled_window, TRUE, TRUE, 0);

    // 创建内容堆栈
    GtkWidget *contentStack = gtk_stack_new();
    content = contentStack;
    gtk_stack_set_transition_type(GTK_STACK(contentStack), GTK_STACK_TRANSITION_TYPE_SLIDE_LEFT_RIGHT);
    gtk_container_add(GTK_CONTAINER(scrolled_window), contentStack);

    // 添加侧边栏按钮
    AddSeparator(sidebarBox);
    homePage = AddBarButton(contentStack, sidebarBox, "主页");
    AddSeparator(sidebarBox);
    AddBarButton(contentStack, sidebarBox, "历史连接");
    // AddSeparator(sidebarBox);
    // AddBarButton(contentStack, sidebarBox, "局域网连接");
    // AddSeparator(sidebarBox);
    // AddBarButton(contentStack, sidebarBox, "发布应用");
    // AddSeparator(sidebarBox);
    // AddBarButton(contentStack, sidebarBox, "已发布应用");
    AddSeparator(sidebarBox);
    AddBarButton(contentStack, sidebarBox, "主机信息");

    // 加载CSS
    LoadCss();

    // 为侧边栏添加样式类
    GtkStyleContext *context = gtk_widget_get_style_context(sidebarBox);
    gtk_style_context_add_class(context, "sidebar");

    return contentStack;
}
