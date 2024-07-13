//
// Created by TieZhu on 24-7-10.
// 用于为UI提供功能函数。
// 例如：按钮回调函数、工具函数等
//

#ifndef UIFUNC_H
#define UIFUNC_H

#include <gtk/gtk.h>

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
/* 工具函数 */
int IsValidIp(char *ip);
int AddOneHistoryRecord(char *ip,char *username,char *password);
int ReadAllHistoryRecords();
void OmitUsername(char *username,char *processedName);

#endif //UIFUNC_H
