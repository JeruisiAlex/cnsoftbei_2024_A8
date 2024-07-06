#ifndef UI_H
#define UI_H

#include <gtk/gtk.h>

// 记录css样式
extern char * CSS;

/* 基本的函数 */
void on_window_destroy(GtkWidget *widget, gpointer data);
void load_css();

/* 构建左侧导航栏 */
extern GtkWidget *active_button;// 存储当前激活的按钮
void on_switch_page(GtkButton *button, gpointer data);
void add_bar_button(GtkWidget *content_stack, GtkWidget *sidebar_box, char *content);

/* 构建右侧内容栈 */
GtkWidget* create_and_add_grid(GtkWidget *content_stack, char *title);
void add_content(GtkWidget *grid, char *content, int row, int col, int type);
void add_switch_in_box(GtkWidget *box);
void add_switch_in_grid(GtkWidget *grid, int row, int col);
void add_history_box(GtkWidget *grid, char *ip, char *username, char *password, int row, int col);
void add_lan_box(GtkWidget *grid, char *ip, int row, int col);
GtkWidget * create_and_add_grid_with_scrollfuc(GtkWidget *content_stack,char * label);
void add_software(GtkWidget *grid,char * imgpath ,char *name, int row, int col);
void add_published_software(GtkWidget *grid,char * imgpath, char *name,char *alias,int row, int col);

#endif //UI_H
