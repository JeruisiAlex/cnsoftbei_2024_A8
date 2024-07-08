// 用于搭建 UI

#ifndef UI_H
#define UI_H

#include <gtk/gtk.h>

// 记录css样式
extern char * css;

/* 基本的函数 */
void OnWindowDestroy(GtkWidget *widget, gpointer data);
void LoadCss();
void createTitle();

/* 构建左侧导航栏 */
extern GtkWidget *activeButton;// 存储当前激活的按钮
void OnSwitchPage(GtkButton *button, gpointer data);
void AddBarButton(GtkWidget *contentStack, GtkWidget *sidebarBox, char *content);
void AddSeparator(GtkWidget* box);

/* 构建右侧内容栈 */
GtkWidget* CreateAndAddGrid(GtkWidget *contentStack, char *title);
void AddContent(GtkWidget *grid, char *content, int row, int col, int type);
void AddSwitchInBox(GtkWidget *box);
void AddSwitchInGrid(GtkWidget *grid, int row, int col);
void AddHistoryBox(GtkWidget *grid, char *ip, char *username, char *password, int row, int col);
void AddLanBox(GtkWidget *grid, char *ip, int row, int col);
GtkWidget * CreateAndAddGridWithScrollFuc(GtkWidget *content_stack,char * label);
void AddSoftware(GtkWidget *grid,char * imgpath ,char *name, int row, int col);
void AddPublishedSoftware(GtkWidget *grid,char * imgpath, char *name,char *alias,int row, int col);
GtkWidget* CreateHome(GtkWidget *content_stack, const char *title);

#endif //UI_H
