using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using static RCHelper;

// adapted from https://www.hallgrimgames.com/blog/2018/11/25/custom-unity-ui-meshes

public class PolyElement : MaskableGraphic
{
    public Texture m_Texture;

    public int m_PolyLen = 3;
    public float m_LineWidth = 10;
    public bool m_FillConnectingLineSegments = false;
    public List<Vector2> m_Vertices = new List<Vector2>();
    public List<Vector2> m_UVs = new List<Vector2>();
    public List<Color> m_Colors = new List<Color>();
    public List<Poly> m_Indices = new List<Poly>();
    public bool m_ZWrite;
    public bool m_Draw;
    public GS_ZTEST m_ZTest;
    public Vector4 m_Scissor;
    public bool m_LargeVertices;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        if (!Application.isPlaying) return;

        Dirty();
    }
#endif

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        Dirty();
    }

    public void Dirty()
    {
        var mat = GameMat.GetUIMaterial(m_ZWrite, m_ZTest);
        if (this.material != mat)
            this.material = mat;

        SetVerticesDirty();
        SetMaterialDirty();
    }

    // if no texture is configured, use the default white texture as mainTexture
    public override Texture mainTexture
    {
        get
        {
            return m_Texture == null ? s_WhiteTexture : m_Texture;
        }
    }

    public unsafe void PopulateFrom(Canvas canvas, GameCommandDrawWidget2D drawWidget2DCmd)
    {
        var bytesPerPrim = drawWidget2DCmd.PrimType == 1 ? 2 : 4;

        // read arrays from emu
        int tFrame = (int)drawWidget2DCmd.TFrame;
        float t = drawWidget2DCmd.TFrame - tFrame;
        int vBufSize = drawWidget2DCmd.VertexCount * 2 * sizeof(short);
        var bytesPositions = EmuInterop.ReadBytes(drawWidget2DCmd.Positions.Address, vBufSize * drawWidget2DCmd.FrameCount);
        var bytesUVs = EmuInterop.ReadBytes(drawWidget2DCmd.UVs.Address, drawWidget2DCmd.VertexCount * 2 * sizeof(short));
        var bytesColors = EmuInterop.ReadBytes(drawWidget2DCmd.Colors.Address, drawWidget2DCmd.VertexCount * sizeof(int));
        var bytesPrims = EmuInterop.ReadBytes(drawWidget2DCmd.Polys.Address, drawWidget2DCmd.PrimCount * bytesPerPrim);

        // get pointers to array
        short* pPositions = (short*)bytesPositions.Value.Ref();
        short* pUVs = (short*)bytesUVs.Value.Ref();
        uint* pColors = (uint*)bytesColors.Value.Ref();
        byte* pPrims = (byte*)bytesPrims.Value.Ref();

        m_PolyLen = bytesPerPrim;
        m_ZWrite = drawWidget2DCmd.Vu1DrawState.ZWrite;
        m_Draw = drawWidget2DCmd.Vu1DrawState.Draw;
        m_ZTest = drawWidget2DCmd.Vu1DrawState.ZTest;
        m_Scissor = drawWidget2DCmd.Vu1DrawState.Scissor;
        m_LargeVertices = drawWidget2DCmd.X > (Constants.SCREEN_WIDTH * 2);

        // build list of vertices/colors/uvs
        for (int i = 0; i < drawWidget2DCmd.VertexCount; i++)
        {
            var offset = (i * 2) + (tFrame * drawWidget2DCmd.VertexCount * 2);
            var v = canvas.RatchetScreenSpaceToUnityPixelSpace(RemapVertex(pPositions[offset + 0]) / drawWidget2DCmd.CanvasWidth, RemapVertex(pPositions[offset + 1]) / drawWidget2DCmd.CanvasHeight);

            // animate between frames
            if (t > 0)
            {
                var offset2 = (i * 2) + ((tFrame + 1) * drawWidget2DCmd.VertexCount * 2);
                var v2 = canvas.RatchetScreenSpaceToUnityPixelSpace(RemapVertex(pPositions[offset2 + 0]) / drawWidget2DCmd.CanvasWidth, RemapVertex(pPositions[offset2 + 1]) / drawWidget2DCmd.CanvasHeight);

                v = Vector2.Lerp(v, v2, t);
            }

            m_Vertices.Add(v);
            m_UVs.Add(Vector2.zero);
            m_Colors.Add(drawWidget2DCmd.Color);
        }

        // build list of indices
        for (int i = 0; i < drawWidget2DCmd.PrimCount; i++)
        {
            switch (drawWidget2DCmd.PrimType)
            {
                case 0: // quad
                    {
                        m_Indices.Add(new PolyElement.Poly(pPrims[i * 4 + 0], pPrims[i * 4 + 1], pPrims[i * 4 + 2], pPrims[i * 4 + 3]));
                        break;
                    }
                case 1: // line
                    {
                        m_Indices.Add(new PolyElement.Poly(pPrims[i * 2 + 0], pPrims[i * 2 + 1]));
                        break;
                    }
                default: throw new NotImplementedException();
            }
        }

        var p = rectTransform.localPosition;
        p.z = 0x00FFFFFF - drawWidget2DCmd.Z;
        rectTransform.localPosition = p;

        rectTransform.anchoredPosition = canvas.RatchetScreenSpaceToUnityPixelSpace(RemapPosX(drawWidget2DCmd.X) / drawWidget2DCmd.CanvasWidth, RemapPosY(drawWidget2DCmd.Y) / drawWidget2DCmd.CanvasHeight);
        rectTransform.localScale = new Vector3(drawWidget2DCmd.ScaleX, drawWidget2DCmd.ScaleY, 1);
        Dirty();
    }

    private Vector4 GetUV1()
    {
        return new Vector4(m_ZWrite ? 1 : 0, m_Draw ? 0 : 1, 0);
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // Clear vertex helper to reset vertices, indices etc.
        vh.Clear();

        UIVertex vert = new UIVertex();

        if (m_PolyLen == 2)
        {
            for (int i = 0; i < m_Indices.Count; ++i)
            {
                var v0 = m_Indices[i].V0;
                var v1 = m_Indices[i].V1;

                // current line segment is continuing from previous line segment
                // so add an additional quad that connects the ends of the previous quad
                // to the beginning of this quad
                // only useful when two connecting segments are at an angle
                if (m_FillConnectingLineSegments && i > 0 && v0 == m_Indices[i - 1].V1)
                {
                    var vp0 = m_Indices[i - 1].V0;
                    var vp1 = m_Indices[i - 1].V1;
                    FillConnectingLineSegment(vh, m_Vertices[vp0], m_Vertices[vp1], m_UVs[vp1], m_Colors[vp1], m_Vertices[v0], m_UVs[v0], m_Colors[v0], m_Vertices[v1]);
                }

                AddLineQuad(vh, m_Vertices[v0], m_UVs[v0], m_Colors[v0], m_Vertices[v1], m_UVs[v1], m_Colors[v1]);
            }
        }
        else
        {
            for (int i = 0; i < m_Vertices.Count; i++)
            {
                vert.position = rectTransform.pivot + m_Vertices[i];
                if (i < m_Colors.Count) vert.color = m_Colors[i];
                if (i < m_UVs.Count) vert.uv0 = m_UVs[i];
                vert.uv1 = GetUV1();
                vh.AddVert(vert);
            }

            foreach (var poly in m_Indices)
            {
                switch (m_PolyLen)
                {
                    case 3: // triangle
                        {
                            vh.AddTriangle(poly.V0, poly.V1, poly.V2);
                            break;
                        }
                    case 4: // quad
                        {
                            vh.AddTriangle(poly.V0, poly.V2, poly.V1);
                            vh.AddTriangle(poly.V3, poly.V2, poly.V1);
                            break;
                        }
                }
            }
        }

        this.SetClippingRectFromSwizzle(m_Scissor);
    }

    private void AddLineQuad(VertexHelper vh, Vector2 v0, Vector2 uv0, Color c0, Vector2 v1, Vector2 uv1, Color c1)
    {
        UIVertex vert = new UIVertex();

        var vIndex = vh.currentVertCount;
        var dt = v1 - v0;
        var norm = Vector2.Perpendicular(dt.normalized);

        // add two vertices offset perpendicular from the first point
        // giving the line some width
        vert.position = (rectTransform.pivot + v0) + (norm * m_LineWidth * 0.5f);
        vert.color = c0;
        vert.uv0 = uv0;
        vert.uv1 = GetUV1();
        vh.AddVert(vert);
        vert.position = (rectTransform.pivot + v0) - (norm * m_LineWidth * 0.5f);
        vh.AddVert(vert);

        vert.position = (rectTransform.pivot + v1) + (norm * m_LineWidth * 0.5f);
        vert.color = c1;
        vert.uv0 = uv1;
        vh.AddVert(vert);
        vert.position = (rectTransform.pivot + v1) - (norm * m_LineWidth * 0.5f);
        vh.AddVert(vert);

        vh.AddTriangle(vIndex + 0, vIndex + 2, vIndex + 1);
        vh.AddTriangle(vIndex + 3, vIndex + 2, vIndex + 1);
    }

    private void FillConnectingLineSegment(VertexHelper vh, Vector2 v0, Vector2 v1, Vector2 uv1, Color c1, Vector2 v2, Vector2 uv2, Color c2, Vector2 v3)
    {
        UIVertex vert = new UIVertex();
        var vIndex = vh.currentVertCount;

        // get ends of first segments
        var dt = v1 - v0;
        var norm = Vector2.Perpendicular(dt.normalized);

        vert.position = (rectTransform.pivot + v1) + (norm * m_LineWidth * 0.5f);
        vert.color = c1;
        vert.uv0 = uv1;
        vert.uv1 = GetUV1();
        vh.AddVert(vert);
        vert.position = (rectTransform.pivot + v1) - (norm * m_LineWidth * 0.5f);
        vh.AddVert(vert);

        // get beginning of second segment
        dt = v3 - v2;
        norm = Vector2.Perpendicular(dt.normalized);

        vert.position = (rectTransform.pivot + v2) + (norm * m_LineWidth * 0.5f);
        vert.color = c2;
        vert.uv0 = uv2;
        vh.AddVert(vert);
        vert.position = (rectTransform.pivot + v2) - (norm * m_LineWidth * 0.5f);
        vh.AddVert(vert);


        vh.AddTriangle(vIndex + 0, vIndex + 2, vIndex + 1);
        vh.AddTriangle(vIndex + 3, vIndex + 2, vIndex + 1);
    }

    private float RemapVertex(float v)
    {
        if (!m_LargeVertices) return v / 16f;
        return (v / 16f) - 1024f;
    }

    private float RemapPosX(float v)
    {
        if (!m_LargeVertices) return v;
        return v % Constants.SCREEN_WIDTH;
    }

    private float RemapPosY(float v)
    {
        if (!m_LargeVertices) return v;
        return (v % Constants.SCREEN_HEIGHT) + 64f;
    }


    [Serializable]
    public struct Poly
    {
        public short V0;
        public short V1;
        public short V2;
        public short V3;

        public Poly(short v0, short v1, short v2, short v3)
        {
            V0 = v0;
            V1 = v1;
            V2 = v2;
            V3 = v3;
        }

        public Poly(short v0, short v1, short v2)
        {
            V0 = v0;
            V1 = v1;
            V2 = v2;
            V3 = 0;
        }

        public Poly(short v0, short v1)
        {
            V0 = v0;
            V1 = v1;
            V2 = 0;
            V3 = 0;
        }
    }
}
