using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace RC
{
    [StructLayout(LayoutKind.Explicit)]
    public struct RCPointer
    {
        [FieldOffset(0x0)]
        public uint Address;

        public RCPointer(uint address) { Address = address; }
    }

    #region Vector

    [StructLayout(LayoutKind.Explicit)]
    public struct BSphere
    {
        [FieldOffset(0x0)]
        public float x;
        [FieldOffset(0x4)]
        public float y;
        [FieldOffset(0x8)]
        public float z;
        [FieldOffset(0xc)]
        public float rad;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct Matrix3x3
    {
        [FieldOffset(0x0)]
        public Vector4 row0;
        [FieldOffset(0x10)]
        public Vector4 row1;
        [FieldOffset(0x20)]
        public Vector4 row2;

        public Matrix4x4 ToMatrix4x4()
        {
            return new Matrix4x4(new Vector4(row0.x, row0.y, row0.z, 0), new Vector4(row1.x, row1.y, row1.z, 0), new Vector4(row2.x, row2.y, row2.z, 0), new Vector4(0,0,0,1));
        }
    }

    #endregion

    #region Moby

    [StructLayout(LayoutKind.Explicit)]
    public struct MobyInstance
    {
        [FieldOffset(0x0)]
        public BSphere bSphere;
        [FieldOffset(0x10)]
        public Vector4 pos;
        [FieldOffset(0x20)]
        public sbyte state;
        [FieldOffset(0x21)]
        public byte group;
        [FieldOffset(0x22)]
        public sbyte mClass;
        [FieldOffset(0x23)]
        public byte alpha;
        [FieldOffset(0x24)]
        public RCPointer pClass;
        [FieldOffset(0x28)]
        public RCPointer pChain;
        [FieldOffset(0x2c)]
        public sbyte collDamage;
        [FieldOffset(0x2d)]
        public sbyte deathCnt;
        [FieldOffset(0x2e)]
        public ushort occlIndex;
        [FieldOffset(0x30)]
        public sbyte updateDist;
        [FieldOffset(0x31)]
        public sbyte drawn;
        [FieldOffset(0x32)]
        public short drawDist;
        [FieldOffset(0x34)]
        public ushort modeBits;
        [FieldOffset(0x36)]
        public ushort modeBits2;
        [FieldOffset(0x38)]
        public ulong lights;
        [FieldOffset(0x40)]
        public RCPointer animSeq;
        [FieldOffset(0x44)]
        public float animSeqT;
        [FieldOffset(0x48)]
        public float animSpeed;
        [FieldOffset(0x4c)]
        public short animIScale;
        [FieldOffset(0x4e)]
        public short poseCacheEntryIndex;
        [FieldOffset(0x50)]
        public RCPointer animLayers;
        [FieldOffset(0x54)]
        public sbyte animSeqId;
        [FieldOffset(0x55)]
        public sbyte animFlags;
        [FieldOffset(0x56)]
        public sbyte lSeq;
        [FieldOffset(0x57)]
        public sbyte jointCnt;
        [FieldOffset(0x58)]
        public RCPointer jointCache;
        [FieldOffset(0x5c)]
        public RCPointer pManipulator;
        [FieldOffset(0x60)]
        public int glow_rgba;
        [FieldOffset(0x64)]
        public sbyte lod_trans;
        [FieldOffset(0x65)]
        public sbyte lod_trans2;
        [FieldOffset(0x66)]
        public sbyte metal;
        [FieldOffset(0x67)]
        public sbyte subState;
        [FieldOffset(0x68)]
        public sbyte prevState;
        [FieldOffset(0x69)]
        public sbyte stateType;
        [FieldOffset(0x6a)]
        public ushort stateTimer;
        [FieldOffset(0x6c)]
        public sbyte soundTrigger;
        [FieldOffset(0x6d)]
        public sbyte soundDesired;
        [FieldOffset(0x6e)]
        public short soundChannel;
        [FieldOffset(0x70)]
        public float scale;
        [FieldOffset(0x74)]
        public ushort bangles;
        [FieldOffset(0x76)]
        public sbyte shadow;
        [FieldOffset(0x77)]
        public sbyte shadow_index;
        [FieldOffset(0x78)]
        public float shadow_plane;
        [FieldOffset(0x7c)]
        public float shadow_range;
        [FieldOffset(0x80)]
        public BSphere lSphere;
        [FieldOffset(0x90)]
        public RCPointer netObject;
        [FieldOffset(0x94)]
        public short updateID;
        [FieldOffset(0x96)]
        public short spad0;
        [FieldOffset(0x98)]
        public RCPointer collData;
        [FieldOffset(0x9c)]
        public int collActive;
        [FieldOffset(0xa0)]
        public uint collCnt;
        [FieldOffset(0xa4)]
        public sbyte grid_min_x;
        [FieldOffset(0xa5)]
        public sbyte grid_min_y;
        [FieldOffset(0xa6)]
        public sbyte grid_max_x;
        [FieldOffset(0xa7)]
        public sbyte grid_max_y;
        [FieldOffset(0xa8)]
        public RCPointer pUpdate;
        [FieldOffset(0xac)]
        public RCPointer pVar;
        [FieldOffset(0xb0)]
        public sbyte mission;
        [FieldOffset(0xb1)]
        public sbyte pad;
        [FieldOffset(0xb2)]
        public short UID;
        [FieldOffset(0xb4)]
        public short bolts;
        [FieldOffset(0xb6)]
        public ushort xp;
        [FieldOffset(0xb8)]
        public RCPointer pParent;
        [FieldOffset(0xbc)]
        public short oClass;
        [FieldOffset(0xbe)]
        public sbyte triggers;
        [FieldOffset(0xbf)]
        public sbyte standarddeathcalled;
        [FieldOffset(0xc0)]
        public Matrix3x3 rMtx;
        [FieldOffset(0xf0)]
        public Vector4 rot;
    };


    [StructLayout(LayoutKind.Explicit)]
    public struct PVar_108A
    {
        [FieldOffset(0x0)]
        public Vector4 aimerPosition;
        [FieldOffset(0x10)]
        public Vector4 aimerNormal;
        [FieldOffset(0x20)]
        public Vector4 unk_20;
        [FieldOffset(0x30)]
        public int unk_30;
        [FieldOffset(0x34)]
        public int aimerVisible;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct PVar_10A5
    {
        [FieldOffset(0x0)]
        public Vector4 aimerPosition;
        [FieldOffset(0x10)]
        public Vector4 aimerNormal;
        [FieldOffset(0x20)]
        public Vector4 unk_20;
        [FieldOffset(0x30)]
        public Vector4 unk_30;
        [FieldOffset(0x40)]
        public int unk_40;
        [FieldOffset(0x44)]
        public int aimerVisible;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct PVar_2094
    {
        [FieldOffset(0x2E)]
        public short aimerVisible;
        [FieldOffset(0x5B0)]
        public Vector4 aimerPosition;
        [FieldOffset(0x5C0)]
        public Vector4 aimerNormal;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct PVar_2106
    {

    };


    #endregion

    #region Camera


    [StructLayout(LayoutKind.Explicit)]
    public struct CameraFov
    {
        [FieldOffset(0x0)]
        public float speed;
        [FieldOffset(0x4)]
        public float ideal;
        [FieldOffset(0x8)]
        public float actual;
        [FieldOffset(0xc)]
        public float gain;
        [FieldOffset(0x10)]
        public float damp;
        [FieldOffset(0x14)]
        public float limit;
        [FieldOffset(0x18)]
        public sbyte changeType;
        [FieldOffset(0x19)]
        public sbyte state;
        [FieldOffset(0x1a)]
        public short timer;
        [FieldOffset(0x1c)]
        public float timerInv;
        [FieldOffset(0x20)]
        public float init;
        [FieldOffset(0x24 + (0x4 * 0))]
        public float pad_0;
        [FieldOffset(0x24 + (0x4 * 1))]
        public float pad_1;
        [FieldOffset(0x24 + (0x4 * 2))]
        public float pad_2;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct CameraControlActivation
    {
        [FieldOffset(0x0)]
        public int activationType;
        [FieldOffset(0x4)]
        public float blendSpeed;
        [FieldOffset(0x8)]
        public sbyte priority;
        [FieldOffset(0x9)]
        public sbyte activate;
        [FieldOffset(0xa)]
        public short deactivate;
        [FieldOffset(0xc)]
        public short repCam;
        [FieldOffset(0xe)]
        public short orgCam;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct PolarSm
    {
        [FieldOffset(0x0)]
        public float azimuth;
        [FieldOffset(0x4)]
        public float elevation;
        [FieldOffset(0x8)]
        public float radius;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct Polar
    {
        [FieldOffset(0x0)]
        public float azimuth;
        [FieldOffset(0x4)]
        public float elevation;
        [FieldOffset(0x8)]
        public float radius;
        [FieldOffset(0xc)]
        public float rotY;
        [FieldOffset(0x10)]
        public float rotZ;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct UpdateCam
    {
        [FieldOffset(0x0 + (0x10 * 0))]
        public Vector4 mtx_0;
        [FieldOffset(0x0 + (0x10 * 1))]
        public Vector4 mtx_1;
        [FieldOffset(0x0 + (0x10 * 2))]
        public Vector4 mtx_2;
        [FieldOffset(0x30)]
        public Vector4 pos;
        [FieldOffset(0x40)]
        public Vector4 rot;
        [FieldOffset(0x50)]
        public Polar pol;
        [FieldOffset(0x64 + (0x4 * 0))]
        public float lPos_0;
        [FieldOffset(0x64 + (0x4 * 1))]
        public float lPos_1;
        [FieldOffset(0x64 + (0x4 * 2))]
        public float lPos_2;
        [FieldOffset(0x70)]
        public RCPointer control;
        [FieldOffset(0x74)]
        public CameraControlActivation activation;
        [FieldOffset(0x84)]
        public short importCameraIdx;
        [FieldOffset(0x86)]
        public short type;
        [FieldOffset(0x88)]
        public sbyte subType;
        [FieldOffset(0x89)]
        public sbyte bumped;
        [FieldOffset(0x8a)]
        public short bumpOff;
        [FieldOffset(0x8c)]
        public short funcIdx;
        [FieldOffset(0x8e)]
        public short active;
        [FieldOffset(0x90)]
        public float fov;
        [FieldOffset(0x94)]
        public int gameCamIdx;
        [FieldOffset(0x98)]
        public float prevExternalMoveZ;
        [FieldOffset(0x9C + (0x4 * 0))]
        public int pad_0;
        [FieldOffset(0x9C + (0x4 * 1))]
        public int pad_1;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct CameraStatics
    {
        [FieldOffset(0x0 + (0x4 * 0))]
        public float heroNoJump_0;
        [FieldOffset(0x0 + (0x4 * 1))]
        public float heroNoJump_1;
        [FieldOffset(0x0 + (0x4 * 2))]
        public float heroNoJump_2;
        [FieldOffset(0xc)]
        public float heroLastZ;
        [FieldOffset(0x10)]
        public float heroLastZSpeed;
        [FieldOffset(0x20)]
        public Vector4 heroUp;
        [FieldOffset(0x30)]
        public Vector4 heroUpActual;
        [FieldOffset(0x40)]
        public Vector4 heroUpLast;
        [FieldOffset(0x50)]
        public Vector4 heroUpSpeed;
        [FieldOffset(0x60)]
        public Vector4 heroLastPos;
        [FieldOffset(0x70)]
        public Vector4 heroMoveVec;
        [FieldOffset(0x80)]
        public Vector4 heroMoveVec2D;
        [FieldOffset(0x90)]
        public Vector4 heroMoveVecUp;
        [FieldOffset(0xa0)]
        public float heroSpeed;
        [FieldOffset(0xa4)]
        public float heroSpeed2D;
        [FieldOffset(0xa8)]
        public float heroSpeedUp;
        [FieldOffset(0xAC + (0x4 * 0))]
        public float heroLastRotZ_0;
        [FieldOffset(0xAC + (0x4 * 1))]
        public float heroLastRotZ_1;
        [FieldOffset(0xAC + (0x4 * 2))]
        public float heroLastRotZ_2;
        [FieldOffset(0xAC + (0x4 * 3))]
        public float heroLastRotZ_3;
        [FieldOffset(0xAC + (0x4 * 4))]
        public float heroLastRotZ_4;
        [FieldOffset(0xc0)]
        public int hotspot;
        [FieldOffset(0xc4)]
        public RCPointer pCamColl;
        [FieldOffset(0xc8)]
        public float fadeSpeed;
        [FieldOffset(0xcc)]
        public float fadeIdeal;
        [FieldOffset(0xd0)]
        public int fadeTimer;
        [FieldOffset(0xd4)]
        public float flashInSpeed;
        [FieldOffset(0xd8)]
        public float flashOutSpeed;
        [FieldOffset(0xdc)]
        public float flashIdeal;
        [FieldOffset(0xe0)]
        public int flashTimer;
        [FieldOffset(0xe4)]
        public RCPointer boss;
        [FieldOffset(0xe8)]
        public int bossTimer;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct CamBlenderPosQuat
    {
        [FieldOffset(0x0)]
        public float quatInterpFac;
        [FieldOffset(0x4)]
        public float quatInterpAdd;
        [FieldOffset(0x8)]
        public float reqQuatInterpAdd;
        [FieldOffset(0xc)]
        public float reqQuatInterpInit;
        [FieldOffset(0x10)]
        public float posInterpFac;
        [FieldOffset(0x14)]
        public float posInterpAdd;
        [FieldOffset(0x18)]
        public float reqPosInterpAdd;
        [FieldOffset(0x1c)]
        public float reqPosInterpInit;
        [FieldOffset(0x20)]
        public Vector4 orgQuat;
        [FieldOffset(0x30)]
        public Vector4 orgPos;
        [FieldOffset(0x40)]
        public Vector4 pos;
        [FieldOffset(0x50)]
        public Vector4 q;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct CamBlenderPolarQuat
    {
        [FieldOffset(0x0)]
        public PolarSm orgPol;
        [FieldOffset(0xc)]
        public int interpFrames;
        [FieldOffset(0x10)]
        public float interpMaxInv;
        [FieldOffset(0x14)]
        public int reqInterpFrames;
        [FieldOffset(0x20)]
        public Vector4 fwd;
        [FieldOffset(0x30)]
        public Vector4 up;
        [FieldOffset(0x40)]
        public Vector4 orgQuat;
        [FieldOffset(0x50)]
        public Vector4 pos;
        [FieldOffset(0x60)]
        public Vector4 q;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct CamBlenderData
    {
        [FieldOffset(0x0)]
        public CamBlenderPosQuat posData;
        [FieldOffset(0x60)]
        public CamBlenderPolarQuat polarData;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct CamBlender
    {
        [FieldOffset(0x0)]
        public short state;
        [FieldOffset(0x2)]
        public sbyte type;
        [FieldOffset(0x3)]
        public sbyte reqType;
        [FieldOffset(0x10)]
        public CamBlenderData blendData;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct CameraWidget
    {
        [FieldOffset(0x0)]
        public RCPointer pCam;
        [FieldOffset(0x4)]
        public RCPointer semaphore;
        [FieldOffset(0x8)]
        public float closest;
        [FieldOffset(0xc)]
        public float interp;
        [FieldOffset(0x10)]
        public RCPointer preFunc;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct CameraShake
    {
        [FieldOffset(0x0)]
        public float strength;
        [FieldOffset(0x4)]
        public float adjust;
        [FieldOffset(0x8)]
        public int time;
        [FieldOffset(0xc)]
        public int div;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct CameraExternal
    {
        [FieldOffset(0x0)]
        public Vector4 move;
    };

    // warning: multiple differing types with the same name, only one recovered
    [StructLayout(LayoutKind.Explicit)]
    public struct CameraHeroData
    {
        [FieldOffset(0x0)]
        public Matrix4x4 mtx;
        [FieldOffset(0x40)]
        public Vector4 pos;
        [FieldOffset(0x50)]
        public Vector4 rot;
        [FieldOffset(0x60)]
        public Vector4 cg;
        [FieldOffset(0x70)]
        public Vector4 moveActualFromExternal;
        [FieldOffset(0x80)]
        public Vector4 groundGravity;
        [FieldOffset(0x90)]
        public Vector4 jumpGravity;
        [FieldOffset(0xa0)]
        public Vector4 sphereCenter;
        [FieldOffset(0xB0 + (0x10 * 0))]
        public Vector4 fpsCamMtx_0;
        [FieldOffset(0xB0 + (0x10 * 1))]
        public Vector4 fpsCamMtx_1;
        [FieldOffset(0xB0 + (0x10 * 2))]
        public Vector4 fpsCamMtx_2;
        [FieldOffset(0xe0)]
        public Vector4 fpsCamPos;
        [FieldOffset(0xf0)]
        public RCPointer pMoby;
        [FieldOffset(0xf4)]
        public RCPointer vehicleMoby;
        [FieldOffset(0xf8)]
        public RCPointer groundMoby;
        [FieldOffset(0xfc)]
        public int desiredCam;
        [FieldOffset(0x100)]
        public int state;
        [FieldOffset(0x104)]
        public int stateType;
        [FieldOffset(0x108)]
        public int previousState;
        [FieldOffset(0x10c)]
        public float moveSpeed;
        [FieldOffset(0x110)]
        public float moveSpeed2D;
        [FieldOffset(0x114)]
        public float groundDist;
        [FieldOffset(0x118)]
        public float groundWaterHeight;
        [FieldOffset(0x11c)]
        public int groundOnGood;
        [FieldOffset(0x120)]
        public short groundOffAny;
        [FieldOffset(0x122)]
        public short groundOffGood;
        [FieldOffset(0x124)]
        public short groundMagnetic;
        [FieldOffset(0x126)]
        public short lockOnStrafing;
        [FieldOffset(0x128)]
        public short jumpFramesToLand;
        [FieldOffset(0x12a)]
        public sbyte jumpDescend;
        [FieldOffset(0x12b)]
        public sbyte critterMode;
        [FieldOffset(0x12c)]
        public sbyte multiplayer;
        [FieldOffset(0x12d)]
        public sbyte fpsActive;
        [FieldOffset(0x12e)]
        public sbyte hotSpotLava;
        [FieldOffset(0x12f)]
        public sbyte hotSpotDeathSand;
        [FieldOffset(0x130)]
        public sbyte hotSpotQuickSand;
        [FieldOffset(0x131)]
        public sbyte hotSpotIceWater;
        [FieldOffset(0x132)]
        public sbyte hotSpotWater;
        [FieldOffset(0x133)]
        public sbyte aiFollowingMe;
        [FieldOffset(0x134)]
        public RCPointer pPad;
        [FieldOffset(0x138)]
        public RCPointer pSwingTargetMoby;
        [FieldOffset(0x13c)]
        public float swingForwardAng;
        [FieldOffset(0x140)]
        public float swingIdealRadius;
        [FieldOffset(0x144)]
        public int timersLedgeCamAdj;
        [FieldOffset(0x148)]
        public float ledgeWallAngZ;
        [FieldOffset(0x14c)]
        public int EOPtime;
        [FieldOffset(0x150)]
        public RCPointer pPath;
        [FieldOffset(0x154 + (0x4 * 0))]
        public int pad_0;
        [FieldOffset(0x154 + (0x4 * 1))]
        public int pad_1;
        [FieldOffset(0x154 + (0x4 * 2))]
        public int pad_2;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct GameCamera
    {
        [FieldOffset(0x0)]
        public Vector4 pos;
        [FieldOffset(0x10)]
        public Vector4 rot;
        [FieldOffset(0x20)]
        public CameraShake shake;
        [FieldOffset(0x30)]
        public CameraShake shakeFwd;
        [FieldOffset(0x40)]
        public CameraShake shakeTilt;
        [FieldOffset(0x50)]
        public RCPointer pCurrentUpdCam;
        [FieldOffset(0x54)]
        public RCPointer pLastUpdCam;
        [FieldOffset(0x60)]
        public CameraStatics camStatics;
        [FieldOffset(0x150)]
        public CameraHeroData camHeroData;
        [FieldOffset(0x2b0)]
        public CamBlender blender;
        [FieldOffset(0x390)]
        public Matrix3x3 uMtx;
        [FieldOffset(0x3c0)]
        public Matrix4x4 bsMtx;
        [FieldOffset(0x400)]
        public CameraWidget widget;
        [FieldOffset(0x420)]
        public CameraExternal external;
        [FieldOffset(0x430)]
        public CameraFov fov;
        [FieldOffset(0x460)]
        public int CamUnderWater;
        [FieldOffset(0x464)]
        public int camTimer;
        [FieldOffset(0x468)]
        public int disableBlendTimer;
    };

    #endregion

    #region Hero

    public enum eWeaponSlotID
    {
        None = -1,
        Wrench,
        DualVipers,
        MagmaCannon,
        Arbiter,
        FusionRifle,
        MineLauncher,
        B6,
        Flail,
        HoloShield,
        WEAPONS_MAX
    }

    public enum eGadgetID : int
    {
        GADGET_UNDEFINED = 0,
        GADGET_WRENCH = 1,
        GADGET_MACHINEGUN = 2,
        GADGET_SHOTGUN = 3,
        GADGET_ROCKETLAUNCHER = 4,
        GADGET_SNIPERGUN = 5,
        GADGET_MINEGUN = 6,
        GADGET_GRENADELAUNCHER = 7,
        GADGET_HOLOSHIELD = 8,
        GADGET_MINITURRET = 9,
        GADGET_BFG = 10,
        GADGET_GRINDRAIL = 11,
        GADGET_EMP = 12,
        GADGET_HACKERRAY = 13,
        GADGET_MP_GRAPPLING_HOOK = 14,
        GADGET_FLAIL = 15,
        GADGET_SHIELDLINK = 16,
        GADGET_MP_CHARGE_BOOTS = 17,
        GADGET_MP_MAGNE_BOOTS = 18,
        GADGET_MP_GRIND_BOOTS = 19,
        GADGET_MP_UNEQUIP = 20,
        TOTAL_GADGETS = 20,
        TOTAL_GADGETS_DEFS_SIZE = 20,
    };

    public enum HERO_STATE_ENUM
    {
        HERO_STATE_IDLE = 0,
        HERO_STATE_LOOK = 1,
        HERO_STATE_WALK = 2,
        HERO_STATE_SKID = 3,
        HERO_STATE_CROUCH = 4,
        HERO_STATE_QUICK_TURN = 5,
        HERO_STATE_FALL = 6,
        HERO_STATE_JUMP = 7,
        HERO_STATE_GLIDE = 8,
        HERO_STATE_RUN_JUMP = 9,
        HERO_STATE_LONG_JUMP = 10,
        HERO_STATE_FLIP_JUMP = 11,
        HERO_STATE_JINK_JUMP = 12,
        HERO_STATE_ROCKET_JUMP = 13,
        HERO_STATE_DOUBLE_JUMP = 14,
        HERO_STATE_HELI_JUMP = 15,
        HERO_STATE_CHARGE_JUMP = 16,
        HERO_STATE_WALL_JUMP = 17,
        HERO_STATE_WATER_JUMP = 18,
        HERO_STATE_COMBO_ATTACK = 19,
        HERO_STATE_JUMP_ATTACK = 20,
        HERO_STATE_THROW_ATTACK = 21,
        HERO_STATE_GET_HIT = 22,
        HERO_STATE_LEDGE_GRAB = 23,
        HERO_STATE_LEDGE_IDLE = 24,
        HERO_STATE_LEDGE_TRAVERSE_LEFT = 25,
        HERO_STATE_LEDGE_TRAVERSE_RIGHT = 26,
        HERO_STATE_LEDGE_JUMP = 27,
        HERO_STATE_VISIBOMB = 28,
        HERO_STATE_TARGETING = 29,
        HERO_STATE_GUN_WAITING = 30,
        HERO_STATE_WALLOPER_ATTACK = 31,
        HERO_STATE_ATTACK_BOUNCE = 32,
        HERO_STATE_ROCKET_STOMP = 33,
        HERO_STATE_GLOVE_ATTACK = 34,
        HERO_STATE_GRAPPLE_SHOOT = 35,
        HERO_STATE_GRAPPLE_PULL = 36,
        HERO_STATE_GRAPPLE_PULL_VEHICLE = 37,
        HERO_STATE_SUCK_CANNON = 38,
        HERO_STATE_GRIND = 39,
        HERO_STATE_GRIND_JUMP = 40,
        HERO_STATE_GRIND_SWITCH_JUMP = 41,
        HERO_STATE_GRIND_ATTACK = 42,
        HERO_STATE_SWING = 43,
        HERO_STATE_SWING_FALL = 44,
        HERO_STATE_RECOIL = 45,
        HERO_STATE_ICE_WALK = 46,
        HERO_STATE_DEVASTATOR = 47,
        HERO_STATE_SLIDE = 48,
        HERO_STATE_VEHICLE = 49,
        HERO_STATE_SWIMUNDER = 50,
        HERO_STATE_IDLEUNDER = 51,
        HERO_STATE_CHARGEUNDER = 52,
        HERO_STATE_SWIMSURF = 53,
        HERO_STATE_IDLESURF = 54,
        HERO_STATE_BOLT_CRANK = 55,
        HERO_STATE_LAVA_JUMP = 56,
        HERO_STATE_DEATH = 57,
        HERO_STATE_BOARD = 58,
        HERO_STATE_MAGNE_WALK = 59,
        HERO_STATE_GRIND_HIT = 62,
        HERO_STATE_GRIND_JUMP_TURN = 63,
        HERO_STATE_VENDOR_BOOTH = 98,
        HERO_STATE_NPC = 99,
        HERO_STATE_WALK_TO_POS = 100,
        HERO_STATE_SKID_TO_POS = 101,
        HERO_STATE_IDLE_TO_POS = 102,
        HERO_STATE_JUMP_TO_POS = 103,
        HERO_STATE_QUICKSAND_SINK = 104,
        HERO_STATE_QUICKSAND_JUMP = 105,
        HERO_STATE_DROWN = 106,
        HERO_STATE_MAGNE_ATTACK = 111,
        HERO_STATE_MAGNE_JUMP = 112,
        HERO_STATE_CUT_SCENE = 113,
        HERO_STATE_WADE = 114,
        HERO_STATE_ZIP = 115,
        HERO_STATE_GET_HIT_SURF = 116,
        HERO_STATE_GET_HIT_UNDER = 117,
        HERO_STATE_DEATH_FALL = 118,
        HERO_STATE_SLOPESLIDE = 120,
        HERO_STATE_JUMP_BOUNCE = 121,
        HERO_STATE_DEATHSAND_SINK = 122,
        HERO_STATE_LAVA_DEATH = 123,
        HERO_STATE_CHARGE = 125,
        HERO_STATE_ICEWATER_FREEZE = 126,
        HERO_STATE_ELECTRIC_DEATH = 127,
        HERO_STATE_ROCKET_HOVER = 128,
        HERO_STATE_ELECTRIC_DEATH_UNDER = 129,
        HERO_STATE_SKATE = 130,
        HERO_STATE_MOON_JUMP = 131,
        HERO_STATE_JET = 132,
        HERO_STATE_THROW_SHURIKEN = 133,
        HERO_STATE_RACEBIKE = 134,
        HERO_STATE_SPEEDBOAT = 135,
        HERO_STATE_HOVERPLANE = 136,
        HERO_STATE_LATCH_GRAB = 137,
        HERO_STATE_LATCH_IDLE = 138,
        HERO_STATE_LATCH_JUMP = 139,
        HERO_STATE_PULLSHOT_ATTACH = 140,
        HERO_STATE_PULLSHOT_PULL = 141,
        HERO_STATE_GET_FLATTENED = 142,
        HERO_STATE_SKYDIVE = 143,
        HERO_STATE_ELECTRIC_GET_HIT = 144,
        HERO_STATE_FLAIL_ATTACK = 145,
        HERO_STATE_MAGIC_TELEPORT = 146,
        HERO_STATE_TELEPORT_IN = 147,
        HERO_STATE_DEATH_NO_FALL = 148,
        HERO_STATE_TAUNT_SQUAT = 149,
        HERO_STATE_TAUNT_ASSPOINT = 150,
        HERO_STATE_TAUNT_ASSRUB = 151,
        HERO_STATE_TURRET_DRIVER = 152,
        HERO_STATE_WAIT_FOR_RESURRECT = 153,
        HERO_STATE_WAIT_FOR_JOIN = 154,
        HERO_STATE_DROPPED = 155,
        HERO_STATE_CNT = 156
    };

    public enum HERO_TYPE_ENUM
    {
        HERO_TYPE_IDLE = 0,
        HERO_TYPE_WALK = 1,
        HERO_TYPE_FALL = 2,
        HERO_TYPE_LEDGE = 3,
        HERO_TYPE_JUMP = 4,
        HERO_TYPE_GLIDE = 5,
        HERO_TYPE_ATTACK = 6,
        HERO_TYPE_GET_HIT = 7,
        HERO_TYPE_SHOOT = 8,
        HERO_TYPE_BUSY = 9,
        HERO_TYPE_BOUNCE = 10,
        HERO_TYPE_STOMP = 11,
        HERO_TYPE_CROUCH = 12,
        HERO_TYPE_GRAPPLE = 13,
        HERO_TYPE_SWING = 14,
        HERO_TYPE_GRIND = 15,
        HERO_TYPE_SLIDE = 16,
        HERO_TYPE_SWIM = 17,
        HERO_TYPE_SURF = 18,
        HERO_TYPE_HYDRO = 19,
        HERO_TYPE_DEATH = 20,
        HERO_TYPE_BOARD = 21,
        HERO_TYPE_RACEBOARD = 22,
        HERO_TYPE_SPIN = 23,
        HERO_TYPE_NPC = 24,
        HERO_TYPE_QUICKSAND = 25,
        HERO_TYPE_ZIP = 26,
        HERO_TYPE_HOLO = 27,
        HERO_TYPE_CHARGE = 28,
        HERO_TYPE_ROCKET_HOVER = 29,
        HERO_TYPE_JET = 30,
        HERO_TYPE_RACEBIKE = 31,
        HERO_TYPE_SPEEDBOAT = 32,
        HERO_TYPE_PULL = 33,
        HERO_TYPE_LATCH = 34,
        HERO_TYPE_LADDER = 36,
        HERO_TYPE_SKYDIVE = 37,
        HERO_TYPE_CNT = 38
    };

    public enum eModBasicType
    {
        MOD_BSC_UNDEFINED = 0,
        MOD_BSC_SPEED = 1,
        MOD_BSC_AMMO = 2,
        MOD_BSC_AIMING = 3,
        MOD_BSC_IMPACT = 4,
        MOD_BSC_AREA = 5,
        MOD_BSC_XP = 6,
        MOD_BSC_JACKPOT = 7,
        MOD_BSC_NANOLEECH = 8,
        TOTAL_BASIC_MODS = 9,
        TOTAL_MOD_BSC_DEFS_SIZE = 9
    };

    public enum eModPostFXType
    {
        MOD_PFX_UNDEFINED = 0,
        MOD_PFX_NAPALM = 1,
        MOD_PFX_ELECTRICDOOM = 2,
        MOD_PFX_FREEZING = 3,
        MOD_PFX_BOMBLETS = 4,
        MOD_PFX_MORPHING = 5,
        MOD_PFX_INFECTION = 6,
        MOD_PFX_PLAGUE = 7,
        MOD_PFX_SHOCK = 8,
        TOTAL_POSTFX_MODS = 9,
        TOTAL_MOD_PFX_DEFS_SIZE = 9
    };

    public enum eModWeaponType
    {
        MOD_WPN_UNDEFINED = 0,
        MOD_WPN_ROCKET_GUIDANCE = 1,
        MOD_WPN_SHOTGUN_WIDTH = 2,
        MOD_WPN_SHOTGUN_LENGTH = 3,
        MOD_WPN_GRENADE_MANUALDET = 4,
        MOD_WPN_MACHGUN_BEAM = 5,
        MOD_WPN_SNIPER_PIERCING = 6,
        TOTAL_WPN_MODS = 7,
        TOTAL_MOD_WPN_DEFS_SIZE = 7
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct Gid
    {
        [FieldOffset(0)]
        public uint UID;

        public uint ObjectIndex => UID & 0b00000000000000000000111111111111;
        public uint ObjectCount => UID & 0b00000000111111111111000000000000;
        public uint ObjectType => UID &  0b00001111000000000000000000000000;
        public uint HostId => UID &      0b11110000000000000000000000000000;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroTimers
    {
        [FieldOffset(0x0)]
        public int state;
        [FieldOffset(0x4)]
        public int stateType;
        [FieldOffset(0x8)]
        public int subState;
        [FieldOffset(0xc)]
        public int animState;
        [FieldOffset(0x10)]
        public int stickOn;
        [FieldOffset(0x14)]
        public int stickOff;
        [FieldOffset(0x18)]
        public short noLedge;
        [FieldOffset(0x1a)]
        public short allowQuickSelect;
        [FieldOffset(0x1c)]
        public int firing;
        [FieldOffset(0x20)]
        public int moveModifierTimer;
        [FieldOffset(0x24)]
        public int boltMultTimer;
        [FieldOffset(0x28)]
        public int wallJumpOk;
        [FieldOffset(0x2c)]
        public short postHitInvinc;
        [FieldOffset(0x2e)]
        public short ignoreHeroColl;
        [FieldOffset(0x30)]
        public short collOff;
        [FieldOffset(0x32)]
        public short invisible;
        [FieldOffset(0x34)]
        public short slide;
        [FieldOffset(0x36)]
        public short bezerker;
        [FieldOffset(0x38)]
        public short noWallJump;
        [FieldOffset(0x3a)]
        public short noJumps;
        [FieldOffset(0x3c)]
        public short boxBreaking;
        [FieldOffset(0x3e)]
        public short noMag;
        [FieldOffset(0x40)]
        public short noChargeJump;
        [FieldOffset(0x42)]
        public short resurrectWait;
        [FieldOffset(0x44)]
        public int timeSinceStrafe;
        [FieldOffset(0x48)]
        public short noHackerSwitch;
        [FieldOffset(0x4a)]
        public short noInput;
        [FieldOffset(0x4c)]
        public short noJumpLookBack;
        [FieldOffset(0x4e)]
        public short noShockAbort;
        [FieldOffset(0x50)]
        public short stuck;
        [FieldOffset(0x52)]
        public short noSwing;
        [FieldOffset(0x54)]
        public short noWaterJump;
        [FieldOffset(0x56)]
        public short noWaterDive;
        [FieldOffset(0x58)]
        public short facialExpression;
        [FieldOffset(0x5a)]
        public short idle;
        [FieldOffset(0x5c)]
        public short bumpPushing;
        [FieldOffset(0x5e)]
        public short lookButton;
        [FieldOffset(0x60)]
        public short edgeStop;
        [FieldOffset(0x62)]
        public short clankRedEye;
        [FieldOffset(0x64)]
        public short edgePath;
        [FieldOffset(0x66)]
        public short magSlope;
        [FieldOffset(0x68)]
        public short ledgeCamAdj;
        [FieldOffset(0x6a)]
        public short screenFlashRed;
        [FieldOffset(0x6c)]
        public short holdDeathPose;
        [FieldOffset(0x6e)]
        public short strafeMove;
        [FieldOffset(0x70)]
        public short noRaisedGunArm;
        [FieldOffset(0x72)]
        public short noExternalRot;
        [FieldOffset(0x74)]
        public short screenFlashOn;
        [FieldOffset(0x76)]
        public short screenFadeOn;
        [FieldOffset(0x78)]
        public int lastVehicleTimer;
        [FieldOffset(0x7c)]
        public float gadgetRefire;
        [FieldOffset(0x80)]
        public int timeAlive;
        [FieldOffset(0x84)]
        public int noFpsCamTimer;
        [FieldOffset(0x88)]
        public int endDeathEarly;
        [FieldOffset(0x8c)]
        public short forceGlide;
        [FieldOffset(0x8e)]
        public short noGrind;
        [FieldOffset(0x90)]
        public short instaGrind;
        [FieldOffset(0x92)]
        public short noCamInputTimer;
        [FieldOffset(0x94)]
        public short postTeleportTimer;
        [FieldOffset(0x96)]
        public short multiKillTimer;
        [FieldOffset(0x98)]
        public short armorLevelTimer;
        [FieldOffset(0x9a)]
        public short damageMuliplierTimer;
        [FieldOffset(0x9c)]
        public int powerupEffectTimer;
        [FieldOffset(0xa0)]
        public short juggernautFadeTimer;
        [FieldOffset(0xa2)]
        public short onFireTimer;
        [FieldOffset(0xa4)]
        public short acidTimer;
        [FieldOffset(0xa6)]
        public short freezeTimer;
        [FieldOffset(0xa8)]
        public short noHelmTimer;
        [FieldOffset(0xaa)]
        public short elecTimer;
        [FieldOffset(0xac)]
        public short boltDistMulTimer;
        [FieldOffset(0xae)]
        public short explodeTimer;
        [FieldOffset(0xb0)]
        public short noDeathTimer;
        [FieldOffset(0xb2)]
        public short invincibilityTimer;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroQueuedSound
    {
        [FieldOffset(0x0)]
        public short active;
        [FieldOffset(0x2)]
        public short sound;
        [FieldOffset(0x4)]
        public short timer;
        [FieldOffset(0x6)]
        public short flags;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroSpecialIdleDef
    {
        [FieldOffset(0x0)]
        public int anim;
        [FieldOffset(0x4)]
        public float frequency;
        [FieldOffset(0x8)]
        public float minRepeatTime;
        [FieldOffset(0xc)]
        public int repeatTimer;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct tNW_GetHitMessage
    {
        [FieldOffset(0x0)]
        public Vector3 momentum;
        [FieldOffset(0xc)]
        public byte sequenceNum;
        [FieldOffset(0xd)]
        public sbyte hitPlayerIndex;
        [FieldOffset(0xe)]
        public sbyte frame;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroCommand
    {
        [FieldOffset(0x0)]
        public int state;
        [FieldOffset(0x4)]
        public RCPointer pCurTarget;
        [FieldOffset(0x8)]
        public int timer;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct FlashVars
    {
        [FieldOffset(0x0)]
        public short timer;
        [FieldOffset(0x2)]
        public short type;
        [FieldOffset(0x4)]
        public int destColor;
        [FieldOffset(0x8)]
        public int srcColor;
        [FieldOffset(0xc)]
        public int flags;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct TargetVars
    {
        [FieldOffset(0x0)]
        public float hitPoints;
        [FieldOffset(0x4)]
        public int maxHitPoints;
        [FieldOffset(0x8 + (0x1 * 0))]
        public byte attackDamage_0;
        [FieldOffset(0x8 + (0x1 * 1))]
        public byte attackDamage_1;
        [FieldOffset(0x8 + (0x1 * 2))]
        public byte attackDamage_2;
        [FieldOffset(0x8 + (0x1 * 3))]
        public byte attackDamage_3;
        [FieldOffset(0x8 + (0x1 * 4))]
        public byte attackDamage_4;
        [FieldOffset(0x8 + (0x1 * 5))]
        public byte attackDamage_5;
        [FieldOffset(0xe)]
        public short hitCount;
        [FieldOffset(0x10)]
        public int flags;
        [FieldOffset(0x14)]
        public float targetHeight;
        [FieldOffset(0x18)]
        public RCPointer mobyThatHurtMeLast;
        [FieldOffset(0x1c)]
        public float camPushDist;
        [FieldOffset(0x20)]
        public float camPushHeight;
        [FieldOffset(0x24)]
        public short damageCounter;
        [FieldOffset(0x26)]
        public short empTimer;
        [FieldOffset(0x28)]
        public short infectedTimer;
        [FieldOffset(0x2a)]
        public short invincTimer;
        [FieldOffset(0x2c)]
        public short bogeyType;
        [FieldOffset(0x2e)]
        public short team;
        [FieldOffset(0x30)]
        public sbyte lookAtMeDist;
        [FieldOffset(0x31)]
        public sbyte lookAtMePriority;
        [FieldOffset(0x32)]
        public sbyte lookAtMeZOfsIn8ths;
        [FieldOffset(0x33)]
        public sbyte lookAtMeJoint;
        [FieldOffset(0x34)]
        public sbyte lookAtMeExpression;
        [FieldOffset(0x35)]
        public sbyte lockOnPriority;
        [FieldOffset(0x36)]
        public sbyte soundType;
        [FieldOffset(0x37)]
        public sbyte targetRadiusIn8ths;
        [FieldOffset(0x38)]
        public sbyte noAutoTrack;
        [FieldOffset(0x39)]
        public sbyte trackSpeedInMps;
        [FieldOffset(0x3a)]
        public sbyte camModOverride;
        [FieldOffset(0x3b)]
        public sbyte destroyMe;
        [FieldOffset(0x3c)]
        public sbyte morphoraySpecial;
        [FieldOffset(0x3d)]
        public sbyte headJoint;
        [FieldOffset(0x3e)]
        public sbyte hitByContinuous;
        [FieldOffset(0x3f)]
        public sbyte infected;
        [FieldOffset(0x40)]
        public sbyte empFxTimer;
        [FieldOffset(0x41)]
        public sbyte weaponTargetedOnMe;
        [FieldOffset(0x42)]
        public sbyte isOrganic;
        [FieldOffset(0x43)]
        public sbyte bundleIndex;
        [FieldOffset(0x44)]
        public sbyte bundleDamage;
        [FieldOffset(0x45)]
        public sbyte firedAt;
        [FieldOffset(0x46)]
        public sbyte weaponThatHurtMeLast;
        [FieldOffset(0x47)]
        public sbyte invalidTarget;
        [FieldOffset(0x48)]
        public int maxDifficultySlotted;
        [FieldOffset(0x4c)]
        public int curDifficultySlotted;
        [FieldOffset(0x50 + (0x4 * 0))]
        public RCPointer pTargettedByBogeys_0;
        [FieldOffset(0x50 + (0x4 * 1))]
        public RCPointer pTargettedByBogeys_1;
        [FieldOffset(0x50 + (0x4 * 2))]
        public RCPointer pTargettedByBogeys_2;
        [FieldOffset(0x50 + (0x4 * 3))]
        public RCPointer pTargettedByBogeys_3;
        [FieldOffset(0x50 + (0x4 * 4))]
        public RCPointer pTargettedByBogeys_4;
        [FieldOffset(0x50 + (0x4 * 5))]
        public RCPointer pTargettedByBogeys_5;
        [FieldOffset(0x50 + (0x4 * 6))]
        public RCPointer pTargettedByBogeys_6;
        [FieldOffset(0x50 + (0x4 * 7))]
        public RCPointer pTargettedByBogeys_7;
        [FieldOffset(0x70)]
        public RCPointer mobyThatFiredAtMe;
        [FieldOffset(0x74)]
        public int targetShadowMask;
        [FieldOffset(0x78)]
        public int damageTypes;
        [FieldOffset(0x7c)]
        public int padA;
        [FieldOffset(0x80)]
        public float morphDamage;
        [FieldOffset(0x84)]
        public float freezeDamage;
        [FieldOffset(0x88)]
        public float infectDamage;
        [FieldOffset(0x8c)]
        public float lastDamage;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct MotionBlur
    {
        [FieldOffset(0x0 + (0x10 * 0))]
        public Vector4 posRing_0;
        [FieldOffset(0x0 + (0x10 * 1))]
        public Vector4 posRing_1;
        [FieldOffset(0x0 + (0x10 * 2))]
        public Vector4 posRing_2;
        [FieldOffset(0x0 + (0x10 * 3))]
        public Vector4 posRing_3;
        [FieldOffset(0x0 + (0x10 * 4))]
        public Vector4 posRing_4;
        [FieldOffset(0x0 + (0x10 * 5))]
        public Vector4 posRing_5;
        [FieldOffset(0x0 + (0x10 * 6))]
        public Vector4 posRing_6;
        [FieldOffset(0x0 + (0x10 * 7))]
        public Vector4 posRing_7;
        [FieldOffset(0x80 + (0x10 * 0))]
        public Vector4 rotRing_0;
        [FieldOffset(0x80 + (0x10 * 1))]
        public Vector4 rotRing_1;
        [FieldOffset(0x80 + (0x10 * 2))]
        public Vector4 rotRing_2;
        [FieldOffset(0x80 + (0x10 * 3))]
        public Vector4 rotRing_3;
        [FieldOffset(0x80 + (0x10 * 4))]
        public Vector4 rotRing_4;
        [FieldOffset(0x80 + (0x10 * 5))]
        public Vector4 rotRing_5;
        [FieldOffset(0x80 + (0x10 * 6))]
        public Vector4 rotRing_6;
        [FieldOffset(0x80 + (0x10 * 7))]
        public Vector4 rotRing_7;
        [FieldOffset(0x100 + (0x4 * 0))]
        public int blurAlphas_0;
        [FieldOffset(0x100 + (0x4 * 1))]
        public int blurAlphas_1;
        [FieldOffset(0x100 + (0x4 * 2))]
        public int blurAlphas_2;
        [FieldOffset(0x100 + (0x4 * 3))]
        public int blurAlphas_3;
        [FieldOffset(0x110 + (0x4 * 0))]
        public int blurSteps_0;
        [FieldOffset(0x110 + (0x4 * 1))]
        public int blurSteps_1;
        [FieldOffset(0x110 + (0x4 * 2))]
        public int blurSteps_2;
        [FieldOffset(0x110 + (0x4 * 3))]
        public int blurSteps_3;
        [FieldOffset(0x120 + (0x4 * 0))]
        public RCPointer blurMobys_0;
        [FieldOffset(0x120 + (0x4 * 1))]
        public RCPointer blurMobys_1;
        [FieldOffset(0x120 + (0x4 * 2))]
        public RCPointer blurMobys_2;
        [FieldOffset(0x120 + (0x4 * 3))]
        public RCPointer blurMobys_3;
        [FieldOffset(0x130 + (0x4 * 0))]
        public float gapReduction_0;
        [FieldOffset(0x130 + (0x4 * 1))]
        public float gapReduction_1;
        [FieldOffset(0x130 + (0x4 * 2))]
        public float gapReduction_2;
        [FieldOffset(0x130 + (0x4 * 3))]
        public float gapReduction_3;
        [FieldOffset(0x140)]
        public short ringIndex;
        [FieldOffset(0x142)]
        public short ringValidSize;
        [FieldOffset(0x144)]
        public RCPointer pTrackedMoby;
        [FieldOffset(0x148)]
        public int blurCnt;
        [FieldOffset(0x14c)]
        public int active;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct Gadget
    {
        [FieldOffset(0x0)]
        public Vector4 jointPos;
        [FieldOffset(0x10)]
        public Vector4 jointRot;
        [FieldOffset(0x20)]
        public RCPointer pMoby;
        [FieldOffset(0x24)]
        public RCPointer pMoby2;
        [FieldOffset(0x28)]
        public bool padButtonDown;
        [FieldOffset(0x2c)]
        public int alignPad;
        [FieldOffset(0x30)]
        public int padButton;
        [FieldOffset(0x34)]
        public int gsSpawnFrame;
        [FieldOffset(0x38)]
        public sbyte noAmmoTime;
        [FieldOffset(0x39)]
        public sbyte unEquipTimer;
        [FieldOffset(0x3a)]
        public sbyte detached;
        [FieldOffset(0x3b)]
        public sbyte unequipTime;
        [FieldOffset(0x3c)]
        public sbyte unEquipStatus;
        [FieldOffset(0x3d)]
        public sbyte unEquipDelay;
        [FieldOffset(0x40)]
        public int equippedTime;
        [FieldOffset(0x44)]
        public int state;
        [FieldOffset(0x48)]
        public int id;
        [FieldOffset(0x4c)]
        public float lightAng;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct PadStream
    {
        [FieldOffset(0x0 + (0x2 * 0))]
        public ushort button_bits_0;
        [FieldOffset(0x0 + (0x2 * 1))]
        public ushort button_bits_1;
        [FieldOffset(0x0 + (0x2 * 2))]
        public ushort button_bits_2;
        [FieldOffset(0x0 + (0x2 * 3))]
        public ushort button_bits_3;
        [FieldOffset(0x8 + (0x2 * 0))]
        public ushort analog_stick_0;
        [FieldOffset(0x8 + (0x2 * 1))]
        public ushort analog_stick_1;
        [FieldOffset(0x8 + (0x2 * 2))]
        public ushort analog_stick_2;
        [FieldOffset(0x8 + (0x2 * 3))]
        public ushort analog_stick_3;
        [FieldOffset(0x10 + (0x2 * 0))]
        public ushort right_analog_stick_0;
        [FieldOffset(0x10 + (0x2 * 1))]
        public ushort right_analog_stick_1;
        [FieldOffset(0x10 + (0x2 * 2))]
        public ushort right_analog_stick_2;
        [FieldOffset(0x10 + (0x2 * 3))]
        public ushort right_analog_stick_3;
        [FieldOffset(0x18)]
        public byte frames_with_btn_diffs;
        [FieldOffset(0x19 + (0x1 * 0))]
        public byte btn_bit_diffs_0;
        [FieldOffset(0x19 + (0x1 * 1))]
        public byte btn_bit_diffs_1;
        [FieldOffset(0x19 + (0x1 * 2))]
        public byte btn_bit_diffs_2;
        [FieldOffset(0x19 + (0x1 * 3))]
        public byte btn_bit_diffs_3;
        [FieldOffset(0x19 + (0x1 * 4))]
        public byte btn_bit_diffs_4;
        [FieldOffset(0x19 + (0x1 * 5))]
        public byte btn_bit_diffs_5;
        [FieldOffset(0x19 + (0x1 * 6))]
        public byte btn_bit_diffs_6;
        [FieldOffset(0x19 + (0x1 * 7))]
        public byte btn_bit_diffs_7;
        [FieldOffset(0x19 + (0x1 * 8))]
        public byte btn_bit_diffs_8;
        [FieldOffset(0x19 + (0x1 * 9))]
        public byte btn_bit_diffs_9;
        [FieldOffset(0x19 + (0x1 * 10))]
        public byte btn_bit_diffs_10;
        [FieldOffset(0x19 + (0x1 * 11))]
        public byte btn_bit_diffs_11;
        [FieldOffset(0x19 + (0x1 * 12))]
        public byte btn_bit_diffs_12;
        [FieldOffset(0x19 + (0x1 * 13))]
        public byte btn_bit_diffs_13;
        [FieldOffset(0x19 + (0x1 * 14))]
        public byte btn_bit_diffs_14;
        [FieldOffset(0x19 + (0x1 * 15))]
        public byte btn_bit_diffs_15;
        [FieldOffset(0x19 + (0x1 * 16))]
        public byte btn_bit_diffs_16;
        [FieldOffset(0x19 + (0x1 * 17))]
        public byte btn_bit_diffs_17;
        [FieldOffset(0x19 + (0x1 * 18))]
        public byte btn_bit_diffs_18;
        [FieldOffset(0x19 + (0x1 * 19))]
        public byte btn_bit_diffs_19;
        [FieldOffset(0x19 + (0x1 * 20))]
        public byte btn_bit_diffs_20;
        [FieldOffset(0x19 + (0x1 * 21))]
        public byte btn_bit_diffs_21;
        [FieldOffset(0x19 + (0x1 * 22))]
        public byte btn_bit_diffs_22;
        [FieldOffset(0x19 + (0x1 * 23))]
        public byte btn_bit_diffs_23;
        [FieldOffset(0x19 + (0x1 * 24))]
        public byte btn_bit_diffs_24;
        [FieldOffset(0x19 + (0x1 * 25))]
        public byte btn_bit_diffs_25;
        [FieldOffset(0x19 + (0x1 * 26))]
        public byte btn_bit_diffs_26;
        [FieldOffset(0x19 + (0x1 * 27))]
        public byte btn_bit_diffs_27;
        [FieldOffset(0x19 + (0x1 * 28))]
        public byte btn_bit_diffs_28;
        [FieldOffset(0x19 + (0x1 * 29))]
        public byte btn_bit_diffs_29;
        [FieldOffset(0x19 + (0x1 * 30))]
        public byte btn_bit_diffs_30;
        [FieldOffset(0x19 + (0x1 * 31))]
        public byte btn_bit_diffs_31;
        [FieldOffset(0x3c)]
        public int cur_btn_bit_offset;
        [FieldOffset(0x40)]
        public ushort prev_frame;
        [FieldOffset(0x42 + (0x8 * 0))]
        public ulong pad_stream_buf_0;
        [FieldOffset(0x42 + (0x8 * 1))]
        public ulong pad_stream_buf_1;
        [FieldOffset(0x42 + (0x8 * 2))]
        public ulong pad_stream_buf_2;
        [FieldOffset(0x42 + (0x8 * 3))]
        public ulong pad_stream_buf_3;
        [FieldOffset(0x42 + (0x8 * 4))]
        public ulong pad_stream_buf_4;
        [FieldOffset(0x42 + (0x8 * 5))]
        public ulong pad_stream_buf_5;
        [FieldOffset(0x42 + (0x8 * 6))]
        public ulong pad_stream_buf_6;
        [FieldOffset(0x42 + (0x8 * 7))]
        public ulong pad_stream_buf_7;
        [FieldOffset(0x42 + (0x8 * 8))]
        public ulong pad_stream_buf_8;
        [FieldOffset(0x42 + (0x8 * 9))]
        public ulong pad_stream_buf_9;
        [FieldOffset(0x42 + (0x8 * 10))]
        public ulong pad_stream_buf_10;
        [FieldOffset(0x42 + (0x8 * 11))]
        public ulong pad_stream_buf_11;
        [FieldOffset(0x42 + (0x8 * 12))]
        public ulong pad_stream_buf_12;
        [FieldOffset(0x42 + (0x8 * 13))]
        public ulong pad_stream_buf_13;
        [FieldOffset(0x42 + (0x8 * 14))]
        public ulong pad_stream_buf_14;
        [FieldOffset(0x42 + (0x8 * 15))]
        public ulong pad_stream_buf_15;
        [FieldOffset(0x42 + (0x8 * 16))]
        public ulong pad_stream_buf_16;
        [FieldOffset(0x42 + (0x8 * 17))]
        public ulong pad_stream_buf_17;
        [FieldOffset(0x42 + (0x8 * 18))]
        public ulong pad_stream_buf_18;
        [FieldOffset(0x42 + (0x8 * 19))]
        public ulong pad_stream_buf_19;
        [FieldOffset(0x42 + (0x8 * 20))]
        public ulong pad_stream_buf_20;
        [FieldOffset(0x42 + (0x8 * 21))]
        public ulong pad_stream_buf_21;
        [FieldOffset(0x42 + (0x8 * 22))]
        public ulong pad_stream_buf_22;
        [FieldOffset(0x42 + (0x8 * 23))]
        public ulong pad_stream_buf_23;
        [FieldOffset(0x42 + (0x8 * 24))]
        public ulong pad_stream_buf_24;
        [FieldOffset(0x42 + (0x8 * 25))]
        public ulong pad_stream_buf_25;
        [FieldOffset(0x42 + (0x8 * 26))]
        public ulong pad_stream_buf_26;
        [FieldOffset(0x42 + (0x8 * 27))]
        public ulong pad_stream_buf_27;
        [FieldOffset(0x42 + (0x8 * 28))]
        public ulong pad_stream_buf_28;
        [FieldOffset(0x42 + (0x8 * 29))]
        public ulong pad_stream_buf_29;
        [FieldOffset(0x42 + (0x8 * 30))]
        public ulong pad_stream_buf_30;
        [FieldOffset(0x42 + (0x8 * 31))]
        public ulong pad_stream_buf_31;
        [FieldOffset(0x42 + (0x8 * 32))]
        public ulong pad_stream_buf_32;
        [FieldOffset(0x42 + (0x8 * 33))]
        public ulong pad_stream_buf_33;
        [FieldOffset(0x42 + (0x8 * 34))]
        public ulong pad_stream_buf_34;
        [FieldOffset(0x42 + (0x8 * 35))]
        public ulong pad_stream_buf_35;
        [FieldOffset(0x42 + (0x8 * 36))]
        public ulong pad_stream_buf_36;
        [FieldOffset(0x42 + (0x8 * 37))]
        public ulong pad_stream_buf_37;
        [FieldOffset(0x42 + (0x8 * 38))]
        public ulong pad_stream_buf_38;
        [FieldOffset(0x42 + (0x8 * 39))]
        public ulong pad_stream_buf_39;
        [FieldOffset(0x42 + (0x8 * 40))]
        public ulong pad_stream_buf_40;
        [FieldOffset(0x42 + (0x8 * 41))]
        public ulong pad_stream_buf_41;
        [FieldOffset(0x42 + (0x8 * 42))]
        public ulong pad_stream_buf_42;
        [FieldOffset(0x42 + (0x8 * 43))]
        public ulong pad_stream_buf_43;
        [FieldOffset(0x42 + (0x8 * 44))]
        public ulong pad_stream_buf_44;
        [FieldOffset(0x42 + (0x8 * 45))]
        public ulong pad_stream_buf_45;
        [FieldOffset(0x42 + (0x8 * 46))]
        public ulong pad_stream_buf_46;
        [FieldOffset(0x42 + (0x8 * 47))]
        public ulong pad_stream_buf_47;
        [FieldOffset(0x42 + (0x8 * 48))]
        public ulong pad_stream_buf_48;
        [FieldOffset(0x42 + (0x8 * 49))]
        public ulong pad_stream_buf_49;
        [FieldOffset(0x1d4)]
        public int totalPadStreamBytes;
        [FieldOffset(0x1d8)]
        public int curFrame;
        [FieldOffset(0x1dc)]
        public int padStreamReady;
    };


    [StructLayout(LayoutKind.Explicit)]
    public struct HeroMove
    {
        [FieldOffset(0x0)]
        public Vector4 behavior;
        [FieldOffset(0x10)]
        public Vector4 external;
        [FieldOffset(0x20)]
        public Vector4 actual;
        [FieldOffset(0x30)]
        public Vector4 actualFromBehavior;
        [FieldOffset(0x40)]
        public Vector4 actualFromBehaviorGrav;
        [FieldOffset(0x50)]
        public Vector4 actualFromBehavior2D;
        [FieldOffset(0x60)]
        public Vector4 actualFromExternal;
        [FieldOffset(0x70)]
        public Vector4 taper;
        [FieldOffset(0x80)]
        public float speed;
        [FieldOffset(0x84)]
        public float speed2D;
        [FieldOffset(0x88)]
        public float forwardSpeed;
        [FieldOffset(0x8c)]
        public float ascent;
        [FieldOffset(0x90)]
        public float zSpeed;
        [FieldOffset(0x94)]
        public float externalSpeed;
        [FieldOffset(0x98 + (0x4 * 0))]
        public int pad_0;
        [FieldOffset(0x98 + (0x4 * 1))]
        public int pad_1;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroColl
    {
        [FieldOffset(0x0)]
        public Vector4 normal;
        [FieldOffset(0x10)]
        public Vector4 ip;
        [FieldOffset(0x20)]
        public float top;
        [FieldOffset(0x24)]
        public float bot;
        [FieldOffset(0x28)]
        public float ideal_top;
        [FieldOffset(0x2c)]
        public float ideal_bot;
        [FieldOffset(0x30)]
        public float idealRadius;
        [FieldOffset(0x34)]
        public float radius;
        [FieldOffset(0x38)]
        public float radiusSpeed;
        [FieldOffset(0x3c)]
        public RCPointer pContactMoby;
        [FieldOffset(0x40)]
        public RCPointer pBumpMoby;
        [FieldOffset(0x44)]
        public float bumpPushSpeed;
        [FieldOffset(0x48)]
        public float distToWall;
        [FieldOffset(0x4c)]
        public float wallAng;
        [FieldOffset(0x50)]
        public float wallSlope;
        [FieldOffset(0x54)]
        public sbyte wallIsCrate;
        [FieldOffset(0x55)]
        public sbyte wallIsMoby;
        [FieldOffset(0x56)]
        public sbyte contact;
        [FieldOffset(0x57)]
        public sbyte cpad;
        [FieldOffset(0x58)]
        public float ledgeHeight;
        [FieldOffset(0x5c)]
        public float ledgeDist;
        [FieldOffset(0x60)]
        public int atLedge;
        [FieldOffset(0x64)]
        public RCPointer pWallJumpMoby;
        [FieldOffset(0x68 + (0x4 * 0))]
        public int pad_0;
        [FieldOffset(0x68 + (0x4 * 1))]
        public int pad_1;
        [FieldOffset(0x70)]
        public float radiusSqd;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroGround
    {
        [FieldOffset(0x0)]
        public Vector4 normal;
        [FieldOffset(0x10)]
        public Vector4 waterNormal;
        [FieldOffset(0x20)]
        public Vector4 gravity;
        [FieldOffset(0x30)]
        public Vector4 point;
        [FieldOffset(0x40)]
        public Vector4 lastGoodPos;
        [FieldOffset(0x50)]
        public Vector4 externalBootGrav;
        [FieldOffset(0x60 + (0x4 * 0))]
        public float feetHeights_0;
        [FieldOffset(0x60 + (0x4 * 1))]
        public float feetHeights_1;
        [FieldOffset(0x68 + (0x4 * 0))]
        public float pitchSlopes_0;
        [FieldOffset(0x68 + (0x4 * 1))]
        public float pitchSlopes_1;
        [FieldOffset(0x70 + (0x4 * 0))]
        public float rollSlopes_0;
        [FieldOffset(0x70 + (0x4 * 1))]
        public float rollSlopes_1;
        [FieldOffset(0x78)]
        public float height;
        [FieldOffset(0x7c)]
        public float dist;
        [FieldOffset(0x80)]
        public float slope;
        [FieldOffset(0x84)]
        public float pitchSlope;
        [FieldOffset(0x88)]
        public float rollSlope;
        [FieldOffset(0x8c)]
        public float angz;
        [FieldOffset(0x90)]
        public float waterHeight;
        [FieldOffset(0x94)]
        public float quicksandHeight;
        [FieldOffset(0x98)]
        public int underWater;
        [FieldOffset(0x9c)]
        public RCPointer pMoby;
        [FieldOffset(0xa0)]
        public int onGood;
        [FieldOffset(0xa4)]
        public float speed;
        [FieldOffset(0xa8)]
        public short magnetic;
        [FieldOffset(0xaa)]
        public short stickLanding;
        [FieldOffset(0xac)]
        public short offAny;
        [FieldOffset(0xae)]
        public short offGood;
        [FieldOffset(0xb0)]
        public int oscillating;
        [FieldOffset(0xb4)]
        public float oscPos1;
        [FieldOffset(0xb8)]
        public float oscPos2;
        [FieldOffset(0xBC + (0x4 * 0))]
        public int pad_0;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroTrack
    {
        [FieldOffset(0x0)]
        public Vector4 idealPos;
        [FieldOffset(0x10)]
        public Vector4 idealRot;
        [FieldOffset(0x20)]
        public Vector4 idealWallPos;
        [FieldOffset(0x30)]
        public Vector4 idealWallRot;
        [FieldOffset(0x40)]
        public Vector4 prevVel;
        [FieldOffset(0x50)]
        public RCPointer pMoby;
        [FieldOffset(0x54)]
        public int flags;
        [FieldOffset(0x58 + (0x4 * 0))]
        public int pad_0;
        [FieldOffset(0x58 + (0x4 * 1))]
        public int pad_1;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroAnim
    {
        [FieldOffset(0x0)]
        public float speed;
        [FieldOffset(0x4)]
        public int iscale;
        [FieldOffset(0x8)]
        public int flags;
        [FieldOffset(0xc)]
        public int interping;
        [FieldOffset(0x10)]
        public int env_index;
        [FieldOffset(0x14)]
        public int env_time;
        [FieldOffset(0x18)]
        public float mayaFrm;
        [FieldOffset(0x1c)]
        public float mayaFrmDelt;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroJoints
    {
        [FieldOffset(0x0 + (0x40 * 0))]
        public Matrix4x4 gadgetMtxs_0;
        [FieldOffset(0x0 + (0x40 * 1))]
        public Matrix4x4 gadgetMtxs_1;
        [FieldOffset(0x0 + (0x40 * 2))]
        public Matrix4x4 gadgetMtxs_2;
        [FieldOffset(0x0 + (0x40 * 3))]
        public Matrix4x4 gadgetMtxs_3;
        [FieldOffset(0x0 + (0x40 * 4))]
        public Matrix4x4 gadgetMtxs_4;
        [FieldOffset(0x0 + (0x40 * 5))]
        public Matrix4x4 gadgetMtxs_5;

        public Matrix4x4? GetGadgetMtx(int i)
        {
            switch (i)
            {
                case 0: return gadgetMtxs_0;
                case 1: return gadgetMtxs_1;
                case 2: return gadgetMtxs_2;
                case 3: return gadgetMtxs_3;
                case 4: return gadgetMtxs_4;
                case 5: return gadgetMtxs_5;
                default: return null;
            }
        }
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroAnimLayers
    {
        [FieldOffset(0x0 + (0x4 * 0))]
        public RCPointer pArmBlenders_0;
        [FieldOffset(0x0 + (0x4 * 1))]
        public RCPointer pArmBlenders_1;
        [FieldOffset(0x8 + (0x4 * 0))]
        public RCPointer pFiringBlenders_0;
        [FieldOffset(0x8 + (0x4 * 1))]
        public RCPointer pFiringBlenders_1;
        [FieldOffset(0x10)]
        public RCPointer pHeadBlender;
        [FieldOffset(0x14)]
        public int detachingFiring;
        [FieldOffset(0x18)]
        public int armBlenderSharesIdle;
        [FieldOffset(0x1c)]
        public int gadgetAttachedId;
        [FieldOffset(0x20 + (0x4 * 0))]
        public int usingRunAnim_0;
        [FieldOffset(0x20 + (0x4 * 1))]
        public int usingRunAnim_1;
        [FieldOffset(0x20 + (0x4 * 2))]
        public int usingRunAnim_2;
        [FieldOffset(0x20 + (0x4 * 3))]
        public int usingRunAnim_3;
        [FieldOffset(0x30)]
        public int headAnim;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroTweaker
    {
        [FieldOffset(0x0)]
        public Manipulator manip;
        [FieldOffset(0x40)]
        public Vector4 rot;
        [FieldOffset(0x50)]
        public Vector4 speed;
        [FieldOffset(0x60)]
        public Vector4 target;
        [FieldOffset(0x70)]
        public Vector4 trans;
        [FieldOffset(0x80)]
        public Vector4 transSpeed;
        [FieldOffset(0x90)]
        public Vector4 transTarget;
        [FieldOffset(0xa0)]
        public short joint;
        [FieldOffset(0xa2)]
        public short whichMoby;
        [FieldOffset(0xa4)]
        public float gain;
        [FieldOffset(0xa8)]
        public float damp;
        [FieldOffset(0xac)]
        public float scale;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct FpsCamVars
    {
        [FieldOffset(0x0)]
        public Matrix3x3 mtx;
        [FieldOffset(0x30)]
        public Vector4 pos;
        [FieldOffset(0x40)]
        public float z_rot;
        [FieldOffset(0x44)]
        public float z_speed_current;
        [FieldOffset(0x48)]
        public float z_speed_max_quick;
        [FieldOffset(0x4c)]
        public float z_speed_max_aim;
        [FieldOffset(0x50)]
        public float z_accel_quick;
        [FieldOffset(0x54)]
        public float z_accel_aim;
        [FieldOffset(0x58)]
        public float z_target_slowness_factor_quick;
        [FieldOffset(0x5c)]
        public float z_target_slowness_factor_aim;
        [FieldOffset(0x60)]
        public float y_rot;
        [FieldOffset(0x64)]
        public float y_speed_current;
        [FieldOffset(0x68)]
        public float y_speed_max;
        [FieldOffset(0x6c)]
        public float y_accel;
        [FieldOffset(0x70)]
        public float y_target_slowness_factor;
        [FieldOffset(0x74)]
        public float strafe_turn_factor;
        [FieldOffset(0x78)]
        public float strafe_tilt_factor;
        [FieldOffset(0x7c)]
        public float max_target_angle;
        [FieldOffset(0x80)]
        public int state;
        [FieldOffset(0x84)]
        public short quick_turn_input_time;
        [FieldOffset(0x86)]
        public short snap_hold_time;
        [FieldOffset(0x88)]
        public float snap_input;
        [FieldOffset(0x8c)]
        public RCPointer pTarget;
        [FieldOffset(0x90)]
        public Vector4 target_last_pos;
        [FieldOffset(0xa0)]
        public Vector4 target_vel;
        [FieldOffset(0xb0)]
        public float target_blend_fac;
        [FieldOffset(0xb4)]
        public float max_y_rot;
        [FieldOffset(0xb8)]
        public float min_y_rot;
        [FieldOffset(0xbc)]
        public RCPointer pExcludeMoby;
        [FieldOffset(0xc0)]
        public Vector4 ext_pos_ofs;
        [FieldOffset(0xd0)]
        public Vector4 ext_rot_ofs;
        [FieldOffset(0xe0)]
        public short flags;
        [FieldOffset(0xe2)]
        public short allegiance;
        [FieldOffset(0xe4)]
        public RCPointer special_target_func;
        [FieldOffset(0xe8)]
        public RCPointer pWorldMtx;
        [FieldOffset(0xec)]
        public RCPointer pWorldInvMtx;
        [FieldOffset(0xf0)]
        public Vector4 facing_dir;
        [FieldOffset(0x100)]
        public Vector4 internal_facing_dir;
        [FieldOffset(0x110)]
        public Vector4 aim_pos;
        [FieldOffset(0x120)]
        public float range;
        [FieldOffset(0x124)]
        public float ext_extension;
        [FieldOffset(0x128)]
        public float ext_entension_speed;
        [FieldOffset(0x12c)]
        public int cam_slot;
        [FieldOffset(0x130)]
        public RCPointer pHero;
        [FieldOffset(0x134)]
        public float camRadius;
        [FieldOffset(0x138)]
        public int camSettingsIndex;
        [FieldOffset(0x13c)]
        public int karma_pad;
        [FieldOffset(0x140)]
        public Vector4 prevCamPos;
        [FieldOffset(0x150 + (0x4 * 0))]
        public int karma_pad2_0;
        [FieldOffset(0x150 + (0x4 * 1))]
        public int karma_pad2_1;
        [FieldOffset(0x150 + (0x4 * 2))]
        public int karma_pad2_2;
        [FieldOffset(0x150 + (0x4 * 3))]
        public int karma_pad2_3;
    };


    [StructLayout(LayoutKind.Explicit)]
    public struct HeroWeaponPosRec
    {
        [FieldOffset(0x0 + (0x40 * 0))]
        public Matrix4x4 fpGunMtx_0;
        [FieldOffset(0x0 + (0x40 * 1))]
        public Matrix4x4 fpGunMtx_1;
        [FieldOffset(0x80 + (0x40 * 0))]
        public Matrix4x4 tpGunMtx_0;
        [FieldOffset(0x80 + (0x40 * 1))]
        public Matrix4x4 tpGunMtx_1;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroWalkToPos
    {
        [FieldOffset(0x0)]
        public Vector4 idealPos;
        [FieldOffset(0x10)]
        public Vector4 idealRot;
        [FieldOffset(0x20)]
        public int abortOnArrival;
        [FieldOffset(0x24)]
        public int walkToTeleport;
        [FieldOffset(0x28)]
        public int teleportWaitTime;
        [FieldOffset(0x2c)]
        public RCPointer pTeleTarget;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroFps
    {
        [FieldOffset(0x0)]
        public Matrix3x3 worldMtx;
        [FieldOffset(0x30)]
        public Matrix3x3 worldInvMtx;
        [FieldOffset(0x60)]
        public Quaternion gunFPSQuat;
        [FieldOffset(0x70)]
        public Vector4 camEffVel;
        [FieldOffset(0x80)]
        public Vector4 camPos;
        [FieldOffset(0x90)]
        public Vector4 camDir;
        [FieldOffset(0xa0)]
        public FpsCamVars fVars;
        [FieldOffset(0x200)]
        public float camYSpeed;
        [FieldOffset(0x204)]
        public float camZSpeed;
        [FieldOffset(0x208)]
        public float gunInterp;
        [FieldOffset(0x20c)]
        public float gunInterpSpeed;
        [FieldOffset(0x210)]
        public int active;
        [FieldOffset(0x214)]
        public float gunWithdrawSpeed;
        [FieldOffset(0x218)]
        public float gunWithdrawDist;
        [FieldOffset(0x21c)]
        public float gunWithdrawIdeal;
        [FieldOffset(0x220)]
        public float bobRot;
        [FieldOffset(0x224)]
        public float bobRotSpeed;
        [FieldOffset(0x228)]
        public float bobAmp;
        [FieldOffset(0x22c)]
        public float camHeroOfs;
        [FieldOffset(0x230)]
        public float camHeroOfsSpeed;
        [FieldOffset(0x234)]
        public int ignoreGroundHeight;
        [FieldOffset(0x238)]
        public float reticulePulseAng;
        [FieldOffset(0x23c)]
        public int reticuleFadeInTimer;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroSurf
    {
        [FieldOffset(0x0)]
        public float sinkDepth;
        [FieldOffset(0x4)]
        public float sinkRate;
        [FieldOffset(0x8)]
        public int minSwimTimer;
        [FieldOffset(0xc)]
        public float surfHeight;
        [FieldOffset(0x10)]
        public float surfHeightSpeed;
        [FieldOffset(0x14)]
        public RCPointer pIceCube;
        [FieldOffset(0x18)]
        public float bobt;
        [FieldOffset(0x1c)]
        public float bobz;
        [FieldOffset(0x20)]
        public float swingAng1;
        [FieldOffset(0x24)]
        public float swingAng2;
        [FieldOffset(0x28 + (0x4 * 0))]
        public int pad_0;
        [FieldOffset(0x28 + (0x4 * 1))]
        public int pad_1;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroSwim
    {
        [FieldOffset(0x0)]
        public Vector4 padWindUp;
        [FieldOffset(0x10)]
        public int diveTime;
        [FieldOffset(0x14)]
        public float bankSpeed;
        [FieldOffset(0x18)]
        public float pitchSpeed;
        [FieldOffset(0x1c)]
        public float yawSpeed;
        [FieldOffset(0x20)]
        public int bubbleTimer;
        [FieldOffset(0x24)]
        public int minSwimTimer;
        [FieldOffset(0x28)]
        public short sbytegeBubbleTimer;
        [FieldOffset(0x2a)]
        public short soundTimer;
        [FieldOffset(0x2c)]
        public float buoyancySpeed;
        [FieldOffset(0x30)]
        public short padUpTime;
        [FieldOffset(0x32)]
        public short padDownTime;
        [FieldOffset(0x34)]
        public short ring_timer;
        [FieldOffset(0x36)]
        public short drop_timer;
        [FieldOffset(0x38)]
        public short wake_timer;
        [FieldOffset(0x3a)]
        public short plunge_bubbles;
        [FieldOffset(0x3c)]
        public short solidRiseTimer;
        [FieldOffset(0x3e)]
        public short riseTimer;
        [FieldOffset(0x40)]
        public int riseTapCnt;
        [FieldOffset(0x44)]
        public int timeRiseLastPressed;
        [FieldOffset(0x48 + (0x4 * 0))]
        public int pad_0;
        [FieldOffset(0x48 + (0x4 * 1))]
        public int pad_1;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroGrind
    {
        [FieldOffset(0x0)]
        public Vector4 closestPoint;
        [FieldOffset(0x10)]
        public Vector4 rootPos;
        [FieldOffset(0x20)]
        public Vector4 lHandPos;
        [FieldOffset(0x30)]
        public Vector4 rHandPos;
        [FieldOffset(0x40)]
        public Vector4 closestPointSwitch;
        [FieldOffset(0x50)]
        public Vector4 rampTarget;
        [FieldOffset(0x60)]
        public RCPointer pPath;
        [FieldOffset(0x64)]
        public int curNode;
        [FieldOffset(0x68)]
        public float curSegLen;
        [FieldOffset(0x6c)]
        public int valid;
        [FieldOffset(0x70)]
        public int EOPtime;
        [FieldOffset(0x74)]
        public float speed;
        [FieldOffset(0x78)]
        public int pathDir;
        [FieldOffset(0x7c)]
        public float ang;
        [FieldOffset(0x80)]
        public float slope;
        [FieldOffset(0x84)]
        public int wrap;
        [FieldOffset(0x88)]
        public float heroAngOfs;
        [FieldOffset(0x8c)]
        public int animDir;
        [FieldOffset(0x90)]
        public int switching;
        [FieldOffset(0x94)]
        public RCPointer pSwitchPath;
        [FieldOffset(0x98)]
        public int switchCurNode;
        [FieldOffset(0x9c)]
        public float switchCurSegLen;
        [FieldOffset(0xa0)]
        public int switchWrap;
        [FieldOffset(0xa4)]
        public float slopeSpeed;
        [FieldOffset(0xa8)]
        public int switchTime;
        [FieldOffset(0xac)]
        public int leanDir;
        [FieldOffset(0xb0)]
        public float userSpeed;
        [FieldOffset(0xb4)]
        public int grappleTrigger;
        [FieldOffset(0xb8)]
        public int onRamp;
        [FieldOffset(0xbc)]
        public short rampTrigger;
        [FieldOffset(0xbe)]
        public short damage;
        [FieldOffset(0xc0)]
        public int rampJumpQueued;
        [FieldOffset(0xc4)]
        public int useRampJumpTarget;
        [FieldOffset(0xc8)]
        public float rampJumpGravity;
        [FieldOffset(0xcc)]
        public float getHitSpeed;
        [FieldOffset(0xd0)]
        public float aimDirOfs;
        [FieldOffset(0xd4)]
        public float aimDirSpeed;
        [FieldOffset(0xd8)]
        public RCPointer pTarget;
        [FieldOffset(0xdc)]
        public float centeringSpeed;
        [FieldOffset(0xe0)]
        public float twirlRot;
        [FieldOffset(0xe4)]
        public float twirlRotSpeed;
        [FieldOffset(0xe8)]
        public float twirlCamRotz;
        [FieldOffset(0xec)]
        public short twirlTimer;
        [FieldOffset(0xee)]
        public short twirlCamOn;
        [FieldOffset(0xf0)]
        public float twirlCamRotZspeed;
        [FieldOffset(0xf4)]
        public RCPointer cdo;
        [FieldOffset(0xf8)]
        public int noGroundTime;
        [FieldOffset(0xfc)]
        public short turnDir;
        [FieldOffset(0xfe)]
        public short pad;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroShadow
    {
        [FieldOffset(0x0)]
        public float slope;
        [FieldOffset(0x4)]
        public float plane;
        [FieldOffset(0x8)]
        public float range;
        [FieldOffset(0xc)]
        public int sample_id;
        [FieldOffset(0x10 + (0x4 * 0))]
        public int pad_0;
        [FieldOffset(0x10 + (0x4 * 1))]
        public int pad_1;
        [FieldOffset(0x18 + (0x4 * 0))]
        public float sample_pos_0;
        [FieldOffset(0x18 + (0x4 * 1))]
        public float sample_pos_1;
        [FieldOffset(0x18 + (0x4 * 2))]
        public float sample_pos_2;
        [FieldOffset(0x18 + (0x4 * 3))]
        public float sample_pos_3;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct Manipulator
    {
        [FieldOffset(0x0)]
        public sbyte animJoint;
        [FieldOffset(0x1)]
        public sbyte state;
        [FieldOffset(0x2)]
        public sbyte scaleOn;
        [FieldOffset(0x3)]
        public sbyte absolute;
        [FieldOffset(0x4)]
        public int jointId;
        [FieldOffset(0x8)]
        public RCPointer pChain;
        [FieldOffset(0xc)]
        public float interp;
        [FieldOffset(0x10)]
        public Quaternion q;
        [FieldOffset(0x20)]
        public Vector4 scale;
        [FieldOffset(0x30)]
        public Vector4 trans;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroEyes
    {
        [FieldOffset(0x0 + (0x40 * 0))]
        public Manipulator manips_0;
        [FieldOffset(0x0 + (0x40 * 1))]
        public Manipulator manips_1;
        [FieldOffset(0x0 + (0x40 * 2))]
        public Manipulator manips_2;
        [FieldOffset(0x0 + (0x40 * 3))]
        public Manipulator manips_3;
        [FieldOffset(0x0 + (0x40 * 4))]
        public Manipulator manips_4;
        [FieldOffset(0x0 + (0x40 * 5))]
        public Manipulator manips_5;
        [FieldOffset(0x0 + (0x40 * 6))]
        public Manipulator manips_6;
        [FieldOffset(0x1C0 + (0x40 * 0))]
        public Manipulator clankpackManips_0;
        [FieldOffset(0x1C0 + (0x40 * 1))]
        public Manipulator clankpackManips_1;
        [FieldOffset(0x1C0 + (0x40 * 2))]
        public Manipulator clankpackManips_2;
        [FieldOffset(0x1C0 + (0x40 * 3))]
        public Manipulator clankpackManips_3;
        [FieldOffset(0x2c0)]
        public int blink_timer;
        [FieldOffset(0x2c4)]
        public int blink_next;
        [FieldOffset(0x2c8)]
        public int blink_frequency;
        [FieldOffset(0x2cc)]
        public short clankBlinkCountdown;
        [FieldOffset(0x2ce)]
        public short clankBlinkTime;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroNPJumpThrustStage
    {
        [FieldOffset(0x0)]
        public float initThrust;
        [FieldOffset(0x4)]
        public float thrustDelta;
        [FieldOffset(0x8)]
        public int time;
        [FieldOffset(0xc)]
        public int pad;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroJumpNonParab
    {
        [FieldOffset(0x0)]
        public int startThrustTime;
        [FieldOffset(0x4)]
        public int endThrustTime;
        [FieldOffset(0x8)]
        public float thrustStartFrm;
        [FieldOffset(0xc)]
        public float peakFrm;
        [FieldOffset(0x10)]
        public int timeToPeakFrm;
        [FieldOffset(0x14)]
        public RCPointer thrustTable;
        [FieldOffset(0x18)]
        public int thrustStageIndex;
        [FieldOffset(0x1c)]
        public int thrustStageTimer;
        [FieldOffset(0x20)]
        public float thrust;
        [FieldOffset(0x24)]
        public float sbytegeJumpThrust;
        [FieldOffset(0x28)]
        public float descendGravity;
        [FieldOffset(0x2c)]
        public int pad;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroJump
    {
        [FieldOffset(0x0)]
        public HeroJumpNonParab nonParab;
        [FieldOffset(0x30)]
        public Vector4 takeoffPos;
        [FieldOffset(0x40)]
        public Vector4 snapJumpThrustVec;
        [FieldOffset(0x50)]
        public Vector4 snapJumpForwardVec;
        [FieldOffset(0x60)]
        public float camHeight;
        [FieldOffset(0x64)]
        public float turnSpeed;
        [FieldOffset(0x68)]
        public float wallJumpXySpeed;
        [FieldOffset(0x6c)]
        public sbyte land_timer;
        [FieldOffset(0x6d)]
        public sbyte useNonParabAscent;
        [FieldOffset(0x6e)]
        public sbyte descend;
        [FieldOffset(0x6f)]
        public sbyte noGlide;
        [FieldOffset(0x70)]
        public float maxFallSpeed;
        [FieldOffset(0x74)]
        public float maxXySpeed;
        [FieldOffset(0x78)]
        public float ideal_height;
        [FieldOffset(0x7c)]
        public short pushOffTime;
        [FieldOffset(0x7e)]
        public short framesToLand;
        [FieldOffset(0x80)]
        public float up_thrust;
        [FieldOffset(0x84)]
        public float up_thrust_total;
        [FieldOffset(0x88)]
        public float coll_bot;
        [FieldOffset(0x8c)]
        public float ledgeJumpSpeed;
        [FieldOffset(0x90)]
        public float snapJumpThrustAng;
        [FieldOffset(0x94)]
        public float snapJumpForwardAng;
        [FieldOffset(0x98)]
        public float peakFrm;
        [FieldOffset(0x9c)]
        public float landFrm;
        [FieldOffset(0xa0)]
        public float gameLandFrm;
        [FieldOffset(0xa4)]
        public int snapJumpDir;
        [FieldOffset(0xa8)]
        public float snapJumpRunThrust;
        [FieldOffset(0xac)]
        public float snapJumpSpeed;
        [FieldOffset(0xb0)]
        public Vector4 wallJumpDir;
        [FieldOffset(0xc0)]
        public Vector4 wallJumpDirCur;
        [FieldOffset(0xd0)]
        public float accel;
        [FieldOffset(0xd4)]
        public float decel;
        [FieldOffset(0xd8)]
        public float minHeight;
        [FieldOffset(0xdc)]
        public float maxHeight;
        [FieldOffset(0xe0)]
        public float fallThresh;
        [FieldOffset(0xe4)]
        public float animMayaScale;
        [FieldOffset(0xe8)]
        public short maxUpTime;
        [FieldOffset(0xea)]
        public short bailoutSafetyTime;
        [FieldOffset(0xec)]
        public short particleTimer;
        [FieldOffset(0xee)]
        public short particleTimer2;
        [FieldOffset(0xf0)]
        public float gravity;
        [FieldOffset(0xf4)]
        public short minForwardThrust;
        [FieldOffset(0xf6)]
        public short minTimeToGlide;
        [FieldOffset(0xf8)]
        public short onIce;
        [FieldOffset(0xfa)]
        public short minTimeToFall;
        [FieldOffset(0xfc)]
        public sbyte gloveAttackOk;
        [FieldOffset(0xfd)]
        public sbyte strafingFlip;
        [FieldOffset(0xfe)]
        public sbyte flipJumpOk;
        [FieldOffset(0xff)]
        public sbyte doubleJumpOk;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroLockOn
    {
        [FieldOffset(0x0)]
        public Vector4 strafeIdealVec;
        [FieldOffset(0x10)]
        public RCPointer pMoby;
        [FieldOffset(0x14)]
        public short strafing;
        [FieldOffset(0x16)]
        public sbyte strafingBack;
        [FieldOffset(0x17)]
        public sbyte strafingDir;
        [FieldOffset(0x18)]
        public float strafeRot;
        [FieldOffset(0x1c)]
        public float strafeRotSpeed;
        [FieldOffset(0x20)]
        public float backSpeed;
        [FieldOffset(0x24)]
        public int strafeRotTimer;
        [FieldOffset(0x28)]
        public short strafeLeftDampTimer;
        [FieldOffset(0x2a)]
        public short strafeRightDampTimer;
        [FieldOffset(0x2c)]
        public int strafeTurnDiffZero;
        [FieldOffset(0x30)]
        public Vector4 curRetPos;
        [FieldOffset(0x40)]
        public RCPointer curRetMoby;
        [FieldOffset(0x44)]
        public float fadeInterp;
        [FieldOffset(0x48)]
        public float retRot;
        [FieldOffset(0x4c)]
        public float strafeTurnDiff;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroFireDir
    {
        [FieldOffset(0x0)]
        public Matrix3x3 m;
        [FieldOffset(0x30)]
        public Vector4 v;
        [FieldOffset(0x40)]
        public Vector4 rot;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroZip
    {
        [FieldOffset(0x0)]
        public Vector4 closestPoint;
        [FieldOffset(0x10)]
        public Vector4 idealVec;
        [FieldOffset(0x20)]
        public RCPointer pPath;
        [FieldOffset(0x24)]
        public int curNode;
        [FieldOffset(0x28)]
        public float curSegLen;
        [FieldOffset(0x2c)]
        public int valid;
        [FieldOffset(0x30)]
        public int EOPtime;
        [FieldOffset(0x34)]
        public float speed;
        [FieldOffset(0x38)]
        public int pathDir;
        [FieldOffset(0x3c)]
        public int wrap;
        [FieldOffset(0x40)]
        public float ang;
        [FieldOffset(0x44)]
        public float slope;
        [FieldOffset(0x48)]
        public float centeringSpeed;
        [FieldOffset(0x4C + (0x4 * 0))]
        public int pad_0;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroThrust
    {
        [FieldOffset(0x0)]
        public float ideal;
        [FieldOffset(0x4)]
        public float actual;
        [FieldOffset(0x8 + (0x4 * 0))]
        public int pad_0;
        [FieldOffset(0x8 + (0x4 * 1))]
        public int pad_1;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroTurn
    {
        [FieldOffset(0x0)]
        public Vector4 idealVec;
        [FieldOffset(0x10)]
        public float ideal;
        [FieldOffset(0x14)]
        public float speed;
        [FieldOffset(0x18)]
        public float diff;
        [FieldOffset(0x1c)]
        public int pad;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroWalk
    {
        [FieldOffset(0x0)]
        public Vector4 idealTurnVec;
        [FieldOffset(0x10)]
        public float iceMotionDotProduct;
        [FieldOffset(0x14)]
        public float lastGroundSlope;
        [FieldOffset(0x18)]
        public float idealTurnAng;
        [FieldOffset(0x1c)]
        public sbyte ideal_motion;
        [FieldOffset(0x1d)]
        public sbyte skateThrust;
        [FieldOffset(0x1e)]
        public sbyte long_trans;
        [FieldOffset(0x1f)]
        public sbyte qturning;
        [FieldOffset(0x20)]
        public int idealAngSet;
        [FieldOffset(0x24 + (0x4 * 0))]
        public int pad_0;
        [FieldOffset(0x24 + (0x4 * 1))]
        public int pad_1;
        [FieldOffset(0x24 + (0x4 * 2))]
        public int pad_2;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroAttack
    {
        [FieldOffset(0x0)]
        public Vector4 near;
        [FieldOffset(0x10)]
        public Vector4 far;
        [FieldOffset(0x20)]
        public Vector4 oldNear;
        [FieldOffset(0x30)]
        public Vector4 oldFar;
        [FieldOffset(0x40)]
        public Vector4 wrenchHandle;
        [FieldOffset(0x50)]
        public Vector4 wrenchTip;
        [FieldOffset(0x60)]
        public Vector4 idealVec;
        [FieldOffset(0x70)]
        public Vector4 bounceVec;
        [FieldOffset(0x80)]
        public RCPointer pTarget;
        [FieldOffset(0x84)]
        public RCPointer pMoby;
        [FieldOffset(0x88)]
        public int rotSet;
        [FieldOffset(0x8c)]
        public float rot;
        [FieldOffset(0x90)]
        public float bounceAng;
        [FieldOffset(0x94)]
        public float speedFactor;
        [FieldOffset(0x98)]
        public RCPointer pGunPointMoby;
        [FieldOffset(0x9c)]
        public short id;
        [FieldOffset(0x9e)]
        public short soundPlayed;
        [FieldOffset(0xa0)]
        public float descend;
        [FieldOffset(0xa4)]
        public float aimAngz;
        [FieldOffset(0xa8)]
        public float aimAngy;
        [FieldOffset(0xac)]
        public int throwAttackDamageID;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroAttackDef
    {
        [FieldOffset(0x0)]
        public int type;
        [FieldOffset(0x4)]
        public int stage;
        [FieldOffset(0x8)]
        public int endComboFrm;
        [FieldOffset(0xc)]
        public int inputFrm;
        [FieldOffset(0x10)]
        public int transFrm;
        [FieldOffset(0x14)]
        public int jumpTransFrm;
        [FieldOffset(0x18)]
        public int etcTransFrm;
        [FieldOffset(0x1c)]
        public int startDamFrm;
        [FieldOffset(0x20)]
        public int stopDamFrm;
        [FieldOffset(0x24)]
        public int startBlurFrm;
        [FieldOffset(0x28)]
        public int stopBlurFrm;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroMobys
    {
        [FieldOffset(0x0)]
        public RCPointer ground;
        [FieldOffset(0x4)]
        public RCPointer hero;
        [FieldOffset(0x8 + (0x4 * 0))]
        public int pad_0;
        [FieldOffset(0x8 + (0x4 * 1))]
        public int pad_1;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroCharge
    {
        [FieldOffset(0x0)]
        public Vector4 padWindUp;
        [FieldOffset(0x10)]
        public float groundSpeed;
        [FieldOffset(0x14)]
        public int hitEdge;
        [FieldOffset(0x18 + (0x4 * 0))]
        public int pad_0;
        [FieldOffset(0x18 + (0x4 * 1))]
        public int pad_1;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroHotspots
    {
        [FieldOffset(0x0)]
        public short index;
        [FieldOffset(0x2)]
        public sbyte ice;
        [FieldOffset(0x3)]
        public sbyte magictele;
        [FieldOffset(0x4)]
        public sbyte water;
        [FieldOffset(0x5)]
        public sbyte lava;
        [FieldOffset(0x6)]
        public sbyte quicksand;
        [FieldOffset(0x7)]
        public sbyte magnetic;
        [FieldOffset(0x8)]
        public sbyte noStand;
        [FieldOffset(0x9)]
        public sbyte deathsand;
        [FieldOffset(0xa)]
        public sbyte icewater;
        [FieldOffset(0xb)]
        public sbyte groundType;
        [FieldOffset(0xc)]
        public int pad;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroWind
    {
        [FieldOffset(0x0)]
        public Vector4 vel;
        [FieldOffset(0x10)]
        public float speed;
        [FieldOffset(0x14)]
        public float angy;
        [FieldOffset(0x18)]
        public float angz;
        [FieldOffset(0x1C + (0x4 * 0))]
        public int pad_0;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroDust
    {
        [FieldOffset(0x0)]
        public float vel;
        [FieldOffset(0x4)]
        public float velvar;
        [FieldOffset(0x8)]
        public int timer;
        [FieldOffset(0xc)]
        public short rate;
        [FieldOffset(0xe)]
        public short flags;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroFall
    {
        [FieldOffset(0x0)]
        public float gravity;
        [FieldOffset(0x4)]
        public float xyDecel;
        [FieldOffset(0x8)]
        public float xRotSpeed;
        [FieldOffset(0xc)]
        public float yRotSpeed;
        [FieldOffset(0x10)]
        public float xRotSpeedIdeal;
        [FieldOffset(0x14)]
        public float yRotSpeedIdeal;
        [FieldOffset(0x18)]
        public float glideTaperSpeed;
        [FieldOffset(0x1C + (0x4 * 0))]
        public int pad_0;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroLedge
    {
        [FieldOffset(0x0)]
        public Vector4 idealWallPos;
        [FieldOffset(0x10)]
        public Vector4 idealGrabPos;
        [FieldOffset(0x20)]
        public float groundHeight;
        [FieldOffset(0x24)]
        public float wallAngZ;
        [FieldOffset(0x28)]
        public int valid;
        [FieldOffset(0x2c)]
        public float gravity;
        [FieldOffset(0x30)]
        public float camHeight;
        [FieldOffset(0x34)]
        public int flags;
        [FieldOffset(0x38)]
        public RCPointer pMoby;
        [FieldOffset(0x3c)]
        public int pad;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroTractorBeam
    {
        [FieldOffset(0x0)]
        public RCPointer pTarget;
        [FieldOffset(0x4)]
        public float zRotOfs;
        [FieldOffset(0x8)]
        public float zRotSpeed;
        [FieldOffset(0xc)]
        public float xySpeed;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroGrapple
    {
        [FieldOffset(0x0)]
        public float speed;
        [FieldOffset(0x4)]
        public RCPointer pTarget;
        [FieldOffset(0x8)]
        public int valid;
        [FieldOffset(0xc)]
        public float timeToTarget;
        [FieldOffset(0x10)]
        public float distToTarget;
        [FieldOffset(0x14)]
        public int straightenOut;
        [FieldOffset(0x18)]
        public int vehicle;
        [FieldOffset(0x1c)]
        public float cableLen;
        [FieldOffset(0x20)]
        public float idealCableLen;
        [FieldOffset(0x24)]
        public float targetScore;
        [FieldOffset(0x28)]
        public int earlyAbort;
        [FieldOffset(0x2C + (0x4 * 0))]
        public int pad_0;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroPullShot
    {
        [FieldOffset(0x0)]
        public RCPointer pTarget;
        [FieldOffset(0x4)]
        public int valid;
        [FieldOffset(0x8)]
        public float targetScore;
        [FieldOffset(0xc)]
        public int connected;
        [FieldOffset(0x10 + (0x4 * 0))]
        public int pad_0;
        [FieldOffset(0x10 + (0x4 * 1))]
        public int pad_1;
        [FieldOffset(0x10 + (0x4 * 2))]
        public int pad_2;
        [FieldOffset(0x10 + (0x4 * 3))]
        public int pad_3;
        [FieldOffset(0x10 + (0x4 * 4))]
        public int pad_4;
        [FieldOffset(0x10 + (0x4 * 5))]
        public int pad_5;
        [FieldOffset(0x10 + (0x4 * 6))]
        public int pad_6;
        [FieldOffset(0x10 + (0x4 * 7))]
        public int pad_7;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroDynamo
    {
        [FieldOffset(0x0)]
        public RCPointer pTarget;
        [FieldOffset(0x4)]
        public int valid;
        [FieldOffset(0x8)]
        public float targetScore;
        [FieldOffset(0xc)]
        public int trigger;
        [FieldOffset(0x10)]
        public RCPointer last_pTarget;
        [FieldOffset(0x14 + (0x4 * 0))]
        public int pad_0;
        [FieldOffset(0x14 + (0x4 * 1))]
        public int pad_1;
        [FieldOffset(0x14 + (0x4 * 2))]
        public int pad_2;
        [FieldOffset(0x14 + (0x4 * 3))]
        public int pad_3;
        [FieldOffset(0x14 + (0x4 * 4))]
        public int pad_4;
        [FieldOffset(0x14 + (0x4 * 5))]
        public int pad_5;
        [FieldOffset(0x14 + (0x4 * 6))]
        public int pad_6;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroSwing
    {
        [FieldOffset(0x0)]
        public RCPointer pNextTarget;
        [FieldOffset(0x4)]
        public RCPointer pTarget;
        [FieldOffset(0x8)]
        public float targetScore;
        [FieldOffset(0xc)]
        public short connected;
        [FieldOffset(0xe)]
        public sbyte valid;
        [FieldOffset(0xf)]
        public sbyte qSwitchMe;
        [FieldOffset(0x10)]
        public float idealRadius;
        [FieldOffset(0x14)]
        public float curCableLen;
        [FieldOffset(0x18)]
        public float radialSpeed;
        [FieldOffset(0x1c)]
        public float forwardAng;
        [FieldOffset(0x20)]
        public float gravity;
        [FieldOffset(0x24)]
        public float firstSwingSpeed;
        [FieldOffset(0x28)]
        public float alignRotSpeed;
        [FieldOffset(0x2c)]
        public short animScaleQueued;
        [FieldOffset(0x2e)]
        public short firstSwing;
        [FieldOffset(0x30)]
        public float swingElv;
        [FieldOffset(0x34)]
        public float radialGain;
        [FieldOffset(0x38)]
        public float radialDamp;
        [FieldOffset(0x3c)]
        public float radialLimit;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroQuickSand
    {
        [FieldOffset(0x0)]
        public int timesFallen;
        [FieldOffset(0x4 + (0x4 * 0))]
        public int pad_0;
        [FieldOffset(0x4 + (0x4 * 1))]
        public int pad_1;
        [FieldOffset(0x4 + (0x4 * 2))]
        public int pad_2;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroHeadIdle
    {
        [FieldOffset(0x0)]
        public Vector4 rotOffset;
        [FieldOffset(0x10)]
        public int timer;
        [FieldOffset(0x14)]
        public float gain;
        [FieldOffset(0x18)]
        public float damp;
        [FieldOffset(0x1C + (0x4 * 0))]
        public int pad_0;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroTailIdle
    {
        [FieldOffset(0x0 + (0x10 * 0))]
        public Vector4 rotOffsets_0;
        [FieldOffset(0x0 + (0x10 * 1))]
        public Vector4 rotOffsets_1;
        [FieldOffset(0x0 + (0x10 * 2))]
        public Vector4 rotOffsets_2;
        [FieldOffset(0x0 + (0x10 * 3))]
        public Vector4 rotOffsets_3;
        [FieldOffset(0x40 + (0x4 * 0))]
        public int timers_0;
        [FieldOffset(0x40 + (0x4 * 1))]
        public int timers_1;
        [FieldOffset(0x40 + (0x4 * 2))]
        public int timers_2;
        [FieldOffset(0x40 + (0x4 * 3))]
        public int timers_3;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HeroPlayerConstants
    {
        [FieldOffset(0x0)]
        public int mobyNum;
        [FieldOffset(0x4)]
        public float maxWalkSpeed;
        [FieldOffset(0x8)]
        public float kneeHeight;
        [FieldOffset(0xc)]
        public float kneeCheckDist;
        [FieldOffset(0x10)]
        public float colRadius;
        [FieldOffset(0x14)]
        public float colTop;
        [FieldOffset(0x18)]
        public float colBot;
        [FieldOffset(0x1c)]
        public float colBotFall;
        [FieldOffset(0x20)]
        public int jumpPushOffTime;
        [FieldOffset(0x24)]
        public float jumpPeakFrm;
        [FieldOffset(0x28)]
        public float jumpLandFrm;
        [FieldOffset(0x2c)]
        public float jumpGameLandFrm;
        [FieldOffset(0x30)]
        public float jumpMaxHeight;
        [FieldOffset(0x34)]
        public float jumpMinHeight;
        [FieldOffset(0x38)]
        public int jumpMaxUpTime;
        [FieldOffset(0x3c)]
        public float jumpGravity;
        [FieldOffset(0x40)]
        public float jumpMaxXySpeed;
        [FieldOffset(0x44)]
        public float fallGravity;
        [FieldOffset(0x48)]
        public float maxFallSpeed;
        [FieldOffset(0x4c)]
        public float walkAnimSpeedMul;
        [FieldOffset(0x50)]
        public float walkAnimSpeedLimLower;
        [FieldOffset(0x54)]
        public float walkAnimSpeedLimUpper;
        [FieldOffset(0x58)]
        public float jogAnimSpeedMul;
        [FieldOffset(0x5c)]
        public float jogAnimSpeedLimLower;
        [FieldOffset(0x60)]
        public float jogAnimSpeedLimUpper;
        [FieldOffset(0x64 + (0x4 * 0))]
        public int pad_0;
        [FieldOffset(0x64 + (0x4 * 1))]
        public int pad_1;
        [FieldOffset(0x64 + (0x4 * 2))]
        public int pad_2;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct tNW_GadgetEventMessage
    {
        [FieldOffset(0x0)]
        public short gadgetId;
        [FieldOffset(0x2)]
        public sbyte playerIndex;
        [FieldOffset(0x3)]
        public sbyte gadgetEventType;
        [FieldOffset(0x4)]
        public sbyte extraData;
        [FieldOffset(0x8)]
        public int activeTime;
        [FieldOffset(0xc)]
        public uint targetUID;
        [FieldOffset(0x10)]
        public Vector3 firingLoc;
        [FieldOffset(0x1c)]
        public Vector3 targetDir;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct GadgetEvent
    {
        [FieldOffset(0x0)]
        public byte gadgetID;
        [FieldOffset(0x1)]
        public byte cPlayerIndex;
        [FieldOffset(0x2)]
        public sbyte cGadgetType;
        [FieldOffset(0x3)]
        public sbyte gadgetEventType;
        [FieldOffset(0x4)]
        public int iActiveTime;
        [FieldOffset(0x8)]
        public uint targetUID;
        [FieldOffset(0x10)]
        public Vector4 targetOffsetQuat;
        [FieldOffset(0x20)]
        public RCPointer pNextGadgetEvent;
        [FieldOffset(0x24)]
        public tNW_GadgetEventMessage gadgetEventMsg;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct GadgetEntry
    {
        [FieldOffset(0x0)]
        public short level;
        [FieldOffset(0x2)]
        public short sAmmo;
        [FieldOffset(0x4)]
        public uint sXP;
        [FieldOffset(0x8)]
        public int iActionFrame;
        [FieldOffset(0xc)]
        public eModPostFXType modActivePostFX;
        [FieldOffset(0x10)]
        public eModWeaponType modActiveWeapon;
        [FieldOffset(0x14 + (0x4 * 0))]
        public eModBasicType modActiveBasic_0;
        [FieldOffset(0x14 + (0x4 * 1))]
        public eModBasicType modActiveBasic_1;
        [FieldOffset(0x14 + (0x4 * 2))]
        public eModBasicType modActiveBasic_2;
        [FieldOffset(0x14 + (0x4 * 3))]
        public eModBasicType modActiveBasic_3;
        [FieldOffset(0x14 + (0x4 * 4))]
        public eModBasicType modActiveBasic_4;
        [FieldOffset(0x14 + (0x4 * 5))]
        public eModBasicType modActiveBasic_5;
        [FieldOffset(0x14 + (0x4 * 6))]
        public eModBasicType modActiveBasic_6;
        [FieldOffset(0x14 + (0x4 * 7))]
        public eModBasicType modActiveBasic_7;
        [FieldOffset(0x14 + (0x4 * 8))]
        public eModBasicType modActiveBasic_8;
        [FieldOffset(0x14 + (0x4 * 9))]
        public eModBasicType modActiveBasic_9;
        [FieldOffset(0x3C + (0x4 * 0))]
        public eModWeaponType modWeapon_0;
        [FieldOffset(0x3C + (0x4 * 1))]
        public eModWeaponType modWeapon_1;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct GadgetBox
    {
        [FieldOffset(0x0)]
        public sbyte initialized;
        [FieldOffset(0x1)]
        public sbyte level;
        [FieldOffset(0x2 + (0x1 * 0))]
        public sbyte bButtonDown_0;
        [FieldOffset(0x2 + (0x1 * 1))]
        public sbyte bButtonDown_1;
        [FieldOffset(0x2 + (0x1 * 2))]
        public sbyte bButtonDown_2;
        [FieldOffset(0x2 + (0x1 * 3))]
        public sbyte bButtonDown_3;
        [FieldOffset(0x2 + (0x1 * 4))]
        public sbyte bButtonDown_4;
        [FieldOffset(0x2 + (0x1 * 5))]
        public sbyte bButtonDown_5;
        [FieldOffset(0x2 + (0x1 * 6))]
        public sbyte bButtonDown_6;
        [FieldOffset(0x2 + (0x1 * 7))]
        public sbyte bButtonDown_7;
        [FieldOffset(0x2 + (0x1 * 8))]
        public sbyte bButtonDown_8;
        [FieldOffset(0x2 + (0x1 * 9))]
        public sbyte bButtonDown_9;
        [FieldOffset(0xC + (0x2 * 0))]
        public short sButtonUpFrames_0;
        [FieldOffset(0xC + (0x2 * 1))]
        public short sButtonUpFrames_1;
        [FieldOffset(0xC + (0x2 * 2))]
        public short sButtonUpFrames_2;
        [FieldOffset(0xC + (0x2 * 3))]
        public short sButtonUpFrames_3;
        [FieldOffset(0xC + (0x2 * 4))]
        public short sButtonUpFrames_4;
        [FieldOffset(0xC + (0x2 * 5))]
        public short sButtonUpFrames_5;
        [FieldOffset(0xC + (0x2 * 6))]
        public short sButtonUpFrames_6;
        [FieldOffset(0xC + (0x2 * 7))]
        public short sButtonUpFrames_7;
        [FieldOffset(0xC + (0x2 * 8))]
        public short sButtonUpFrames_8;
        [FieldOffset(0xC + (0x2 * 9))]
        public short sButtonUpFrames_9;
        [FieldOffset(0x20)]
        public sbyte cNumGadgetEvents;
        [FieldOffset(0x21 + (0x1 * 0))]
        public sbyte modBasic_0;
        [FieldOffset(0x21 + (0x1 * 1))]
        public sbyte modBasic_1;
        [FieldOffset(0x21 + (0x1 * 2))]
        public sbyte modBasic_2;
        [FieldOffset(0x21 + (0x1 * 3))]
        public sbyte modBasic_3;
        [FieldOffset(0x21 + (0x1 * 4))]
        public sbyte modBasic_4;
        [FieldOffset(0x21 + (0x1 * 5))]
        public sbyte modBasic_5;
        [FieldOffset(0x21 + (0x1 * 6))]
        public sbyte modBasic_6;
        [FieldOffset(0x21 + (0x1 * 7))]
        public sbyte modBasic_7;
        [FieldOffset(0x2a)]
        public short modPostFX;
        [FieldOffset(0x2c)]
        public RCPointer pNextGadgetEvent;
        [FieldOffset(0x30 + (0x44 * 0))]
        public GadgetEvent gadgetEventSlots_0;
        [FieldOffset(0x30 + (0x44 * 1))]
        public GadgetEvent gadgetEventSlots_1;
        [FieldOffset(0x30 + (0x44 * 2))]
        public GadgetEvent gadgetEventSlots_2;
        [FieldOffset(0x30 + (0x44 * 3))]
        public GadgetEvent gadgetEventSlots_3;
        [FieldOffset(0x30 + (0x44 * 4))]
        public GadgetEvent gadgetEventSlots_4;
        [FieldOffset(0x30 + (0x44 * 5))]
        public GadgetEvent gadgetEventSlots_5;
        [FieldOffset(0x30 + (0x44 * 6))]
        public GadgetEvent gadgetEventSlots_6;
        [FieldOffset(0x30 + (0x44 * 7))]
        public GadgetEvent gadgetEventSlots_7;
        [FieldOffset(0x30 + (0x44 * 8))]
        public GadgetEvent gadgetEventSlots_8;
        [FieldOffset(0x30 + (0x44 * 9))]
        public GadgetEvent gadgetEventSlots_9;
        [FieldOffset(0x30 + (0x44 * 10))]
        public GadgetEvent gadgetEventSlots_10;
        [FieldOffset(0x30 + (0x44 * 11))]
        public GadgetEvent gadgetEventSlots_11;
        [FieldOffset(0x30 + (0x44 * 12))]
        public GadgetEvent gadgetEventSlots_12;
        [FieldOffset(0x30 + (0x44 * 13))]
        public GadgetEvent gadgetEventSlots_13;
        [FieldOffset(0x30 + (0x44 * 14))]
        public GadgetEvent gadgetEventSlots_14;
        [FieldOffset(0x30 + (0x44 * 15))]
        public GadgetEvent gadgetEventSlots_15;
        [FieldOffset(0x30 + (0x44 * 16))]
        public GadgetEvent gadgetEventSlots_16;
        [FieldOffset(0x30 + (0x44 * 17))]
        public GadgetEvent gadgetEventSlots_17;
        [FieldOffset(0x30 + (0x44 * 18))]
        public GadgetEvent gadgetEventSlots_18;
        [FieldOffset(0x30 + (0x44 * 19))]
        public GadgetEvent gadgetEventSlots_19;
        [FieldOffset(0x30 + (0x44 * 20))]
        public GadgetEvent gadgetEventSlots_20;
        [FieldOffset(0x30 + (0x44 * 21))]
        public GadgetEvent gadgetEventSlots_21;
        [FieldOffset(0x30 + (0x44 * 22))]
        public GadgetEvent gadgetEventSlots_22;
        [FieldOffset(0x30 + (0x44 * 23))]
        public GadgetEvent gadgetEventSlots_23;
        [FieldOffset(0x30 + (0x44 * 24))]
        public GadgetEvent gadgetEventSlots_24;
        [FieldOffset(0x30 + (0x44 * 25))]
        public GadgetEvent gadgetEventSlots_25;
        [FieldOffset(0x30 + (0x44 * 26))]
        public GadgetEvent gadgetEventSlots_26;
        [FieldOffset(0x30 + (0x44 * 27))]
        public GadgetEvent gadgetEventSlots_27;
        [FieldOffset(0x30 + (0x44 * 28))]
        public GadgetEvent gadgetEventSlots_28;
        [FieldOffset(0x30 + (0x44 * 29))]
        public GadgetEvent gadgetEventSlots_29;
        [FieldOffset(0x30 + (0x44 * 30))]
        public GadgetEvent gadgetEventSlots_30;
        [FieldOffset(0x30 + (0x44 * 31))]
        public GadgetEvent gadgetEventSlots_31;
        [FieldOffset(0xA30 + (0x44 * 0))]
        public GadgetEntry gadgets_0;
        [FieldOffset(0xA30 + (0x44 * 1))]
        public GadgetEntry gadgets_1;
        [FieldOffset(0xA30 + (0x44 * 2))]
        public GadgetEntry gadgets_2;
        [FieldOffset(0xA30 + (0x44 * 3))]
        public GadgetEntry gadgets_3;
        [FieldOffset(0xA30 + (0x44 * 4))]
        public GadgetEntry gadgets_4;
        [FieldOffset(0xA30 + (0x44 * 5))]
        public GadgetEntry gadgets_5;
        [FieldOffset(0xA30 + (0x44 * 6))]
        public GadgetEntry gadgets_6;
        [FieldOffset(0xA30 + (0x44 * 7))]
        public GadgetEntry gadgets_7;
        [FieldOffset(0xA30 + (0x44 * 8))]
        public GadgetEntry gadgets_8;
        [FieldOffset(0xA30 + (0x44 * 9))]
        public GadgetEntry gadgets_9;
        [FieldOffset(0xA30 + (0x44 * 10))]
        public GadgetEntry gadgets_10;
        [FieldOffset(0xA30 + (0x44 * 11))]
        public GadgetEntry gadgets_11;
        [FieldOffset(0xA30 + (0x44 * 12))]
        public GadgetEntry gadgets_12;
        [FieldOffset(0xA30 + (0x44 * 13))]
        public GadgetEntry gadgets_13;
        [FieldOffset(0xA30 + (0x44 * 14))]
        public GadgetEntry gadgets_14;
        [FieldOffset(0xA30 + (0x44 * 15))]
        public GadgetEntry gadgets_15;
        [FieldOffset(0xA30 + (0x44 * 16))]
        public GadgetEntry gadgets_16;
        [FieldOffset(0xA30 + (0x44 * 17))]
        public GadgetEntry gadgets_17;
        [FieldOffset(0xA30 + (0x44 * 18))]
        public GadgetEntry gadgets_18;
        [FieldOffset(0xA30 + (0x44 * 19))]
        public GadgetEntry gadgets_19;
        [FieldOffset(0xA30 + (0x44 * 20))]
        public GadgetEntry gadgets_20;
        [FieldOffset(0xA30 + (0x44 * 21))]
        public GadgetEntry gadgets_21;
        [FieldOffset(0xA30 + (0x44 * 22))]
        public GadgetEntry gadgets_22;
        [FieldOffset(0xA30 + (0x44 * 23))]
        public GadgetEntry gadgets_23;
        [FieldOffset(0xA30 + (0x44 * 24))]
        public GadgetEntry gadgets_24;
        [FieldOffset(0xA30 + (0x44 * 25))]
        public GadgetEntry gadgets_25;
        [FieldOffset(0xA30 + (0x44 * 26))]
        public GadgetEntry gadgets_26;
        [FieldOffset(0xA30 + (0x44 * 27))]
        public GadgetEntry gadgets_27;
        [FieldOffset(0xA30 + (0x44 * 28))]
        public GadgetEntry gadgets_28;
        [FieldOffset(0xA30 + (0x44 * 29))]
        public GadgetEntry gadgets_29;
        [FieldOffset(0xA30 + (0x44 * 30))]
        public GadgetEntry gadgets_30;
        [FieldOffset(0xA30 + (0x44 * 31))]
        public GadgetEntry gadgets_31;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct Hero
    {
        [FieldOffset(0x0)]
        public Gid gid;
        [FieldOffset(0x4)]
        public int MasterHostId;
        [FieldOffset(0x8)]
        public int State;
        [FieldOffset(0xc)]
        public RCPointer pNext;
        [FieldOffset(0x10)]
        public RCPointer pPrev;
        [FieldOffset(0x14)]
        public RCPointer pVTable;
        [FieldOffset(0x20)]
        public Matrix4x4 mtx;
        [FieldOffset(0x60)]
        public Matrix4x4 invMtx;
        [FieldOffset(0xa0)]
        public Vector4 pos;
        [FieldOffset(0xb0)]
        public Vector4 rot;
        [FieldOffset(0xc0)]
        public Vector4 rotSpeed;
        [FieldOffset(0xd0)]
        public Vector4 sphereCenter;
        [FieldOffset(0xe0)]
        public Vector4 missileTarget;
        [FieldOffset(0xf0)]
        public Vector4 cg;
        [FieldOffset(0x100)]
        public Vector4 mtxFxScale;
        [FieldOffset(0x110)]
        public Vector4 lastPos;
        [FieldOffset(0x120)]
        public Vector4 stickInput;
        [FieldOffset(0x130)]
        public HeroMove move;
        [FieldOffset(0x1d0)]
        public HeroColl coll;
        [FieldOffset(0x250)]
        public HeroGround ground;
        [FieldOffset(0x310)]
        public HeroTrack track;
        [FieldOffset(0x370)]
        public HeroTimers timers;
        [FieldOffset(0x430)]
        public HeroHotspots hotspots;
        [FieldOffset(0x440)]
        public HeroGrind grind;
        [FieldOffset(0x540)]
        public HeroZip zip;
        [FieldOffset(0x590)]
        public HeroFireDir fireDir;
        [FieldOffset(0x5e0)]
        public HeroLockOn lockOn;
        [FieldOffset(0x630)]
        public HeroMobys mobys;
        [FieldOffset(0x640)]
        public HeroAnim anim;
        [FieldOffset(0x660)]
        public HeroJoints joints;
        [FieldOffset(0x7e0)]
        public HeroAnimLayers animLayers;
        [FieldOffset(0x820 + (0xB0 * 0))]
        public HeroTweaker tweaker_0;
        [FieldOffset(0x820 + (0xB0 * 1))]
        public HeroTweaker tweaker_1;
        [FieldOffset(0x820 + (0xB0 * 2))]
        public HeroTweaker tweaker_2;
        [FieldOffset(0x820 + (0xB0 * 3))]
        public HeroTweaker tweaker_3;
        [FieldOffset(0x820 + (0xB0 * 4))]
        public HeroTweaker tweaker_4;
        [FieldOffset(0x820 + (0xB0 * 5))]
        public HeroTweaker tweaker_5;
        [FieldOffset(0x820 + (0xB0 * 6))]
        public HeroTweaker tweaker_6;
        [FieldOffset(0x820 + (0xB0 * 7))]
        public HeroTweaker tweaker_7;
        [FieldOffset(0x820 + (0xB0 * 8))]
        public HeroTweaker tweaker_8;
        [FieldOffset(0x820 + (0xB0 * 9))]
        public HeroTweaker tweaker_9;
        [FieldOffset(0x820 + (0xB0 * 10))]
        public HeroTweaker tweaker_10;
        [FieldOffset(0x820 + (0xB0 * 11))]
        public HeroTweaker tweaker_11;
        [FieldOffset(0x820 + (0xB0 * 12))]
        public HeroTweaker tweaker_12;
        [FieldOffset(0x820 + (0xB0 * 13))]
        public HeroTweaker tweaker_13;
        [FieldOffset(0x820 + (0xB0 * 14))]
        public HeroTweaker tweaker_14;
        [FieldOffset(0x820 + (0xB0 * 15))]
        public HeroTweaker tweaker_15;
        [FieldOffset(0x820 + (0xB0 * 16))]
        public HeroTweaker tweaker_16;
        [FieldOffset(0x820 + (0xB0 * 17))]
        public HeroTweaker tweaker_17;
        [FieldOffset(0x1480)]
        public HeroShadow shadow;
        [FieldOffset(0x14b0)]
        public HeroEyes eyes;
        [FieldOffset(0x1780)]
        public HeroThrust thrust;
        [FieldOffset(0x1790)]
        public HeroTurn turn;
        [FieldOffset(0x17b0)]
        public HeroAttack attack;
        [FieldOffset(0x1860)]
        public HeroHeadIdle headIdle;
        [FieldOffset(0x1880)]
        public HeroTailIdle tailIdle;
        [FieldOffset(0x18d0)]
        public HeroFps fps;
        [FieldOffset(0x1b10)]
        public HeroWeaponPosRec weaponPosRec;
        [FieldOffset(0x1c10)]
        public HeroWalkToPos walkToPos;
        [FieldOffset(0x1c40)]
        public HeroSurf surf;
        [FieldOffset(0x1c70)]
        public HeroWalk walk;
        [FieldOffset(0x1ca0)]
        public HeroJump jump;
        [FieldOffset(0x1da0)]
        public HeroLedge ledge;
        [FieldOffset(0x1de0)]
        public HeroTractorBeam tractorBeam;
        [FieldOffset(0x1df0)]
        public HeroCharge sbytege;
        [FieldOffset(0x1e10)]
        public HeroSwim swim;
        [FieldOffset(0x1e60)]
        public HeroWind wind;
        [FieldOffset(0x1e80)]
        public HeroFall fall;
        [FieldOffset(0x1ea0)]
        public HeroGrapple grapple;
        [FieldOffset(0x1ed0)]
        public HeroSwing swing;
        [FieldOffset(0x1f10)]
        public HeroPullShot pullShot;
        [FieldOffset(0x1f40)]
        public HeroDynamo dynamo;
        [FieldOffset(0x1f70)]
        public HeroQuickSand quicksand;
        [FieldOffset(0x1f80)]
        public HeroDust dust;
        [FieldOffset(0x1f90)]
        public HeroCommand command;
        [FieldOffset(0x1f9c)]
        public TargetVars target;
        [FieldOffset(0x2030)]
        public MotionBlur motionBlur;
        [FieldOffset(0x2180)]
        public MotionBlur wrenchMotionBlur;
        [FieldOffset(0x22D0 + (0x50 * 0))]
        public Gadget gadgets_0;
        [FieldOffset(0x22D0 + (0x50 * 1))]
        public Gadget gadgets_1;
        [FieldOffset(0x22D0 + (0x50 * 2))]
        public Gadget gadgets_2;
        [FieldOffset(0x22D0 + (0x50 * 3))]
        public Gadget gadgets_3;
        [FieldOffset(0x22D0 + (0x50 * 4))]
        public Gadget gadgets_4;
        [FieldOffset(0x22D0 + (0x50 * 5))]
        public Gadget gadgets_5;
        [FieldOffset(0x24B0 + (0x4 * 0))]
        public int assGadgets_0;
        [FieldOffset(0x24B0 + (0x4 * 1))]
        public int assGadgets_1;
        [FieldOffset(0x24B0 + (0x4 * 2))]
        public int assGadgets_2;
        [FieldOffset(0x24B0 + (0x4 * 3))]
        public int assGadgets_3;
        [FieldOffset(0x24B0 + (0x4 * 4))]
        public int assGadgets_4;
        [FieldOffset(0x24B0 + (0x4 * 5))]
        public int assGadgets_5;
        [FieldOffset(0x24d0)]
        public Vector4 prevHandPos;
        [FieldOffset(0x24E0 + (0x10 * 0))]
        public Vector4 gadgetGlowPos_0;
        [FieldOffset(0x24E0 + (0x10 * 1))]
        public Vector4 gadgetGlowPos_1;
        [FieldOffset(0x24E0 + (0x10 * 2))]
        public Vector4 gadgetGlowPos_2;
        [FieldOffset(0x24E0 + (0x10 * 3))]
        public Vector4 gadgetGlowPos_3;
        [FieldOffset(0x24E0 + (0x10 * 4))]
        public Vector4 gadgetGlowPos_4;
        [FieldOffset(0x24E0 + (0x10 * 5))]
        public Vector4 gadgetGlowPos_5;
        [FieldOffset(0x24E0 + (0x10 * 6))]
        public Vector4 gadgetGlowPos_6;
        [FieldOffset(0x24E0 + (0x10 * 7))]
        public Vector4 gadgetGlowPos_7;
        [FieldOffset(0x2560 + (0x4 * 0))]
        public int gadgetGlowRGBA_0;
        [FieldOffset(0x2560 + (0x4 * 1))]
        public int gadgetGlowRGBA_1;
        [FieldOffset(0x2560 + (0x4 * 2))]
        public int gadgetGlowRGBA_2;
        [FieldOffset(0x2560 + (0x4 * 3))]
        public int gadgetGlowRGBA_3;
        [FieldOffset(0x2560 + (0x4 * 4))]
        public int gadgetGlowRGBA_4;
        [FieldOffset(0x2560 + (0x4 * 5))]
        public int gadgetGlowRGBA_5;
        [FieldOffset(0x2560 + (0x4 * 6))]
        public int gadgetGlowRGBA_6;
        [FieldOffset(0x2560 + (0x4 * 7))]
        public int gadgetGlowRGBA_7;
        [FieldOffset(0x2580 + (0x4 * 0))]
        public float gadgetGlowSize_0;
        [FieldOffset(0x2580 + (0x4 * 1))]
        public float gadgetGlowSize_1;
        [FieldOffset(0x2580 + (0x4 * 2))]
        public float gadgetGlowSize_2;
        [FieldOffset(0x2580 + (0x4 * 3))]
        public float gadgetGlowSize_3;
        [FieldOffset(0x2580 + (0x4 * 4))]
        public float gadgetGlowSize_4;
        [FieldOffset(0x2580 + (0x4 * 5))]
        public float gadgetGlowSize_5;
        [FieldOffset(0x2580 + (0x4 * 6))]
        public float gadgetGlowSize_6;
        [FieldOffset(0x2580 + (0x4 * 7))]
        public float gadgetGlowSize_7;
        [FieldOffset(0x25A0 + (0x4 * 0))]
        public float gadgetGlowOfs_0;
        [FieldOffset(0x25A0 + (0x4 * 1))]
        public float gadgetGlowOfs_1;
        [FieldOffset(0x25A0 + (0x4 * 2))]
        public float gadgetGlowOfs_2;
        [FieldOffset(0x25A0 + (0x4 * 3))]
        public float gadgetGlowOfs_3;
        [FieldOffset(0x25A0 + (0x4 * 4))]
        public float gadgetGlowOfs_4;
        [FieldOffset(0x25A0 + (0x4 * 5))]
        public float gadgetGlowOfs_5;
        [FieldOffset(0x25A0 + (0x4 * 6))]
        public float gadgetGlowOfs_6;
        [FieldOffset(0x25A0 + (0x4 * 7))]
        public float gadgetGlowOfs_7;
        [FieldOffset(0x25c0)]
        public short gadgetGlowCnt;
        [FieldOffset(0x25c4)]
        public float heroSpeedAdjuster;
        [FieldOffset(0x25c8)]
        public sbyte playerPostDrawFxRegistered;
        [FieldOffset(0x25c9)]
        public sbyte playerPostPreDrawFxRegistered;
        [FieldOffset(0x25ca)]
        public sbyte playerFontDrawRegistered;
        [FieldOffset(0x25cc)]
        public HERO_STATE_ENUM state;
        [FieldOffset(0x25d0)]
        public int subState;
        [FieldOffset(0x25d4)]
        public HERO_TYPE_ENUM stateType;
        [FieldOffset(0x25d8)]
        public HERO_STATE_ENUM previousState;
        [FieldOffset(0x25dc)]
        public HERO_TYPE_ENUM previousType;
        [FieldOffset(0x25e0)]
        public int previousStateTimer;
        [FieldOffset(0x25e4)]
        public HERO_STATE_ENUM prePreviousState;
        [FieldOffset(0x25e8)]
        public HERO_TYPE_ENUM prePreviousType;
        [FieldOffset(0x25ec + (4 * 0))]
        public HERO_STATE_ENUM stateHistory_0;
        [FieldOffset(0x25ec + (4 * 1))]
        public HERO_STATE_ENUM stateHistory_1;
        [FieldOffset(0x25ec + (4 * 2))]
        public HERO_STATE_ENUM stateHistory_2;
        [FieldOffset(0x25ec + (4 * 3))]
        public HERO_STATE_ENUM stateHistory_3;
        [FieldOffset(0x25ec + (4 * 4))]
        public HERO_STATE_ENUM stateHistory_4;
        [FieldOffset(0x25ec + (4 * 5))]
        public HERO_STATE_ENUM stateHistory_5;
        [FieldOffset(0x25ec + (4 * 6))]
        public HERO_STATE_ENUM stateHistory_6;
        [FieldOffset(0x25ec + (4 * 7))]
        public HERO_STATE_ENUM stateHistory_7;
        [FieldOffset(0x260c + (4 * 0))]
        public HERO_TYPE_ENUM stateTypeHistory_0;
        [FieldOffset(0x260c + (4 * 1))]
        public HERO_TYPE_ENUM stateTypeHistory_1;
        [FieldOffset(0x260c + (4 * 2))]
        public HERO_TYPE_ENUM stateTypeHistory_2;
        [FieldOffset(0x260c + (4 * 3))]
        public HERO_TYPE_ENUM stateTypeHistory_3;
        [FieldOffset(0x260c + (4 * 4))]
        public HERO_TYPE_ENUM stateTypeHistory_4;
        [FieldOffset(0x260c + (4 * 5))]
        public HERO_TYPE_ENUM stateTypeHistory_5;
        [FieldOffset(0x260c + (4 * 6))]
        public HERO_TYPE_ENUM stateTypeHistory_6;
        [FieldOffset(0x260c + (4 * 7))]
        public HERO_TYPE_ENUM stateTypeHistory_7;
        [FieldOffset(0x262C + (0x4 * 0))]
        public int stateTimerHistory_0;
        [FieldOffset(0x262C + (0x4 * 1))]
        public int stateTimerHistory_1;
        [FieldOffset(0x262C + (0x4 * 2))]
        public int stateTimerHistory_2;
        [FieldOffset(0x262C + (0x4 * 3))]
        public int stateTimerHistory_3;
        [FieldOffset(0x262C + (0x4 * 4))]
        public int stateTimerHistory_4;
        [FieldOffset(0x262C + (0x4 * 5))]
        public int stateTimerHistory_5;
        [FieldOffset(0x262C + (0x4 * 6))]
        public int stateTimerHistory_6;
        [FieldOffset(0x262C + (0x4 * 7))]
        public int stateTimerHistory_7;
        [FieldOffset(0x264c)]
        public int stateHistoryLen;
        [FieldOffset(0x2650)]
        public int effectsLevel;
        [FieldOffset(0x2654)]
        public float minDistToLocalCamera;
        [FieldOffset(0x2658)]
        public sbyte cycleFiring;
        [FieldOffset(0x2659)]
        public sbyte gravityType;
        [FieldOffset(0x265a)]
        public sbyte firing;
        [FieldOffset(0x265c)]
        public int clankIdleTimer;
        [FieldOffset(0x2660)]
        public sbyte raisedGunArm;
        [FieldOffset(0x2661)]
        public sbyte inShallowWater;
        [FieldOffset(0x2662)]
        public sbyte invisible;
        [FieldOffset(0x2663)]
        public sbyte hideWeapon;
        [FieldOffset(0x2664)]
        public sbyte gadgetsOff;
        [FieldOffset(0x2665)]
        public sbyte gadgetNotReady;
        [FieldOffset(0x2666)]
        public sbyte wrenchOnly;
        [FieldOffset(0x2667)]
        public sbyte holdingDeathAnim;
        [FieldOffset(0x2668)]
        public sbyte hideWrench;
        [FieldOffset(0x2669)]
        public sbyte spawnBoltsToMe;
        [FieldOffset(0x266a)]
        public sbyte aiFollowingMe;
        [FieldOffset(0x266b)]
        public sbyte forceWrenchSwitch;
        [FieldOffset(0x266c)]
        public sbyte forceSwingSwitch;
        [FieldOffset(0x266d)]
        public sbyte isLocal;
        [FieldOffset(0x266e)]
        public sbyte inBaseHack;
        [FieldOffset(0x266f)]
        public sbyte shieldTrigger;
        [FieldOffset(0x2670)]
        public sbyte curSeg;
        [FieldOffset(0x2671)]
        public sbyte handGadgetType;
        [FieldOffset(0x2672)]
        public sbyte externalUpdate;
        [FieldOffset(0x2674 + (0x4 * 0))]
        public int hudGadgets_0;
        [FieldOffset(0x2674 + (0x4 * 1))]
        public int hudGadgets_1;
        [FieldOffset(0x2674 + (0x4 * 2))]
        public int hudGadgets_2;
        [FieldOffset(0x2674 + (0x4 * 3))]
        public int hudGadgets_3;
        [FieldOffset(0x2674 + (0x4 * 4))]
        public int hudGadgets_4;
        [FieldOffset(0x2674 + (0x4 * 5))]
        public int hudGadgets_5;
        [FieldOffset(0x268C + (0x4 * 0))]
        public int desiredGadgets_0;
        [FieldOffset(0x268C + (0x4 * 1))]
        public int desiredGadgets_1;
        [FieldOffset(0x268C + (0x4 * 2))]
        public int desiredGadgets_2;
        [FieldOffset(0x268C + (0x4 * 3))]
        public int desiredGadgets_3;
        [FieldOffset(0x268C + (0x4 * 4))]
        public int desiredGadgets_4;
        [FieldOffset(0x268C + (0x4 * 5))]
        public int desiredGadgets_5;
        [FieldOffset(0x26A4 + (0x4 * 0))]
        public int preemptedGadgets_0;
        [FieldOffset(0x26A4 + (0x4 * 1))]
        public int preemptedGadgets_1;
        [FieldOffset(0x26A4 + (0x4 * 2))]
        public int preemptedGadgets_2;
        [FieldOffset(0x26A4 + (0x4 * 3))]
        public int preemptedGadgets_3;
        [FieldOffset(0x26A4 + (0x4 * 4))]
        public int preemptedGadgets_4;
        [FieldOffset(0x26A4 + (0x4 * 5))]
        public int preemptedGadgets_5;
        [FieldOffset(0x26BC + (0x4 * 0))]
        public int restoreGadgetFlags_0;
        [FieldOffset(0x26BC + (0x4 * 1))]
        public int restoreGadgetFlags_1;
        [FieldOffset(0x26BC + (0x4 * 2))]
        public int restoreGadgetFlags_2;
        [FieldOffset(0x26BC + (0x4 * 3))]
        public int restoreGadgetFlags_3;
        [FieldOffset(0x26BC + (0x4 * 4))]
        public int restoreGadgetFlags_4;
        [FieldOffset(0x26BC + (0x4 * 5))]
        public int restoreGadgetFlags_5;
        [FieldOffset(0x26d4)]
        public int loadingGadget;
        [FieldOffset(0x26d8)]
        public int hackerRemovedGadget;
        [FieldOffset(0x26dc)]
        public sbyte currIdleAnim;
        [FieldOffset(0x26dd)]
        public sbyte machineGunFire;
        [FieldOffset(0x26e0)]
        public int lastHackTime;
        [FieldOffset(0x26e4)]
        public RCPointer pGadgetBox;
        [FieldOffset(0x26E8 + (0x2 * 0))]
        public short minesPendingExplode_0;
        [FieldOffset(0x26E8 + (0x2 * 1))]
        public short minesPendingExplode_1;
        [FieldOffset(0x26F0 + (0x10 * 0))]
        public Vector4 failsafePosRing_0;
        [FieldOffset(0x26F0 + (0x10 * 1))]
        public Vector4 failsafePosRing_1;
        [FieldOffset(0x26F0 + (0x10 * 2))]
        public Vector4 failsafePosRing_2;
        [FieldOffset(0x26F0 + (0x10 * 3))]
        public Vector4 failsafePosRing_3;
        [FieldOffset(0x26F0 + (0x10 * 4))]
        public Vector4 failsafePosRing_4;
        [FieldOffset(0x26F0 + (0x10 * 5))]
        public Vector4 failsafePosRing_5;
        [FieldOffset(0x26F0 + (0x10 * 6))]
        public Vector4 failsafePosRing_6;
        [FieldOffset(0x26F0 + (0x10 * 7))]
        public Vector4 failsafePosRing_7;
        [FieldOffset(0x26F0 + (0x10 * 8))]
        public Vector4 failsafePosRing_8;
        [FieldOffset(0x26F0 + (0x10 * 9))]
        public Vector4 failsafePosRing_9;
        [FieldOffset(0x26F0 + (0x10 * 10))]
        public Vector4 failsafePosRing_10;
        [FieldOffset(0x26F0 + (0x10 * 11))]
        public Vector4 failsafePosRing_11;
        [FieldOffset(0x26F0 + (0x10 * 12))]
        public Vector4 failsafePosRing_12;
        [FieldOffset(0x26F0 + (0x10 * 13))]
        public Vector4 failsafePosRing_13;
        [FieldOffset(0x26F0 + (0x10 * 14))]
        public Vector4 failsafePosRing_14;
        [FieldOffset(0x26F0 + (0x10 * 15))]
        public Vector4 failsafePosRing_15;
        [FieldOffset(0x26F0 + (0x10 * 16))]
        public Vector4 failsafePosRing_16;
        [FieldOffset(0x26F0 + (0x10 * 17))]
        public Vector4 failsafePosRing_17;
        [FieldOffset(0x26F0 + (0x10 * 18))]
        public Vector4 failsafePosRing_18;
        [FieldOffset(0x26F0 + (0x10 * 19))]
        public Vector4 failsafePosRing_19;
        [FieldOffset(0x26F0 + (0x10 * 20))]
        public Vector4 failsafePosRing_20;
        [FieldOffset(0x26F0 + (0x10 * 21))]
        public Vector4 failsafePosRing_21;
        [FieldOffset(0x26F0 + (0x10 * 22))]
        public Vector4 failsafePosRing_22;
        [FieldOffset(0x26F0 + (0x10 * 23))]
        public Vector4 failsafePosRing_23;
        [FieldOffset(0x26F0 + (0x10 * 24))]
        public Vector4 failsafePosRing_24;
        [FieldOffset(0x26F0 + (0x10 * 25))]
        public Vector4 failsafePosRing_25;
        [FieldOffset(0x26F0 + (0x10 * 26))]
        public Vector4 failsafePosRing_26;
        [FieldOffset(0x26F0 + (0x10 * 27))]
        public Vector4 failsafePosRing_27;
        [FieldOffset(0x26F0 + (0x10 * 28))]
        public Vector4 failsafePosRing_28;
        [FieldOffset(0x26F0 + (0x10 * 29))]
        public Vector4 failsafePosRing_29;
        [FieldOffset(0x26F0 + (0x10 * 30))]
        public Vector4 failsafePosRing_30;
        [FieldOffset(0x26F0 + (0x10 * 31))]
        public Vector4 failsafePosRing_31;
        [FieldOffset(0x28F0 + (0x4 * 0))]
        public float rotZRing_0;
        [FieldOffset(0x28F0 + (0x4 * 1))]
        public float rotZRing_1;
        [FieldOffset(0x28F0 + (0x4 * 2))]
        public float rotZRing_2;
        [FieldOffset(0x28F0 + (0x4 * 3))]
        public float rotZRing_3;
        [FieldOffset(0x28F0 + (0x4 * 4))]
        public float rotZRing_4;
        [FieldOffset(0x28F0 + (0x4 * 5))]
        public float rotZRing_5;
        [FieldOffset(0x28F0 + (0x4 * 6))]
        public float rotZRing_6;
        [FieldOffset(0x28F0 + (0x4 * 7))]
        public float rotZRing_7;
        [FieldOffset(0x28F0 + (0x4 * 8))]
        public float rotZRing_8;
        [FieldOffset(0x28F0 + (0x4 * 9))]
        public float rotZRing_9;
        [FieldOffset(0x28F0 + (0x4 * 10))]
        public float rotZRing_10;
        [FieldOffset(0x28F0 + (0x4 * 11))]
        public float rotZRing_11;
        [FieldOffset(0x28F0 + (0x4 * 12))]
        public float rotZRing_12;
        [FieldOffset(0x28F0 + (0x4 * 13))]
        public float rotZRing_13;
        [FieldOffset(0x28F0 + (0x4 * 14))]
        public float rotZRing_14;
        [FieldOffset(0x28F0 + (0x4 * 15))]
        public float rotZRing_15;
        [FieldOffset(0x28F0 + (0x4 * 16))]
        public float rotZRing_16;
        [FieldOffset(0x28F0 + (0x4 * 17))]
        public float rotZRing_17;
        [FieldOffset(0x28F0 + (0x4 * 18))]
        public float rotZRing_18;
        [FieldOffset(0x28F0 + (0x4 * 19))]
        public float rotZRing_19;
        [FieldOffset(0x28F0 + (0x4 * 20))]
        public float rotZRing_20;
        [FieldOffset(0x28F0 + (0x4 * 21))]
        public float rotZRing_21;
        [FieldOffset(0x28F0 + (0x4 * 22))]
        public float rotZRing_22;
        [FieldOffset(0x28F0 + (0x4 * 23))]
        public float rotZRing_23;
        [FieldOffset(0x28F0 + (0x4 * 24))]
        public float rotZRing_24;
        [FieldOffset(0x28F0 + (0x4 * 25))]
        public float rotZRing_25;
        [FieldOffset(0x28F0 + (0x4 * 26))]
        public float rotZRing_26;
        [FieldOffset(0x28F0 + (0x4 * 27))]
        public float rotZRing_27;
        [FieldOffset(0x28F0 + (0x4 * 28))]
        public float rotZRing_28;
        [FieldOffset(0x28F0 + (0x4 * 29))]
        public float rotZRing_29;
        [FieldOffset(0x28F0 + (0x4 * 30))]
        public float rotZRing_30;
        [FieldOffset(0x28F0 + (0x4 * 31))]
        public float rotZRing_31;
        [FieldOffset(0x2970 + (0x30 * 0))]
        public Matrix3x3 gadgetRotRing_0;
        [FieldOffset(0x2970 + (0x30 * 1))]
        public Matrix3x3 gadgetRotRing_1;
        [FieldOffset(0x2970 + (0x30 * 2))]
        public Matrix3x3 gadgetRotRing_2;
        [FieldOffset(0x2970 + (0x30 * 3))]
        public Matrix3x3 gadgetRotRing_3;
        [FieldOffset(0x2970 + (0x30 * 4))]
        public Matrix3x3 gadgetRotRing_4;
        [FieldOffset(0x2970 + (0x30 * 5))]
        public Matrix3x3 gadgetRotRing_5;
        [FieldOffset(0x2970 + (0x30 * 6))]
        public Matrix3x3 gadgetRotRing_6;
        [FieldOffset(0x2970 + (0x30 * 7))]
        public Matrix3x3 gadgetRotRing_7;
        [FieldOffset(0x2970 + (0x30 * 8))]
        public Matrix3x3 gadgetRotRing_8;
        [FieldOffset(0x2970 + (0x30 * 9))]
        public Matrix3x3 gadgetRotRing_9;
        [FieldOffset(0x2970 + (0x30 * 10))]
        public Matrix3x3 gadgetRotRing_10;
        [FieldOffset(0x2970 + (0x30 * 11))]
        public Matrix3x3 gadgetRotRing_11;
        [FieldOffset(0x2970 + (0x30 * 12))]
        public Matrix3x3 gadgetRotRing_12;
        [FieldOffset(0x2970 + (0x30 * 13))]
        public Matrix3x3 gadgetRotRing_13;
        [FieldOffset(0x2970 + (0x30 * 14))]
        public Matrix3x3 gadgetRotRing_14;
        [FieldOffset(0x2970 + (0x30 * 15))]
        public Matrix3x3 gadgetRotRing_15;
        [FieldOffset(0x2c70)]
        public int rotZringIndex;
        [FieldOffset(0x2c74)]
        public int rotZringValidSize;
        [FieldOffset(0x2c78)]
        public int failsafeRingIndex;
        [FieldOffset(0x2c7c)]
        public int failsafeRingValidSize;
        [FieldOffset(0x2c80)]
        public int gadgetRotRingIndex;
        [FieldOffset(0x2c84)]
        public int gadgetRotRingValidSize;
        [FieldOffset(0x2C88 + (0x4 * 0))]
        public int cameraPosRing_0;
        [FieldOffset(0x2C88 + (0x4 * 1))]
        public int cameraPosRing_1;
        [FieldOffset(0x2C88 + (0x4 * 2))]
        public int cameraPosRing_2;
        [FieldOffset(0x2C88 + (0x4 * 3))]
        public int cameraPosRing_3;
        [FieldOffset(0x2C88 + (0x4 * 4))]
        public int cameraPosRing_4;
        [FieldOffset(0x2C88 + (0x4 * 5))]
        public int cameraPosRing_5;
        [FieldOffset(0x2C88 + (0x4 * 6))]
        public int cameraPosRing_6;
        [FieldOffset(0x2C88 + (0x4 * 7))]
        public int cameraPosRing_7;
        [FieldOffset(0x2CA8 + (0x4 * 0))]
        public int cameraRotRing_0;
        [FieldOffset(0x2CA8 + (0x4 * 1))]
        public int cameraRotRing_1;
        [FieldOffset(0x2CA8 + (0x4 * 2))]
        public int cameraRotRing_2;
        [FieldOffset(0x2CA8 + (0x4 * 3))]
        public int cameraRotRing_3;
        [FieldOffset(0x2CA8 + (0x4 * 4))]
        public int cameraRotRing_4;
        [FieldOffset(0x2CA8 + (0x4 * 5))]
        public int cameraRotRing_5;
        [FieldOffset(0x2CA8 + (0x4 * 6))]
        public int cameraRotRing_6;
        [FieldOffset(0x2CA8 + (0x4 * 7))]
        public int cameraRotRing_7;
        [FieldOffset(0x2cc8)]
        public int camRingIndex;
        [FieldOffset(0x2ccc)]
        public int camRingValidSize;
        [FieldOffset(0x2cd0)]
        public Vector4 camPos;
        [FieldOffset(0x2ce0)]
        public Vector4 camRot;
        [FieldOffset(0x2cf0)]
        public Matrix3x3 camUMtx;
        [FieldOffset(0x2D20 + (0x8 * 0))]
        public HeroQueuedSound queuedSounds_0;
        [FieldOffset(0x2D20 + (0x8 * 1))]
        public HeroQueuedSound queuedSounds_1;
        [FieldOffset(0x2D30 + (0x4 * 0))]
        public int loopingSounds_0;
        [FieldOffset(0x2D30 + (0x4 * 1))]
        public int loopingSounds_1;
        [FieldOffset(0x2D30 + (0x4 * 2))]
        public int loopingSounds_2;
        [FieldOffset(0x2D30 + (0x4 * 3))]
        public int loopingSounds_3;
        [FieldOffset(0x2D30 + (0x4 * 4))]
        public int loopingSounds_4;
        [FieldOffset(0x2D30 + (0x4 * 5))]
        public int loopingSounds_5;
        [FieldOffset(0x2D30 + (0x4 * 6))]
        public int loopingSounds_6;
        [FieldOffset(0x2D30 + (0x4 * 7))]
        public int loopingSounds_7;
        [FieldOffset(0x2D30 + (0x4 * 8))]
        public int loopingSounds_8;
        [FieldOffset(0x2D54 + (0x4 * 0))]
        public RCPointer loopingSoundMobys_0;
        [FieldOffset(0x2D54 + (0x4 * 1))]
        public RCPointer loopingSoundMobys_1;
        [FieldOffset(0x2D54 + (0x4 * 2))]
        public RCPointer loopingSoundMobys_2;
        [FieldOffset(0x2D54 + (0x4 * 3))]
        public RCPointer loopingSoundMobys_3;
        [FieldOffset(0x2D54 + (0x4 * 4))]
        public RCPointer loopingSoundMobys_4;
        [FieldOffset(0x2D54 + (0x4 * 5))]
        public RCPointer loopingSoundMobys_5;
        [FieldOffset(0x2D54 + (0x4 * 6))]
        public RCPointer loopingSoundMobys_6;
        [FieldOffset(0x2D54 + (0x4 * 7))]
        public RCPointer loopingSoundMobys_7;
        [FieldOffset(0x2D54 + (0x4 * 8))]
        public RCPointer loopingSoundMobys_8;
        [FieldOffset(0x2D78 + (0x10 * 0))]
        public HeroSpecialIdleDef SpecialIdles_0;
        [FieldOffset(0x2D78 + (0x10 * 1))]
        public HeroSpecialIdleDef SpecialIdles_1;
        [FieldOffset(0x2D78 + (0x10 * 2))]
        public HeroSpecialIdleDef SpecialIdles_2;
        [FieldOffset(0x2D78 + (0x10 * 3))]
        public HeroSpecialIdleDef SpecialIdles_3;
        [FieldOffset(0x2DB8 + (0x4 * 0))]
        public int specialIdleHistory_0;
        [FieldOffset(0x2DB8 + (0x4 * 1))]
        public int specialIdleHistory_1;
        [FieldOffset(0x2DB8 + (0x4 * 2))]
        public int specialIdleHistory_2;
        [FieldOffset(0x2DB8 + (0x4 * 3))]
        public int specialIdleHistory_3;
        [FieldOffset(0x2dc8)]
        public int specialIdleHistoryIndex;
        [FieldOffset(0x2dcc)]
        public int specialIdleHistorySize;
        [FieldOffset(0x2dd0)]
        public int specialIdleID;
        [FieldOffset(0x2dd4)]
        public int firingAnim;
        [FieldOffset(0x2dd8)]
        public int firingGadget;
        [FieldOffset(0x2ddc)]
        public int desiredCam;
        [FieldOffset(0x2de0)]
        public ulong savedLights;
        [FieldOffset(0x2de8)]
        public int lightFxOn;
        [FieldOffset(0x2dec)]
        public RCPointer pHeadTargetMoby;
        [FieldOffset(0x2df0)]
        public RCPointer pSheepMoby;
        [FieldOffset(0x2df4)]
        public RCPointer pWhoHitMe;
        [FieldOffset(0x2df8)]
        public RCPointer pWhoSheepedMe;
        [FieldOffset(0x2dfc)]
        public RCPointer pAcidDamager;
        [FieldOffset(0x2e00)]
        public int acidDamagerGadgetId;
        [FieldOffset(0x2e04)]
        public bool useAcidDamagerGadgetId;
        [FieldOffset(0x2e05)]
        public bool sheepMeLongTime;
        [FieldOffset(0x2e08)]
        public float stickStrength;
        [FieldOffset(0x2e0c)]
        public float stickRawAngle;
        [FieldOffset(0x2e10)]
        public int targetModeDelay;
        [FieldOffset(0x2e14)]
        public float waterDepth;
        [FieldOffset(0x2e18)]
        public short edgePath;
        [FieldOffset(0x2e1c)]
        public float glideDescentRate;
        [FieldOffset(0x2e20)]
        public float hitPoints;
        [FieldOffset(0x2e24)]
        public float boltPickupRadiusXY;
        [FieldOffset(0x2e28)]
        public float boltPickupZthresh;
        [FieldOffset(0x2e2c)]
        public short noClank;
        [FieldOffset(0x2e30)]
        public float skidDecel;
        [FieldOffset(0x2e34)]
        public short mtxFxActive;
        [FieldOffset(0x2e38)]
        public float analogStickStrength;
        [FieldOffset(0x2e3c)]
        public short deathFallChannel;
        [FieldOffset(0x2e3e)]
        public short wallJumpAngLimiter;
        [FieldOffset(0x2e40)]
        public sbyte wallJumpHeightFactor;
        [FieldOffset(0x2e41)]
        public sbyte wallJumpChainCnt;
        [FieldOffset(0x2e44)]
        public float wallJumpDist;
        [FieldOffset(0x2e48)]
        public float moonJumpIdealHeight;
        [FieldOffset(0x2e4c)]
        public float moonJumpGravity;
        [FieldOffset(0x2e50)]
        public short sbytegeDoubleTapTimer;
        [FieldOffset(0x2e52)]
        public short sbytegeDownTimer;
        [FieldOffset(0x2e54)]
        public short sbytegeDelayTimer;
        [FieldOffset(0x2e56)]
        public short tweakersReset;
        [FieldOffset(0x2e58)]
        public int gadgetFilter;
        [FieldOffset(0x2e5c)]
        public int gadgetFilterSize;
        [FieldOffset(0x2e60)]
        public int targetGadgetFilterSize;
        [FieldOffset(0x2e64)]
        public float FPS_InterpSpeed;
        [FieldOffset(0x2e68)]
        public float FPS_InterpAccel;
        [FieldOffset(0x2e6c)]
        public int FPS_StartTime;
        [FieldOffset(0x2e70)]
        public sbyte FPSTapTimer;
        [FieldOffset(0x2e71)]
        public sbyte FPSTapCount;
        [FieldOffset(0x2e72)]
        public sbyte strafeTapTimer;
        [FieldOffset(0x2e73)]
        public sbyte strafeTapCount;
        [FieldOffset(0x2e74)]
        public sbyte lockedStrafeModeOn;
        [FieldOffset(0x2e78)]
        public uint lastDamagedMeOwnerUID;
        [FieldOffset(0x2e7c)]
        public int lastDamagedMeGadgetId;
        [FieldOffset(0x2e80)]
        public float lastDamagedMeHp;
        [FieldOffset(0x2e84)]
        public sbyte restoreCamAzDir;
        [FieldOffset(0x2e85)]
        public sbyte restoreCamRotSpd;
        [FieldOffset(0x2e88)]
        public int rAmb;
        [FieldOffset(0x2e8c)]
        public int gAmb;
        [FieldOffset(0x2e90)]
        public int bAmb;
        [FieldOffset(0x2e94)]
        public RCPointer lastWeaponTarget;
        [FieldOffset(0x2E98 + (0x4 * 0))]
        public RCPointer recentTargets_0;
        [FieldOffset(0x2E98 + (0x4 * 1))]
        public RCPointer recentTargets_1;
        [FieldOffset(0x2E98 + (0x4 * 2))]
        public RCPointer recentTargets_2;
        [FieldOffset(0x2E98 + (0x4 * 3))]
        public RCPointer recentTargets_3;
        [FieldOffset(0x2ea8)]
        public int lastWeaponTargetTime;
        [FieldOffset(0x2eac)]
        public tNW_GetHitMessage getHitMessage;
        [FieldOffset(0x2ebc)]
        public FlashVars flashVars;
        [FieldOffset(0x2ecc)]
        public sbyte isGetHitMsgPending;
        [FieldOffset(0x2ecd)]
        public sbyte lookAndCrouch;
        [FieldOffset(0x2ece)]
        public sbyte lookAndThrowWrench;
        [FieldOffset(0x2ecf)]
        public sbyte earlyThrowAbort;
        [FieldOffset(0x2ed0)]
        public sbyte lookAndGetHit;
        [FieldOffset(0x2ed1)]
        public sbyte lastDeathWasSuicide;
        [FieldOffset(0x2ed2)]
        public sbyte noWeaponSwitching;
        [FieldOffset(0x2ed3)]
        public sbyte noWrenchEquip;
        [FieldOffset(0x2ed4)]
        public sbyte jackpotMult;
        [FieldOffset(0x2ed5)]
        public sbyte rocketHitMe;
        [FieldOffset(0x2ed6)]
        public sbyte explode;
        [FieldOffset(0x2ed7)]
        public bool deathWasCalled;
        [FieldOffset(0x2ed8)]
        public int hudHealthTimer;
        [FieldOffset(0x2edc)]
        public bool pauseOn;
        [FieldOffset(0x2edd)]
        public sbyte pauseTimer;
        [FieldOffset(0x2ee0)]
        public int tauntOverrideTimer;
        [FieldOffset(0x2ee4)]
        public sbyte playerType;
        [FieldOffset(0x2ee8)]
        public RCPointer flagMoby;
        [FieldOffset(0x2eec)]
        public RCPointer playerConst;
        [FieldOffset(0x2ef0)]
        public RCPointer pMoby;
        [FieldOffset(0x2ef4)]
        public RCPointer pVehicle;
        [FieldOffset(0x2ef8)]
        public RCPointer pVehiclePending;
        [FieldOffset(0x2efc)]
        public RCPointer camera;
        [FieldOffset(0x2f00)]
        public RCPointer pPad;
        [FieldOffset(0x2f04)]
        public float cheatX;
        [FieldOffset(0x2f08)]
        public float cheatY;
        [FieldOffset(0x2f0c)]
        public float cheatZ;
        [FieldOffset(0x2f10)]
        public int mpIndex;
        [FieldOffset(0x2f14)]
        public int mpTeam;
        [FieldOffset(0x2f18)]
        public int vehicleState;
        [FieldOffset(0x2f1c)]
        public int vehicleStateTimer;
        [FieldOffset(0x2f20)]
        public int pointsLastKill;
        [FieldOffset(0x2f24)]
        public float maxHP;
        [FieldOffset(0x2f28)]
        public RCPointer pNetPlayer;
        //[FieldOffset(0x2f2c)]
        //public tNW_PlayerStateMessage newStateMessage;
        [FieldOffset(0x2f48)]
        public int timeOfReceivedStateMsg;
        [FieldOffset(0x2f4c)]
        public RCPointer lastVehicleMoby;
        [FieldOffset(0x2f50)]
        public int iLastVehicleOffTime;
        [FieldOffset(0x2f54)]
        public RCPointer pLastGrindRail;
        [FieldOffset(0x2f58)]
        public float closestCamDistSqr;
        [FieldOffset(0x2f5c)]
        public int bSphereVis;
        [FieldOffset(0x2f60)]
        public float m_shortTermLinkDirtyTime;
        [FieldOffset(0x2f64)]
        public float m_accumLinkDirtyTime;
        [FieldOffset(0x2f70)]
        public Vector4 initialPos;
        [FieldOffset(0x2f80)]
        public Vector4 initialRot;
        [FieldOffset(0x2f90)]
        public float stashedWallCheckHeight;
        [FieldOffset(0x2f94)]
        public float stashedWallCheckDist;
        [FieldOffset(0x2f98)]
        public float stashedWallDist;
        [FieldOffset(0x2f9c)]
        public int lookModeTimeout;
        [FieldOffset(0x2fa0)]
        public int lookModeDelay;
        [FieldOffset(0x2fa4)]
        public int baseExperience;
        [FieldOffset(0x2fa8)]
        public int Experience;
        [FieldOffset(0x2fac)]
        public int currentAcumXp;
        [FieldOffset(0x2fb0)]
        public float damageMultipler;
        [FieldOffset(0x2fb4)]
        public int armorLevel;
        [FieldOffset(0x2fb8)]
        public bool limitBreakFull;
        [FieldOffset(0x2fbc)]
        public float limitBreakPercentage;
        [FieldOffset(0x2fc0)]
        public float movementSpeedModifier;
        [FieldOffset(0x2fc4)]
        public sbyte grindRailWeaponLock;
        [FieldOffset(0x2fc5)]
        public sbyte currArmAnim;
        [FieldOffset(0x2fc6)]
        public short activePadFrame;
        [FieldOffset(0x2fc8)]
        public sbyte alreadyPlayedNoAmmoClick;
        [FieldOffset(0x2fc9)]
        public sbyte sceneInvis;
        [FieldOffset(0x2fca)]
        public sbyte ninjaCheatActive;
        [FieldOffset(0x2fcb)]
        public sbyte hadWind;
        [FieldOffset(0x2fcc)]
        public RCPointer pWrenchReplacement;
        [FieldOffset(0x2fd0)]
        public int startLimitBreakDiff;
        [FieldOffset(0x2fe0)]
        public int frameSentStick;
        [FieldOffset(0x2fe4)]
        public float damageDone;
        [FieldOffset(0x2fe8)]
        public short numKills;
        [FieldOffset(0x2fea)]
        public sbyte slot;
        [FieldOffset(0x2fec)]
        public PadStream padStream;
        [FieldOffset(0x31cc)]
        public byte startGameButtonOffFrames;
        [FieldOffset(0x31cd)]
        public byte curPadMsgSequenceNum;
        [FieldOffset(0x31ce)]
        public sbyte curPadMsgFrame;
        [FieldOffset(0x31cf)]
        public sbyte framesUntilStateRot;
        [FieldOffset(0x31d0)]
        public sbyte mapTimer;
        [FieldOffset(0x31d2)]
        public short lastMineId;
        [FieldOffset(0x31d4)]
        public HeroCommand command2;
        [FieldOffset(0x31e0)]
        public RCPointer pRespawnDest;

        public int GetGadgetCount() => 6;
        public Gadget? GetGadget(int i)
        {
            switch (i)
            {
                case 0: return gadgets_0;
                case 1: return gadgets_1;
                case 2: return gadgets_2;
                case 3: return gadgets_3;
                case 4: return gadgets_4;
                case 5: return gadgets_5;
                default: return null;
            }
        }
    };

    #endregion

    #region Pad

    [Flags]
    public enum PadButtons
    {
        None = 0,
        Select = 1 << 0,
        L3 = 1 << 1,
        R3 = 1 << 2,
        Start = 1 << 3,
        Up = 1 << 4,
        Right = 1 << 5,
        Down = 1 << 6,
        Left = 1 << 7,
        L2 = 1 << 8,
        R2 = 1 << 9,
        L1 = 1 << 10,
        R1 = 1 << 11,
        Triangle = 1 << 12,
        Circle = 1 << 13,
        Cross = 1 << 14,
        Square = 1 << 15,
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct PAD
    {
        [FieldOffset(((0x0) + (0x80 * 0)) + (0x8 * 0))]
        public ulong pad_buf_0_0;
        [FieldOffset(((0x0) + (0x80 * 0)) + (0x8 * 1))]
        public ulong pad_buf_0_1;
        [FieldOffset(((0x0) + (0x80 * 0)) + (0x8 * 2))]
        public ulong pad_buf_0_2;
        [FieldOffset(((0x0) + (0x80 * 0)) + (0x8 * 3))]
        public ulong pad_buf_0_3;
        [FieldOffset(((0x0) + (0x80 * 0)) + (0x8 * 4))]
        public ulong pad_buf_0_4;
        [FieldOffset(((0x0) + (0x80 * 0)) + (0x8 * 5))]
        public ulong pad_buf_0_5;
        [FieldOffset(((0x0) + (0x80 * 0)) + (0x8 * 6))]
        public ulong pad_buf_0_6;
        [FieldOffset(((0x0) + (0x80 * 0)) + (0x8 * 7))]
        public ulong pad_buf_0_7;
        [FieldOffset(((0x0) + (0x80 * 0)) + (0x8 * 8))]
        public ulong pad_buf_0_8;
        [FieldOffset(((0x0) + (0x80 * 0)) + (0x8 * 9))]
        public ulong pad_buf_0_9;
        [FieldOffset(((0x0) + (0x80 * 0)) + (0x8 * 10))]
        public ulong pad_buf_0_10;
        [FieldOffset(((0x0) + (0x80 * 0)) + (0x8 * 11))]
        public ulong pad_buf_0_11;
        [FieldOffset(((0x0) + (0x80 * 0)) + (0x8 * 12))]
        public ulong pad_buf_0_12;
        [FieldOffset(((0x0) + (0x80 * 0)) + (0x8 * 13))]
        public ulong pad_buf_0_13;
        [FieldOffset(((0x0) + (0x80 * 0)) + (0x8 * 14))]
        public ulong pad_buf_0_14;
        [FieldOffset(((0x0) + (0x80 * 0)) + (0x8 * 15))]
        public ulong pad_buf_0_15;
        [FieldOffset(((0x0) + (0x80 * 1)) + (0x8 * 0))]
        public ulong pad_buf_1_0;
        [FieldOffset(((0x0) + (0x80 * 1)) + (0x8 * 1))]
        public ulong pad_buf_1_1;
        [FieldOffset(((0x0) + (0x80 * 1)) + (0x8 * 2))]
        public ulong pad_buf_1_2;
        [FieldOffset(((0x0) + (0x80 * 1)) + (0x8 * 3))]
        public ulong pad_buf_1_3;
        [FieldOffset(((0x0) + (0x80 * 1)) + (0x8 * 4))]
        public ulong pad_buf_1_4;
        [FieldOffset(((0x0) + (0x80 * 1)) + (0x8 * 5))]
        public ulong pad_buf_1_5;
        [FieldOffset(((0x0) + (0x80 * 1)) + (0x8 * 6))]
        public ulong pad_buf_1_6;
        [FieldOffset(((0x0) + (0x80 * 1)) + (0x8 * 7))]
        public ulong pad_buf_1_7;
        [FieldOffset(((0x0) + (0x80 * 1)) + (0x8 * 8))]
        public ulong pad_buf_1_8;
        [FieldOffset(((0x0) + (0x80 * 1)) + (0x8 * 9))]
        public ulong pad_buf_1_9;
        [FieldOffset(((0x0) + (0x80 * 1)) + (0x8 * 10))]
        public ulong pad_buf_1_10;
        [FieldOffset(((0x0) + (0x80 * 1)) + (0x8 * 11))]
        public ulong pad_buf_1_11;
        [FieldOffset(((0x0) + (0x80 * 1)) + (0x8 * 12))]
        public ulong pad_buf_1_12;
        [FieldOffset(((0x0) + (0x80 * 1)) + (0x8 * 13))]
        public ulong pad_buf_1_13;
        [FieldOffset(((0x0) + (0x80 * 1)) + (0x8 * 14))]
        public ulong pad_buf_1_14;
        [FieldOffset(((0x0) + (0x80 * 1)) + (0x8 * 15))]
        public ulong pad_buf_1_15;
        [FieldOffset((0x100) + (0x4 * 0))]
        public float analog_0;
        [FieldOffset((0x100) + (0x4 * 1))]
        public float analog_1;
        [FieldOffset((0x100) + (0x4 * 2))]
        public float analog_2;
        [FieldOffset((0x100) + (0x4 * 3))]
        public float analog_3;
        [FieldOffset((0x100) + (0x4 * 4))]
        public float analog_4;
        [FieldOffset((0x100) + (0x4 * 5))]
        public float analog_5;
        [FieldOffset((0x100) + (0x4 * 6))]
        public float analog_6;
        [FieldOffset((0x100) + (0x4 * 7))]
        public float analog_7;
        [FieldOffset((0x100) + (0x4 * 8))]
        public float analog_8;
        [FieldOffset((0x100) + (0x4 * 9))]
        public float analog_9;
        [FieldOffset((0x100) + (0x4 * 10))]
        public float analog_10;
        [FieldOffset((0x100) + (0x4 * 11))]
        public float analog_11;
        [FieldOffset((0x100) + (0x4 * 12))]
        public float analog_12;
        [FieldOffset((0x100) + (0x4 * 13))]
        public float analog_13;
        [FieldOffset((0x100) + (0x4 * 14))]
        public float analog_14;
        [FieldOffset((0x100) + (0x4 * 15))]
        public float analog_15;
        [FieldOffset((0x140) + (0x4 * 0))]
        public float hudAnalog_0;
        [FieldOffset((0x140) + (0x4 * 1))]
        public float hudAnalog_1;
        [FieldOffset((0x140) + (0x4 * 2))]
        public float hudAnalog_2;
        [FieldOffset((0x140) + (0x4 * 3))]
        public float hudAnalog_3;
        [FieldOffset((0x140) + (0x4 * 4))]
        public float hudAnalog_4;
        [FieldOffset((0x140) + (0x4 * 5))]
        public float hudAnalog_5;
        [FieldOffset((0x140) + (0x4 * 6))]
        public float hudAnalog_6;
        [FieldOffset((0x140) + (0x4 * 7))]
        public float hudAnalog_7;
        [FieldOffset((0x140) + (0x4 * 8))]
        public float hudAnalog_8;
        [FieldOffset((0x140) + (0x4 * 9))]
        public float hudAnalog_9;
        [FieldOffset((0x140) + (0x4 * 10))]
        public float hudAnalog_10;
        [FieldOffset((0x140) + (0x4 * 11))]
        public float hudAnalog_11;
        [FieldOffset((0x140) + (0x4 * 12))]
        public float hudAnalog_12;
        [FieldOffset((0x140) + (0x4 * 13))]
        public float hudAnalog_13;
        [FieldOffset((0x140) + (0x4 * 14))]
        public float hudAnalog_14;
        [FieldOffset((0x140) + (0x4 * 15))]
        public float hudAnalog_15;
        [FieldOffset((0x180) + (0x1 * 0))]
        public byte profile_0;
        [FieldOffset((0x180) + (0x1 * 1))]
        public byte profile_1;
        [FieldOffset((0x180) + (0x1 * 2))]
        public byte profile_2;
        [FieldOffset((0x180) + (0x1 * 3))]
        public byte profile_3;
        [FieldOffset((0x184) + (0x1 * 0))]
        public byte vib_profile_0;
        [FieldOffset((0x184) + (0x1 * 1))]
        public byte vib_profile_1;
        [FieldOffset((0x184) + (0x1 * 2))]
        public byte vib_profile_2;
        [FieldOffset((0x184) + (0x1 * 3))]
        public byte vib_profile_3;
        [FieldOffset((0x188) + (0x1 * 0))]
        public byte act_direct_0;
        [FieldOffset((0x188) + (0x1 * 1))]
        public byte act_direct_1;
        [FieldOffset((0x188) + (0x1 * 2))]
        public byte act_direct_2;
        [FieldOffset((0x188) + (0x1 * 3))]
        public byte act_direct_3;
        [FieldOffset(0x18c)]
        public short invalidTimer;
        [FieldOffset(0x18e)]
        public short ringIndex;
        [FieldOffset(0x190)]
        public int ringValidSize;
        [FieldOffset(0x194)]
        public int socket;
        [FieldOffset(0x198)]
        public int phase;
        [FieldOffset(0x19c)]
        public int state;
        [FieldOffset(0x1a0)]
        public int bits;
        [FieldOffset(0x1a4)]
        public int bitsOn;
        [FieldOffset(0x1a8)]
        public int bitsOff;
        [FieldOffset(0x1ac)]
        public int bitsPrev;
        [FieldOffset(0x1b0)]
        public int digitalBits;
        [FieldOffset(0x1b4)]
        public int digitalBitsOn;
        [FieldOffset(0x1b8)]
        public int digitalBitsOff;
        [FieldOffset(0x1bc)]
        public int digitalBitsPrev;
        [FieldOffset(0x1c0)]
        public int hudBits;
        [FieldOffset(0x1c4)]
        public int hudBitsOn;
        [FieldOffset(0x1c8)]
        public int hudBitsOff;
        [FieldOffset(0x1cc)]
        public int hudDivert;
        [FieldOffset(0x1d0)]
        public int handsOff;
        [FieldOffset(0x1d4)]
        public int handsOffStick;
        [FieldOffset(0x1d8)]
        public int useAnalog;
        [FieldOffset(0x1dc)]
        public int term_id;
        [FieldOffset((0x1e0) + (0x4 * 0))]
        public int bitsOnRing_0;
        [FieldOffset((0x1e0) + (0x4 * 1))]
        public int bitsOnRing_1;
        [FieldOffset((0x1e0) + (0x4 * 2))]
        public int bitsOnRing_2;
        [FieldOffset((0x1e0) + (0x4 * 3))]
        public int bitsOnRing_3;
        [FieldOffset((0x1e0) + (0x4 * 4))]
        public int bitsOnRing_4;
        [FieldOffset((0x1e0) + (0x4 * 5))]
        public int bitsOnRing_5;
        [FieldOffset((0x1e0) + (0x4 * 6))]
        public int bitsOnRing_6;
        [FieldOffset((0x1e0) + (0x4 * 7))]
        public int bitsOnRing_7;
        [FieldOffset((0x1e0) + (0x4 * 8))]
        public int bitsOnRing_8;
        [FieldOffset((0x1e0) + (0x4 * 9))]
        public int bitsOnRing_9;
        [FieldOffset((0x1e0) + (0x4 * 10))]
        public int bitsOnRing_10;
        [FieldOffset((0x1e0) + (0x4 * 11))]
        public int bitsOnRing_11;
        [FieldOffset((0x1e0) + (0x4 * 12))]
        public int bitsOnRing_12;
        [FieldOffset((0x1e0) + (0x4 * 13))]
        public int bitsOnRing_13;
        [FieldOffset((0x1e0) + (0x4 * 14))]
        public int bitsOnRing_14;
        [FieldOffset((0x1e0) + (0x4 * 15))]
        public int bitsOnRing_15;
        [FieldOffset((0x1e0) + (0x4 * 16))]
        public int bitsOnRing_16;
        [FieldOffset((0x1e0) + (0x4 * 17))]
        public int bitsOnRing_17;
        [FieldOffset((0x1e0) + (0x4 * 18))]
        public int bitsOnRing_18;
        [FieldOffset((0x1e0) + (0x4 * 19))]
        public int bitsOnRing_19;
        [FieldOffset((0x1e0) + (0x4 * 20))]
        public int bitsOnRing_20;
        [FieldOffset((0x1e0) + (0x4 * 21))]
        public int bitsOnRing_21;
        [FieldOffset((0x1e0) + (0x4 * 22))]
        public int bitsOnRing_22;
        [FieldOffset((0x1e0) + (0x4 * 23))]
        public int bitsOnRing_23;
        [FieldOffset((0x1e0) + (0x4 * 24))]
        public int bitsOnRing_24;
        [FieldOffset((0x1e0) + (0x4 * 25))]
        public int bitsOnRing_25;
        [FieldOffset((0x1e0) + (0x4 * 26))]
        public int bitsOnRing_26;
        [FieldOffset((0x1e0) + (0x4 * 27))]
        public int bitsOnRing_27;
        [FieldOffset((0x1e0) + (0x4 * 28))]
        public int bitsOnRing_28;
        [FieldOffset((0x1e0) + (0x4 * 29))]
        public int bitsOnRing_29;
        [FieldOffset((0x258) + (0x4 * 0))]
        public float analogAngRing_0;
        [FieldOffset((0x258) + (0x4 * 1))]
        public float analogAngRing_1;
        [FieldOffset((0x258) + (0x4 * 2))]
        public float analogAngRing_2;
        [FieldOffset((0x258) + (0x4 * 3))]
        public float analogAngRing_3;
        [FieldOffset((0x258) + (0x4 * 4))]
        public float analogAngRing_4;
        [FieldOffset((0x258) + (0x4 * 5))]
        public float analogAngRing_5;
        [FieldOffset((0x258) + (0x4 * 6))]
        public float analogAngRing_6;
        [FieldOffset((0x258) + (0x4 * 7))]
        public float analogAngRing_7;
        [FieldOffset((0x258) + (0x4 * 8))]
        public float analogAngRing_8;
        [FieldOffset((0x258) + (0x4 * 9))]
        public float analogAngRing_9;
        [FieldOffset((0x258) + (0x4 * 10))]
        public float analogAngRing_10;
        [FieldOffset((0x258) + (0x4 * 11))]
        public float analogAngRing_11;
        [FieldOffset((0x258) + (0x4 * 12))]
        public float analogAngRing_12;
        [FieldOffset((0x258) + (0x4 * 13))]
        public float analogAngRing_13;
        [FieldOffset((0x258) + (0x4 * 14))]
        public float analogAngRing_14;
        [FieldOffset((0x258) + (0x4 * 15))]
        public float analogAngRing_15;
        [FieldOffset((0x258) + (0x4 * 16))]
        public float analogAngRing_16;
        [FieldOffset((0x258) + (0x4 * 17))]
        public float analogAngRing_17;
        [FieldOffset((0x258) + (0x4 * 18))]
        public float analogAngRing_18;
        [FieldOffset((0x258) + (0x4 * 19))]
        public float analogAngRing_19;
        [FieldOffset((0x258) + (0x4 * 20))]
        public float analogAngRing_20;
        [FieldOffset((0x258) + (0x4 * 21))]
        public float analogAngRing_21;
        [FieldOffset((0x258) + (0x4 * 22))]
        public float analogAngRing_22;
        [FieldOffset((0x258) + (0x4 * 23))]
        public float analogAngRing_23;
        [FieldOffset((0x258) + (0x4 * 24))]
        public float analogAngRing_24;
        [FieldOffset((0x258) + (0x4 * 25))]
        public float analogAngRing_25;
        [FieldOffset((0x258) + (0x4 * 26))]
        public float analogAngRing_26;
        [FieldOffset((0x258) + (0x4 * 27))]
        public float analogAngRing_27;
        [FieldOffset((0x258) + (0x4 * 28))]
        public float analogAngRing_28;
        [FieldOffset((0x258) + (0x4 * 29))]
        public float analogAngRing_29;
        [FieldOffset((0x2d0) + (0x4 * 0))]
        public float analogMagRing_0;
        [FieldOffset((0x2d0) + (0x4 * 1))]
        public float analogMagRing_1;
        [FieldOffset((0x2d0) + (0x4 * 2))]
        public float analogMagRing_2;
        [FieldOffset((0x2d0) + (0x4 * 3))]
        public float analogMagRing_3;
        [FieldOffset((0x2d0) + (0x4 * 4))]
        public float analogMagRing_4;
        [FieldOffset((0x2d0) + (0x4 * 5))]
        public float analogMagRing_5;
        [FieldOffset((0x2d0) + (0x4 * 6))]
        public float analogMagRing_6;
        [FieldOffset((0x2d0) + (0x4 * 7))]
        public float analogMagRing_7;
        [FieldOffset((0x2d0) + (0x4 * 8))]
        public float analogMagRing_8;
        [FieldOffset((0x2d0) + (0x4 * 9))]
        public float analogMagRing_9;
        [FieldOffset((0x2d0) + (0x4 * 10))]
        public float analogMagRing_10;
        [FieldOffset((0x2d0) + (0x4 * 11))]
        public float analogMagRing_11;
        [FieldOffset((0x2d0) + (0x4 * 12))]
        public float analogMagRing_12;
        [FieldOffset((0x2d0) + (0x4 * 13))]
        public float analogMagRing_13;
        [FieldOffset((0x2d0) + (0x4 * 14))]
        public float analogMagRing_14;
        [FieldOffset((0x2d0) + (0x4 * 15))]
        public float analogMagRing_15;
        [FieldOffset((0x2d0) + (0x4 * 16))]
        public float analogMagRing_16;
        [FieldOffset((0x2d0) + (0x4 * 17))]
        public float analogMagRing_17;
        [FieldOffset((0x2d0) + (0x4 * 18))]
        public float analogMagRing_18;
        [FieldOffset((0x2d0) + (0x4 * 19))]
        public float analogMagRing_19;
        [FieldOffset((0x2d0) + (0x4 * 20))]
        public float analogMagRing_20;
        [FieldOffset((0x2d0) + (0x4 * 21))]
        public float analogMagRing_21;
        [FieldOffset((0x2d0) + (0x4 * 22))]
        public float analogMagRing_22;
        [FieldOffset((0x2d0) + (0x4 * 23))]
        public float analogMagRing_23;
        [FieldOffset((0x2d0) + (0x4 * 24))]
        public float analogMagRing_24;
        [FieldOffset((0x2d0) + (0x4 * 25))]
        public float analogMagRing_25;
        [FieldOffset((0x2d0) + (0x4 * 26))]
        public float analogMagRing_26;
        [FieldOffset((0x2d0) + (0x4 * 27))]
        public float analogMagRing_27;
        [FieldOffset((0x2d0) + (0x4 * 28))]
        public float analogMagRing_28;
        [FieldOffset((0x2d0) + (0x4 * 29))]
        public float analogMagRing_29;
        [FieldOffset(0x348)]
        public int unmaskedBits;
        [FieldOffset(0x34c)]
        public int lagIndex;
        [FieldOffset(0x350)]
        public int lagValidSize;
        [FieldOffset((0x354) + (0x4 * 0))]
        public int bits_Lagged_0;
        [FieldOffset((0x354) + (0x4 * 1))]
        public int bits_Lagged_1;
        [FieldOffset((0x354) + (0x4 * 2))]
        public int bits_Lagged_2;
        [FieldOffset((0x354) + (0x4 * 3))]
        public int bits_Lagged_3;
        [FieldOffset((0x354) + (0x4 * 4))]
        public int bits_Lagged_4;
        [FieldOffset((0x354) + (0x4 * 5))]
        public int bits_Lagged_5;
        [FieldOffset((0x354) + (0x4 * 6))]
        public int bits_Lagged_6;
        [FieldOffset((0x370) + (0x4 * 0))]
        public int digitalBits_Lagged_0;
        [FieldOffset((0x370) + (0x4 * 1))]
        public int digitalBits_Lagged_1;
        [FieldOffset((0x370) + (0x4 * 2))]
        public int digitalBits_Lagged_2;
        [FieldOffset((0x370) + (0x4 * 3))]
        public int digitalBits_Lagged_3;
        [FieldOffset((0x370) + (0x4 * 4))]
        public int digitalBits_Lagged_4;
        [FieldOffset((0x370) + (0x4 * 5))]
        public int digitalBits_Lagged_5;
        [FieldOffset((0x370) + (0x4 * 6))]
        public int digitalBits_Lagged_6;
        [FieldOffset(((0x38c) + (0x40 * 0)) + (0x4 * 0))]
        public float analog_Lagged_0_0;
        [FieldOffset(((0x38c) + (0x40 * 0)) + (0x4 * 1))]
        public float analog_Lagged_0_1;
        [FieldOffset(((0x38c) + (0x40 * 0)) + (0x4 * 2))]
        public float analog_Lagged_0_2;
        [FieldOffset(((0x38c) + (0x40 * 0)) + (0x4 * 3))]
        public float analog_Lagged_0_3;
        [FieldOffset(((0x38c) + (0x40 * 0)) + (0x4 * 4))]
        public float analog_Lagged_0_4;
        [FieldOffset(((0x38c) + (0x40 * 0)) + (0x4 * 5))]
        public float analog_Lagged_0_5;
        [FieldOffset(((0x38c) + (0x40 * 0)) + (0x4 * 6))]
        public float analog_Lagged_0_6;
        [FieldOffset(((0x38c) + (0x40 * 0)) + (0x4 * 7))]
        public float analog_Lagged_0_7;
        [FieldOffset(((0x38c) + (0x40 * 0)) + (0x4 * 8))]
        public float analog_Lagged_0_8;
        [FieldOffset(((0x38c) + (0x40 * 0)) + (0x4 * 9))]
        public float analog_Lagged_0_9;
        [FieldOffset(((0x38c) + (0x40 * 0)) + (0x4 * 10))]
        public float analog_Lagged_0_10;
        [FieldOffset(((0x38c) + (0x40 * 0)) + (0x4 * 11))]
        public float analog_Lagged_0_11;
        [FieldOffset(((0x38c) + (0x40 * 0)) + (0x4 * 12))]
        public float analog_Lagged_0_12;
        [FieldOffset(((0x38c) + (0x40 * 0)) + (0x4 * 13))]
        public float analog_Lagged_0_13;
        [FieldOffset(((0x38c) + (0x40 * 0)) + (0x4 * 14))]
        public float analog_Lagged_0_14;
        [FieldOffset(((0x38c) + (0x40 * 0)) + (0x4 * 15))]
        public float analog_Lagged_0_15;
        [FieldOffset(((0x38c) + (0x40 * 1)) + (0x4 * 0))]
        public float analog_Lagged_1_0;
        [FieldOffset(((0x38c) + (0x40 * 1)) + (0x4 * 1))]
        public float analog_Lagged_1_1;
        [FieldOffset(((0x38c) + (0x40 * 1)) + (0x4 * 2))]
        public float analog_Lagged_1_2;
        [FieldOffset(((0x38c) + (0x40 * 1)) + (0x4 * 3))]
        public float analog_Lagged_1_3;
        [FieldOffset(((0x38c) + (0x40 * 1)) + (0x4 * 4))]
        public float analog_Lagged_1_4;
        [FieldOffset(((0x38c) + (0x40 * 1)) + (0x4 * 5))]
        public float analog_Lagged_1_5;
        [FieldOffset(((0x38c) + (0x40 * 1)) + (0x4 * 6))]
        public float analog_Lagged_1_6;
        [FieldOffset(((0x38c) + (0x40 * 1)) + (0x4 * 7))]
        public float analog_Lagged_1_7;
        [FieldOffset(((0x38c) + (0x40 * 1)) + (0x4 * 8))]
        public float analog_Lagged_1_8;
        [FieldOffset(((0x38c) + (0x40 * 1)) + (0x4 * 9))]
        public float analog_Lagged_1_9;
        [FieldOffset(((0x38c) + (0x40 * 1)) + (0x4 * 10))]
        public float analog_Lagged_1_10;
        [FieldOffset(((0x38c) + (0x40 * 1)) + (0x4 * 11))]
        public float analog_Lagged_1_11;
        [FieldOffset(((0x38c) + (0x40 * 1)) + (0x4 * 12))]
        public float analog_Lagged_1_12;
        [FieldOffset(((0x38c) + (0x40 * 1)) + (0x4 * 13))]
        public float analog_Lagged_1_13;
        [FieldOffset(((0x38c) + (0x40 * 1)) + (0x4 * 14))]
        public float analog_Lagged_1_14;
        [FieldOffset(((0x38c) + (0x40 * 1)) + (0x4 * 15))]
        public float analog_Lagged_1_15;
        [FieldOffset(((0x38c) + (0x40 * 2)) + (0x4 * 0))]
        public float analog_Lagged_2_0;
        [FieldOffset(((0x38c) + (0x40 * 2)) + (0x4 * 1))]
        public float analog_Lagged_2_1;
        [FieldOffset(((0x38c) + (0x40 * 2)) + (0x4 * 2))]
        public float analog_Lagged_2_2;
        [FieldOffset(((0x38c) + (0x40 * 2)) + (0x4 * 3))]
        public float analog_Lagged_2_3;
        [FieldOffset(((0x38c) + (0x40 * 2)) + (0x4 * 4))]
        public float analog_Lagged_2_4;
        [FieldOffset(((0x38c) + (0x40 * 2)) + (0x4 * 5))]
        public float analog_Lagged_2_5;
        [FieldOffset(((0x38c) + (0x40 * 2)) + (0x4 * 6))]
        public float analog_Lagged_2_6;
        [FieldOffset(((0x38c) + (0x40 * 2)) + (0x4 * 7))]
        public float analog_Lagged_2_7;
        [FieldOffset(((0x38c) + (0x40 * 2)) + (0x4 * 8))]
        public float analog_Lagged_2_8;
        [FieldOffset(((0x38c) + (0x40 * 2)) + (0x4 * 9))]
        public float analog_Lagged_2_9;
        [FieldOffset(((0x38c) + (0x40 * 2)) + (0x4 * 10))]
        public float analog_Lagged_2_10;
        [FieldOffset(((0x38c) + (0x40 * 2)) + (0x4 * 11))]
        public float analog_Lagged_2_11;
        [FieldOffset(((0x38c) + (0x40 * 2)) + (0x4 * 12))]
        public float analog_Lagged_2_12;
        [FieldOffset(((0x38c) + (0x40 * 2)) + (0x4 * 13))]
        public float analog_Lagged_2_13;
        [FieldOffset(((0x38c) + (0x40 * 2)) + (0x4 * 14))]
        public float analog_Lagged_2_14;
        [FieldOffset(((0x38c) + (0x40 * 2)) + (0x4 * 15))]
        public float analog_Lagged_2_15;
        [FieldOffset(((0x38c) + (0x40 * 3)) + (0x4 * 0))]
        public float analog_Lagged_3_0;
        [FieldOffset(((0x38c) + (0x40 * 3)) + (0x4 * 1))]
        public float analog_Lagged_3_1;
        [FieldOffset(((0x38c) + (0x40 * 3)) + (0x4 * 2))]
        public float analog_Lagged_3_2;
        [FieldOffset(((0x38c) + (0x40 * 3)) + (0x4 * 3))]
        public float analog_Lagged_3_3;
        [FieldOffset(((0x38c) + (0x40 * 3)) + (0x4 * 4))]
        public float analog_Lagged_3_4;
        [FieldOffset(((0x38c) + (0x40 * 3)) + (0x4 * 5))]
        public float analog_Lagged_3_5;
        [FieldOffset(((0x38c) + (0x40 * 3)) + (0x4 * 6))]
        public float analog_Lagged_3_6;
        [FieldOffset(((0x38c) + (0x40 * 3)) + (0x4 * 7))]
        public float analog_Lagged_3_7;
        [FieldOffset(((0x38c) + (0x40 * 3)) + (0x4 * 8))]
        public float analog_Lagged_3_8;
        [FieldOffset(((0x38c) + (0x40 * 3)) + (0x4 * 9))]
        public float analog_Lagged_3_9;
        [FieldOffset(((0x38c) + (0x40 * 3)) + (0x4 * 10))]
        public float analog_Lagged_3_10;
        [FieldOffset(((0x38c) + (0x40 * 3)) + (0x4 * 11))]
        public float analog_Lagged_3_11;
        [FieldOffset(((0x38c) + (0x40 * 3)) + (0x4 * 12))]
        public float analog_Lagged_3_12;
        [FieldOffset(((0x38c) + (0x40 * 3)) + (0x4 * 13))]
        public float analog_Lagged_3_13;
        [FieldOffset(((0x38c) + (0x40 * 3)) + (0x4 * 14))]
        public float analog_Lagged_3_14;
        [FieldOffset(((0x38c) + (0x40 * 3)) + (0x4 * 15))]
        public float analog_Lagged_3_15;
        [FieldOffset(((0x38c) + (0x40 * 4)) + (0x4 * 0))]
        public float analog_Lagged_4_0;
        [FieldOffset(((0x38c) + (0x40 * 4)) + (0x4 * 1))]
        public float analog_Lagged_4_1;
        [FieldOffset(((0x38c) + (0x40 * 4)) + (0x4 * 2))]
        public float analog_Lagged_4_2;
        [FieldOffset(((0x38c) + (0x40 * 4)) + (0x4 * 3))]
        public float analog_Lagged_4_3;
        [FieldOffset(((0x38c) + (0x40 * 4)) + (0x4 * 4))]
        public float analog_Lagged_4_4;
        [FieldOffset(((0x38c) + (0x40 * 4)) + (0x4 * 5))]
        public float analog_Lagged_4_5;
        [FieldOffset(((0x38c) + (0x40 * 4)) + (0x4 * 6))]
        public float analog_Lagged_4_6;
        [FieldOffset(((0x38c) + (0x40 * 4)) + (0x4 * 7))]
        public float analog_Lagged_4_7;
        [FieldOffset(((0x38c) + (0x40 * 4)) + (0x4 * 8))]
        public float analog_Lagged_4_8;
        [FieldOffset(((0x38c) + (0x40 * 4)) + (0x4 * 9))]
        public float analog_Lagged_4_9;
        [FieldOffset(((0x38c) + (0x40 * 4)) + (0x4 * 10))]
        public float analog_Lagged_4_10;
        [FieldOffset(((0x38c) + (0x40 * 4)) + (0x4 * 11))]
        public float analog_Lagged_4_11;
        [FieldOffset(((0x38c) + (0x40 * 4)) + (0x4 * 12))]
        public float analog_Lagged_4_12;
        [FieldOffset(((0x38c) + (0x40 * 4)) + (0x4 * 13))]
        public float analog_Lagged_4_13;
        [FieldOffset(((0x38c) + (0x40 * 4)) + (0x4 * 14))]
        public float analog_Lagged_4_14;
        [FieldOffset(((0x38c) + (0x40 * 4)) + (0x4 * 15))]
        public float analog_Lagged_4_15;
        [FieldOffset(((0x38c) + (0x40 * 5)) + (0x4 * 0))]
        public float analog_Lagged_5_0;
        [FieldOffset(((0x38c) + (0x40 * 5)) + (0x4 * 1))]
        public float analog_Lagged_5_1;
        [FieldOffset(((0x38c) + (0x40 * 5)) + (0x4 * 2))]
        public float analog_Lagged_5_2;
        [FieldOffset(((0x38c) + (0x40 * 5)) + (0x4 * 3))]
        public float analog_Lagged_5_3;
        [FieldOffset(((0x38c) + (0x40 * 5)) + (0x4 * 4))]
        public float analog_Lagged_5_4;
        [FieldOffset(((0x38c) + (0x40 * 5)) + (0x4 * 5))]
        public float analog_Lagged_5_5;
        [FieldOffset(((0x38c) + (0x40 * 5)) + (0x4 * 6))]
        public float analog_Lagged_5_6;
        [FieldOffset(((0x38c) + (0x40 * 5)) + (0x4 * 7))]
        public float analog_Lagged_5_7;
        [FieldOffset(((0x38c) + (0x40 * 5)) + (0x4 * 8))]
        public float analog_Lagged_5_8;
        [FieldOffset(((0x38c) + (0x40 * 5)) + (0x4 * 9))]
        public float analog_Lagged_5_9;
        [FieldOffset(((0x38c) + (0x40 * 5)) + (0x4 * 10))]
        public float analog_Lagged_5_10;
        [FieldOffset(((0x38c) + (0x40 * 5)) + (0x4 * 11))]
        public float analog_Lagged_5_11;
        [FieldOffset(((0x38c) + (0x40 * 5)) + (0x4 * 12))]
        public float analog_Lagged_5_12;
        [FieldOffset(((0x38c) + (0x40 * 5)) + (0x4 * 13))]
        public float analog_Lagged_5_13;
        [FieldOffset(((0x38c) + (0x40 * 5)) + (0x4 * 14))]
        public float analog_Lagged_5_14;
        [FieldOffset(((0x38c) + (0x40 * 5)) + (0x4 * 15))]
        public float analog_Lagged_5_15;
        [FieldOffset(((0x38c) + (0x40 * 6)) + (0x4 * 0))]
        public float analog_Lagged_6_0;
        [FieldOffset(((0x38c) + (0x40 * 6)) + (0x4 * 1))]
        public float analog_Lagged_6_1;
        [FieldOffset(((0x38c) + (0x40 * 6)) + (0x4 * 2))]
        public float analog_Lagged_6_2;
        [FieldOffset(((0x38c) + (0x40 * 6)) + (0x4 * 3))]
        public float analog_Lagged_6_3;
        [FieldOffset(((0x38c) + (0x40 * 6)) + (0x4 * 4))]
        public float analog_Lagged_6_4;
        [FieldOffset(((0x38c) + (0x40 * 6)) + (0x4 * 5))]
        public float analog_Lagged_6_5;
        [FieldOffset(((0x38c) + (0x40 * 6)) + (0x4 * 6))]
        public float analog_Lagged_6_6;
        [FieldOffset(((0x38c) + (0x40 * 6)) + (0x4 * 7))]
        public float analog_Lagged_6_7;
        [FieldOffset(((0x38c) + (0x40 * 6)) + (0x4 * 8))]
        public float analog_Lagged_6_8;
        [FieldOffset(((0x38c) + (0x40 * 6)) + (0x4 * 9))]
        public float analog_Lagged_6_9;
        [FieldOffset(((0x38c) + (0x40 * 6)) + (0x4 * 10))]
        public float analog_Lagged_6_10;
        [FieldOffset(((0x38c) + (0x40 * 6)) + (0x4 * 11))]
        public float analog_Lagged_6_11;
        [FieldOffset(((0x38c) + (0x40 * 6)) + (0x4 * 12))]
        public float analog_Lagged_6_12;
        [FieldOffset(((0x38c) + (0x40 * 6)) + (0x4 * 13))]
        public float analog_Lagged_6_13;
        [FieldOffset(((0x38c) + (0x40 * 6)) + (0x4 * 14))]
        public float analog_Lagged_6_14;
        [FieldOffset(((0x38c) + (0x40 * 6)) + (0x4 * 15))]
        public float analog_Lagged_6_15;
        [FieldOffset(0x54c)]
        public byte port;
        [FieldOffset(0x54d)]
        public byte repeatDelay;
        [FieldOffset(0x54e)]
        public byte repeatSpeed;
        [FieldOffset(0x54f)]
        public byte repeatCounter;
        [FieldOffset(0x550)]
        public byte multi_tap_connected;
        [FieldOffset(0x551)]
        public byte disconnected;
        [FieldOffset((0x552) + (0x1 * 0))]
        public byte act_align_0;
        [FieldOffset((0x552) + (0x1 * 1))]
        public byte act_align_1;
        [FieldOffset((0x552) + (0x1 * 2))]
        public byte act_align_2;
        [FieldOffset((0x552) + (0x1 * 3))]
        public byte act_align_3;
        [FieldOffset((0x552) + (0x1 * 4))]
        public byte act_align_4;
        [FieldOffset((0x552) + (0x1 * 5))]
        public byte act_align_5;
        [FieldOffset(0x558)]
        public byte slot;
        [FieldOffset(0x559)]
        public sbyte initialized;
        [FieldOffset(0x55c)]
        public int rterm_id;
        [FieldOffset(0x560)]
        public int id;
        [FieldOffset(0x564)]
        public int exid;
        [FieldOffset(0x568)]
        public int lagFrames;
        [FieldOffset(0x56c)]
        public RCPointer RawPadInputCallback;
        [FieldOffset(0x570)]
        public RCPointer pCallbackData;
        [FieldOffset((0x574) + (0x1 * 0))]
        public byte rdata_0;
        [FieldOffset((0x574) + (0x1 * 1))]
        public byte rdata_1;
        [FieldOffset((0x574) + (0x1 * 2))]
        public byte rdata_2;
        [FieldOffset((0x574) + (0x1 * 3))]
        public byte rdata_3;
        [FieldOffset((0x574) + (0x1 * 4))]
        public byte rdata_4;
        [FieldOffset((0x574) + (0x1 * 5))]
        public byte rdata_5;
        [FieldOffset((0x574) + (0x1 * 6))]
        public byte rdata_6;
        [FieldOffset((0x574) + (0x1 * 7))]
        public byte rdata_7;
        [FieldOffset((0x574) + (0x1 * 8))]
        public byte rdata_8;
        [FieldOffset((0x574) + (0x1 * 9))]
        public byte rdata_9;
        [FieldOffset((0x574) + (0x1 * 10))]
        public byte rdata_10;
        [FieldOffset((0x574) + (0x1 * 11))]
        public byte rdata_11;
        [FieldOffset((0x574) + (0x1 * 12))]
        public byte rdata_12;
        [FieldOffset((0x574) + (0x1 * 13))]
        public byte rdata_13;
        [FieldOffset((0x574) + (0x1 * 14))]
        public byte rdata_14;
        [FieldOffset((0x574) + (0x1 * 15))]
        public byte rdata_15;
        [FieldOffset((0x574) + (0x1 * 16))]
        public byte rdata_16;
        [FieldOffset((0x574) + (0x1 * 17))]
        public byte rdata_17;
        [FieldOffset((0x574) + (0x1 * 18))]
        public byte rdata_18;
        [FieldOffset((0x574) + (0x1 * 19))]
        public byte rdata_19;
        [FieldOffset((0x574) + (0x1 * 20))]
        public byte rdata_20;
        [FieldOffset((0x574) + (0x1 * 21))]
        public byte rdata_21;
        [FieldOffset((0x574) + (0x1 * 22))]
        public byte rdata_22;
        [FieldOffset((0x574) + (0x1 * 23))]
        public byte rdata_23;
        [FieldOffset((0x574) + (0x1 * 24))]
        public byte rdata_24;
        [FieldOffset((0x574) + (0x1 * 25))]
        public byte rdata_25;
        [FieldOffset((0x574) + (0x1 * 26))]
        public byte rdata_26;
        [FieldOffset((0x574) + (0x1 * 27))]
        public byte rdata_27;
        [FieldOffset((0x574) + (0x1 * 28))]
        public byte rdata_28;
        [FieldOffset((0x574) + (0x1 * 29))]
        public byte rdata_29;
        [FieldOffset((0x574) + (0x1 * 30))]
        public byte rdata_30;
        [FieldOffset((0x574) + (0x1 * 31))]
        public byte rdata_31;
        [FieldOffset((0x594) + (0x4 * 0))]
        public int ipad_0;
        [FieldOffset((0x594) + (0x4 * 1))]
        public int ipad_1;
        [FieldOffset((0x594) + (0x4 * 2))]
        public int ipad_2;
        [FieldOffset((0x594) + (0x4 * 3))]
        public int ipad_3;
        [FieldOffset((0x594) + (0x4 * 4))]
        public int ipad_4;
        [FieldOffset((0x594) + (0x4 * 5))]
        public int ipad_5;
        [FieldOffset((0x594) + (0x4 * 6))]
        public int ipad_6;
        [FieldOffset((0x594) + (0x4 * 7))]
        public int ipad_7;
        [FieldOffset((0x594) + (0x4 * 8))]
        public int ipad_8;
        [FieldOffset((0x594) + (0x4 * 9))]
        public int ipad_9;
    };

#endregion

    #region Teams

    public enum eTeamID
    { 
        BLUE,
        RED,
        GREEN,
        ORANGE,
        YELLOW,
        PURPLE,
        AQUA,
        PINK,
        OLIVE,
        MAROON,
        WHITE
    };

    #endregion

    #region Game Mode

    public enum eJuggernautVis
    {
        JUGGERNAUT_VIS_ALWAYS,
        JUGGERNAUT_VIS_HIT,
        JUGGERNAUT_VIS_MOVE,
        JUGGERNAUT_VIS_MAX
    }

    public enum eJuggernautHealing
    {
        JUGGERNAUT_HEAL_OFF,
        JUGGERNAUT_HEAL_LOW,
        JUGGERNAUT_HEAL_NORMAL,
        JUGGERNAUT_HEAL_HIGH,
        JUGGERNAUT_HEAL_MAX
    }

    public enum eConquestNodeType
    {
        CONQUEST_NODE_TYPE_HACKER_ORBS,
        CONQUEST_NODE_TYPE_BOLT_CRANKS,
        CONQUEST_NODE_TYPE_MAX
    }

    public enum eConquestSpecialRules
    {
        CONQUEST_SPECIAL_RULES_NONE,
        CONQUEST_SPECIAL_RULES_LOCKDOWN,
        CONQUEST_SPECIAL_RULES_HOME_NODES,
        CONQUEST_SPECIAL_RULES_MAX
    }

    public enum eVehicleID
    {
        VEHICLE_PUMA,
        VEHICLE_HOVERBIKE,
        VEHICLE_HOVERSHIP,
        VEHICLE_LANDSTALKER,
        VEHICLE_MAX
    }

    public enum eGameRuleIds
    {
        GAMERULE_CQ,
        GAMERULE_CTF,
        GAMERULE_DM,
        GAMERULE_KOTH,
        GAMERULE_JUGGY
    };

    public enum eRadar
    {
        RADAR_OFF,
        RADAR_ON,
        RADAR_SHORT
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct FragMsg
    {
        [FieldOffset(0x0)]
        public int killer;
        [FieldOffset(0x4)]
        public int killee;
        [FieldOffset(0x8)]
        public int killType;
        [FieldOffset(0xC)]
        public int displayFramesLeft;
        [FieldOffset(0x10)]
        public int delayFrames;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HudRadar_Blip_s
    {
        [FieldOffset(0x0)]
        public short iType;
        [FieldOffset(0x4)]
        public int iTeam;
        [FieldOffset(0x8)]
        public float fX;
        [FieldOffset(0xc)]
        public float fY;
        [FieldOffset(0x10)]
        public float fRot;
        [FieldOffset(0x14)]
        public RCPointer pMoby;
        [FieldOffset(0x18)]
        public short iLife;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct HudRadar_BlipParams_s
    {
        [FieldOffset(0x0)]
        public int iIcon;
        [FieldOffset(0x4)]
        public sbyte iTeam;
        [FieldOffset(0x5)]
        public sbyte iVariant;
        [FieldOffset(0x8)]
        public float fSize;
        [FieldOffset(0xc)]
        public sbyte iFlags;
    };

    #endregion

    #region Skins

    public enum eSkinID
    {
        SKIN_ID_RATCHET = 0,
        SKIN_ID_BATTLEBOT = 1,
        SKIN_ID_DREADBOT = 2,
        SKIN_ID_EVISCERATOR = 3,
        SKIN_ID_SHELLSHOCK = 4,
        SKIN_ID_SQUIDZOR = 5,
        SKIN_ID_THEMUSCLE = 6,
        SKIN_ID_MRSUNSHINE = 7,
        SKIN_ID_VENUS = 8,
        SKIN_ID_ACE = 9,
        SKIN_ID_REAPER = 10,
        SKIN_ID_KIDNOVA = 11,
        SKIN_ID_STARSHIELD = 12,
        SKIN_ID_KINGCLAUDE = 13,
        SKIN_ID_THRILLER = 14,
        SKIN_ID_ALPHACLANK = 15,
        SKIN_ID_W3RM = 16,
        SKIN_ID_LANDSHARK = 17,
        SKIN_ID_VERNON = 18,
        SKIN_ID_JAK = 19,
        SKIN_ID_RENEGADE = 20,
        SKIN_ID_EUGENE = 21,
    }


    #endregion

    #region Patch


    [StructLayout(LayoutKind.Explicit)]
    public struct SimulatedPlayer
    {
        [FieldOffset(0x00)]
        public int Active;
        [FieldOffset(0x04)]
        public int EquippedGadget;
        [FieldOffset(0x08)]
        public short CurrentState;
        [FieldOffset(0x0A)]
        public short CurrentStateId;
        [FieldOffset(0x0C)]
        public RCPointer pHero;
        [FieldOffset(0x10)]
        public GameCamera GameCamera;
        [FieldOffset(0x480)]
        public PAD Pad;
        [FieldOffset(0xA40)]
        public Vector4 CurrentPosition;
        [FieldOffset(0xA50)]
        public Vector4 CurrentRotation;
        [FieldOffset(0xA60)]
        public float CameraYaw;
        [FieldOffset(0xA64)]
        public float CameraPitch;
        [FieldOffset(0xA68)]
        public ulong ClientId;
        [FieldOffset(0xA70)]
        public short UpdateId;
        [FieldOffset(0xA72)]
        public short LastStateId;
        [FieldOffset(0xA74)]
        public short LastUpdateId;
        [FieldOffset(0xA76)]
        public bool ForceFlinch;
        [FieldOffset(0xA77)]
        public byte TicksSinceLastFlinch;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct PlayerConfig
    {
        [FieldOffset(0x00)]
        public int CameraSpeed;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct GameConfig
    {
        [FieldOffset(0x00)]
        public uint EnabledWeapons;
        [FieldOffset(0x04)]
        public byte Gamerule;
        [FieldOffset(0x05)]
        public byte MapId;
        [FieldOffset(0x06)]
        public byte UnlimitedAmmo;
        [FieldOffset(0x07)]
        public byte SpawnWithWeapons;
        [FieldOffset(0x08)]
        public byte SpawnWithChargeboots;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct Config
    {
        [FieldOffset(0x00)]
        public int Ready;
        [FieldOffset(0x04)]
        public int HasCollision;
        [FieldOffset(0x08)]
        public int PlayerCount;
        [FieldOffset(0x0C)]
        public ulong rPad_0;
        [FieldOffset(0x14)]
        public ulong rPad_1;
        [FieldOffset(0x1C)]
        public ulong rPad_2;
        [FieldOffset(0x24)]
        public ulong rPad_3;
        [FieldOffset(0x2C)]
        public RCPointer PlayerConfig;
        [FieldOffset(0x30)]
        public GameCamera GameConfig;
        [FieldOffset(0x3C)]
        public bool ForceFlinch;
    };

    public enum eCustomGameMode
    {
        CUSTOM_MODE_NONE = 0,
        CUSTOM_MODE_1000_KILLS,
        CUSTOM_MODE_GUN_GAME,
        CUSTOM_MODE_INFECTED,
        CUSTOM_MODE_PAYLOAD,
        CUSTOM_MODE_SEARCH_AND_DESTROY,
        CUSTOM_MODE_SURVIVAL,
        CUSTOM_MODE_TEAM_DEFENDER,
        CUSTOM_MODE_TRAINING,

        // always at the end to indicate how many items there are
        CUSTOM_MODE_COUNT
    };

    #endregion

    public static class Extensions
    {
        public static string ToName(this eTeamID team)
        {
            switch (team)
            {
                case eTeamID.BLUE: return "Blue";
                case eTeamID.RED: return "Red";
                case eTeamID.GREEN: return "Green";
                case eTeamID.ORANGE: return "Orange";
                case eTeamID.YELLOW: return "Yellow";
                case eTeamID.PURPLE: return "Purple";
                case eTeamID.PINK: return "Pink";
                case eTeamID.AQUA: return "Aqua";
                case eTeamID.OLIVE: return "Olive";
                case eTeamID.MAROON: return "Maroon";
                case eTeamID.WHITE: return "White";
                default: return String.Empty;
            }
        }

        public static eGadgetID ToGadget(this eWeaponSlotID weapon)
        {
            switch (weapon)
            {
                case eWeaponSlotID.Wrench: return eGadgetID.GADGET_WRENCH;
                case eWeaponSlotID.DualVipers: return eGadgetID.GADGET_MACHINEGUN;
                case eWeaponSlotID.MagmaCannon: return eGadgetID.GADGET_SHOTGUN;
                case eWeaponSlotID.Arbiter: return eGadgetID.GADGET_ROCKETLAUNCHER;
                case eWeaponSlotID.FusionRifle: return eGadgetID.GADGET_SNIPERGUN;
                case eWeaponSlotID.MineLauncher: return eGadgetID.GADGET_MINEGUN;
                case eWeaponSlotID.B6: return eGadgetID.GADGET_GRENADELAUNCHER;
                case eWeaponSlotID.Flail: return eGadgetID.GADGET_FLAIL;
                case eWeaponSlotID.HoloShield: return eGadgetID.GADGET_HOLOSHIELD;
                default: return eGadgetID.GADGET_UNDEFINED;
            }
        }

        public static eWeaponSlotID ToWeaponSlot(this eGadgetID gadget)
        {
            switch (gadget)
            {
                case eGadgetID.GADGET_WRENCH: return eWeaponSlotID.Wrench;
                case eGadgetID.GADGET_MACHINEGUN: return eWeaponSlotID.DualVipers;
                case eGadgetID.GADGET_SHOTGUN: return eWeaponSlotID.MagmaCannon;
                case eGadgetID.GADGET_ROCKETLAUNCHER: return eWeaponSlotID.Arbiter;
                case eGadgetID.GADGET_SNIPERGUN: return eWeaponSlotID.FusionRifle;
                case eGadgetID.GADGET_MINEGUN: return eWeaponSlotID.MineLauncher;
                case eGadgetID.GADGET_GRENADELAUNCHER: return eWeaponSlotID.B6;
                case eGadgetID.GADGET_FLAIL: return eWeaponSlotID.Flail;
                case eGadgetID.GADGET_HOLOSHIELD: return eWeaponSlotID.HoloShield;
                default: return eWeaponSlotID.None;
            }
        }

    }

}
