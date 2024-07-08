#include "../../include/ui.h"
/* 实现左侧导航栏功能 */

// 切换堆栈页面的回调函数
void OnSwitchPage(GtkButton *button, gpointer data) {
    GtkStack *stack = GTK_STACK(data);
    const gchar *pageName = gtk_button_get_label(button);
    gtk_stack_set_visible_child_name(stack, pageName);

    // 如果有当前激活的按钮，恢复其背景颜色
    if (activeButton) {
        gtk_widget_set_name(activeButton, "inactive-button");
    }

    // 将当前按钮设置为激活状态
    gtk_widget_set_name(GTK_WIDGET(button), "active-button");
    activeButton = GTK_WIDGET(button);
}

// 增加左侧导航栏按钮
void AddBarButton(GtkWidget *contentStack, GtkWidget *sidebarBox, char *content) {
    GtkWidget *button;
    button = gtk_button_new_with_label(content);
    g_signal_connect(button, "clicked", G_CALLBACK(OnSwitchPage), contentStack);
    gtk_widget_set_margin_top(button, 0);
    gtk_widget_set_margin_bottom(button, 0);
    gtk_widget_set_margin_start(button, 0);
    gtk_widget_set_margin_end(button, 0);
    gtk_widget_set_size_request(button, 200, 50); // 设置按钮大小
    gtk_widget_set_name(button, "inactive-button"); // 设置初始样式
    gtk_box_pack_start(GTK_BOX(sidebarBox), button, FALSE, FALSE, 0);

    if(activeButton == NULL) {
        activeButton = GTK_WIDGET(button);
        gtk_widget_set_name(GTK_WIDGET(button), "active-button");
    }
}

// 在box中增加分隔符，让按钮之间可以分开
void AddSeparator(GtkWidget* box) {
    // 创建一个空白的标签作为小间隔
    GtkWidget* spacer = gtk_label_new(NULL); // 没有文本的标签
    gtk_widget_set_size_request(spacer, -1, 10); // 设置高度为5像素
    gtk_box_pack_start(GTK_BOX(box), spacer, FALSE, FALSE, 0);
}


