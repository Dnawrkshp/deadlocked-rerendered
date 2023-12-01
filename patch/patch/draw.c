#include "draw.h"
#include <libdlsp/helpers.h>

//--------------------------------------------------------
BEGIN_OVERLAY_LOOKUP(vu1Buf_lookup, void*)
  OVERLAY_LOOKUP_ENTRY_NTSC(MAIN_MENU, 0x00221e20)
  OVERLAY_LOOKUP_ENTRY_NTSC(CATACROM, 0x00223210)
END_OVERLAY_LOOKUP(vu1Buf_lookup)

void* GetVu1Buf(void)
{
  void* vu1BufAddr = GetOverlayAddress((void**)&vu1Buf_lookup);
  if (!vu1BufAddr) return 0; // default

  return (void*)PEEK_U32(vu1BufAddr);
}

//--------------------------------------------------------
int GetVu1Use(void)
{
  void* vu1BufAddr = GetOverlayAddress((void**)&vu1Buf_lookup);
  if (!vu1BufAddr) return 0; // default

  return PEEK_I32((u32)vu1BufAddr + 0x10);
}

//--------------------------------------------------------
int GetVu1BufId(void)
{
  void* vu1BufAddr = GetOverlayAddress((void**)&vu1Buf_lookup);
  if (!vu1BufAddr) return 0; // default

  return PEEK_I32((u32)vu1BufAddr + 0x0c);
}

//--------------------------------------------------------
BEGIN_OVERLAY_LOOKUP(draw2dZ_lookup, u32*)
  OVERLAY_LOOKUP_ENTRY_NTSC(MAIN_MENU, 0x0021EC20)
  OVERLAY_LOOKUP_ENTRY_NTSC(CATACROM, 0x0021E540)
END_OVERLAY_LOOKUP(draw2dZ_lookup)

u32 GetDraw2dZ(void)
{
  void* draw2dZAddr = GetOverlayAddress((void**)&draw2dZ_lookup);
  if (!draw2dZAddr) return 0; // default

  return PEEK_U32(draw2dZAddr);
}
