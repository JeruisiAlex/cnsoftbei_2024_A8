//
// Created by Jeruisi on 24-7-5.
//

#ifndef RDP_H
#define RDP_H
struct SET{
    char *address;
    char *username;
    char *password;
} set;
int initialization(char *address, char *username, char * password);
int open();

#endif //RDP_H
