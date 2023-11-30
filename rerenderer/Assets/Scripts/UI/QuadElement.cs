using UnityEngine;
using UnityEngine.UI;

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

        var i = vh.currentVertCount;

        UIVertex vert = new UIVertex();
        vert.color = this.color;  // Do not forget to set this, otherwise 

        vert.position = rectTransform.pivot + m_Point0;
        vert.color = m_Color0;
        vert.uv0 = m_UV0;
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
}