/***************************************************
 * FILENAME :		main.c
 * 
 * AUTHOR :			Daniel "Dnawrkshp" Gerendasy
 */

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

//--------------------------------------------------------------------------
void TryPokeU32(u32 addr, u32 fromVal, u32 toVal) {
	u32* a = (u32*)addr;
	if (*a == fromVal)
		*a = toVal;
}

//--------------------------------------------------------------------------
int isInputKBM(void)
{
  return config.camDeltaX != 0 || config.camDeltaY != 0;
}

//--------------------------------------------------------------------------
void hook_OnDrawQuad(vec2f *vPoints, struct DrawParams *pParams)
{
  GameCommandDrawQuad_t cmd;

  // get hooked func
  DrawQuad_f drawQuadFunc = (DrawQuad_f)GetOverlayAddress((void**)&DrawQuad_lookup);
  if (!drawQuadFunc) return;

  // remove hook so we can call base
  POKE_U32((u32)drawQuadFunc + 0, 0x27BDFF90);
  POKE_U32((u32)drawQuadFunc + 4, 0x3C030022);

  // call base
  drawQuadFunc(vPoints, pParams); 

  // pass upstream
  memcpy(cmd.Points, vPoints, sizeof(cmd.Points));
  memcpy(&cmd.Params, pParams, sizeof(cmd.Params));
  writeCommand(GAME_CMD_DRAW_QUAD, &cmd, sizeof(GameCommandDrawQuad_t));

  // reinstall hook
  HOOK_J((u32)drawQuadFunc + 0, &hook_OnDrawQuad);
  POKE_U32((u32)drawQuadFunc + 4, 0);
}

//--------------------------------------------------------------------------
void hook_OnFontPrint(float x, float y, u64 rgba, char* s, int length, float scaleX, float scaleY, FontAlignment alignment, char bEnableDropShadow, u64 dropShadowColor, float dropShadowXOffset, float dropShadowYOffset)
{
  void* ra;

	// pointer to moby is stored in $v0
	asm volatile (
		"move %0, $ra"
		: : "r" (ra)
	);

  // get hooked func
  FontPrint_f fontPrintFunc = (FontPrint_f)GetOverlayAddress((void**)&FontPrint_lookup);
  if (!fontPrintFunc) return;

  // remove hook so we can call base
  POKE_U32((u32)fontPrintFunc + 0, 0x27BDFF20);
  POKE_U32((u32)fontPrintFunc + 4, 0x3C013F80);

  // call base
  fontPrintFunc(x, y, rgba, s, length, scaleX, scaleX, alignment, bEnableDropShadow, dropShadowColor, dropShadowXOffset, dropShadowYOffset); 

  // pass upstream
  if (1) {
    GameCommandDrawText_t cmd;
    cmd.ReturnAddress = (u32)ra;
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
  }

  // reinstall hook
  HOOK_J((u32)fontPrintFunc + 0, &hook_OnFontPrint);
  POKE_U32((u32)fontPrintFunc + 4, 0);
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
  
  // hook UpdatePad
  void* updatePadFunc = GetOverlayAddress((void**)&UpdatePad_lookup);
  if (updatePadFunc) {
    HOOK_JAL(updatePadFunc + 0x39C, &hook_OnSceReadPad);
  }

  // hook FontPrint
  void* fontPrintFunc = GetOverlayAddress((void**)&FontPrint_lookup);
  if (fontPrintFunc) {
    HOOK_J((u32)fontPrintFunc + 0, &hook_OnFontPrint);
    POKE_U32((u32)fontPrintFunc + 4, 0);
  }
  
  // hook DrawQuad
  void* drawQuadFunc = GetOverlayAddress((void**)&DrawQuad_lookup);
  if (drawQuadFunc) {
    HOOK_J((u32)drawQuadFunc + 0, &hook_OnDrawQuad);
    POKE_U32((u32)drawQuadFunc + 4, 0);
  }
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
  
  // process incoming commands
	processCommands();

  // send tick end
  writeCommand(GAME_CMD_ON_TICK, NULL, 0);
	return 0;
}
