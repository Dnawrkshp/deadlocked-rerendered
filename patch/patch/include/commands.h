#ifndef __DZO_COMMANDS_H__
#define __DZO_COMMANDS_H__

#include <libdlsp/math3d.h>
#include <libdlsp/draw.h>

enum GameCommandIds {
  GAME_CMD_NONE = 0,
  GAME_CMD_ON_TICK = 1,
  GAME_CMD_MOBY_SPAWNED = 2,
  GAME_CMD_MOBY_DESTROYED = 3,
  GAME_CMD_DRAW_RETICULE = 4,
  GAME_CMD_DRAW_TEXT = 5,
  GAME_CMD_DRAW_QUAD = 6,
};

typedef struct GameCommandMobySpawned {
  vec4 Position;
  float Scale;
  u32 AddressOf;
  u16 MobyClass;
} GameCommandMobySpawned_t;

typedef struct GameCommandMobyDestroyed {
  u32 AddressOf;
  u16 MobyClass;
} GameCommandMobyDestroyed_t;

typedef struct GameCommandDrawReticule {
  vec4 WSPosition;
  int Id;
  u32 Moby;
  u32 Color;
  float X;
  float Y;
  float Width;
  float Height;
  float Scale;
  float Rotation;
} GameCommandDrawReticule_t;

typedef struct GameCommandDrawText {
  u32 ReturnAddress;
  u32 Color;
  int Length;
  int Alignment;
  u32 DropShadowColor;
  float X;
  float Y;
  float ScaleX;
  float ScaleY;
  float DropShadowXOffset;
  float DropShadowYOffset;
  short Width;
  short Height;
  char EnableDropShadow;
  char Font;
  char Message[64];
} GameCommandDrawText_t;

typedef struct GameCommandDrawQuad {
  vec2f Points[4];
  struct DrawParams Params;
} GameCommandDrawQuad_t;

void processCommands(void);
void writeCommand(enum GameCommandIds id, void * data, int size);
void writeCustomPatchCommand(int id, int size, void * data);
void clearCommandBuffer(void);

#endif // __DZO_COMMANDS_H__
