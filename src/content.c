#include "../include/ui.h"

/* 实现右侧内容栈的功能 */

// 创建网格并将其添加到内容堆栈的函数
GtkWidget* create_and_add_grid(GtkWidget *content_stack, const char *title) {
    GtkWidget *grid = gtk_grid_new();
    gtk_grid_set_row_spacing(GTK_GRID(grid), 10);
    gtk_grid_set_column_spacing(GTK_GRID(grid), 10);
    gtk_widget_set_margin_top(grid, 50);
    gtk_widget_set_margin_bottom(grid, 50);
    gtk_widget_set_margin_start(grid, 50);
    gtk_widget_set_margin_end(grid, 50);

    // 添加网格到内容堆栈
    gtk_stack_add_titled(GTK_STACK(content_stack), grid, title, title);

    return grid;
}

// 添加右侧内容
/*
 type:
   -1:虚线下划线的普通文本
   0：标题类文本（字更大）
   1：普通文本
 */
void add_content(GtkWidget *grid, const char *content, int row, int col, int type) {
    GtkWidget *label = gtk_label_new(content);
    gtk_label_set_xalign(GTK_LABEL(label), 0.0); // 设置左对齐
    gtk_grid_attach(GTK_GRID(grid), label, col, row, 1, 1);

    if(type == 1) {
        gtk_widget_set_name(label, "classic-label");
    }
    else if(type == -1) {
        gtk_widget_set_name(label, "inline-label");
    }
    else {
        gtk_widget_set_name(label, "head-label");
    }
}

// 添加 switch 到网格
void add_switch(GtkWidget *grid, int row, int col) {
    GtkWidget *fixed = gtk_fixed_new(); // 创建一个固定大小的容器
    GtkWidget *switch_button = gtk_switch_new();
    gtk_widget_set_name(switch_button, "custom-switch"); // 设置 switch 的样式类
    gtk_widget_set_size_request(switch_button, 40, 20); // 设置 switch 的大小
    gtk_fixed_put(GTK_FIXED(fixed), switch_button, 0, 0); // 将 switch 放入 fixed 容器
    gtk_widget_set_margin_top(switch_button, 10); // 设置 switch 的上边距

    gtk_grid_attach(GTK_GRID(grid), fixed, col, row, 1, 1);
}

// 添加历史连接
void add_history_box(GtkWidget *grid, const char *ip, const char *username, const char *password, int row, int col) {
    GtkWidget *button = gtk_button_new(); // 创建按钮
    GtkWidget *box = gtk_box_new(GTK_ORIENTATION_VERTICAL, 5); // 创建垂直盒子

    GtkWidget *ip_label = gtk_label_new(ip);
    GtkWidget *username_label = gtk_label_new(username);
    GtkWidget *password_label = gtk_label_new(password);

    gtk_widget_set_name(ip_label,"inline-label");
    gtk_widget_set_name(username_label,"head-label");
    gtk_widget_set_name(password_label,"head-label");

    gtk_box_pack_start(GTK_BOX(box), ip_label, FALSE, FALSE, 0);
    gtk_box_pack_start(GTK_BOX(box), username_label, FALSE, FALSE, 0);
    gtk_box_pack_start(GTK_BOX(box), password_label, FALSE, FALSE, 0);

    gtk_container_add(GTK_CONTAINER(button), box); // 将盒子添加到按钮中

    gtk_widget_set_margin_top(button, 5); // 设置按钮的上边距
    gtk_widget_set_margin_bottom(button, 5); // 设置按钮的下边距
    gtk_widget_set_margin_start(button, 5); // 设置按钮的左边距
    gtk_widget_set_margin_end(button, 5); // 设置按钮的右边距

    // 设置按钮的大小
    gtk_widget_set_size_request(button, 200, 200); // 调整宽度和高度

    gtk_widget_set_name(button,"inactive-clickbox");

    gtk_grid_attach(GTK_GRID(grid), button, col, row, 1, 1);
}

// 添加局域网连接
void add_lan_box(GtkWidget *grid, const char *ip, int row, int col) {
    GtkWidget *button = gtk_button_new();
    GtkWidget *box = gtk_box_new(GTK_ORIENTATION_VERTICAL, 5); // 创建垂直盒子

    GtkWidget *image = gtk_image_new_from_file("../assets/monitor.png");
    gtk_box_pack_start(GTK_BOX(box), image, FALSE, FALSE, 0);

    GtkWidget *ip_label = gtk_label_new(ip);
    gtk_widget_set_name(ip_label,"inline-label");
    gtk_box_pack_start(GTK_BOX(box), ip_label, FALSE, FALSE, 0);

    gtk_container_add(GTK_CONTAINER(button), box); // 将盒子添加到按钮中

    gtk_widget_set_margin_top(button, 5); // 设置按钮的上边距
    gtk_widget_set_margin_bottom(button, 5); // 设置按钮的下边距
    gtk_widget_set_margin_start(button, 5); // 设置按钮的左边距
    gtk_widget_set_margin_end(button, 5); // 设置按钮的右边距

    // 设置按钮的大小
    gtk_widget_set_size_request(button, 200, 200); // 调整宽度和高度

    gtk_widget_set_name(button,"inactive-clickbox");

    gtk_grid_attach(GTK_GRID(grid), button, col, row, 1, 1);
}
