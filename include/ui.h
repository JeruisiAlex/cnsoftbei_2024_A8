#ifndef UI_H
#define UI_H

#include <gtk/gtk.h>

// 记录css样式
extern char * CSS;
void on_window_destroy(GtkWidget *widget, gpointer data);

// 存储当前激活的按钮
extern GtkWidget *active_button;
void on_switch_page(GtkButton *button, gpointer data);
void add_bar_button(GtkWidget *content_stack, GtkWidget *sidebar_box, const char *content);

GtkWidget* create_and_add_grid(GtkWidget *content_stack, const char *title);
void add_content(GtkWidget *grid, const char *content, int row, int col, int type);
void add_switch(GtkWidget *grid, int row, int col);
void add_history_box(GtkWidget *grid, const char *ip, const char *username, const char *password, int row, int col);
void add_lan_box(GtkWidget *grid, const char *ip, int row, int col);


#endif //UI_H
