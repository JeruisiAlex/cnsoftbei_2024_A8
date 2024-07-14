#include "../../include/ui.h"
#include "../../include/err.h"
#include "../../include/network.h"
#include "../../include/uifunc.h"
#include "../../include/kernel.h"

#include <regex.h>
#include <gdk/gdk.h>

double windowWidth;
double windowHeight;

struct NetworkInfo *historyRecords = NULL;
int cnt = 0;

GtkWidget *window;

GtkWidget *homePage;
GtkWidget *content;

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

// 点击确认按钮后，检查IP、用户名、密码格式，并调用连接函数
void ClickConfirm(GtkWidget *widget, gpointer data) {
    GtkWidget *dialog = GTK_WIDGET(data);

    // 获取输入框的内容
    char ip[100];
    strcpy(ip,gtk_entry_get_text(GTK_ENTRY(ipEntry)));
    char username[100];
    strcpy(username, gtk_entry_get_text(GTK_ENTRY(usernameEntry)));
    char password[200];
    strcpy(password,gtk_entry_get_text(GTK_ENTRY(passwordEntry)));

    if(!IsValidIp(ip)) {
        gtk_label_set_text(GTK_LABEL(errorLabel), "IP不合法");
        gtk_widget_show(errorLabel); // 确保显示标签
    }
    else if(strlen(username) == 0 || strlen(username) > 20) {
        gtk_label_set_text(GTK_LABEL(errorLabel), "用户名长度不合法");
        gtk_widget_show(errorLabel); // 确保显示标签
    }
    else if(strlen(password) == 0 || strlen(password) > 127) {
        gtk_label_set_text(GTK_LABEL(errorLabel), "密码长度不合法");
        gtk_widget_show(errorLabel); // 确保显示标签
    }
    else {

        // 跳转到主页
        OnSwitchPage(GTK_BUTTON(homePage),content);

        // 开始连接
        pthread_mutex_lock(&isConnectMutex);
        isConnect = 1;
        pthread_mutex_unlock(&isConnectMutex);



        // 调用网络层连接
        SetNetworkInfo(ip,username,password);
        if(ConnectToServer() == 1) {
            UnconnectHome();

            pthread_mutex_lock(&isConnectMutex);
            isConnect = 0;
            pthread_mutex_unlock(&isConnectMutex);

            ErrDialog("连接失败！");
        }
        else{
            // 输入合法，执行相关操作
            AddOneHistoryRecord(ip,username,password);
            AddHistoryBox(ip,username,password);
        }

        gtk_widget_destroy(dialog); // 销毁对话框
    }
}

// 检查IP是否合法
int IsValidIp(char *ip) {
    char *ip_regex = "^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\."
                       "(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\."
                       "(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\."
                       "(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
    regex_t regex;
    int ret;

    // 编译正则表达式
    ret = regcomp(&regex, ip_regex, REG_EXTENDED);
    if (ret) {
        return 0;
    }

    // 执行正则表达式匹配
    ret = regexec(&regex, ip, 0, NULL, 0);
    regfree(&regex);

    if (!ret) {
        return 1; // 匹配成功
    }

    return 0; // 不匹配

}

// 右键点击发布应用，会出现“打开”、“卸载”的选择
gboolean RightClickToolBar(GtkWidget *widget, GdkEventButton *event, gpointer data) {
    // 判断是否为鼠标右键点击
    if (event->button == GDK_BUTTON_SECONDARY) {
        GtkWidget *menu = GTK_WIDGET(data);
        gtk_menu_popup_at_pointer(GTK_MENU(menu), (GdkEvent *)event);
        return TRUE; // 阻止事件继续传播
    }
    return FALSE;
}

// 在记录中追加一个历史连接记录
int AddOneHistoryRecord(char *ip,char *username,char *password) {

    FILE *file = fopen(HISTORY_PATH,"ab");
    if(file == NULL) {
        printf("RECORDS ERR:HISTORY NOT FOUND!\n");
        return HISTORY_NF;
    }

    // 写入IP
    fwrite(ip, strlen(ip) + 1, 1, file);

    // 写入username
    fwrite(username, strlen(username) + 1, 1, file);

    // 写入password
    fwrite(password, strlen(password) + 1, 1, file);

    fclose(file);
    return 0;
}

// 从数据中，读出所有的历史连接记录
int ReadAllHistoryRecords() {

    FILE* file = fopen(HISTORY_PATH, "rb");
    if (file == NULL) {
        printf("RECORDS ERR:HISTORY NOT FOUND!\n");
        return HISTORY_NF;
    }

    struct NetworkInfo info[200];
    int i = 0; // 0:正在读IP 1:正在读用户名 2:正在读密码
    int j = 0; // 正在读第几个字符
    char ch = '\0';

    while(fread(&ch,1,1,file) == 1) {

        while (ch != '\0') {
            if(i == 0) {
                info[cnt].address[j] = ch;
            }
            else if(i == 1) {
                info[cnt].username[j] = ch;
            }
            else {
                info[cnt].password[j] = ch;
            }
            j++;
            fread(&ch,1,1,file);
        }

        if(i == 0) {
            info[cnt].address[j] = ch;
        }
        else if(i == 1) {
            info[cnt].username[j] = ch;
        }
        else {
            info[cnt].password[j] = ch;
        }
        i++;
        j = 0;
        if(i == 3) {
            i = 0;
            cnt++;
        }

    }

    historyRecords = (struct NetworkInfo *)malloc(cnt * sizeof(struct NetworkInfo));
    memcpy(historyRecords,info,cnt * sizeof(struct NetworkInfo));

    fclose(file);
    return 0;
}

// 点击“断开连接”按钮的回调函数
void ClickUnconnect(GtkWidget *widget, gpointer data) {
    UnconnectHome();
    DisconnectToServer();
}

/*
 * 功能：省略部分用户名。
 *
 * 逻辑：如果用户名长度小于等于5，则不处理。其他情况下，只显示前5个字母，之后跟省略号
 */
void OmitUsername(char *username,char *processedName) {

    int len = strlen(username);

    if (len > 5) {
        memcpy(processedName, username, 5);
        strcpy(processedName + 5, "...");
    } else {
        strcpy(processedName,username);
    }

}

// 弹出错误框
void ErrDialog(char *content) {

    GtkWidget *dialog = gtk_dialog_new_with_buttons(NULL,
                                                    GTK_WINDOW(window),
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
    GtkWidget *mainBox = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 10);
    gtk_container_add(GTK_CONTAINER(contentArea), mainBox);

    // 加载并缩放图像
    GError *error = NULL;
    GdkPixbuf *pixbuf = gdk_pixbuf_new_from_file("assets/err.png", &error);
    if (!error) {
        GdkPixbuf *scaled_pixbuf = gdk_pixbuf_scale_simple(pixbuf, (gint) (windowHeight / 6.0), (gint) (windowHeight / 6.0), GDK_INTERP_BILINEAR);
        GtkWidget *image = gtk_image_new_from_pixbuf(scaled_pixbuf);
        gtk_box_pack_start(GTK_BOX(mainBox), image, FALSE, FALSE, 0);
        g_object_unref(scaled_pixbuf); // 释放缩放后的 Pixbuf

        // 为图标设置边距
        gtk_widget_set_margin_start(image, (gint)(windowWidth / 30.0));
        gtk_widget_set_margin_end(image, (gint)(windowWidth / 30.0));
        gtk_widget_set_margin_top(image, 10);
        gtk_widget_set_margin_bottom(image, 10);

    } else {
        printf("IMAGE NOT FOUND FROM ERR DIALOG!");
        return;
    }
    g_object_unref(pixbuf); // 释放原始 Pixbuf

    // 创建提示信息
    GtkWidget *label = gtk_label_new(content);
    gtk_widget_set_margin_top(label, 5); // 设置顶部边距
    gtk_widget_set_margin_bottom(label, 5); // 设置底部边距
    gtk_widget_set_margin_start(label, 5); // 设置顶部边距
    gtk_widget_set_margin_end(label, 5); // 设置底部边距
    gtk_widget_set_name(label,"warning-label");
    gtk_box_pack_start(GTK_BOX(mainBox),label,FALSE,FALSE,0);

    // 显示所有控件
    gtk_widget_show_all(dialog);

    // 运行对话框
    gtk_dialog_run(GTK_DIALOG(dialog));

    // 用户响应后销毁对话框
    gtk_widget_destroy(dialog);

}


static guint last_click_time = 0;

// 双击文件夹
gboolean ClickFolder(GtkWidget *widget, GdkEventButton *event, gpointer userData) {
    if (event->type == GDK_BUTTON_PRESS && event->button == 1) {
        guint time = gtk_get_current_event_time();
        if (time - last_click_time <= 250) {  // 检测双击
            const gchar *folderName = (const gchar *)g_object_get_data(G_OBJECT(widget), "folderName");
            g_print("双击事件：文件夹名称是 %s\n", folderName);
            last_click_time = 0;  // 重置时间
        } else {
            last_click_time = time;  // 更新上次点击时间
        }
    }
    return FALSE;  // 允许其他事件继续处理
}

// 用于已发布应用的移除
void ClickRemove(GtkWidget *menuitem, GtkWidget *eventBox) {
    gtk_widget_hide(eventBox);  // 隐藏 widget，可选，确保它不再显示
    gtk_container_remove(GTK_CONTAINER(gtk_widget_get_parent(eventBox)), eventBox);  // 从其父容器中移除
}

// 发布应用
void CilckPublish(GtkMenuItem *menuitem, gpointer data) {
    GtkWidget *button = GTK_WIDGET(data);  // 将user_data转换回GtkWidget类型
    const gchar *name = g_object_get_data(G_OBJECT(button), "name");  // 获取存储的name
    g_print("发布事件：应用名称是 %s\n", name);
}

// 点击历史连接盒子
void ClickHistory(GtkWidget *widget, gpointer data) {

    // 跳转到主页
    OnSwitchPage(GTK_BUTTON(homePage),content);

    pthread_mutex_lock(&isConnectMutex);
    isConnect = 1;
    pthread_mutex_unlock(&isConnectMutex);

    struct NetworkInfo *info = (struct NetworkInfo *)data;
    SetNetworkInfo(info->address,info->username,info->password);
    if(ConnectToServer() == 1) {
        UnconnectHome();

        pthread_mutex_lock(&isConnectMutex);
        isConnect = 0;
        pthread_mutex_unlock(&isConnectMutex);

        ErrDialog("连接失败！");
    }
    free(info);
}

void ClickReconnect(GtkWidget *widget, gpointer data) {
    ReConnectToRemoteApp();
    // 移除并销毁按钮
    gtk_widget_destroy(data);
}