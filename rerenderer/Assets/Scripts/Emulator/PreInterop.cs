using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PreInterop : MonoBehaviour
{
    private static float _time = 0f;
    private static float _alpha = 0;
    private static float _rawAlpha = 0;
    public static float UnclampedAlpha => _rawAlpha;
    public static float Alpha => Config.Singleton.DisableFrameInterpolation ? 1 : _alpha;
    //public static float Alpha => Config.Singleton.DisableFrameInterpolation ? 1 : EmuInterop.EmulatorAlpha;

    uint lastFrameIdx = 0;

    private void Awake()
    {
        EmuInterop.OnPreTick -= EmuInterop_OnPreTick;
        EmuInterop.OnPreTick += EmuInterop_OnPreTick;
    }

    // Update is called once per frame
    void Update()
    {
        //GameController.Singleton?.ProcessLocalInput();

        //EmuInterop.Tick(false);
        if (!PostInterop.Ready)
            return;

        _rawAlpha = (Time.time - _time) / Time.fixedDeltaTime;
        _alpha = Mathf.Clamp01(_rawAlpha);
    }

    void FixedUpdate()
    {
        GameController.Singleton?.ProcessLocalInput();
        EmuInterop.Tick();
        if (!PostInterop.Active)
            return;
    }


    private void EmuInterop_OnPreTick()
    {
        // read in commands
        if (EmuInterop.ConnectedAndGameStarted)
        {
            var commandBufferSize = EmuInterop.ReadInt32(Constants.CmdInAddress - 4) ?? 0;
            var commandBuffer = EmuInterop.ReadBytes(Constants.CmdInAddress, commandBufferSize + 1);
            if (commandBuffer != null)
            {
                using (var ms = new MemoryStream(commandBuffer.Value.Buffer, (int)commandBuffer.Value.Offset, (int)commandBuffer.Value.Length, false))
                {
                    using (var reader = new BinaryReader(ms))
                    {
                        while (true)
                        {
                            var cmdId = (GameCommandIds)reader.ReadByte();
                            if (cmdId == GameCommandIds.GAME_CMD_NONE)
                                break;

                            var size = reader.ReadInt16();
                            var readerPosAtCommandPayload = reader.BaseStream.Position;

                            try
                            {
                                switch (cmdId)
                                {
                                    case GameCommandIds.GAME_CMD_ON_TICK_END:
                                        {
                                            var cmd = new GameCommandOnTickEnd();
                                            cmd.Deserialize(reader, size);
                                            GameManager.Singleton.IncomingCommands.Add(cmd);
                                            break;
                                        }
                                    case GameCommandIds.GAME_CMD_MOBY_SPAWNED:
                                        {
                                            var cmd = new GameCommandMobySpawned();
                                            cmd.Deserialize(reader, size);
                                            GameManager.Singleton.IncomingCommands.Add(cmd);
                                            break;
                                        }
                                    case GameCommandIds.GAME_CMD_MOBY_DESTROYED:
                                        {
                                            var cmd = new GameCommandMobyDestroyed();
                                            cmd.Deserialize(reader, size);
                                            GameManager.Singleton.IncomingCommands.Add(cmd);
                                            break;
                                        }
                                    case GameCommandIds.GAME_CMD_DRAW_RETICULE:
                                        {
                                            var cmd = new GameCommandDrawReticule();
                                            cmd.Deserialize(reader, size);
                                            GameManager.Singleton.IncomingCommands.Add(cmd);
                                            break;
                                        }
                                    case GameCommandIds.GAME_CMD_DRAW_TEXT:
                                        {
                                            var cmd = new GameCommandDrawText();
                                            cmd.Deserialize(reader, size);
                                            GameManager.Singleton.IncomingCommands.Add(cmd);
                                            break;
                                        }
                                    case GameCommandIds.GAME_CMD_DRAW_QUAD:
                                        {
                                            var cmd = new GameCommandDrawQuad();
                                            cmd.Deserialize(reader, size);
                                            GameManager.Singleton.IncomingCommands.Add(cmd);
                                            break;
                                        }
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogError($"error reading command {cmdId}: {ex.Message} {ex.StackTrace}");

                                // skip to next command
                                var read = reader.BaseStream.Position - readerPosAtCommandPayload;
                                var bytesToSkip = size - read;
                                reader.BaseStream.Position += bytesToSkip;
                            }

                            // ensure we've read size number of bytes
                            var bytesRead = reader.BaseStream.Position - readerPosAtCommandPayload;
                            if (bytesRead != size)
                                throw new System.Exception($"read {bytesRead} of expected {size} for {cmdId}");
                        }
                    }
                }
            }
        }

        _time = Time.time;
        _alpha = 0;
        _rawAlpha = 0;
    }

}
