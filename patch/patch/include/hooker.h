#ifndef __DL_HOOKER_H__
#define __DL_HOOKER_H__

#include <tamtypes.h>

enum HookId
{
  HOOK_ID_EMPTY = 0,
  HOOK_ID_SCE_READ_PAD,
  HOOK_ID_FONT_PRINT,
  HOOK_ID_DRAW_SCREEN_GQUAD,
  HOOK_ID_DRAW_QUAD,
  HOOK_ID_DRAW_WIDGET_2D,
  HOOK_ID_VU1_ADD_GS_REGISTER,
  HOOK_ID_VU1_SET_SCISSOR,
  HOOK_ID_COUNT,
};

//--------------------------------------------------------
void hookerInstallFunctionEntrypoint(enum HookId id, void* at, void* callback);
void hookerRestoreFunctionEntrypoint(enum HookId id, void* at);
void hookerInstallFunctionCall(enum HookId id, void* at, void* callback);

#endif // __DL_HOOKER_H__
