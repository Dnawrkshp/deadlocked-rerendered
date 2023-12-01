#include <libdlsp/helpers.h>
#include "hooker.h"

u32 hookerFunctionEntrypointInstructionBackup[HOOK_ID_COUNT][3] = {};

//--------------------------------------------------------
void hookerInstallFunctionEntrypoint(enum HookId id, void* at, void* callback)
{
  if (!at) return;
  if (!callback) return;

  // build j to callback
  u32 targetInstr1 = 0x08000000 | ((u32)callback >> 2);

  // only install if not already installed
  u32 currentInstr1 = PEEK_U32(at);
  if (currentInstr1 != targetInstr1) {

    // backup
    hookerFunctionEntrypointInstructionBackup[id][0] = currentInstr1;
    hookerFunctionEntrypointInstructionBackup[id][1] = PEEK_U32(at + 4);
    hookerFunctionEntrypointInstructionBackup[id][2] = 1;

    // write jump and nop
    POKE_U32(at + 0, targetInstr1);
    POKE_U32(at + 4, 0x00000000);
  }
}

//--------------------------------------------------------
void hookerRestoreFunctionEntrypoint(enum HookId id, void* at)
{
  if (!at) return;
  if (!hookerFunctionEntrypointInstructionBackup[id][2]) return;

  POKE_U32(at + 0, hookerFunctionEntrypointInstructionBackup[id][0]);
  POKE_U32(at + 4, hookerFunctionEntrypointInstructionBackup[id][1]);
}

//--------------------------------------------------------
void hookerInstallFunctionCall(enum HookId id, void* at, void* callback)
{
  if (!at) return;
  if (!callback) return;

  HOOK_JAL(at, callback);
}
