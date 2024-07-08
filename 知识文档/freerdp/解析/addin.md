# addin.h

推测该文件应该是用于连接一些外接程序，但是其中出现了一些用于 rdp 虚拟通道的 API 函数，暂时不清楚这些 API 的具体作用。

## 一些宏定义

``` C
#define FREERDP_ADDIN_CLIENT 0x00000001
#define FREERDP_ADDIN_SERVER 0x00000002

#define FREERDP_ADDIN_STATIC 0x00000010
#define FREERDP_ADDIN_DYNAMIC 0x00000020

#define FREERDP_ADDIN_NAME 0x00000100
#define FREERDP_ADDIN_SUBSYSTEM 0x00000200
#define FREERDP_ADDIN_TYPE 0x00000400

#define FREERDP_ADDIN_CHANNEL_STATIC 0x00001000
#define FREERDP_ADDIN_CHANNEL_DYNAMIC 0x00002000
#define FREERDP_ADDIN_CHANNEL_DEVICE 0x00004000
#define FREERDP_ADDIN_CHANNEL_ENTRYEX 0x00008000
```



## 结构体 FREERDP_ADDIN

```C
struct _FREERDP_ADDIN
{
	DWORD dwFlags;
	CHAR cName[16];
	CHAR cType[16];
	CHAR cSubsystem[16];
};
typedef struct _FREERDP_ADDIN FREERDP_ADDIN;

typedef PVIRTUALCHANNELENTRY (*FREERDP_LOAD_CHANNEL_ADDIN_ENTRY_FN)(LPCSTR pszName, LPCSTR pszSubsystem, LPCSTR pszType, DWORD dwFlags);
```

推测可能是用于连接外接程序的结构体



## API

```C
FREERDP_API LPSTR freerdp_get_library_install_path(void); 
FREERDP_API LPSTR freerdp_get_dynamic_addin_install_path(void ;
FREERDP_API int freerdp_register_addin_provider(FREERDP_LOAD_CHANNEL_ADDIN_ENTRY_FN provider, DWORD dwFlags);
FREERDP_API PVIRTUALCHANNELENTRY freerdp_load_dynamic_addin(LPCSTR pszFileName, LPCSTR pszPath, LPCSTR pszEntryName);
FREERDP_API PVIRTUALCHANNELENTRY freerdp_load_dynamic_channel_addin_entry(LPCSTR pszName, LPCSTR pszSubsystem, LPCSTR pszType, DWORD dwFlags);
FREERDP_API PVIRTUALCHANNELENTRY freerdp_load_channel_addin_entry(LPCSTR pszName, LPCSTR pszSubsystem, LPCSTR pszType, DWORD dwFlags);
```

- **LPSTR**: 指向一个以 null 结尾的 ANSI 字符串的指针。等同于 `char*`。

  **LPCSTR**: 指向一个常量 ANSI 字符串的指针。等同于 `const char*`。

  **LPWSTR**: 指向一个以 null 结尾的宽字符（Unicode）字符串的指针。等同于 `wchar_t*`。

  **LPCWSTR**: 指向一个常量宽字符（Unicode）字符串的指针。等同于 `const wchar_t*`。

- **DWORD**：32 位的无符号整数。

- **PVIRTUALCHANNELENTRY** 是在远程桌面协议 (RDP) 的虚拟通道开发中使用的一种指针类型。它指向一个函数，该函数用于初始化虚拟通道。虚拟通道是用于在客户端和服务器之间传输数据的专用通信路径。