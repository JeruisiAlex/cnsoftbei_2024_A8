// 用于搭建 UI

#ifndef UI_H
#define UI_H

#include <gtk/gtk.h>

// 记录css样式
static char * css =
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

extern double wdPercen; // 窗口宽度比例
extern double htPercen; // 窗口高度比例
extern  gint screenWidth;
extern gint screenHeight;

int CreateUI(int argc,char *argv[]);

/* 基本的函数 */

void OnWindowDestroy(GtkWidget *widget, gpointer data);
void LoadCss();
void CreateTitle(GtkWidget* window);
GtkWidget * CreateWindow();
GtkWidget * CreateBoxFrame(GtkWidget *window);


/* 构建左侧导航栏 */

static GtkWidget *activeButton = NULL;// 存储当前激活的按钮
GtkWidget * CreateBar(GtkWidget *mainBox);
void OnSwitchPage(GtkButton *button, gpointer data);
void AddBarButton(GtkWidget *contentStack, GtkWidget *sidebarBox, char *content);
void AddSeparator(GtkWidget* box);

/* 构建右侧内容栈 */
void CreateContent(GtkWidget* window,GtkWidget* contentStack);
GtkWidget* CreateAndAddGrid(GtkWidget *contentStack, char *title);
void AddContent(GtkWidget *grid, char *content, int row, int col, int type);
void AddSwitchInBox(GtkWidget *box);
void AddSwitchInGrid(GtkWidget *grid, int row, int col);
void AddHistoryBox(GtkWidget *rightBox, char *ip, char *username, char *password, int row, int col);
void AddLanBox(GtkWidget *grid, char *ip, int row, int col);
GtkWidget * CreateAndAddGridWithScrollFuc(GtkWidget *content_stack,char * label);
void AddSoftware(GtkWidget *grid,char * imgpath ,char *name, int row, int col);
void AddPublishedSoftware(GtkWidget *grid,char * imgpath, char *name,char *alias,int row, int col);
GtkWidget * CreateHome(GtkWidget* contentStack,char * label);

#endif //UI_H
