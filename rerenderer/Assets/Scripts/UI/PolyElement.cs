using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// adapted from https://www.hallgrimgames.com/blog/2018/11/25/custom-unity-ui-meshes

public class PolyElement : MaskableGraphic
{
    public Texture m_Texture;

    public int PolyLen = 3;
    public float LineWidth = 10;
    public bool FillConnectingLineSegments = false;
    public List<Vector2> m_Vertices = new List<Vector2>();
    public List<Vector2> m_UVs = new List<Vector2>();
    public List<Color> m_Colors = new List<Color>();
    public List<Poly> m_Indices = new List<Poly>();

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        SetVerticesDirty();
        SetMaterialDirty();
    }

    public void Dirty()
    {
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

    // actually update our mesh
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // Clear vertex helper to reset vertices, indices etc.
        vh.Clear();

        UIVertex vert = new UIVertex();

        if (PolyLen == 2)
        {
            for (int i = 0; i < m_Indices.Count; ++i)
            {
                var v0 = m_Indices[i].V0;
                var v1 = m_Indices[i].V1;

                // current line segment is continuing from previous line segment
                // so add an additional quad that connects the ends of the previous quad
                // to the beginning of this quad
                // only useful when two connecting segments are at an angle
                if (FillConnectingLineSegments && i > 0 && v0 == m_Indices[i - 1].V1)
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
                vh.AddVert(vert);
            }

            foreach (var poly in m_Indices)
            {
                switch (PolyLen)
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
    }

    private void AddLineQuad(VertexHelper vh, Vector2 v0, Vector2 uv0, Color c0, Vector2 v1, Vector2 uv1, Color c1)
    {
        UIVertex vert = new UIVertex();

        var vIndex = vh.currentVertCount;
        var dt = v1 - v0;
        var norm = Vector2.Perpendicular(dt.normalized);

        // add two vertices offset perpendicular from the first point
        // giving the line some width
        vert.position = (rectTransform.pivot + v0) + (norm * LineWidth * 0.5f);
        vert.color = c0;
        vert.uv0 = uv0;
        vh.AddVert(vert);
        vert.position = (rectTransform.pivot + v0) - (norm * LineWidth * 0.5f);
        vh.AddVert(vert);

        vert.position = (rectTransform.pivot + v1) + (norm * LineWidth * 0.5f);
        vert.color = c1;
        vert.uv0 = uv1;
        vh.AddVert(vert);
        vert.position = (rectTransform.pivot + v1) - (norm * LineWidth * 0.5f);
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

        vert.position = (rectTransform.pivot + v1) + (norm * LineWidth * 0.5f);
        vert.color = c1;
        vert.uv0 = uv1;
        vh.AddVert(vert);
        vert.position = (rectTransform.pivot + v1) - (norm * LineWidth * 0.5f);
        vh.AddVert(vert);

        // get beginning of second segment
        dt = v3 - v2;
        norm = Vector2.Perpendicular(dt.normalized);

        vert.position = (rectTransform.pivot + v2) + (norm * LineWidth * 0.5f);
        vert.color = c2;
        vert.uv0 = uv2;
        vh.AddVert(vert);
        vert.position = (rectTransform.pivot + v2) - (norm * LineWidth * 0.5f);
        vh.AddVert(vert);


        vh.AddTriangle(vIndex + 0, vIndex + 2, vIndex + 1);
        vh.AddTriangle(vIndex + 3, vIndex + 2, vIndex + 1);
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
