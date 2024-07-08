#include "./include/ui.h"
#include "./include/info.h"

#include <stdio.h>
#include <string.h>
#include "include/rdp.h"
#include "include/xrdp.h"
#include <stdlib.h>

GtkWidget *activeButton = NULL;
char *css =
    "#headerbar {"
    "  font-weight: bold;"
    "  font-size: 30px;"
    "  font-family: Times New Roman, sans-serif;"
    "}"
    "#sidebar {"
    "  background-color: #F5F5F5;"
    "}"
    "#inactive-button {"
    "  background-color: #F5F5F5;"
    "  color: #515A5A;" // 灰黑色
    "  font-size: 25px;"
    "}"
    "#active-button {"
    "  background-color: white;"
    "  color: black;"  // 黑色
    "  font-size: 25px;"
    "}"
    "#inactive-clickbox {"
    "  background-color: #F2F3F4;"
    "  color: black;"  // 黑色
    "}"
    "#inactive-clickbox:hover {"
    "  background-color: #D3D3D3;"  // 灰色
    "}"
    "#inline-label {"
    "  font-size: 23px;"
    "  text-decoration: none;"
    "  border-bottom: 2px dashed #85C1E9;"
    "  padding-bottom: 2px;"
    "}"
    "#classic-label {"
    "  font-size: 20px;"
    "}"
    "#head-label {"
    "  font-size: 23px;"
    "}"
    "#custom-switch {"
    "  min-width: 60px;" /* 调整宽度 */
    "  min-height: 30px;" /* 调整高度 */
    "  border-radius: 30px;" /* 调整边框半径 */
    "  background: #ccc;"
    "}"
    "#custom-switch:checked {"
    "  background: #5DADE2;"
    "}"
    "#custom-switch slider {"
    "  min-width: 30px;" /* 调整滑块宽度 */
    "  min-height: 30px;" /* 调整滑块高度 */
    "  border-radius: 30px;" /* 调整滑块边框半径 */
    "  background: white;"
    "  transition: 0.5s;"
    "}"
    "scrollbar {"
    "  background-color: #D6EAF8;"
    "}"
    "scrollbar slider {"
    "  background-color: #A0A0A0;"
    "  border-radius: 10px;"
    "  min-width: 15px;" /* 滑块的宽度 */
    "  min-height: 10px;" /* 滑块的高度 */
    "  background-color: #85C1E9;"
    "}";

char hostName[256];

int main(int argc, char *argv[]) {

    // 找到ip
    if(GetName() != 0) {
        printf("主机名获取失败，请联系丁铁柱！\n");
        return 0;
    }
    printf("HOST NAME:%s\n",hostName);

    GtkWidget *window;
    GtkWidget *header_bar;
    GtkWidget *main_box;
    GtkWidget *sidebar_box;
    GtkWidget *content_stack;
    GtkWidget *content_grid1, *content_grid2, *content_grid3, *content_grid4, *content_grid5;

    // 记录现在的行数
    int row1 = 0, row2 = 0, row3 = 0, row4 = 0,row5 = 0;

    // 初始化GTK
    gtk_init(&argc, &argv);

    // 创建主窗口
    window = gtk_window_new(GTK_WINDOW_TOPLEVEL);
    gtk_window_set_title(GTK_WINDOW(window), "Remote-operation");
    gtk_window_set_default_size(GTK_WINDOW(window), 1200, 800);
    g_signal_connect(window, "destroy", G_CALLBACK(OnWindowDestroy), NULL);

    // 创建自定义标题栏
    header_bar = gtk_header_bar_new();
    gtk_header_bar_set_title(GTK_HEADER_BAR(header_bar), "SKRO");
    gtk_header_bar_set_show_close_button(GTK_HEADER_BAR(header_bar), TRUE);
    gtk_widget_set_name(header_bar, "headerbar");
    gtk_window_set_titlebar(GTK_WINDOW(window), header_bar);

    // 创建主盒子
    main_box = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 0);
    gtk_container_add(GTK_CONTAINER(window), main_box);

    // 创建侧边栏盒子
    sidebar_box = gtk_box_new(GTK_ORIENTATION_VERTICAL, 0);
    gtk_box_set_homogeneous(GTK_BOX(sidebar_box), FALSE); // 不均匀分布
    gtk_widget_set_name(sidebar_box, "sidebar");
    gtk_box_pack_start(GTK_BOX(main_box), sidebar_box, FALSE, FALSE, 0);

    // 创建滚动窗口
    GtkWidget *scrolled_window = gtk_scrolled_window_new(NULL, NULL);
    gtk_scrolled_window_set_policy(GTK_SCROLLED_WINDOW(scrolled_window), GTK_POLICY_AUTOMATIC, GTK_POLICY_AUTOMATIC);
    gtk_box_pack_start(GTK_BOX(main_box), scrolled_window, TRUE, TRUE, 0);

    // 创建内容堆栈
    content_stack = gtk_stack_new();
    gtk_stack_set_transition_type(GTK_STACK(content_stack), GTK_STACK_TRANSITION_TYPE_SLIDE_LEFT_RIGHT);
    gtk_container_add(GTK_CONTAINER(scrolled_window), content_stack);

    // 添加侧边栏按钮
    AddSeparator(sidebar_box);
    AddBarButton(content_stack, sidebar_box, "主页");
    AddSeparator(sidebar_box);
    AddBarButton(content_stack, sidebar_box, "主机信息");
    AddSeparator(sidebar_box);
    AddBarButton(content_stack, sidebar_box, "局域网连接");
    AddSeparator(sidebar_box);
    AddBarButton(content_stack, sidebar_box, "应用程序");
    AddSeparator(sidebar_box);
    AddBarButton(content_stack, sidebar_box, "发布程序");
    AddSeparator(sidebar_box);

    // 使用函数创建并添加网格
    content_grid1 = CreateHome(content_stack, "主页");
    content_grid2 = CreateAndAddGrid(content_stack, "主机信息");
    content_grid3 = CreateAndAddGridWithScrollFuc(content_stack, "局域网连接");
    content_grid4 = CreateAndAddGridWithScrollFuc(content_stack, "应用程序");
    content_grid5 = CreateAndAddGridWithScrollFuc(content_stack, "发布程序");

    // 添加内容到历史连接
    for(row1;row2<10;row2++) {
        AddHistoryBox(content_grid1, "192.168.0.1", "用户名: admin", "密码: ******", row1, 0);
        AddHistoryBox(content_grid1, "192.168.0.2", "用户名: user", "密码: ******", row1, 1);
        AddHistoryBox(content_grid1, "192.168.0.3", "用户名: TieZhu", "密码: ******", row1, 3);
        AddHistoryBox(content_grid1, "192.168.0.4", "用户名: pp", "密码: ******", row1, 4);
    }

    // 添加内容到主机信息
    AddContent(content_grid2, "主机名：", row2, 0, 0);
    AddContent(content_grid2, hostName, row2, 1, -1);
    row2++;
    AddContent(content_grid2, "端口：", row2, 0, 0);
    AddContent(content_grid2, PORT, row2, 1, -1);
    // add_content(content_grid1, "开机启动：", row2, 0, 0);
    // add_switch(content_grid1, row2, 1); // 添加 switch
    // row2++;

    // 添加内容到局域网连接
    AddLanBox(content_grid3,"IP：192.168.0.5",row3,0);
    AddLanBox(content_grid3,"IP：192.168.0.5",row3,1);

    // 添加内容到应用程序
    AddSoftware(content_grid4,"../assets/software/clion.svg","Clion",row4,0);

    // 添加内容到发布程序
    AddPublishedSoftware(content_grid5,"../assets/software/clion.svg","Clion 2024 2.4","别名：Clion",row5,0);

    // 加载CSS
    LoadCss();

    // 为侧边栏添加样式类
    GtkStyleContext *context = gtk_widget_get_style_context(sidebar_box);
    gtk_style_context_add_class(context, "sidebar");

    // 显示所有组件
    gtk_widget_show_all(window);

    // 进入GTK主循环
    gtk_main();

    return 0;
}