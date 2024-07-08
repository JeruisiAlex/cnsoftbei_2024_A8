# codecs.h

用于编码管理的结构体

## struct

```c
struct rdp_codecs
{
	rdpContext* context; // 反向链接自己所在的 rdpContext
	RFX_CONTEXT* rfx;
	NSC_CONTEXT* nsc;
	H264_CONTEXT* h264;
	CLEAR_CONTEXT* clear;
	PROGRESSIVE_CONTEXT* progressive;
	BITMAP_PLANAR_CONTEXT* planar;
	BITMAP_INTERLEAVED_CONTEXT* interleaved;
};
```

## API

```C
FREERDP_API BOOL freerdp_client_codecs_prepare(rdpCodecs* codecs, UINT32 flags, UINT32 width, UINT32 height);
FREERDP_API BOOL freerdp_client_codecs_reset(rdpCodecs* codecs, UINT32 flags, UINT32 width, UINT32 height);
FREERDP_API rdpCodecs* codecs_new(rdpContext* context); // 初始化一个 rdpCodecs 结构体
FREERDP_API void codecs_free(rdpCodecs* codecs); // 释放 rdpCodecs
```

