#include <tamtypes.h>
#include <string.h>
#include "commands.h"

#define COMMAND_QUEUE_INDEX                   (*(int*)(0x02080000 - 4))
#define COMMAND_QUEUE_OUT                     ((void*)0x02080000)
#define COMMAND_QUEUE_IN                      ((void*)0x02090000)

int commandQueueIndex = 0;

//--------------------------------------------------------------------------
void writeCommand(enum GameCommandIds id, void * data, int size)
{
  u8 * commandBuffer = (u8*)COMMAND_QUEUE_OUT;
  int commandQueueIndex = COMMAND_QUEUE_INDEX;
  short size16 = size;

  // create header
  // 0-1    | id
  // 1-3    | size
  commandBuffer[commandQueueIndex++] = id;
  memcpy(&commandBuffer[commandQueueIndex], &size16, sizeof(size16));
  commandQueueIndex += sizeof(size16);

  // write data
  if (size > 0)
    memcpy(&commandBuffer[commandQueueIndex], data, size);
  commandQueueIndex += size;

  // ensure end of buffer is NONE
  commandBuffer[commandQueueIndex] = GAME_CMD_NONE;

  COMMAND_QUEUE_INDEX = commandQueueIndex;
  //DPRINTF("OUT:CMD %d (size %d)\n", id, size);
}

//--------------------------------------------------------------------------
void processCommands(void)
{
  u8 * commandBuffer = (u8*)COMMAND_QUEUE_IN;
  int bufferIdx = 0;

  // parse incoming commands
  while (commandBuffer[bufferIdx] != GAME_CMD_NONE) {

    //DPRINTF("IN:CMD %d\n", commandBuffer[bufferIdx]);
    switch (commandBuffer[bufferIdx]) {
      
    }

    bufferIdx += 1;
  }

  // reset buffer
  memset(COMMAND_QUEUE_IN, 0, 0x10000);
}

//--------------------------------------------------------------------------
void clearCommandBuffer(void)
{
  COMMAND_QUEUE_INDEX = 0;
  *(u8*)COMMAND_QUEUE_OUT = 0;
}
