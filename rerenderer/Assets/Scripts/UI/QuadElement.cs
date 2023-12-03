using System;
using UnityEngine;
using UnityEngine.UI;
using static RCHelper;

// adapted from https://www.hallgrimgames.com/blog/2018/11/25/custom-unity-ui-meshes

public class QuadElement : MaskableGraphic
{
    public Texture m_Texture;
    public Vector2 m_Point0 = Vector2.zero;
    public Vector2 m_Point1 = Vector2.right;
    public Vector2 m_Point2 = Vector2.one;
    public Vector2 m_Point3 = Vector2.up;
    public Color m_Color0 = Color.white;
    public Color m_Color1 = Color.white;
    public Color m_Color2 = Color.white;
    public Color m_Color3 = Color.white;
    public Vector2 m_UV0 = Vector2.zero;
    public Vector2 m_UV1 = Vector2.right;
    public Vector2 m_UV2 = Vector2.one;
    public Vector2 m_UV3 = Vector2.up;
    public Color m_ShadowColor = Color.black;
    public Vector2 m_ShadowOffset = Vector2.zero;
    public UsingFlags m_Using;
    public int m_TexId;
    public bool m_ZWrite;
    public bool m_Draw;
    public GS_ZTEST m_ZTest;
    public Vector4 m_Scissor = new Vector4(0, 1, 0, 1);

    [Flags]
    public enum UsingFlags
    {
        FLATGRAY = 0,
        TEXTURE = 1,
        FADE = 2,
        DRAW_SHADOW = 4,
        COLOR = 8,
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
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

    public void PopulateFrom(Canvas canvas, GameCommandDrawQuad drawQuadCmd)
    {
        // translate to unity screen space
        // set ui element position to midpoint
        // set vertex positions relative to midpoint
        var p0 = canvas.RatchetScreenSpaceToUnityPixelSpace(drawQuadCmd.Point0.x, drawQuadCmd.Point0.y);
        var p1 = canvas.RatchetScreenSpaceToUnityPixelSpace(drawQuadCmd.Point1.x, drawQuadCmd.Point1.y);
        var p2 = canvas.RatchetScreenSpaceToUnityPixelSpace(drawQuadCmd.Point2.x, drawQuadCmd.Point2.y);
        var p3 = canvas.RatchetScreenSpaceToUnityPixelSpace(drawQuadCmd.Point3.x, drawQuadCmd.Point3.y);
        var oS = canvas.RatchetScreenSpaceToUnityPixelSpace(drawQuadCmd.ShadowX, drawQuadCmd.ShadowY);

        var midpoint = (p0 + p1 + p2 + p3) / 4;
        rectTransform.anchoredPosition = midpoint;

        m_Point0 = p0 - midpoint;
        m_Point1 = p1 - midpoint;
        m_Point2 = p2 - midpoint;
        m_Point3 = p3 - midpoint;
        m_Color0 = drawQuadCmd.Color0;
        m_Color1 = drawQuadCmd.Color1;
        m_Color2 = drawQuadCmd.Color2;
        m_Color3 = drawQuadCmd.Color3;
        m_ShadowOffset = oS;
        m_ShadowColor = drawQuadCmd.ShadowColor;
        m_Texture = null;
        m_TexId = drawQuadCmd.Icon;
        m_Using = (QuadElement.UsingFlags)drawQuadCmd.Using;
        m_ZWrite = drawQuadCmd.Vu1DrawState.ZWrite;
        m_Draw = drawQuadCmd.Vu1DrawState.Draw;
        m_ZTest = drawQuadCmd.Vu1DrawState.ZTest;
        m_Scissor = drawQuadCmd.Vu1DrawState.Scissor;

        // no color
        if (!m_Using.HasFlag(UsingFlags.COLOR))
        {
            m_Color0 = Color.gray;
            m_Color1 = Color.gray;
            m_Color2 = Color.gray;
            m_Color3 = Color.gray;
        }

        // texture
        if (m_Using.HasFlag(UsingFlags.TEXTURE))
        {
            // lookup texture
            if (drawQuadCmd.Image.HasValue())
                m_Texture = GamePIF.GetPIFTex(drawQuadCmd.Image);

            if (!m_Texture && m_TexId > 0)
                m_Texture = GameTex.GetEffectTex(m_TexId);
        }
        
        // fade
        if (m_Using.HasFlag(UsingFlags.FADE))
        {
            m_Color0.a *= drawQuadCmd.Fade;
            m_Color1.a *= drawQuadCmd.Fade;
            m_Color2.a *= drawQuadCmd.Fade;
            m_Color3.a *= drawQuadCmd.Fade;
        }

        var p = rectTransform.localPosition;
        p.z = 0x00FFFFFF - drawQuadCmd.Z;
        rectTransform.localPosition = p;

        // todo: dirty only on change
        Dirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // Clear vertex helper to reset vertices, indices etc.
        vh.Clear();

        //if (Using.HasFlag(UsingFlags.FADE))
        //    return;

        if (m_Using.HasFlag(UsingFlags.DRAW_SHADOW))
            AddShadowQuad(vh);

        AddQuad(vh);

        this.SetClippingRectFromSwizzle(m_Scissor);
    }

    private Vector4 GetUV1()
    {
        return new Vector4(m_ZWrite ? 1 : 0, m_Draw ? 0 : 1, m_Texture ? 1 : 0);
    }

    private void AddQuad(VertexHelper vh)
    {
        var i = vh.currentVertCount;
        UIVertex vert = new UIVertex();

        vert.position = rectTransform.pivot + m_Point0;
        vert.color = m_Color0;
        vert.uv0 = m_UV0;
        vert.uv1 = GetUV1();
        vh.AddVert(vert);

        vert.position = rectTransform.pivot + m_Point1;
        vert.color = m_Color1;
        vert.uv0 = m_UV1;
        vh.AddVert(vert);

        vert.position = rectTransform.pivot + m_Point2;
        vert.color = m_Color2;
        vert.uv0 = m_UV2;
        vh.AddVert(vert);

        vert.position = rectTransform.pivot + m_Point3;
        vert.color = m_Color3;
        vert.uv0 = m_UV3;
        vh.AddVert(vert);

        vh.AddTriangle(i + 0, i + 2, i + 1);
        vh.AddTriangle(i + 3, i + 2, i + 1);
    }

    private void AddShadowQuad(VertexHelper vh)
    {
        var i = vh.currentVertCount;
        UIVertex vert = new UIVertex();

        vert.position = rectTransform.pivot + m_Point0 + m_ShadowOffset;
        vert.color = m_ShadowColor;
        vert.uv0 = m_UV0;
        vert.uv1 = GetUV1();
        vh.AddVert(vert);

        vert.position = rectTransform.pivot + m_Point1 + m_ShadowOffset;
        vert.color = m_ShadowColor;
        vert.uv0 = m_UV1;
        vh.AddVert(vert);

        vert.position = rectTransform.pivot + m_Point2 + m_ShadowOffset;
        vert.color = m_ShadowColor;
        vert.uv0 = m_UV2;
        vh.AddVert(vert);

        vert.position = rectTransform.pivot + m_Point3 + m_ShadowOffset;
        vert.color = m_ShadowColor;
        vert.uv0 = m_UV3;
        vh.AddVert(vert);

        vh.AddTriangle(i + 0, i + 2, i + 1);
        vh.AddTriangle(i + 3, i + 2, i + 1);
    }
}
