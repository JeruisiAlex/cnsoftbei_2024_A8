#include "../../include/ui.h"
#include "../../include/err.h"
#include "../../include/network.h"

#include <regex.h>

double windowWidth;
double windowHeight;

struct NWInfo *historyRecords = NULL;
int cnt = 0;

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
        // 输入合法，执行相关操作
        printf("%s\n",username);
        AddOneHistoryRecord(ip,username,password);
        AddHistoryBox(ip,username,password);
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

    struct NWInfo info[200];
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

    historyRecords = (struct NWInfo *)malloc(cnt * sizeof(struct NWInfo));
    memcpy(historyRecords,info,cnt * sizeof(struct NWInfo));

    fclose(file);
    return 0;
}