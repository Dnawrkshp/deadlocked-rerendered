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
    public uint ReturnAddress { get; set; }
    public Color Color { get; set; }
    public int Length { get; set; }
    public int Alignment { get; set; }
    public bool EnableDropShadow { get; set; }
    public Color DropShadowColor { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float ScaleX { get; set; }
    public float ScaleY { get; set; }
    public float DropShadowOffsetX { get; set; }
    public float DropShadowOffsetY { get; set; }
    public byte Font { get; set; }
    public short Width { get; set; }
    public short Height { get; set; }
    public string Message { get; set; }

    public void Deserialize(BinaryReader reader, int size)
    {
        ReturnAddress = reader.ReadUInt32();
        Color = reader.ReadUInt32().RCRgbaToColor();
        Length = reader.ReadInt32();
        Alignment = reader.ReadInt32();
        DropShadowColor = reader.ReadUInt32().RCRgbaToColor();
        X = reader.ReadSingle();
        Y = reader.ReadSingle();
        ScaleX = reader.ReadSingle();
        ScaleY = reader.ReadSingle();
        DropShadowOffsetX = reader.ReadSingle();
        DropShadowOffsetY = reader.ReadSingle();
        Width = reader.ReadInt16();
        Height = reader.ReadInt16();
        EnableDropShadow = reader.ReadBoolean();
        Font = reader.ReadByte();
        Message = reader.ReadString(64);
        reader.ReadBytes(2);
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(ReturnAddress);
        writer.Write((byte)(Color.r * 255));
        writer.Write((byte)(Color.g * 255));
        writer.Write((byte)(Color.b * 255));
        writer.Write((byte)(Color.a * 128));
        writer.Write(Length);
        writer.Write(Alignment);
        writer.Write((byte)(DropShadowColor.r * 255));
        writer.Write((byte)(DropShadowColor.g * 255));
        writer.Write((byte)(DropShadowColor.b * 255));
        writer.Write((byte)(DropShadowColor.a * 128));
        writer.Write(X);
        writer.Write(Y);
        writer.Write(ScaleX);
        writer.Write(ScaleY);
        writer.Write(DropShadowOffsetX);
        writer.Write(DropShadowOffsetY);
        writer.Write(Width);
        writer.Write(Height);
        writer.Write(EnableDropShadow);
        writer.Write(Font);

        var msgBytes = Encoding.UTF8.GetBytes(Message ?? "");
        if (msgBytes.Length >= 64)
        {
            writer.Write(msgBytes, 0, 64);
        }
        else
        {
            writer.Write(msgBytes);
            writer.Write(new byte[64 - msgBytes.Length]);
        }

        writer.Write(new byte[2]);
    }
}

public class GameCommandDrawQuad : IGameCommand
{
    public GameCommandIds Id => GameCommandIds.GAME_CMD_DRAW_QUAD;
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

    public void Deserialize(BinaryReader reader, int size)
    {
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
        reader.ReadBytes(4);
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(Point0.x);
        writer.Write(Point0.y);
        writer.Write(Point1.x);
        writer.Write(Point1.y);
        writer.Write(Point2.x);
        writer.Write(Point2.y);
        writer.Write(Point3.x);
        writer.Write(Point3.y);
        writer.Write(Using);
        writer.Write(Icon);
        writer.Write(Image.Address);
        writer.Write(ShadowX);
        writer.Write(ShadowY);
        writer.Write((byte)(ShadowColor.r * 255));
        writer.Write((byte)(ShadowColor.g * 255));
        writer.Write((byte)(ShadowColor.b * 255));
        writer.Write((byte)(ShadowColor.a * 128));
        writer.Write((byte)(Color0.r * 255));
        writer.Write((byte)(Color0.g * 255));
        writer.Write((byte)(Color0.b * 255));
        writer.Write((byte)(Color0.a * 128));
        writer.Write((byte)(Color1.r * 255));
        writer.Write((byte)(Color1.g * 255));
        writer.Write((byte)(Color1.b * 255));
        writer.Write((byte)(Color1.a * 128));
        writer.Write((byte)(Color2.r * 255));
        writer.Write((byte)(Color2.g * 255));
        writer.Write((byte)(Color2.b * 255));
        writer.Write((byte)(Color2.a * 128));
        writer.Write((byte)(Color3.r * 255));
        writer.Write((byte)(Color3.g * 255));
        writer.Write((byte)(Color3.b * 255));
        writer.Write((byte)(Color3.a * 128));
        writer.Write(Fade);
        writer.Write(0);
    }
}

public class GameCommandDrawWidget2D : IGameCommand
{
    public GameCommandIds Id => GameCommandIds.GAME_CMD_DRAW_WIDGET2D;
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
    public float ScaleX { get; set; }
    public float ScaleY { get; set; }
    public float Theta { get; set; }
    public Color Color { get; set; }
    public float TFrame { get; set; }

    public void Deserialize(BinaryReader reader, int size)
    {
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
        ScaleX = reader.ReadSingle();
        ScaleY = reader.ReadSingle();
        Theta = reader.ReadSingle();
        Color = reader.ReadUInt32().RCRgbaToColor();
        TFrame = reader.ReadSingle();
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}
