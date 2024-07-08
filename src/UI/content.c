#include "../../include/ui.h"

/* 实现右侧内容栈的功能 */

// 创建网格并将其添加到内容堆栈的函数
GtkWidget* CreateAndAddGrid(GtkWidget *contentStack, char *title) {
    GtkWidget *grid = gtk_grid_new();
    gtk_grid_set_row_spacing(GTK_GRID(grid), 10);
    gtk_grid_set_column_spacing(GTK_GRID(grid), 10);
    gtk_widget_set_margin_top(grid, 50);
    gtk_widget_set_margin_bottom(grid, 50);
    gtk_widget_set_margin_start(grid, 50);
    gtk_widget_set_margin_end(grid, 50);

    // 添加网格到内容堆栈
    gtk_stack_add_titled(GTK_STACK(contentStack), grid, title, title);

    return grid;
}

// 添加右侧内容
/*
 type:
   -1:虚线下划线的普通文本
   0：标题类文本（字更大）
   1：普通文本
 */
void AddContent(GtkWidget *grid, char *content, int row, int col, int type) {
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

// 添加开关到box
void AddSwitchInBox(GtkWidget *box) {
    GtkWidget *fixed = gtk_fixed_new(); // 创建一个固定大小的容器
    GtkWidget *switchButton = gtk_switch_new();
    gtk_widget_set_name(switchButton, "custom-switch"); // 设置 switch 的样式类
    gtk_widget_set_size_request(switchButton, 40, 20); // 设置 switch 的大小
    gtk_fixed_put(GTK_FIXED(fixed), switchButton, 0, 0); // 将 switch 放入 fixed 容器
    gtk_widget_set_margin_top(switchButton, 20); // 设置 switch 的上边距

    gtk_box_pack_start(GTK_BOX(box), fixed, FALSE, FALSE, 0); // 将 fixed 容器添加到 box 中
}

// 添加开关到网格
void AddSwitchInGrid(GtkWidget *grid, int row, int col) {
    GtkWidget *fixed = gtk_fixed_new(); // 创建一个固定大小的容器
    GtkWidget *switchButton = gtk_switch_new();
    gtk_widget_set_name(switchButton, "custom-switch"); // 设置 switch 的样式类
    gtk_widget_set_size_request(switchButton, 40, 20); // 设置 switch 的大小
    gtk_fixed_put(GTK_FIXED(fixed), switchButton, 0, 0); // 将 switch 放入 fixed 容器
    gtk_widget_set_margin_top(switchButton, 10); // 设置 switch 的上边距

    gtk_grid_attach(GTK_GRID(grid), fixed, col, row, 1, 1);
}

// 添加历史连接
void AddHistoryBox(GtkWidget *grid, char *ip, char *username, char *password, int row, int col) {
    GtkWidget *button = gtk_button_new(); // 创建按钮
    GtkWidget *box = gtk_box_new(GTK_ORIENTATION_VERTICAL, 5); // 创建垂直盒子

    GtkWidget *ipLabel = gtk_label_new(ip);
    GtkWidget *usernameLabel = gtk_label_new(username);
    GtkWidget *password_label = gtk_label_new(password);

    gtk_widget_set_name(ipLabel,"inline-label");
    gtk_widget_set_name(usernameLabel,"head-label");
    gtk_widget_set_name(password_label,"head-label");

    gtk_box_pack_start(GTK_BOX(box), ipLabel, FALSE, FALSE, 0);
    gtk_box_pack_start(GTK_BOX(box), usernameLabel, FALSE, FALSE, 0);
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
void AddLanBox(GtkWidget *grid, char *ip, int row, int col) {
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

// 创建并添加网络到内容栈（保证有滚动窗口的功能）
GtkWidget * CreateAndAddGridWithScrollFuc(GtkWidget *content_stack,char * label) {

    GtkWidget *scrolled_window = gtk_scrolled_window_new(NULL, NULL);
    gtk_scrolled_window_set_policy(GTK_SCROLLED_WINDOW(scrolled_window), GTK_POLICY_NEVER, GTK_POLICY_AUTOMATIC);
    gtk_stack_add_titled(GTK_STACK(content_stack), scrolled_window, label, label);

    GtkWidget *grid = gtk_grid_new();
    gtk_grid_set_row_spacing(GTK_GRID(grid), 10);
    gtk_grid_set_column_spacing(GTK_GRID(grid), 10);
    gtk_widget_set_margin_top(grid, 50);
    gtk_widget_set_margin_bottom(grid, 50);
    gtk_widget_set_margin_start(grid, 50);
    gtk_widget_set_margin_end(grid, 50);

    gtk_widget_set_name(scrolled_window,"scrollbar");

    gtk_container_add(GTK_CONTAINER(scrolled_window), grid);
    return grid;
}

// 添加应用程序框
void AddSoftware(GtkWidget *grid,char * imgpath ,char *name, int row, int col) {
    // 创建一个新的 GtkEventBox 以便能够实现悬停效果
    GtkWidget *event_box = gtk_event_box_new();
    gtk_widget_set_name(event_box, "inactive-clickbox");

    // 创建一个水平的 GtkBox 并将其添加到 GtkEventBox 中
    GtkWidget *box = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 5);
    gtk_container_add(GTK_CONTAINER(event_box), box);

    // 为 event_box 设置边距
    gtk_container_set_border_width(GTK_CONTAINER(event_box), 10);

    // 创建图标
    GtkWidget *image = gtk_image_new_from_file(imgpath);
    gtk_box_pack_start(GTK_BOX(box), image, FALSE, FALSE, 0);

    // 为图标设置边距
    gtk_widget_set_margin_start(image, 40);
    gtk_widget_set_margin_end(image, 70);
    gtk_widget_set_margin_top(image, 10);
    gtk_widget_set_margin_bottom(image, 10);

    // 创建垂直的 GtkBox 用于应用名称和开关按钮
    GtkWidget *inbox = gtk_box_new(GTK_ORIENTATION_VERTICAL, 5);

    // 创建应用名称标签
    GtkWidget *name_label = gtk_label_new(name);
    gtk_widget_set_name(name_label, "inline-label");
    gtk_box_pack_start(GTK_BOX(inbox), name_label, FALSE, FALSE, 0);

    // 为应用名称标签设置边距
    gtk_widget_set_margin_top(name_label, 10);
    gtk_widget_set_margin_bottom(name_label, 10);

    // 创建一个水平的 GtkBox 用于“是否连接”标签和开关按钮
    GtkWidget *ininbox = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 5);

    // 创建“是否连接”标签
    GtkWidget *connect_label = gtk_label_new("是否连接：");
    gtk_widget_set_name(connect_label, "head-label");
    gtk_box_pack_start(GTK_BOX(ininbox), connect_label, FALSE, FALSE, 0);

    // 为“是否连接”标签设置边距
    gtk_widget_set_margin_top(connect_label, 10);
    gtk_widget_set_margin_bottom(connect_label, 10);

    // 添加开关按钮到 ininbox
    AddSwitchInBox(ininbox);

    // 将 ininbox 添加到 inbox 中
    gtk_box_pack_start(GTK_BOX(inbox), ininbox, FALSE, FALSE, 0);

    // 将 inbox 添加到主水平 box 中
    gtk_box_pack_start(GTK_BOX(box), inbox, FALSE, FALSE, 0);

    // 设置 event_box 大小
    gtk_widget_set_size_request(event_box, 430, 180); // 调整宽度和高度

    // 将 event_box 添加到主 grid 中
    gtk_grid_attach(GTK_GRID(grid), event_box, col, row, 1, 1);
}

// 添加发布程序框
void AddPublishedSoftware(GtkWidget *grid,char * imgpath, char *name,char *alias,int row, int col) {
    // 创建一个新的 GtkEventBox 以便能够实现悬停效果
    GtkWidget *event_box = gtk_event_box_new();
    gtk_widget_set_name(event_box, "inactive-clickbox");

    // 创建一个水平的 GtkBox 并将其添加到 GtkEventBox 中
    GtkWidget *box = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 5);
    gtk_container_add(GTK_CONTAINER(event_box), box);

    // 为 event_box 设置边距
    gtk_container_set_border_width(GTK_CONTAINER(event_box), 10);

    // 创建图标
    GtkWidget *image = gtk_image_new_from_file(imgpath);
    gtk_box_pack_start(GTK_BOX(box), image, FALSE, FALSE, 0);

    // 为图标设置边距
    gtk_widget_set_margin_start(image, 40);
    gtk_widget_set_margin_end(image, 70);
    gtk_widget_set_margin_top(image, 10);
    gtk_widget_set_margin_bottom(image, 10);

    // 创建垂直的 GtkBox 用于应用名称和别名
    GtkWidget *inbox = gtk_box_new(GTK_ORIENTATION_VERTICAL, 5);

    // 创建应用名称标签
    GtkWidget *label = gtk_label_new(name);
    gtk_widget_set_name(label, "inline-label");
    gtk_box_pack_start(GTK_BOX(inbox), label, FALSE, FALSE, 0);

    // 为应用名称标签设置边距
    gtk_widget_set_margin_top(label, 10);
    gtk_widget_set_margin_bottom(label, 10);

    // 创建别名标签
    label = gtk_label_new(alias);
    gtk_widget_set_name(label, "head-label");
    gtk_box_pack_start(GTK_BOX(inbox), label, FALSE, FALSE, 0);

    // 为别名标签设置边距
    gtk_widget_set_margin_top(label, 10);
    gtk_widget_set_margin_bottom(label, 10);

    // 将 inbox 添加到主水平 box 中
    gtk_box_pack_start(GTK_BOX(box), inbox, FALSE, FALSE, 0);

    // 设置 event_box 大小
    gtk_widget_set_size_request(event_box, 430, 180); // 调整宽度和高度

    // 将 event_box 添加到主 grid 中
    gtk_grid_attach(GTK_GRID(grid), event_box, col, row, 1, 1);
}

// 创建主页
GtkWidget* CreateHome(GtkWidget *content_stack, const char *title) {
    GtkWidget *grid = gtk_grid_new();
    gtk_grid_set_row_spacing(GTK_GRID(grid), 10);
    gtk_grid_set_column_spacing(GTK_GRID(grid), 10);
    gtk_widget_set_margin_top(grid, 50);
    gtk_widget_set_margin_bottom(grid, 50);
    gtk_widget_set_margin_start(grid, 50);
    gtk_widget_set_margin_end(grid, 50);

    // 创建左侧内容
    GtkWidget *left_box = gtk_box_new(GTK_ORIENTATION_VERTICAL, 5);
    gtk_widget_set_size_request(left_box, 400, -1); // 设置左侧宽度

    // 添加左侧内容到左侧 box
    GtkWidget *label = gtk_label_new("左侧内容");
    gtk_box_pack_start(GTK_BOX(left_box), label, FALSE, FALSE, 0);

    // 创建右侧滚动窗口
    GtkWidget *right_scrolled_window = gtk_scrolled_window_new(NULL, NULL);
    gtk_scrolled_window_set_policy(GTK_SCROLLED_WINDOW(right_scrolled_window), GTK_POLICY_AUTOMATIC, GTK_POLICY_AUTOMATIC);
    gtk_widget_set_size_request(right_scrolled_window, 600, -1); // 设置右侧宽度

    // 创建右侧内容区域
    GtkWidget *right_box = gtk_box_new(GTK_ORIENTATION_VERTICAL, 5);

    // 添加右侧内容到右侧 box
    label = gtk_label_new("右侧内容");
    gtk_box_pack_start(GTK_BOX(right_box), label, FALSE, FALSE, 0);

    // 将右侧内容区域添加到滚动窗口
    gtk_container_add(GTK_CONTAINER(right_scrolled_window), right_box);

    // 将左侧和右侧内容添加到主 grid
    gtk_grid_attach(GTK_GRID(grid), left_box, 0, 0, 1, 1);
    gtk_grid_attach(GTK_GRID(grid), right_scrolled_window, 1, 0, 1, 1);

    // 添加 grid 到内容堆栈
    gtk_stack_add_titled(GTK_STACK(content_stack), grid, title, title);

    return grid;
}