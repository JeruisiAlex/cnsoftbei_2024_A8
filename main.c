#include "./include/ui.h"

GtkWidget * active_button = NULL;
char * CSS =
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
    "}";

int main(int argc, char *argv[]) {

    GtkWidget *window;
    GtkWidget *header_bar;
    GtkWidget *main_box;
    GtkWidget *sidebar_box;
    GtkWidget *content_stack;
    GtkWidget *content_grid1, *content_grid2, *content_grid3, *content_grid4, *content_grid5;
    GtkCssProvider *provider;
    GdkDisplay *display;
    GdkScreen *screen;

    // 记录现在的行数
    int row1 = 0, row2 = 0, row3 = 0, row4 = 0;

    // 初始化GTK
    gtk_init(&argc, &argv);

    // 创建主窗口
    window = gtk_window_new(GTK_WINDOW_TOPLEVEL);
    gtk_window_set_title(GTK_WINDOW(window), "Remote-operation");
    gtk_window_set_default_size(GTK_WINDOW(window), 1200, 800);
    g_signal_connect(window, "destroy", G_CALLBACK(on_window_destroy), NULL);

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

    // 创建内容堆栈
    content_stack = gtk_stack_new();
    gtk_stack_set_transition_type(GTK_STACK(content_stack), GTK_STACK_TRANSITION_TYPE_SLIDE_LEFT_RIGHT);
    gtk_box_pack_start(GTK_BOX(main_box), content_stack, TRUE, TRUE, 0);

    // 添加侧边栏按钮
    add_bar_button(content_stack, sidebar_box, "主机信息");
    add_bar_button(content_stack, sidebar_box, "历史连接");
    add_bar_button(content_stack, sidebar_box, "局域网连接");
    add_bar_button(content_stack, sidebar_box, "应用程序");
    add_bar_button(content_stack, sidebar_box, "发布程序");

    // 使用函数创建并添加网格
    content_grid1 = create_and_add_grid(content_stack, "主机信息");
    content_grid2 = create_and_add_grid(content_stack, "历史连接");
    content_grid3 = create_and_add_grid(content_stack, "局域网连接");
    content_grid4 = create_and_add_grid(content_stack, "应用程序");
    content_grid5 = create_and_add_grid(content_stack, "发布程序");

    // 添加内容到主机信息
    add_content(content_grid1, "IP：", row1, 0, 0);
    add_content(content_grid1, "192.168.112.128", row1, 1, -1);
    row1++;
    add_content(content_grid1, "端口：", row1, 0, 0);
    add_content(content_grid1, "1314", row1, 1, -1);
    // add_content(content_grid1, "开机启动：", row1, 0, 0);
    // add_switch(content_grid1, row1, 1); // 添加 switch
    // row1++;

    // 添加内容到历史连接
    add_history_box(content_grid2, "192.168.0.1", "用户名: admin", "密码: 123456", row2, 0);
    add_history_box(content_grid2, "192.168.0.2", "用户名: user", "密码: password", row2, 1);

    // 添加内容到局域网连接
    add_lan_box(content_grid3,"192.168.0.5",row3,0);
    add_lan_box(content_grid3,"192.168.0.5",row3,1);

    // 添加内容到应用程序

    // 添加内容到发布程序

    // 创建并加载CSS提供者
    provider = gtk_css_provider_new();
    gtk_css_provider_load_from_data(GTK_CSS_PROVIDER(provider),CSS, -1, NULL);

    // 获取屏幕和显示
    display = gdk_display_get_default();
    screen = gdk_display_get_default_screen(display);

    // 添加CSS样式到屏幕
    gtk_style_context_add_provider_for_screen(screen, GTK_STYLE_PROVIDER(provider), GTK_STYLE_PROVIDER_PRIORITY_USER);

    // 为侧边栏添加样式类
    GtkStyleContext *context = gtk_widget_get_style_context(sidebar_box);
    gtk_style_context_add_class(context, "sidebar");

    // 显示所有组件
    gtk_widget_show_all(window);

    // 进入GTK主循环
    gtk_main();

    return 0;
}
