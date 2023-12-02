#ifndef __DL_DRAW_H__
#define __DL_DRAW_H__

#include <libdlsp/overlay.h>
#include <tamtypes.h>


#define UNDERLAY_FIXED_VU1BUF_NTSC                      (0x00221e20)
#define UNDERLAY_FIXED_VU1USE_NTSC                      (0x00221e30)
#define UNDERLAY_FIXED_VU1BUFID_NTSC                    (0x00221e2c)

//--------------------------------------------------------
DECLARE_OVERLAY_LOOKUP(vu1Buf_lookup, void*);
DECLARE_OVERLAY_LOOKUP(draw2dZ_lookup, u32*);

//--------------------------------------------------------
void* GetVu1Buf(void);
int GetVu1Use(void);
int GetVu1BufId(void);
u32 GetDraw2dZ(void);

#endif // __DL_DRAW_H__
