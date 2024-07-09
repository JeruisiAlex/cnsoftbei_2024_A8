#include "../../include/ui.h"

#include <arpa/inet.h>

double windowWidth;
double windowHeight;

GtkWidget *ipEntry;
GtkWidget *usernameEntry;
GtkWidget *passwordEntry;
GtkWidget *errorLabel;

// 用于手动添加IP
void ClickAddIP(GtkWidget *widget, gpointer data) {
    GtkWidget *dialog = gtk_dialog_new_with_buttons(NULL,
                                                    GTK_WINDOW(data),
                                                    GTK_DIALOG_MODAL | GTK_DIALOG_DESTROY_WITH_PARENT,
                                                    NULL);

    // 创建并设置自定义标题栏
    GtkWidget *headerBar = gtk_header_bar_new();
    gtk_header_bar_set_show_close_button(GTK_HEADER_BAR(headerBar), TRUE);
    gtk_header_bar_set_title(GTK_HEADER_BAR(headerBar), "SKRO");
    gtk_widget_set_name(headerBar, "headbar");
    gtk_window_set_titlebar(GTK_WINDOW(dialog), headerBar);

    // 设置对话框的大小
    gtk_window_set_default_size(GTK_WINDOW(dialog), (gint)(windowWidth / 2.0), (gint) (windowHeight / 3.0));

    GtkWidget *contentArea = gtk_dialog_get_content_area(GTK_DIALOG(dialog));

    // 创建主盒子
    GtkWidget *mainBox = gtk_box_new(GTK_ORIENTATION_VERTICAL, 10);
    gtk_container_add(GTK_CONTAINER(contentArea), mainBox);

    // // 创建标签并添加到主盒子中
    // GtkWidget *label = gtk_label_new("请输入信息");
    // gtk_widget_set_halign(label, GTK_ALIGN_START); // 设置标签居左
    // gtk_box_pack_start(GTK_BOX(main_box), label, FALSE, FALSE, 0);
    // gtk_widget_set_name(label,"head-label");

    // 创建IP输入框及其标签
    GtkWidget *ipBox = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 10); // 水平布局，间距10
    GtkWidget *ipLabel = gtk_label_new("IP：");
    ipEntry = gtk_entry_new();
    gtk_entry_set_placeholder_text(GTK_ENTRY(ipEntry), "IP");
    gtk_box_pack_start(GTK_BOX(ipBox), ipLabel, FALSE, FALSE, 0);
    gtk_box_pack_start(GTK_BOX(ipBox), ipEntry, TRUE, TRUE, 0);
    gtk_widget_set_margin_top(ipBox, 5); // 设置顶部边距
    gtk_widget_set_margin_bottom(ipBox, 5); // 设置底部边距
    gtk_widget_set_margin_start(ipBox, 5); // 设置顶部边距
    gtk_widget_set_margin_end(ipBox, 5); // 设置底部边距
    gtk_widget_set_name(ipLabel,"head-label");

    // 创建用户名输入框及其标签
    GtkWidget *usernameBox = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 10);
    GtkWidget *usernameLabel = gtk_label_new("用户名：");
    usernameEntry = gtk_entry_new();
    gtk_entry_set_placeholder_text(GTK_ENTRY(usernameEntry), "Username");
    gtk_box_pack_start(GTK_BOX(usernameBox), usernameLabel, FALSE, FALSE, 0);
    gtk_box_pack_start(GTK_BOX(usernameBox), usernameEntry, TRUE, TRUE, 0);
    gtk_widget_set_margin_top(usernameBox, 5); // 设置顶部边距
    gtk_widget_set_margin_bottom(usernameBox, 5); // 设置底部边距
    gtk_widget_set_margin_start(usernameBox, 5); // 设置顶部边距
    gtk_widget_set_margin_end(usernameBox, 5); // 设置底部边距
    gtk_widget_set_name(usernameLabel,"head-label");

    // 创建密码输入框及其标签
    GtkWidget *passwordBox = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 10);
    GtkWidget *passwordLabel = gtk_label_new("密码：");
    passwordEntry = gtk_entry_new();
    gtk_entry_set_visibility(GTK_ENTRY(passwordEntry), FALSE); // 隐藏输入的字符
    gtk_entry_set_invisible_char(GTK_ENTRY(passwordEntry), '*'); // 设置不可见字符为星号
    gtk_entry_set_placeholder_text(GTK_ENTRY(passwordEntry), "Password");
    gtk_box_pack_start(GTK_BOX(passwordBox), passwordLabel, FALSE, FALSE, 0);
    gtk_box_pack_start(GTK_BOX(passwordBox), passwordEntry, TRUE, TRUE, 0);
    gtk_widget_set_margin_top(passwordBox, 5); // 设置顶部边距
    gtk_widget_set_margin_bottom(passwordBox, 5); // 设置底部边距
    gtk_widget_set_margin_start(passwordBox, 5); // 设置顶部边距
    gtk_widget_set_margin_end(passwordBox, 5); // 设置底部边距
    gtk_widget_set_name(passwordLabel,"head-label");

    // 将输入框组添加到主盒子
    gtk_box_pack_start(GTK_BOX(mainBox), ipBox, FALSE, FALSE, 0);
    gtk_box_pack_start(GTK_BOX(mainBox), usernameBox, FALSE, FALSE, 0);
    gtk_box_pack_start(GTK_BOX(mainBox), passwordBox, FALSE, FALSE, 0);

    // 创建用于显示错误信息的标签
    errorLabel = gtk_label_new("");
    gtk_widget_set_name(errorLabel, "error-label");
    gtk_box_pack_start(GTK_BOX(mainBox), errorLabel, FALSE, FALSE, 0);
    gtk_widget_set_name(errorLabel,"warning-label");

    // 创建自定义确认按钮
    GtkWidget *confirmButton = gtk_button_new_with_label("确认");
    gtk_widget_set_size_request(confirmButton, (gint) (windowHeight / 3.0), 30); // 设置按钮大小

    // 创建一个水平盒子，用于将按钮放在右下角
    GtkWidget *buttonBox = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 0);
    gtk_box_pack_end(GTK_BOX(buttonBox), confirmButton, FALSE, FALSE, 5); // 将按钮放在右边，并设置边距

    // 将按钮盒子添加到主盒子
    gtk_box_pack_end(GTK_BOX(mainBox), buttonBox, FALSE, FALSE, 5);

    // 连接自定义确认按钮的回调函数
    g_signal_connect(confirmButton, "clicked", G_CALLBACK(ClickConfirm), dialog);

    // 显示所有控件
    gtk_widget_show_all(dialog);

    // 运行对话框
    gtk_dialog_run(GTK_DIALOG(dialog));
    
}

void ClickConfirm(GtkWidget *widget, gpointer data) {
    GtkWidget *dialog = GTK_WIDGET(data);

    // 获取输入框的内容
    const char *ip = gtk_entry_get_text(GTK_ENTRY(ipEntry));
    const char *username = gtk_entry_get_text(GTK_ENTRY(usernameEntry));
    const char *password = gtk_entry_get_text(GTK_ENTRY(passwordEntry));

    if (strlen(ip) > 0 && IsValidIpv(ip) && strlen(username) > 0 && strlen(username) <= 20 && strlen(password) > 0 && strlen(password) <= 127) {
        // 输入合法，执行相关操作
        gtk_widget_destroy(dialog); // 销毁对话框
    } else {
        gtk_label_set_text(GTK_LABEL(errorLabel), "输入不合法");
        gtk_widget_show(errorLabel); // 确保显示标签
    }
}


int IsValidIpv(const char *ip) {

    // 检验是否是合法的ipv4
    struct sockaddr_in sa1;
    int result = inet_pton(AF_INET, ip, &(sa1.sin_addr));
    // inet_pton（地址到文本表示的转换）函数将点分十进制的IPv4地址转换为二进制形式，如果转换成功，返回值将为正数。
    if(result == 0) {
        return 0;
    }

    // 检验是否是合法的ipv6
    struct sockaddr_in6 sa2;
    result = inet_pton(AF_INET6, ip, &(sa2.sin6_addr));
    return result != 0;
}
