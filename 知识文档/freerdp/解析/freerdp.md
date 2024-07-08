# freerdp.h

freerdp的主要文件，很多入口用的结构体，包括连接的初始化都是在这个头文件定义的。

## 一些结构体的重命名

其中最重要的应该是 `freerdp` 与  `rdpContext` 这两个结构体

```c
typedef struct rdp_rdp rdpRdp;
typedef struct rdp_gdi rdpGdi;
typedef struct rdp_rail rdpRail;
typedef struct rdp_cache rdpCache;
typedef struct rdp_channels rdpChannels;
typedef struct rdp_graphics rdpGraphics;
typedef struct rdp_metrics rdpMetrics;
typedef struct rdp_codecs rdpCodecs;

typedef struct rdp_freerdp freerdp; // freerdp远程连接启动的入口结构体
typedef struct rdp_context rdpContext; // 用于定义 freerdp 上下文的结构体
typedef struct rdp_freerdp_peer freerdp_peer;

typedef struct rdp_client_context rdpClientContext;
typedef struct rdp_client_entry_points_v1 RDP_CLIENT_ENTRY_POINTS_V1;
typedef RDP_CLIENT_ENTRY_POINTS_V1 RDP_CLIENT_ENTRY_POINTS;
```

## 证书回调的标志名



```C
#define VERIFY_CERT_FLAG_NONE 0x00
#define VERIFY_CERT_FLAG_LEGACY 0x02
#define VERIFY_CERT_FLAG_REDIRECT 0x10
#define VERIFY_CERT_FLAG_GATEWAY 0x20
#define VERIFY_CERT_FLAG_CHANGED 0x40
#define VERIFY_CERT_FLAG_MISMATCH 0x80
#define VERIFY_CERT_FLAG_MATCH_LEGACY_SHA1 0x100
```

## struct freerdp

该结构体由 rdp 的**客户端**进行建立，使用 **freerdp_new()** 进行分配，使用 **freerdp_free()** 进行释放。下面的偏移量单位应该是 64 位，即8字节。

- `ALIGN64`：按照8字节进行对齐。
- `UINT64`：unsigned long int，根据结构体可以推测本处使用的大小应该是 64 位
- `DWORD`：unsigned int

```C
struct rdp_freerdp{
		ALIGN64 rdpContext* context; // (偏移量0) 指向 rdpContext 的指针，这允许客户端使用其他上下文信息，具体见 rdpContext 结构体的解析，使用 freerdp_context_new() 进行初始化，使用 freerdp_context_free() 进行释放。
		ALIGN64 RDP_CLIENT_ENTRY_POINTS* pClientEntryPoints; // (偏移量1) 指向 rdp_client_entry_points_v1 结构体的指针，该结构体位于 client.h 中，具体作用暂时未知。
		UINT64 paddingA[16 - 2]; // 用于填充对齐
		ALIGN64 rdpInput* input; // (偏移量16) 连接的输入句柄，通过调用 freerdp_context_new() 初始化。
		ALIGN64 rdpUpdate* update; // (偏移量17) 更新显示参数,用于注册显示事件回调和设置,通过调用 freerdp_context_new() 初始化。
		ALIGN64 rdpSettings* settings; // (偏移量18) 指向 rdpSettings 结构的指针，用于维护所需的 RDP 设置，通过调用 freerdp_context_new() 初始化。
		ALIGN64 rdpAutoDetect* autodetect; // (偏移量19) 连接的自动检测句柄，通过调用 freerdp_context_new() 初始化。
		ALIGN64 rdpHeartbeat* heartbeat; // (偏移量21)？ 
		UINT64 paddingB[32 - 21]; // 用于填充对齐 
		ALIGN64 size_t ContextSize; // (偏移量32) 设置 context 的大小，freerdp_context_new() 将使用此大小分配上下文缓冲区。如果没有特殊要求，应该是 sizeof(rdpContext) 的大小，如果有特殊要求，也应该至少是 sizeof(rdpContext) 的信息。
		ALIGN64 pContextNew ContextNew; // (偏移量33)上下文分配的回调函数。可以在调用 freerdp_context_new() 之前设置，以便在分配和初始化后执行。如果不需要，必须设置为 NULL。
		ALIGN64 pContextFree ContextFree; // (偏移量34) 上下文释放的回调函数。可以在调用 freerdp_context_free() 之前设置，以便在释放之前执行。如果不需要，必须设置为 NULL。
		UINT64 paddingC[47 - 35]; // 填充字段
		ALIGN64 UINT ConnectionCallbackState; 
		ALIGN64 pPreConnect PreConnect; // (偏移量48) 预连接操作的回调函数。可以在调用 freerdp_connect() 之前设置，以便在实际连接发生之前执行.如果不需要，必须设置为 NULL。
		ALIGN64 pPostConnect PostConnect; // (偏移量49) 连接操作后的回调函数。可以在调用 freerdp_connect() 之前设置，以便在实际连接成功后执行。如果不需要，必须设置为 NULL。
		ALIGN64 pAuthenticate Authenticate; // (偏移量50) 认证回调函数,如果在连接未提供用户名/密码时获取用户名/密码。
		ALIGN64 pVerifyCertificate VerifyCertificate; // (偏移量51) 证书验证回调函数。已废弃：使用 VerifyChangedCertificateEx。
		ALIGN64 pVerifyChangedCertificate VerifyChangedCertificate; // (偏移量52) 变更证书验证回调函数。用于当证书与存储的指纹不同时时验证。已废弃：使用 VerifyChangedCertificateEx。 
		ALIGN64 pVerifyX509Certificate VerifyX509Certificate; // (偏移量53) 证书验证回调函数（PEM 格式） 
		ALIGN64 pLogonErrorInfo LogonErrorInfo;  // (偏移量54) 登录错误信息回调函数。
		ALIGN64 pPostDisconnect PostDisconnect;  // (偏移量55) 用于清理连接回调函数分配的资源的回调函数。
		ALIGN64 pAuthenticate GatewayAuthenticate;  // (偏移量56) 网关认证回调函数。用于在连接未提供用户名/密码时获取用户名/密码。
		ALIGN64 pPresentGatewayMessage PresentGatewayMessage;  // (偏移量57) 网关同意消息回调函数，用于向用户呈现同意消息。
		UINT64 paddingD[64 - 58]; // 填充字段
		ALIGN64 pSendChannelData SendChannelData; // (偏移量64) 发送数据到通道的回调函数。默认情况下，freerdp_new() 将其设置为 freerdp_send_channel_data()，最终调用 freerdp_channel_send()。
		ALIGN64 pReceiveChannelData ReceiveChannelData; // (偏移量65) 从通道接收数据的回调函数。由 freerdp_channel_process() 调用（如果不为 NULL）。客户端通常使用调用 freerdp_channels_data() 的函数来执行所需任务。
		ALIGN64 pVerifyCertificateEx VerifyCertificateEx; // (偏移量66) 证书验证回调函数。
		ALIGN64 pVerifyChangedCertificateEx VerifyChangedCertificateEx; // (偏移量67) 变更证书验证回调函数。用于当证书与存储的指纹不同时时验证。
		UINT64 paddingE[80 - 68]; // 填充字段
	};
```

## struct rdpContext

```C
struct rdp_context{
		ALIGN64 freerdp* instance; // 反向链接自己所在的 freerdp 结构体
		ALIGN64 freerdp_peer* peer; // 指向客户端对等体的指针。在对等体初始化期间由 freerdp_peer_context_new() 调用设置。此字段仅在服务器端使用。
		ALIGN64 BOOL ServerMode; // 当上下文处于服务器模式时为 true。
		ALIGN64 UINT32 LastError; // 上次错误的错误码。
		UINT64 paddingA[16 - 4]; // 填充字段
		ALIGN64 int argc; // 启动时传递给程序的参数数量,用于保持此数据可用，并在稍后使用，通常在连接初始化之前使用。@see freerdp_parse_args()
		ALIGN64 char** argv; // 启动时传递给程序的参数列表。用于保持此数据可用，并在稍后使用，通常在连接初始化之前使用。@see freerdp_parse_args()
		ALIGN64 wPubSub* pubSub; // 指向发布订阅机制的指针，用于事件处理。
		ALIGN64 HANDLE channelErrorEvent; // 通道错误事件的句柄。
		ALIGN64 UINT channelErrorNum; // 通道错误号码。
		ALIGN64 char* errorDescription; // 错误描述字符串。
		UINT64 paddingB[32 - 22]; // 填充字段
		ALIGN64 rdpRdp* rdp; // 指向 rdp_rdp 结构的指针，用于保存连接的参数。 由 freerdp_context_new() 分配，并由 freerdp_context_free() 释放。找不到具体的结构体定义。
		ALIGN64 rdpGdi* gdi; // 指向 rdp_gdi 结构的指针，用于保存 GDI 设置。由 gdi_init() 分配，并由 gdi_free() 释放。必须在释放 rdp_context 结构之前释放它。
		ALIGN64 rdpRail* rail; // 指向 rdpRail 结构的指针，用于远程应用集成（RAIL）。找不到具体的结构体定义。
		ALIGN64 rdpCache* cache; // 指向 rdpCache 结构的指针，用于缓存管理。
		ALIGN64 rdpChannels* channels;  // 指向 rdpChannels 结构的指针，用于通道管理。找不到具体的结构体定义。
		ALIGN64 rdpGraphics* graphics;  // 指向 rdpGraphics 结构的指针，用于图形处理。
		ALIGN64 rdpInput* input; // 指向 rdpInput 结构的指针，用于输入处理。
		ALIGN64 rdpUpdate* update;  // 指向 rdpUpdate 结构的指针，用于更新显示。
		ALIGN64 rdpSettings* settings; // 指向 rdpSettings 结构的指针，用于保持 RDP 设置。
		ALIGN64 rdpMetrics* metrics; //  指向 rdpMetrics 结构的指针，用于度量指标。
		ALIGN64 rdpCodecs* codecs; // 指向 rdpCodecs 结构的指针，用于编码管理。
		ALIGN64 rdpAutoDetect* autodetect; // 指向 rdpAutoDetect 结构的指针，用于自动检测。
		ALIGN64 HANDLE abortEvent; // 终止事件的句柄，用于中止操作。
		ALIGN64 int disconnectUltimatum;  //  断开连接的最终决定。
		UINT64 paddingC[64 - 46]; // 填充字段
		UINT64 paddingD[96 - 64]; // 填充字段
		UINT64 paddingE[128 - 96]; // 填充字段
	};
```

