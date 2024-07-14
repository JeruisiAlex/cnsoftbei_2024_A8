#include "../../include/ui.h"
#include "../../include/kernel.h"
#include "../../include/uifunc.h"

#include <string.h>

double windowWidth;
double windowHeight;

struct NetworkInfo *historyRecords;
int cnt;

GtkWidget * connectState;
GtkWidget * spinner;
/*
 *1：主页
 *2：历史连接
 *3：发布应用
 *4：已发布应用
 *5：主机信息
 */
GtkWidget *contentGrid1,*contentGrid2,*contentGrid3,*contentGrid4,*contentGrid5;

/*
 *1：历史连接
 *2：发布应用
 *3：已发布应用
 */
// 记录现在的行数
int row1 = 0, row2 = 0, row3 = 0;
// 记录一行最多多少个盒子
int maxCol1 = 2, maxCol2 = 5, maxCol3 = 2;
// 记录现在的列数
int col1 = 1,col2 = 0,col3 = 0;

/* 实现右侧内容栈的功能 */

void CreateContent(GtkWidget* window,GtkWidget* contentStack) {

    // 使用函数创建并添加网格
    contentGrid1 = CreateHome(contentStack, "主页");
    contentGrid2 = CreateAndAddGridWithScrollFuc(contentStack, "历史连接");
    // contentGrid3 = CreateAndAddGridWithScrollFuc(contentStack, "局域网连接");
    // contentGrid3 = CreatePublishSoftware(contentStack, "发布应用");
    // contentGrid4 = CreateAndAddGridWithScrollFuc(contentStack, "已发布应用");
    contentGrid5 = CreateAndAddGrid(contentStack, "主机信息");

    int res = ReadAllHistoryRecords(); // 找到历史记录

    ConnectedHome("123","123");
    ShowUnconnectButton();
    ShowReconnectButton();

    // // 自动连接历史连接
    // if(res == 0) {
    //     if(cnt == 0) {
    //         // 暂未连接
    //         UnconnectHome();
    //     }
    //     else {
    //         // 正在连接最近连接的历史连接
    //         ConnectingHome(historyRecords[cnt-1].address);
    //     }
    // }

    // 添加内容到历史连接
    AddIPBox(window);
    if(res == 0) {
        struct NetworkInfo *temp = historyRecords;

        for(int i=0;i<cnt;i++) {
            AddHistoryBox((temp + i)->address,(temp + i)->username,(temp + i)->password);
        }

        if(cnt != 0) {
            row1 = (cnt - 1) / 3;
        }
    }

    // 添加内容到局域网连接
    // AddLanBox("IP：192.168.0.5");
    // AddLanBox("IP：192.168.0.5");

    // 添加内容到已发布应用
    // AddFolder("opt");
    // AddSoftware("clion",NULL,1);
    // AddSoftware("clion",NULL,1);
    // AddSoftware("clion",NULL,1);
    // AddSoftware("clion",NULL,1);

    // 添加内容到已发布应用
    // AddPublishedSoftware("../assets/software/clion.svg","Clion 2024 2.4","Clion");

    // 添加内容到主机信息
    AddContent(contentGrid5, "主机名：", 0, 0, 0);
    AddContent(contentGrid5, hostName, 0, 1, -1);
    AddContent(contentGrid5, "端口：", 1, 0, 0);
    AddContent(contentGrid5, PORT, 1, 1, -1);
    // add_content(content_grid1, "开机启动：", 2, 0, 0);
    // add_switch(content_grid1, 2, 1); // 添加 switch
    // row6++;
}


// 创建网格并将其添加到内容堆栈的函数
GtkWidget* CreateAndAddGrid(GtkWidget *contentStack, char *title) {
    GtkWidget *grid = gtk_grid_new();
    gtk_grid_set_row_spacing(GTK_GRID(grid), (gint)(windowWidth / 30.0));
    gtk_grid_set_column_spacing(GTK_GRID(grid), 10);
    gtk_widget_set_margin_top(grid, 10);
    gtk_widget_set_margin_bottom(grid, 10);
    gtk_widget_set_margin_start(grid, (gint)(windowWidth / 30.0));
    gtk_widget_set_margin_end(grid, (gint)(windowWidth / 30.0));

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
    gtk_widget_set_size_request(switchButton, (gint)(windowWidth / 24.0), 10); // 设置 switch 的大小
    gtk_fixed_put(GTK_FIXED(fixed), switchButton, 0, 0); // 将 switch 放入 fixed 容器
    gtk_widget_set_margin_top(switchButton, 10); // 设置 switch 的上边距

    gtk_box_pack_start(GTK_BOX(box), fixed, FALSE, FALSE, 0); // 将 fixed 容器添加到 box 中
}

// 添加开关到网格
void AddSwitchInGrid(GtkWidget *grid, int row, int col) {
    GtkWidget *fixed = gtk_fixed_new(); // 创建一个固定大小的容器
    GtkWidget *switchButton = gtk_switch_new();
    gtk_widget_set_name(switchButton, "custom-switch"); // 设置 switch 的样式类
    gtk_widget_set_size_request(switchButton, (gint)(windowWidth / 24.0), 10); // 设置 switch 的大小
    gtk_fixed_put(GTK_FIXED(fixed), switchButton, 0, 0); // 将 switch 放入 fixed 容器
    gtk_widget_set_margin_top(switchButton, 10); // 设置 switch 的上边距

    gtk_grid_attach(GTK_GRID(grid), fixed, col, row, 1, 1);
}

// 添加历史连接
void AddHistoryBox(char *ip, char *username, char *password) {
    if(!IsRepeatedHistory(ip)) {
        GtkWidget *button = gtk_button_new(); // 创建按钮
        GtkWidget *box = gtk_box_new(GTK_ORIENTATION_VERTICAL, 5); // 创建垂直盒子

        char name[35] = "用户名：";
        char processedName[10] = "\0";
        OmitUsername(username,processedName); // 处理用户名，防止用户名过长，导致UI不符合设计
        strcat(name,processedName);

        GtkWidget *ipLabel = gtk_label_new(ip);
        GtkWidget *usernameLabel = gtk_label_new(name);
        GtkWidget *password_label = gtk_label_new("密码：*********");

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
        gtk_widget_set_size_request(button, (gint)(windowWidth / 5.0), 200); // 调整宽度和高度

        gtk_widget_set_name(button,"inactive-clickbox");

        row1 += col1 / maxCol1;
        col1 %= maxCol1;

        gtk_grid_attach(GTK_GRID(contentGrid2), button, col1, row1, 1, 1);

        // 添加点击事件
        struct NetworkInfo *in = (struct NetworkInfo *)malloc(sizeof(struct NetworkInfo));
        strcpy(in->address,ip);
        strcpy(in->username,username);
        strcpy(in->password,password);
        g_signal_connect(button, "clicked", G_CALLBACK(ClickHistory), in);

        // 为按钮指定唯一标识
        g_object_set_data(G_OBJECT(button),"ip",ip);

        // 显示新按钮及其所有子控件
        gtk_widget_show_all(contentGrid2);

        col1++;
    }
}

// 添加局域网连接
void AddLanBox(char *ip) {
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
    gtk_widget_set_size_request(button, windowWidth / 5.0, 200); // 调整宽度和高度

    gtk_widget_set_name(button,"inactive-clickbox");

    row2 += col2 / maxCol2;
    col2 %= maxCol2;

    gtk_grid_attach(GTK_GRID(contentGrid3), button, col2, row2, 1, 1);

    // 显示新按钮及其所有子控件
    gtk_widget_show_all(contentGrid3);

    col2++;
}

// 创建并添加网络到内容栈（保证有滚动窗口的功能）
GtkWidget * CreateAndAddGridWithScrollFuc(GtkWidget *content_stack,char * label) {
    // 创建滚动窗口
    GtkWidget *scrolledWindow = gtk_scrolled_window_new(NULL, NULL);
    gtk_scrolled_window_set_policy(GTK_SCROLLED_WINDOW(scrolledWindow), GTK_POLICY_NEVER, GTK_POLICY_AUTOMATIC);
    gtk_stack_add_titled(GTK_STACK(content_stack), scrolledWindow, label, label);

    // 创建水平盒子，用于居中对齐
    GtkWidget *hbox = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 0);
    gtk_widget_set_halign(hbox, GTK_ALIGN_CENTER); // 设置水平居中对齐

    // 创建网格
    GtkWidget *grid = gtk_grid_new();
    gtk_grid_set_row_spacing(GTK_GRID(grid), (gint)(windowWidth / 30.0));
    gtk_grid_set_column_spacing(GTK_GRID(grid), 10);
    gtk_widget_set_margin_top(grid, 10);
    gtk_widget_set_margin_bottom(grid, 10);
    gtk_widget_set_margin_start(grid, (gint)(windowWidth / 30.0));
    gtk_widget_set_margin_end(grid, (gint)(windowWidth / 30.0));

    // 将网格添加到水平盒子中
    gtk_box_pack_start(GTK_BOX(hbox), grid, FALSE, FALSE, 0);

    // 将水平盒子添加到滚动窗口中
    gtk_container_add(GTK_CONTAINER(scrolledWindow), hbox);

    gtk_widget_set_name(scrolledWindow, "scrollbar");

    return grid;
}

/*
 * 功能：搭建“发布应用”页面的框架。包含构建“返回上级目录"按钮。
 * 思路：因为“发布应用”的内容不能居中，需要滚动窗口，左上角的按钮，故单独分出一个函数。
 */
GtkWidget *CreatePublishSoftware(GtkWidget *contentStack,char *label) {

    // 创建滚动窗口
    GtkWidget *scrolledWindow = gtk_scrolled_window_new(NULL, NULL);
    gtk_scrolled_window_set_policy(GTK_SCROLLED_WINDOW(scrolledWindow), GTK_POLICY_NEVER, GTK_POLICY_AUTOMATIC);
    gtk_stack_add_titled(GTK_STACK(contentStack), scrolledWindow, label, label);

    // 创建一个主盒子，垂直方向
    GtkWidget *mainBox = gtk_box_new(GTK_ORIENTATION_VERTICAL, 0);
    gtk_container_add(GTK_CONTAINER(scrolledWindow), mainBox);

    // 创建按钮
    GtkWidget *button = gtk_button_new();
    GtkWidget *box = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 5); // 创建水平盒子

    // 加载图片并调整大小
    GdkPixbuf *pixbuf = gdk_pixbuf_new_from_file("../assets/return.png", NULL); // 加载原始图片
    GdkPixbuf *scaledPixbuf = gdk_pixbuf_scale_simple(pixbuf, (gint)(windowWidth / 30.0), (gint)(windowWidth / 30.0), GDK_INTERP_BILINEAR); // 调整大小
    GtkWidget *image = gtk_image_new_from_pixbuf(scaledPixbuf); // 使用调整后的图片创建图像控件

    // 添加图片到盒子，并设置水平和垂直居中
    gtk_box_pack_start(GTK_BOX(box), image, TRUE, TRUE, 0);

    // 添加文字到盒子
    GtkWidget *lb = gtk_label_new("返回上级目录");
    gtk_widget_set_name(lb, "head-label");
    gtk_box_pack_start(GTK_BOX(box), lb, TRUE, TRUE, 0);

    // 将盒子添加到按钮中
    gtk_container_add(GTK_CONTAINER(button), box);

    // 设置按钮的外边距
    gtk_widget_set_margin_top(button, 5);
    gtk_widget_set_margin_bottom(button, 5);
    gtk_widget_set_margin_start(button, 5);
    gtk_widget_set_margin_end(button, 5);

    // 设置按钮的大小
    gtk_widget_set_size_request(button, windowWidth * 2 / 30.0, 50);

    // 创建一个水平盒子用于左上角布局
    GtkWidget *buttonBox = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 0);
    gtk_box_pack_start(GTK_BOX(buttonBox), button, FALSE, FALSE, 0);

    // 将按钮盒子添加到主盒子中
    gtk_box_pack_start(GTK_BOX(mainBox), buttonBox, FALSE, FALSE, 0);

    // 创建水平盒子，用于左对齐
    GtkWidget *hbox = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 0);
    gtk_widget_set_halign(hbox, GTK_ALIGN_START); // 设置水平左对齐

    // 创建网格
    GtkWidget *grid = gtk_grid_new();
    gtk_grid_set_row_spacing(GTK_GRID(grid), (gint)(windowWidth / 30.0));
    gtk_grid_set_column_spacing(GTK_GRID(grid), 10);
    gtk_widget_set_margin_top(grid, 10);
    gtk_widget_set_margin_bottom(grid, 10);
    gtk_widget_set_margin_start(grid, (gint)(windowWidth / 30.0));
    gtk_widget_set_margin_end(grid, (gint)(windowWidth / 30.0));

    // 将网格添加到水平盒子中
    gtk_box_pack_start(GTK_BOX(hbox), grid, FALSE, FALSE, 0);

    // 将水平盒子添加到主盒子中
    gtk_box_pack_start(GTK_BOX(mainBox), hbox, TRUE, TRUE, 0);

    gtk_widget_set_name(scrolledWindow, "scrollbar");

    return grid;

}

// 添加已发布应用框
void AddPublishedSoftware(char * imgpath, char *name,char *alias) {
    // 创建一个新的 GtkEventBox 以便能够实现悬停效果
    GtkWidget *eventBox = gtk_event_box_new();
    gtk_widget_set_name(eventBox, "inactive-clickbox");

    // 创建一个水平的 GtkBox 并将其添加到 GtkEventBox 中
    GtkWidget *box = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 5);
    gtk_container_add(GTK_CONTAINER(eventBox), box);

    // 为 event_box 设置边距
    gtk_container_set_border_width(GTK_CONTAINER(eventBox), 10);

    // 创建图标
    GtkWidget *image = gtk_image_new_from_file(imgpath);
    gtk_box_pack_start(GTK_BOX(box), image, FALSE, FALSE, 0);

    // 为图标设置边距
    gtk_widget_set_margin_start(image, (gint)(windowWidth / 30.0));
    gtk_widget_set_margin_end(image, (gint)(windowWidth / 30.0));
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
    char t[100] = "别名";
    strcat(t,alias);
    label = gtk_label_new(t);
    gtk_widget_set_name(label, "head-label");
    gtk_box_pack_start(GTK_BOX(inbox), label, FALSE, FALSE, 0);

    // 为别名标签设置边距
    gtk_widget_set_margin_top(label, 10);
    gtk_widget_set_margin_bottom(label, 10);

    // 将 inbox 添加到主水平 box 中
    gtk_box_pack_start(GTK_BOX(box), inbox, FALSE, FALSE, 0);

    // 设置 event_box 大小
    gtk_widget_set_size_request(eventBox, (gint)(windowWidth * 3 / 8.0), 150); // 调整宽度和高度宽度和高度

    // 创建右键菜单
    GtkWidget *menu = gtk_menu_new();

    GtkWidget *menuItemOpen = gtk_menu_item_new_with_label("打开");
    // g_signal_connect(menuItemOpen, "activate", G_CALLBACK(onMenuItemActivate), "打开");
    gtk_menu_shell_append(GTK_MENU_SHELL(menu), menuItemOpen);

    GtkWidget *menuItemRemove = gtk_menu_item_new_with_label("移除");
    g_signal_connect_data(G_OBJECT(menuItemRemove), "activate", G_CALLBACK(ClickRemove), eventBox, NULL, 0);
    gtk_menu_shell_append(GTK_MENU_SHELL(menu), menuItemRemove);

    gtk_widget_show_all(menu);

    // 连接鼠标右键事件到 event_box
    g_signal_connect(eventBox, "button-press-event", G_CALLBACK(RightClickToolBar), menu);

    // 确定行列
    row3 += col3 / maxCol3;
    col3 %= maxCol3;

    // 将 event_box 添加到主 grid 中
    gtk_grid_attach(GTK_GRID(contentGrid4), eventBox, col3, row3, 1, 1);

    // 显示盒子
    gtk_widget_show_all(eventBox);

    col3++;
}

// 创建主页。因为主页没有滑动窗口，且主页需要居中对其。
GtkWidget * CreateHome(GtkWidget* contentStack,char * label) {

    // 创建垂直盒子，用于居中对齐
    GtkWidget *hbox = gtk_box_new(GTK_ORIENTATION_VERTICAL, 0);
    gtk_widget_set_halign(hbox, GTK_ALIGN_CENTER); // 设置水平居中对齐
    gtk_widget_set_valign(hbox, GTK_ALIGN_CENTER); // 设置垂直居中对齐

    // 创建网格
    GtkWidget *grid = gtk_grid_new();
    gtk_grid_set_row_spacing(GTK_GRID(grid), (gint)(windowWidth / 30.0));
    gtk_grid_set_column_spacing(GTK_GRID(grid), 10);
    gtk_widget_set_margin_top(grid, 10);
    gtk_widget_set_margin_bottom(grid, 10);
    gtk_widget_set_margin_start(grid, (gint)(windowWidth / 30.0));
    gtk_widget_set_margin_end(grid, (gint)(windowWidth / 30.0));

    // 将网格添加到水平盒子中
    gtk_box_pack_start(GTK_BOX(hbox), grid, FALSE, FALSE, 0);
    gtk_stack_add_titled(GTK_STACK(contentStack), hbox, label, label);

    return grid;
}

// 手动添加IP的盒子
void AddIPBox(GtkWidget * window) {
    // 创建按钮
    GtkWidget *button = gtk_button_new();
    GtkWidget *box = gtk_box_new(GTK_ORIENTATION_VERTICAL, 5); // 创建垂直盒子

    // 加载图片并调整大小
    GdkPixbuf *pixbuf = gdk_pixbuf_new_from_file("assets/add.png", NULL); // 加载原始图片
    GdkPixbuf *scaled_pixbuf = gdk_pixbuf_scale_simple(pixbuf, (gint)(windowWidth / 10.0), (gint)(windowWidth / 10.0), GDK_INTERP_BILINEAR); // 调整大小
    GtkWidget *image = gtk_image_new_from_pixbuf(scaled_pixbuf); // 使用调整后的图片创建图像控件

    // 添加图片到盒子，并设置水平和垂直居中
    gtk_box_pack_start(GTK_BOX(box), image, TRUE, TRUE, 0);

    // 将盒子添加到按钮中
    gtk_container_add(GTK_CONTAINER(button), box);

    // 设置按钮的外边距
    gtk_widget_set_margin_top(button, 5);
    gtk_widget_set_margin_bottom(button, 5);
    gtk_widget_set_margin_start(button, 5);
    gtk_widget_set_margin_end(button, 5);

    // 设置按钮的大小
    gtk_widget_set_size_request(button, windowWidth / 5.0, 200);

    // 设置按钮的名字，以便样式化
    gtk_widget_set_name(button,"inactive-clickbox");

    // 连接点击事件到按钮
    g_signal_connect(button, "clicked", G_CALLBACK(ClickAddIP), window);

    // 将按钮添加到网格中
    gtk_grid_attach(GTK_GRID(contentGrid2), button, 0, 0, 1, 1);

    // 清理
    g_object_unref(pixbuf);
    g_object_unref(scaled_pixbuf);
}

/*
 * 移除当前页面的所有box
 * flag:若为1，则表示0行第0列的盒子不用删除（比如局域网连接页面）；若为0，则表示全删。
 * row：总行数。
 * col：一行有多少个子组件。
*/
void RemoveAllChild(GtkWidget *grid,int row,int col,int flag) {
    int i = 0;

    for(;i<=row;i++) {
        for(int j=0;j<col;j++) {
            if(i == 0 && flag == 1 && j == 0) {
                continue;
            }
            GtkWidget *child = gtk_grid_get_child_at(GTK_GRID(grid), j, i);// 获取第i+1行第j+1列的子部件
            if (child != NULL) {
                gtk_container_remove(GTK_CONTAINER(grid), child);
            }
        }
    }

}

// 移除所有局域网盒子。用于Jeruisi外部调用
void RemoveAllLanBox() {
    RemoveAllChild(contentGrid3,row2,maxCol2,1);
}


// 移除所有已发布程序
void RemoveAllPublishedSoftware() {
    RemoveAllChild(contentGrid4,row3,maxCol3,0);
}

// 功能：设置主页为“正在连接”状态
void ConnectingHome(char * ip) {

    RemoveAllChild(contentGrid1,5,1,0);

    // 创建一个水平的盒子
    GtkWidget *box = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 5);

    // 显示"正在连接"
    GtkWidget *label = gtk_label_new("正在连接：");
    gtk_widget_set_name(label, "head-label");
    gtk_box_pack_start(GTK_BOX(box), label, FALSE, FALSE, 0);

    // 显示正在连接的IP
    label = gtk_label_new(ip);
    gtk_widget_set_name(label,"inline-label");
    gtk_box_pack_start(GTK_BOX(box), label, FALSE, FALSE, 0);

    // 将盒子放入网格
    gtk_grid_attach(GTK_GRID(contentGrid1), box, 0, 0, 1, 1);

    // 加载动画
    GtkWidget *spinner = gtk_spinner_new();
    gtk_widget_set_name(spinner,"spinner");
    gtk_widget_set_size_request(spinner, 100, 100);  // 设置spinner的宽度和高度
    gtk_spinner_start(GTK_SPINNER(spinner)); // 启动加载动画
    gtk_grid_attach(GTK_GRID(contentGrid1), spinner, 0, 1, 1, 1);

    // 显示
    gtk_widget_show_all(contentGrid1);
}

// 设置主页为未连接状态
void UnconnectHome() {

    RemoveAllChild(contentGrid1,5,1,0);

    // 显示"未连接"
    GtkWidget *label = gtk_label_new("未连接服务端");
    gtk_widget_set_name(label, "head-label");
    gtk_grid_attach(GTK_GRID(contentGrid1), label, 0, 0, 1, 1);

    // 显示
    gtk_widget_show_all(contentGrid1);

}

// 设置主页为已经连接状态
void ConnectedHome(char *ip,char *hostName) {

    RemoveAllChild(contentGrid1,5,1,0);

    // 显示"连接成功"
    GtkWidget *label = gtk_label_new("连接成功");
    gtk_widget_set_name(label, "head-label");
    gtk_grid_attach(GTK_GRID(contentGrid1), label, 0, 0, 1, 1);

    // 显示连接的IP
    char temp[100] = "IP：";
    strcat(temp,ip);
    label = gtk_label_new(temp);
    gtk_widget_set_name(label,"inline-label");
    gtk_grid_attach(GTK_GRID(contentGrid1), label, 0, 1, 1, 1);

    // 显示主机名
    char temp2[200] = "主机名：";
    strcat(temp2,hostName);
    label = gtk_label_new(temp2);
    gtk_widget_set_name(label,"inline-label");
    gtk_grid_attach(GTK_GRID(contentGrid1), label, 0, 2, 1, 1);

    // 显示
    gtk_widget_show_all(contentGrid1);

}

// 在“发布应用”界面添加一个文件夹按钮
void AddFolder(char * folderName) {

    GtkWidget *button = gtk_button_new();
    GtkWidget *box = gtk_box_new(GTK_ORIENTATION_VERTICAL, 5);

    // 加载图片并调整大小
    GdkPixbuf *pixbuf = gdk_pixbuf_new_from_file("../assets/folder.png", NULL);
    GdkPixbuf *scaledPixbuf = gdk_pixbuf_scale_simple(pixbuf, (gint)(windowWidth / 10.0), (gint)(windowWidth / 10.0), GDK_INTERP_BILINEAR);
    GtkWidget *image = gtk_image_new_from_pixbuf(scaledPixbuf);

    // 添加图片到盒子
    gtk_box_pack_start(GTK_BOX(box), image, TRUE, TRUE, 0);

    // 添加文字到盒子
    GtkWidget *label = gtk_label_new(folderName);
    gtk_box_pack_start(GTK_BOX(box), label, TRUE, TRUE, 0);

    // 将盒子添加到按钮中
    gtk_container_add(GTK_CONTAINER(button), box);

    // 设置按钮相关数据
    g_object_set_data(G_OBJECT(button), "folderName", folderName);
    gtk_widget_set_name(button,"software");
    gtk_widget_set_size_request(button, (gint)(windowWidth/7.0), 100); // 设置按钮大小

    // 清理
    g_object_unref(pixbuf);
    g_object_unref(scaledPixbuf);

    // 连接按钮事件，用于处理双击
    g_signal_connect(button, "button_press_event", G_CALLBACK(ClickFolder), NULL);

    // 使按钮接收事件
    gtk_widget_add_events(button, GDK_BUTTON_PRESS_MASK);

    // 确定行列
    row2 +=  col2 / maxCol2;
    col2 %= maxCol2;

    // 将按钮添加到网格中
    gtk_grid_attach(GTK_GRID(contentGrid3), button, col2, row2, 1, 1);

    // 显示网格
    gtk_widget_show_all(contentGrid3);

    col2++;
}

/*
 * 功能：在发布应用界面添加一个按钮
 * 参数：
 *   name：应用名称
 *   iconData：应用图标二进制流
 */
void AddSoftware(char * name,char * iconData,int iconLength) {

    GtkWidget *button = gtk_button_new();
    GtkWidget *box = gtk_box_new(GTK_ORIENTATION_VERTICAL, 5);

    // // 创建GdkPixbufLoader
    // GdkPixbufLoader *loader = gdk_pixbuf_loader_new();
    // gdk_pixbuf_loader_write(loader, (const guchar *)iconData, iconLength, NULL); // 加载数据
    // gdk_pixbuf_loader_close(loader, NULL);
    //
    // // 从loader获取GdkPixbuf
    // GdkPixbuf *pixbuf = gdk_pixbuf_loader_get_pixbuf(loader);
    //
    // // 调整图片大小
    // GdkPixbuf *scaledPixbuf = gdk_pixbuf_scale_simple(pixbuf, (gint)(windowWidth / 10.0), (gint)(windowWidth / 10.0), GDK_INTERP_BILINEAR);
    // GtkWidget *image = gtk_image_new_from_pixbuf(scaledPixbuf);
    //
    // // 添加图片到盒子
    // gtk_box_pack_start(GTK_BOX(box), image, TRUE, TRUE, 0);

    // 加载图片并调整大小
    GdkPixbuf *pixbuf = gdk_pixbuf_new_from_file("../assets/software/clion.svg", NULL);
    GdkPixbuf *scaledPixbuf = gdk_pixbuf_scale_simple(pixbuf, (gint)(windowWidth / 13.0), (gint)(windowWidth / 13.0), GDK_INTERP_BILINEAR);
    GtkWidget *image = gtk_image_new_from_pixbuf(scaledPixbuf);

    // 添加图片到盒子
    gtk_box_pack_start(GTK_BOX(box), image, TRUE, TRUE, 0);

    // 添加文字到盒子
    GtkWidget *label = gtk_label_new(name);
    gtk_box_pack_start(GTK_BOX(box), label, TRUE, TRUE, 0);

    // 将盒子添加到按钮中
    gtk_container_add(GTK_CONTAINER(button), box);

    // 设置按钮相关数据
    g_object_set_data(G_OBJECT(button), "name", name);
    gtk_widget_set_name(button,"software");
    gtk_widget_set_size_request(button, (gint)(windowWidth/7.0), 100); // 设置按钮大小

    // 创建右键菜单
    GtkWidget *menu = gtk_menu_new();

    GtkWidget *menuItemPublish = gtk_menu_item_new_with_label("发布");
    g_signal_connect_data(G_OBJECT(menuItemPublish), "activate", G_CALLBACK(CilckPublish), button, NULL, 0);
    gtk_menu_shell_append(GTK_MENU_SHELL(menu), menuItemPublish);

    gtk_widget_show_all(menu);

    // 右键出现菜单
    g_signal_connect(button, "button-press-event", G_CALLBACK(RightClickToolBar), menu);

    // 清理
    g_object_unref(pixbuf);
    g_object_unref(scaledPixbuf);

    // 确定行列
    row2 +=  col2 / maxCol2;
    col2 %= maxCol2;

    // 将按钮添加到网格中
    gtk_grid_attach(GTK_GRID(contentGrid3), button, col2, row2, 1, 1);

    // 显示网格
    gtk_widget_show_all(contentGrid3);

    col2++;
}

// 移除“发布应用”界面的所有图标
void RemoveAllSoftware() {
    RemoveAllChild(contentGrid3,row2,maxCol2,0);
}

// 显示“断开连接”按钮
void ShowUnconnectButton() {
    // 创建“断开连接”按钮
    GtkWidget *button;
    button = gtk_button_new_with_label("断开连接");
    gtk_widget_set_margin_top(button, 0);
    gtk_widget_set_margin_bottom(button, 0);
    gtk_widget_set_margin_start(button, 0);
    gtk_widget_set_margin_end(button, 0);
    gtk_widget_set_size_request(button, (gint)(windowWidth/6.0), 25); // 设置按钮大小
    g_signal_connect(button,"clicked",G_CALLBACK(ClickUnconnect),NULL);

    // 将按钮放入网格
    gtk_grid_attach(GTK_GRID(contentGrid1), button, 0, 3, 1, 1);

    // 显示
    gtk_widget_show_all(contentGrid1);
}

// 显示“重连应用端”按钮
void ShowReconnectButton() {
    // 创建“重连应用端”按钮
    GtkWidget *button;
    button = gtk_button_new_with_label("重连应用端");
    gtk_widget_set_margin_top(button, 0);
    gtk_widget_set_margin_bottom(button, 0);
    gtk_widget_set_margin_start(button, 0);
    gtk_widget_set_margin_end(button, 0);
    gtk_widget_set_size_request(button, (gint)(windowWidth/6.0), 25); // 设置按钮大小
    g_signal_connect(button,"clicked",G_CALLBACK(ClickReconnect),button);

    // 将按钮放入网格
    gtk_grid_attach(GTK_GRID(contentGrid1), button, 0, 4, 1, 1);

    // 显示
    gtk_widget_show_all(contentGrid1);
}

/* 功能：检测是否是重复的历史连接
 * 参数：
 *   ip：将要加入历史连接的 ip
 *   grid：历史记录在哪个网格中
 */
int IsRepeatedHistory(char *ip) {

    for(int i = 0;i<=row1;i++) {
        for(int j=0;j<col1;j++) {
            if(i == 0 && j == 0) {
                continue;
            }
            GtkWidget *child = gtk_grid_get_child_at(GTK_GRID(contentGrid2), j, i);// 获取第i+1行第j+1列的子部件
            if (child != NULL && strcmp(ip,g_object_get_data(G_OBJECT(child),"ip")) == 0) {
                return 1;
            }
        }
    }

    return 0;
}