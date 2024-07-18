//
// Created by TieZhu on 24-7-10.
// 用于为UI提供功能函数。
// 例如：按钮回调函数、工具函数等
//

#ifndef UIFUNC_H
#define UIFUNC_H

#include <gtk/gtk.h>

#define tutorialPath "/usr/local/share/SKRO/assets/用户手册.pdf"

/* 按钮的回调函数 */
void ClickAddIP(GtkWidget *widget, gpointer user_data);
void ClickConfirm(GtkWidget *widget, gpointer dialog);
gboolean RightClickToolBar(GtkWidget *widget, GdkEventButton *event, gpointer data);
void ClickUnconnect(GtkWidget *widget, gpointer data);
void ClickReconnect(GtkWidget *widget, gpointer data);
gboolean ClickFolder(GtkWidget *widget, GdkEventButton *event, gpointer userData);
void ClickRemove(GtkWidget *menuitem, GtkWidget *eventBox);
void CilckPublish(GtkMenuItem *menuitem, gpointer userData);
void ClickHistory(GtkWidget *widget, gpointer data);
void ClickChangeShareFolder(GtkWidget *widget, gpointer data);
void ClickOpenTutorial(GtkWidget *widget, GdkEventButton *event, gpointer data);
/* 工具函数 */
int IsValidIp(char *ip);
/********** 供Jeruisi调用 ************/
int AddOneHistoryRecord(char *ip,char *username,char *password);
/***********************************/
int ReadAllHistoryRecords();
void OmitString(char *src,char *dst,int maxlen);

#endif //UIFUNC_H
