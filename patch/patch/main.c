#include <tamtypes.h>
#include <stdio.h>
#include <stddef.h>
#include <string.h>
#include <libdlsp/types.h>
#include <libdlsp/enums.h>
#include <libdlsp/underlay.h>
#include <libdlsp/overlay.h>
#include <libdlsp/helpers.h>
#include <libdlsp/math.h>
#include <libdlsp/font.h>
#include <libdlsp/pad.h>
#include <libdlsp/draw.h>

#include "addresses.h"
#include "commands.h"
#include "draw.h"
#include "hooker.h"

#define PRINT_TYPE_SIZE(type)           (printf(#type " %d\n", sizeof(type)))
#define PRINT_VAR_ADDR(var)             (printf(#var " %08X\n", (u32)&var))

int Initialized = 0;

struct Config {
  int ready;
  u8 rPad[32];
  float camDeltaX;
  float camDeltaY;
  int disableRendering;
};

struct Config config __attribute__((section(".config"))) = {
  
};

#if DEBUG

//--------------------------------------------------------------------------
int DEBUG_VALUE = 0;
void updateDebugValue(void)
{
  // makes it easy to test different values without restarting
  static u16 lastPad = 0xFFFF;
  u16 pad = (config.rPad[3] << 8) | config.rPad[2];
  if ((pad & PAD_LEFT) == 0 && (lastPad & PAD_LEFT)) {
    DEBUG_VALUE--;
    printf("%d 0x%x\n", DEBUG_VALUE, DEBUG_VALUE);
  } else if ((pad & PAD_RIGHT) == 0 && (lastPad & PAD_RIGHT)) {
    DEBUG_VALUE++;
    printf("%d 0x%x\n", DEBUG_VALUE, DEBUG_VALUE);
  }

  lastPad = pad;
}

#endif

//--------------------------------------------------------------------------
int isInputKBM(void)
{
  return config.camDeltaX != 0 || config.camDeltaY != 0;
}

//--------------------------------------------------------------------------
void hook_OnDrawWidget2D(struct Widget2D *pwidget, int scr_x, int scr_y, float scale_x, float scale_y, float theta_radians, u32 rgba, float t_frame)
{
  GameCommandDrawWidget2D_t cmd;

  // get hooked func
  DrawWidget2D_f drawWidget2DFunc = (DrawWidget2D_f)GetOverlayAddress((void**)&DrawWidget2D_lookup);
  if (!drawWidget2DFunc) return;

  // remove hook so we can call base
  hookerRestoreFunctionEntrypoint(HOOK_ID_DRAW_WIDGET_2D, drawWidget2DFunc);

  // call base
  drawWidget2DFunc(pwidget, scr_x, scr_y, scale_x, scale_y, theta_radians, rgba, t_frame);

  // pass upstream
  memcpy(&cmd.Widget, pwidget, sizeof(cmd.Widget));
  cmd.X = scr_x;
  cmd.Y = scr_y;
  cmd.ScaleX = scale_x;
  cmd.ScaleY = scale_y;
  cmd.Theta = theta_radians;
  cmd.Color = rgba;
  cmd.TFrame = t_frame;
  writeCommand(GAME_CMD_DRAW_WIDGET2D, &cmd, sizeof(GameCommandDrawWidget2D_t));

  // reinstall hook
  hookerInstallFunctionEntrypoint(HOOK_ID_DRAW_WIDGET_2D, drawWidget2DFunc, &hook_OnDrawWidget2D);
}

//--------------------------------------------------------------------------
void hook_OnDrawQuad(vec2f *vPoints, struct DrawParams *pParams)
{
  GameCommandDrawQuad_t cmd;

  // get hooked func
  DrawQuad_f drawQuadFunc = (DrawQuad_f)GetOverlayAddress((void**)&DrawQuad_lookup);
  if (!drawQuadFunc) return;

  struct vu1Buf {
    u32 w0;
    u32 w1;
    u32 w2;
    u32 w3;
  };

  // determine zwrite/ztest/draw state
  // this can fail if vif packets aren't 0x10 in size
  // todo: add support for all vif packets
  struct vu1Buf *vbuf = (struct vu1Buf*)GetVu1Buf();
  int vbufUse = GetVu1Use();
  int bufIdx = 0;
  int hasZBuf = 0, hasDraw = 0;
  while (bufIdx < vbufUse) {
    if (vbuf->w2 == 0x4e) {
      cmd.ZWrite = vbuf->w1 == 0;
      hasZBuf = 1;
    }
    if (vbuf->w2 == 0x4c) {
      cmd.Draw = vbuf->w1 == 0;
      hasDraw = 1;
    }

    if (hasZBuf && hasDraw) break;
    --vbuf;
    bufIdx += 0x10;
  }

  // pass upstream
  memcpy(cmd.Points, vPoints, sizeof(cmd.Points));
  memcpy(&cmd.Params, pParams, sizeof(cmd.Params));
  cmd.Z = GetDraw2dZ();
  writeCommand(GAME_CMD_DRAW_QUAD, &cmd, sizeof(GameCommandDrawQuad_t));

  // remove hook so we can call base
  hookerRestoreFunctionEntrypoint(HOOK_ID_DRAW_QUAD, drawQuadFunc);

  // call base
  drawQuadFunc(vPoints, pParams); 

  // reinstall hook
  hookerInstallFunctionEntrypoint(HOOK_ID_DRAW_QUAD, drawQuadFunc, &hook_OnDrawQuad);
}

//--------------------------------------------------------------------------
void hook_OnDrawScreenGQuad(u64 *vertices, u32 *colors)
{
  GameCommandDrawQuad_t cmd;
  int i;

  // get hooked func
  DrawScreenGQuad_f drawScreenGQuadFunc = (DrawScreenGQuad_f)GetOverlayAddress((void**)&DrawScreenGQuad_lookup);
  if (!drawScreenGQuadFunc) return;

  // build list of verts
  for (i = 0; i < 4; ++i) {
    u16 x = PEEK_U16((u32)&vertices[i] + 0);
    u16 y = PEEK_U16((u32)&vertices[i] + 2);

    cmd.Points[i][0] = (((x - SCREEN_OFFSET_X) + 8) / 16.0) / GetCanvasToScreenX();
    cmd.Points[i][1] = (((y - SCREEN_OFFSET_Y) + 8) / 16.0) / GetCanvasToScreenY();
  }

  // pass upstream
  cmd.ZWrite = 0;
  cmd.Draw = 1;
  cmd.Z = vertices[0] >> 32;
  cmd.Params.iUsing = 8; // colors
  memcpy(cmd.Params.vColors, colors, sizeof(cmd.Params.vColors));
  writeCommand(GAME_CMD_DRAW_QUAD, &cmd, sizeof(GameCommandDrawQuad_t));

  // remove hook so we can call base
  hookerRestoreFunctionEntrypoint(HOOK_ID_DRAW_SCREEN_GQUAD, drawScreenGQuadFunc);

  // call base
  drawScreenGQuadFunc(vertices, colors); 

  // reinstall hook
  hookerInstallFunctionEntrypoint(HOOK_ID_DRAW_SCREEN_GQUAD, drawScreenGQuadFunc, &hook_OnDrawScreenGQuad);
}

//--------------------------------------------------------------------------
void hook_OnFontPrint(float x, float y, u64 rgba, char* s, int length, float scaleX, float scaleY, FontAlignment alignment, char bEnableDropShadow, u64 dropShadowColor, float dropShadowXOffset, float dropShadowYOffset)
{
  GameCommandDrawText_t cmd;

  // get hooked func
  FontPrint_f fontPrintFunc = (FontPrint_f)GetOverlayAddress((void**)&FontPrint_lookup);
  if (!fontPrintFunc) return;

  // remove hook so we can call base
  hookerRestoreFunctionEntrypoint(HOOK_ID_FONT_PRINT, fontPrintFunc);

  // call base
  fontPrintFunc(x, y, rgba, s, length, scaleX, scaleX, alignment, bEnableDropShadow, dropShadowColor, dropShadowXOffset, dropShadowYOffset); 

  // pass upstream
  cmd.Color = rgba;
  cmd.Length = length;
  cmd.Alignment = alignment;
  cmd.EnableDropShadow = bEnableDropShadow;
  cmd.DropShadowColor = dropShadowColor;
  cmd.X = x;
  cmd.Y = y;
  cmd.ScaleX = scaleX;
  cmd.ScaleY = scaleY;
  cmd.DropShadowXOffset = dropShadowXOffset;
  cmd.DropShadowYOffset = dropShadowYOffset;
  cmd.Font = PEEK_I8(0x0021dec4);
  cmd.Width = FontStringLength(s, length, 1);
  cmd.Height = FontStringHeight(s, length, 1);

  if (s) {
    strncpy(cmd.Message, s, sizeof(cmd.Message));

    if (length >= 0 && length < sizeof(cmd.Message))
      cmd.Message[length] = 0;
  } else {
    memset(cmd.Message, 0, sizeof(cmd.Message));
  }

  writeCommand(GAME_CMD_DRAW_TEXT, &cmd, sizeof(GameCommandDrawText_t));

  // reinstall hook
  hookerInstallFunctionEntrypoint(HOOK_ID_FONT_PRINT, fontPrintFunc, &hook_OnFontPrint);
}

//--------------------------------------------------------------------------
int hook_OnSceReadPad(u32 a0, u32 a1, void* a2) {

  // call base scePadRead
  int r = ((int (*)(u32, u32, void*))UNDERLAY_FIXED_SCEPADREAD)(a0, a1, a2);

  // replace read data with ours
  struct PAD* pad = (struct PAD*)(a2 - 0x574);
  if (pad->slot == 0) {
    memcpy(a2, config.rPad, 32);
  }

  // return base result
  return r;
}

//--------------------------------------------------------------------------
void hook(void) {
  
  // hook sceReadPad inside UpdatePad
  void * updatePadFunc = GetOverlayAddress((void**)&UpdatePad_lookup);
  if (updatePadFunc) {
    hookerInstallFunctionCall(HOOK_ID_SCE_READ_PAD, updatePadFunc + 0x39C, &hook_OnSceReadPad);
  }
  
  // hook FontPrint
  hookerInstallFunctionEntrypoint(HOOK_ID_FONT_PRINT, GetOverlayAddress((void**)&FontPrint_lookup), &hook_OnFontPrint);
  
  // hook DrawQuad
  hookerInstallFunctionEntrypoint(HOOK_ID_DRAW_QUAD, GetOverlayAddress((void**)&DrawQuad_lookup), &hook_OnDrawQuad);

  // hook DrawScreenGQuad
  hookerInstallFunctionEntrypoint(HOOK_ID_DRAW_SCREEN_GQUAD, GetOverlayAddress((void**)&DrawScreenGQuad_lookup), &hook_OnDrawScreenGQuad);

  // hook DrawWidget2D
  hookerInstallFunctionEntrypoint(HOOK_ID_DRAW_WIDGET_2D, GetOverlayAddress((void**)&DrawWidget2D_lookup), &hook_OnDrawWidget2D);
}

//--------------------------------------------------------------------------
void initialize(void) {
  
  memset(&config, 0, sizeof(struct Config));
  config.rPad[1] = 0x79;
  config.rPad[2] = 0xFF;
  config.rPad[3] = 0xFF;
  config.rPad[4] = 0x7F;
  config.rPad[5] = 0x7F;
  config.rPad[6] = 0x7F;
  config.rPad[7] = 0x7F;

  PRINT_TYPE_SIZE(GameCommandMobySpawned_t);
  PRINT_TYPE_SIZE(GameCommandMobyDestroyed_t);
  PRINT_TYPE_SIZE(GameCommandDrawReticule_t);
  PRINT_TYPE_SIZE(GameCommandDrawText_t);
  PRINT_TYPE_SIZE(GameCommandDrawQuad_t);
  PRINT_TYPE_SIZE(GameCommandDrawWidget2D_t);

  PRINT_VAR_ADDR(config.ready);
  PRINT_VAR_ADDR(config.rPad);
  PRINT_VAR_ADDR(config.camDeltaX);
  PRINT_VAR_ADDR(config.camDeltaY);
  PRINT_VAR_ADDR(config.disableRendering);
  
  config.ready = 1;
}

//--------------------------------------------------------------------------
int entrypoint(u64 a0, u64 a1)
{
  // call base
  ((void (*)(u64, u64))UNDERLAY_FIXED_ENTRYPOINT_FUNC)(a0, a1);

  if (!config.ready) {
    initialize();
  } else if (config.ready != 1) {
    config.ready = 1;
  }

  hook();

#if DEBUG
  updateDebugValue();
#endif

  // process incoming commands
  processCommands();

  // send tick end
  writeCommand(GAME_CMD_ON_TICK, NULL, 0);
  return 0;
}
