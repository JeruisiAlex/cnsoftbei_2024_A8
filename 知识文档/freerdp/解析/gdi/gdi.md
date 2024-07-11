# gdi.h

定义了用于图形界面加载相关数据结构。

## struct gdi

```c
struct rdp_gdi
{
	rdpContext* context; /**< 指向 rdpContext 结构的指针。 */

	INT32 width; /**< 绘制区域的宽度。 */
	INT32 height; /**< 绘制区域的高度。 */
	UINT32 stride; /**< 绘制区域的跨度（每行字节数）。 */
	UINT32 dstFormat; /**< 目标图像格式。 */
	UINT32 cursor_x; /**< 当前光标的 X 坐标。 */
	UINT32 cursor_y; /**< 当前光标的 Y 坐标。 */

	HGDI_DC hdc; /**< 主设备上下文句柄，用于绘制操作。 */
	gdiBitmap* primary; /**< 主绘制位图，用于主缓冲区。 */
	gdiBitmap* drawing; /**< 当前绘制的位图。 */
	UINT32 bitmap_size; /**< 位图的大小（字节数）。 */
	UINT32 bitmap_stride; /**< 位图的跨度（每行字节数）。 */
	BYTE* primary_buffer; /**< 主绘制缓冲区指针。 */
	gdiPalette palette; /**< 调色板，用于颜色管理。 */
	gdiBitmap* image; /**< 用于图像绘制的位图。 */
	void (*free)(void*);; /**< 释放函数指针，用于释放 GDI 资源。 */

	BOOL inGfxFrame; /**< 标志是否在图形帧中。 */
	BOOL graphicsReset; /**< 标志图形是否重置（已废弃，将在 FreeRDP v3 中移除）。 */
	BOOL suppressOutput; /**< 是否抑制输出的标志。 */
	UINT16 outputSurfaceId; /**< 输出表面 ID。 */
	RdpgfxClientContext* gfx; /**< 指向 RdpgfxClientContext 结构的指针，用于图形操作。 */
	VideoClientContext* video; /**< 指向 VideoClientContext 结构的指针，用于视频处理。 */
	GeometryClientContext* geometry; /**< 指向 GeometryClientContext 结构的指针，用于几何操作。 */

	wLog* log; /**< 指向日志记录的指针。 */
};
```

## API

```C
FREERDP_API DWORD gdi_rop3_code(BYTE code);
FREERDP_API const char* gdi_rop3_code_string(BYTE code);
FREERDP_API const char* gdi_rop3_string(DWORD rop);
FREERDP_API UINT32 gdi_get_pixel_format(UINT32 bitsPerPixel);
FREERDP_API BOOL gdi_decode_color(rdpGdi* gdi, const UINT32 srcColor, UINT32* color, UINT32* format);
FREERDP_API BOOL gdi_resize(rdpGdi* gdi, UINT32 width, UINT32 height); // 重定义图像的大小
FREERDP_API BOOL gdi_resize_ex(rdpGdi* gdi, UINT32 width, UINT32 height, UINT32 stride, UINT32 format, BYTE* buffer, void (*pfree)(void*)); // 更强大的重定义
FREERDP_API BOOL gdi_init(freerdp* instance, UINT32 format); // 初始化 rdpGdi 结构体 
FREERDP_API BOOL gdi_init_ex(freerdp* instance, UINT32 format, UINT32 stride, BYTE* buffer, void (*pfree)(void*));
FREERDP_API void gdi_free(freerdp* instance); // 释放 rdpGdi 结构体
FREERDP_API BOOL gdi_send_suppress_output(rdpGdi* gdi, BOOL suppress);
```

