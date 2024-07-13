// 用于搭建 UI

#ifndef UI_H
#define UI_H

#include "network.h"

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
    "  font-size: 22px;"
    "}"
    "#active-button {"
    "  background-color: white;"
    "  color: black;"  // 黑色
    "  font-size: 22px;"
    "}"
    "#inactive-clickbox {"
    "  background-color: #F2F3F4;"
    "  color: black;"  // 黑色
    "}"
    "#inactive-clickbox:hover {"
    "  background-color: #D3D3D3;"  // 灰色
    "}"
    "#inline-label {"
    "  font-size: 22px;"
    "  text-decoration: none;"
    "  border-bottom: 2px dashed #85C1E9;"
    "  padding-bottom: 2px;"
    "}"
    "#classic-label {"
    "  font-size: 20px;"
    "}"
    "#head-label {"
    "  font-size: 22px;"
    "}"
    "#warning-label {"
    "  font-size: 22px;"
    "  color : red;"
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
    "}"
    "#spinner {"
    "  color: #85C1E9;"
    "}"
    "#software {"
    "  background-color: white;"
    "  color: black;"  // 黑色
    "}"
    "#software:hover {"
    "  background-color: #D3D3D3;"  // 灰色
    "}";

// 主窗口。创建错误提示框需要。
extern GtkWidget *window;

// 记录窗口宽度
extern double windowWidth;
// 记录窗口高度
extern double windowHeight;
// 设置窗口最小宽度
extern gint minWidth;
// 设置窗口最小高度
extern gint minHeight;

// 记录所有历史记录的结构体数组
extern struct NetworkInfo *historyRecords;
// 记录共有多少个历史连接记录
extern int cnt;
// 记录历史连接数据路径（相对于UI实现的.c文件的路径）
#define HISTORY_PATH "../data/history"

// 记录主页的按钮（因为连接后要跳到主页，此时应该设置主页按钮为激活状态）
extern GtkWidget *homePage;
// 记录内容栈（因为连接后要跳到主页，此时应该将内容栈跳转到主页）
extern GtkWidget *content;

void CreateUI(int argc,char *argv[]);

/* 基本的函数 */

void OnWindowDestroy(GtkWidget *widget, gpointer data);
void LoadCss();
void CreateTitle(GtkWidget* window);
GtkWidget * CreateWindow(int width,int height);
GtkWidget * CreateBoxFrame(GtkWidget *window);


/* 构建左侧导航栏 */

static GtkWidget *activeButton = NULL;// 存储当前激活的按钮
GtkWidget * CreateBar(GtkWidget *mainBox);
void OnSwitchPage(GtkButton *button, gpointer data);
GtkWidget *AddBarButton(GtkWidget *contentStack, GtkWidget *sidebarBox, char *content);
void AddSeparator(GtkWidget* box);

/* 构建右侧内容栈 */
void CreateContent(GtkWidget* window,GtkWidget* contentStack);
GtkWidget* CreateAndAddGrid(GtkWidget *contentStack, char *title);
void AddContent(GtkWidget *grid, char *content, int row, int col, int type);
void AddSwitchInBox(GtkWidget *box);
void AddSwitchInGrid(GtkWidget *grid, int row, int col);
GtkWidget * CreateAndAddGridWithScrollFuc(GtkWidget *content_stack,char * label);
GtkWidget *CreatePublishSoftware(GtkWidget *contentStack,char *label);
GtkWidget * CreateHome(GtkWidget* contentStack,char * label);
void AddIPBox(GtkWidget * window);
void RemoveAllChild(GtkWidget *grid,int row,int col,int flag);
void AddSoftware(char * name,char * iconData,int iconLength);
// 下面的函数供 Jeruisi 调用
void AddHistoryBox(char *ip, char *username, char *password);
void AddLanBox(char *ip);
void AddPublishedSoftware(char * imgpath, char *name,char *alias);
void RemoveAllLanBox();
void RemoveAllPublishedSoftware();
void RemoveAllSoftware();
void ConnectingHome(char * ip);
void UnconnectHome();
void ConnectedHome(char *ip, char *hostName);
void ErrDialog(char *content);
void AddFolder(char * folderName);
void ShowUnconnectButton();
void ShowReconnectButton();

#endif //UI_H
