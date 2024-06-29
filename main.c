#include <stdio.h>
#include <freerdp/freerdp.h>
#include <freerdp/client.h>

int main(int argc, char* argv[]) {
    freerdp* instance;
    BOOL ret;

    // 创建一个 FreeRDP 实例
    instance = freerdp_new();
    if (!instance) {
        fprintf(stderr, "Failed to create FreeRDP instance.\n");
        return -1;
    }

    printf("start to init\n");
    // 初始化上下文
    instance->ContextSize = sizeof(rdpContext);
    printf("init1\n");
    instance->ContextNew = (pContextNew)freerdp_context_new;
    printf("init2\n");
    instance->ContextFree = (pContextFree)freerdp_context_free;
    printf("init3\n");
    if (!freerdp_context_new(instance)) {
        fprintf(stderr, "Failed to create FreeRDP context.\n");
        freerdp_free(instance);
        return -1;
    }

    printf("start to connect\n");
    // 设置连接参数
    instance->context->settings->ServerHostname = strdup("192.168.134.129"); // 替换为实际的服务器 IP 地址
    instance->context->settings->Username = strdup("Jeruisi");           // 替换为实际的用户名
    instance->context->settings->Password = strdup("lhty.9527");           // 替换为实际的密码
    instance->context->settings->DesktopWidth = 1920;
    instance->context->settings->DesktopHeight = 1080;

    // 连接到服务器
    ret = freerdp_connect(instance);
    if (!ret) {
        fprintf(stderr, "Failed to connect to remote desktop.\n");
        freerdp_free(instance);
        return -1;
    }

    printf("Successfully connected to remote desktop.\n");

    // 事件处理循环（这里省略具体实现）

    // 断开连接
    freerdp_disconnect(instance);
    freerdp_free(instance);

    return 0;
}