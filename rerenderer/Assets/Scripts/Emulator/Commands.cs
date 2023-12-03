using RC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public enum GameCommandIds
{
    GAME_CMD_NONE = 0,
    GAME_CMD_ON_TICK_END = 1,
    GAME_CMD_MOBY_SPAWNED = 2,
    GAME_CMD_MOBY_DESTROYED = 3,
    GAME_CMD_DRAW_RETICULE = 4,
    GAME_CMD_DRAW_TEXT = 5,
    GAME_CMD_DRAW_QUAD = 6,
    GAME_CMD_DRAW_WIDGET2D = 7,
}

public interface IGameCommand
{
    GameCommandIds Id { get; }

    void Serialize(BinaryWriter writer);
    void Deserialize(BinaryReader reader, int size);
}

public class Vu1DrawState
{
    public Vector4 Scissor;
    public uint Alpha;
    public RCHelper.GS_ZTEST ZTest;
    public bool ZWrite;
    public bool Draw;

    public void Deserialize(BinaryReader reader)
    {
        var scissorLeft = reader.ReadInt16();
        var scissorRight = reader.ReadInt16();
        var scissorTop = reader.ReadInt16();
        var scissorBottom = reader.ReadInt16();

        Scissor.x = scissorLeft / 16384f;
        Scissor.y = scissorRight / 16384f;
        Scissor.z = scissorTop / 16384f;
        Scissor.w = scissorBottom / 16384f;

        Alpha = reader.ReadUInt32();
        ZTest = (RCHelper.GS_ZTEST)reader.ReadByte();
        ZWrite = reader.ReadBoolean();
        Draw = reader.ReadBoolean();
        reader.ReadBytes(1);
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

public class GameCommandOnTickEnd : IGameCommand
{
    public GameCommandIds Id => GameCommandIds.GAME_CMD_ON_TICK_END;
    
    public void Deserialize(BinaryReader reader, int size)
    {
    }

    public void Serialize(BinaryWriter writer)
    {
    }
}

public class GameCommandMobySpawned : IGameCommand
{
    public GameCommandIds Id => GameCommandIds.GAME_CMD_MOBY_SPAWNED;
    public Vector3 Position { get; set; }
    public float Scale { get; set; }
    public uint MobyInstancePtr { get; set; }
    public ushort MobyClass { get; set; }

    public void Deserialize(BinaryReader reader, int size)
    {
        Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        reader.ReadBytes(4);
        Scale = reader.ReadSingle();
        MobyInstancePtr = reader.ReadUInt32();
        MobyClass = reader.ReadUInt16();
        reader.ReadBytes(2 + 4);
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(Position.x);
        writer.Write(Position.y);
        writer.Write(Position.z);
        writer.Write(0);
        writer.Write(Scale);
        writer.Write(MobyInstancePtr);
        writer.Write(MobyClass);
        writer.Write(new byte[2 + 4]);
    }
}

public class GameCommandMobyDestroyed : IGameCommand
{
    public GameCommandIds Id => GameCommandIds.GAME_CMD_MOBY_DESTROYED;
    public uint MobyInstancePtr { get; set; }
    public ushort MobyClass { get; set; }

    public void Deserialize(BinaryReader reader, int size)
    {
        MobyInstancePtr = reader.ReadUInt32();
        MobyClass = reader.ReadUInt16();
        reader.ReadBytes(2);
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(MobyInstancePtr);
        writer.Write(MobyClass);
        writer.Write(new byte[2]);
    }
}

public class GameCommandDrawReticule : IGameCommand
{
    public GameCommandIds Id => GameCommandIds.GAME_CMD_DRAW_RETICULE;
    public Vector3 WSPosition { get; set; }
    public int SpriteId { get; set; }
    public uint MobyInstancePtr { get; set; }
    public byte[] RGBA { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float Scale { get; set; }
    public float Rotation { get; set; }

    public void Deserialize(BinaryReader reader, int size)
    {
        WSPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        reader.ReadSingle(); // vector4
        SpriteId = reader.ReadInt32();
        MobyInstancePtr = reader.ReadUInt32();
        RGBA = reader.ReadBytes(4);
        X = reader.ReadSingle();
        Y = reader.ReadSingle();
        Width = reader.ReadSingle();
        Height = reader.ReadSingle();
        Scale = reader.ReadSingle();
        Rotation = reader.ReadSingle();
        reader.ReadBytes(12);
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(WSPosition.x);
        writer.Write(WSPosition.y);
        writer.Write(WSPosition.x);
        writer.Write(1.0f);
        writer.Write(SpriteId);
        writer.Write(MobyInstancePtr);
        writer.Write(RGBA ?? new byte[4]);
        writer.Write(X);
        writer.Write(Y);
        writer.Write(Width);
        writer.Write(Height);
        writer.Write(Scale);
        writer.Write(Rotation);
        writer.Write(new byte[12]);
    }
}

public class GameCommandDrawText : IGameCommand
{
    public GameCommandIds Id => GameCommandIds.GAME_CMD_DRAW_TEXT;
    public Vu1DrawState Vu1DrawState { get; set; }
    public Color Color { get; set; }
    public int Length { get; set; }
    public bool EnableDropShadow { get; set; }
    public Color DropShadowColor { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float ScaleX { get; set; }
    public float ScaleY { get; set; }
    public float DropShadowOffsetX { get; set; }
    public float DropShadowOffsetY { get; set; }
    public byte Font { get; set; }
    public string Message { get; set; }

    public void Deserialize(BinaryReader reader, int size)
    {
        Vu1DrawState = new Vu1DrawState();
        Vu1DrawState.Deserialize(reader);
        Color = reader.ReadUInt32().RCRgbaToColor();
        Length = reader.ReadInt32();
        DropShadowColor = reader.ReadUInt32().RCRgbaToColor();
        X = reader.ReadSingle();
        Y = reader.ReadSingle();
        ScaleX = reader.ReadSingle();
        ScaleY = reader.ReadSingle();
        DropShadowOffsetX = reader.ReadSingle();
        DropShadowOffsetY = reader.ReadSingle();
        EnableDropShadow = reader.ReadBoolean();
        Font = reader.ReadByte();
        Message = reader.ReadString(64);
        reader.ReadBytes(2);
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

public class GameCommandDrawQuad : IGameCommand
{
    public GameCommandIds Id => GameCommandIds.GAME_CMD_DRAW_QUAD;
    public Vu1DrawState Vu1DrawState { get; set; }

    public Vector2 Point0 { get; set; }
    public Vector2 Point1 { get; set; }
    public Vector2 Point2 { get; set; }
    public Vector2 Point3 { get; set; }

    public int Using { get; set; }
    public int Icon { get; set; }
    public RCPointer Image { get; set; }
    public float ShadowX { get; set; }
    public float ShadowY { get; set; }
    public Color ShadowColor { get; set; }
    public Color Color0 { get; set; }
    public Color Color1 { get; set; }
    public Color Color2 { get; set; }
    public Color Color3 { get; set; }
    public float Fade { get; set; }

    public uint Z { get; set; }

    public void Deserialize(BinaryReader reader, int size)
    {
        Vu1DrawState = new Vu1DrawState();
        Vu1DrawState.Deserialize(reader);

        Point0 = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        Point1 = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        Point2 = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        Point3 = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        Using = reader.ReadInt32();
        Icon = reader.ReadInt32();
        Image = new RCPointer(reader.ReadUInt32());
        ShadowX = reader.ReadSingle();
        ShadowY = reader.ReadSingle();
        ShadowColor = reader.ReadUInt32().RCRgbaToColor();
        Color0 = reader.ReadUInt32().RCRgbaToColor();
        Color1 = reader.ReadUInt32().RCRgbaToColor();
        Color2 = reader.ReadUInt32().RCRgbaToColor();
        Color3 = reader.ReadUInt32().RCRgbaToColor();
        Fade = reader.ReadSingle();
        Z = reader.ReadUInt32();
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

public class GameCommandDrawWidget2D : IGameCommand
{
    public GameCommandIds Id => GameCommandIds.GAME_CMD_DRAW_WIDGET2D;

    public Vu1DrawState Vu1DrawState { get; set; }

    public RCPointer Positions { get; set; }
    public RCPointer UVs { get; set; }
    public RCPointer Colors { get; set; }
    public RCPointer Polys { get; set; }
    public short VertexCount { get; set; }
    public short PrimCount { get; set; }
    public byte PrimType { get; set; }
    public byte Flags { get; set; }
    public short FrameCount { get; set; }

    public int X { get; set; }
    public int Y { get; set; }
    public float CanvasWidth { get; set; }
    public float CanvasHeight { get; set; }
    public float ScaleX { get; set; }
    public float ScaleY { get; set; }
    public float Theta { get; set; }
    public Color Color { get; set; }
    public float TFrame { get; set; }
    public uint Z { get; set; }

    public void Deserialize(BinaryReader reader, int size)
    {
        Vu1DrawState = new Vu1DrawState();
        Vu1DrawState.Deserialize(reader);

        Positions = new RCPointer(reader.ReadUInt32());
        UVs = new RCPointer(reader.ReadUInt32());
        Colors = new RCPointer(reader.ReadUInt32());
        Polys = new RCPointer(reader.ReadUInt32());
        VertexCount = reader.ReadInt16();
        PrimCount = reader.ReadInt16();
        PrimType = reader.ReadByte();
        Flags = reader.ReadByte();
        FrameCount = reader.ReadInt16();
        reader.ReadBytes(8);

        X = reader.ReadInt32();
        Y = reader.ReadInt32();
        CanvasWidth = reader.ReadSingle();
        CanvasHeight = reader.ReadSingle();
        ScaleX = reader.ReadSingle();
        ScaleY = reader.ReadSingle();
        Theta = reader.ReadSingle();
        Color = reader.ReadUInt32().RCRgbaToColor();
        TFrame = reader.ReadSingle();
        Z = reader.ReadUInt32();
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}
