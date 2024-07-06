#include "../include/ui.h"

/* 实现左侧导航栏功能 */

// 切换堆栈页面的回调函数
void on_switch_page(GtkButton *button, gpointer data) {
    GtkStack *stack = GTK_STACK(data);
    const gchar *page_name = gtk_button_get_label(button);
    gtk_stack_set_visible_child_name(stack, page_name);

    // 如果有当前激活的按钮，恢复其背景颜色
    if (active_button) {
        gtk_widget_set_name(active_button, "inactive-button");
    }

    // 将当前按钮设置为激活状态
    gtk_widget_set_name(GTK_WIDGET(button), "active-button");
    active_button = GTK_WIDGET(button);
}

// 增加左侧导航栏按钮
void add_bar_button(GtkWidget *content_stack, GtkWidget *sidebar_box, const char *content) {
    GtkWidget *button;
    button = gtk_button_new_with_label(content);
    g_signal_connect(button, "clicked", G_CALLBACK(on_switch_page), content_stack);
    gtk_widget_set_margin_top(button, 0);
    gtk_widget_set_margin_bottom(button, 0);
    gtk_widget_set_margin_start(button, 0);
    gtk_widget_set_margin_end(button, 0);
    gtk_widget_set_size_request(button, 200, 50); // 设置按钮大小
    gtk_widget_set_name(button, "inactive-button"); // 设置初始样式
    gtk_box_pack_start(GTK_BOX(sidebar_box), button, FALSE, FALSE, 0);

    if(active_button == NULL) {
        active_button = GTK_WIDGET(button);
        gtk_widget_set_name(GTK_WIDGET(button), "active-button");
    }
}