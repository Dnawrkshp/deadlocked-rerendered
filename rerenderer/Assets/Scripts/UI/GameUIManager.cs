using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameUIManager : MonoBehaviour
    {
        public TMPro.TMP_Text TextPrefab;
        public QuadElement QuadPrefab;
        public PolyElement Widget2DPrefab;
        public Canvas Canvas;
        public List<TMP_FontAsset> Fonts;
        public Vector2 PostScale = Vector2.one;

        private List<TMPro.TMP_Text> textElements = new List<TMPro.TMP_Text>();
        private List<QuadElement> quadElements = new List<QuadElement>();
        private List<PolyElement> widget2DElements = new List<PolyElement>();
        private int currentTextElementIdx = 0;
        private int currentQuadElementIdx = 0;
        private int currentWidget2DElementIdx = 0;
        private Queue<IGameCommand> commandQueue = new Queue<IGameCommand>();

        private void Start()
        {
            var gm = GameManager.Singleton;
            if (gm)
            {
                gm.OnCommand -= OnCommand;
                gm.OnCommand += OnCommand;
            }
        }

        private void OnCommand(IGameCommand cmd)
        {
            switch (cmd)
            {
                case GameCommandOnTickEnd:
                    {
                        Swap();
                        break;
                    }
                case GameCommandDrawText:
                case GameCommandDrawQuad:
                case GameCommandDrawWidget2D:
                    {
                        commandQueue.Enqueue(cmd);
                        break;
                    }
            }
        }

        private unsafe void Swap()
        {
            ResetElementCounters();

            // pop
            while (commandQueue.TryDequeue(out var cmd))
            {
                switch (cmd)
                {
                    case GameCommandDrawWidget2D drawWidget2DCmd:
                        {
                            if (!QuadPrefab) break;

                            var bytesPerPrim = drawWidget2DCmd.PrimType == 1 ? 2 : 4;

                            // read arrays from emu
                            var bytesPositions = EmuInterop.ReadBytes(drawWidget2DCmd.Positions.Address, drawWidget2DCmd.VertexCount * 2 * sizeof(short));
                            if (bytesPositions == null) break;
                            var bytesUVs = EmuInterop.ReadBytes(drawWidget2DCmd.UVs.Address, drawWidget2DCmd.VertexCount * 2 * sizeof(short));
                            if (bytesUVs == null) break;
                            var bytesColors = EmuInterop.ReadBytes(drawWidget2DCmd.Colors.Address, drawWidget2DCmd.VertexCount * sizeof(int));
                            if (bytesColors == null) break;
                            var bytesPrims = EmuInterop.ReadBytes(drawWidget2DCmd.Polys.Address, drawWidget2DCmd.PrimCount * bytesPerPrim);
                            if (bytesPrims == null) break;

                            // get pointers to array
                            short* pPositions = (short*)bytesPositions.Value.Ref();
                            short* pUVs = (short*)bytesUVs.Value.Ref();
                            uint* pColors = (uint*)bytesColors.Value.Ref();
                            byte* pPrims = (byte*)bytesPrims.Value.Ref();

                            var polyElement = GetOrCreateFreeWidget2DElement();
                            var rectTransform = polyElement.GetComponent<RectTransform>();
                            polyElement.PolyLen = bytesPerPrim;

                            // build list of vertices/colors/uvs
                            for (int i = 0; i < drawWidget2DCmd.VertexCount; i++)
                            {
                                polyElement.m_Vertices.Add(Canvas.RatchetScreenSpaceToUnityScreenSpace(pPositions[i * 2 + 0] / 16f, pPositions[i * 2 + 1] / 16f));
                                polyElement.m_UVs.Add(Vector2.zero);
                                polyElement.m_Colors.Add(drawWidget2DCmd.Color);
                            }

                            // build list of indices
                            for (int i = 0; i < drawWidget2DCmd.PrimCount; i++)
                            {
                                switch (drawWidget2DCmd.PrimType)
                                {
                                    case 0: // quad
                                        {
                                            polyElement.m_Indices.Add(new PolyElement.Poly(pPrims[i * 4 + 0], pPrims[i * 4 + 1], pPrims[i * 4 + 2], pPrims[i * 4 + 3]));
                                            break;
                                        }
                                    case 1: // line
                                        {
                                            polyElement.m_Indices.Add(new PolyElement.Poly(pPrims[i * 2 + 0], pPrims[i * 2 + 1]));
                                            break;
                                        }
                                    default: throw new NotImplementedException();
                                }
                            }

                            rectTransform.anchoredPosition = Canvas.RatchetScreenSpaceToUnityScreenSpace(drawWidget2DCmd.X, drawWidget2DCmd.Y);
                            rectTransform.localScale = new Vector3(drawWidget2DCmd.ScaleX, drawWidget2DCmd.ScaleY, 1);
                            polyElement.Dirty();
                            break;
                        }
                    case GameCommandDrawQuad drawQuadCmd:
                        {
                            if (!QuadPrefab) break;

                            var quadElement = GetOrCreateFreeQuadElement();
                            var rectTransform = quadElement.GetComponent<RectTransform>();

                            // translate to unity screen space
                            // set ui element position to midpoint
                            // set vertex positions relative to midpoint
                            var p0 = Canvas.RatchetRelativeScreenSpaceToUnityScreenSpace(drawQuadCmd.Point0.x, drawQuadCmd.Point0.y);
                            var p1 = Canvas.RatchetRelativeScreenSpaceToUnityScreenSpace(drawQuadCmd.Point1.x, drawQuadCmd.Point1.y);
                            var p2 = Canvas.RatchetRelativeScreenSpaceToUnityScreenSpace(drawQuadCmd.Point2.x, drawQuadCmd.Point2.y);
                            var p3 = Canvas.RatchetRelativeScreenSpaceToUnityScreenSpace(drawQuadCmd.Point3.x, drawQuadCmd.Point3.y);

                            var midpoint = (p0 + p1 + p2 + p3) / 4;
                            rectTransform.anchoredPosition = midpoint;
                            quadElement.m_Point0 = p0 - midpoint;
                            quadElement.m_Point1 = p1 - midpoint;
                            quadElement.m_Point2 = p2 - midpoint;
                            quadElement.m_Point3 = p3 - midpoint;
                            quadElement.m_Color0 = drawQuadCmd.Color0;
                            quadElement.m_Color1 = drawQuadCmd.Color1;
                            quadElement.m_Color2 = drawQuadCmd.Color2;
                            quadElement.m_Color3 = drawQuadCmd.Color3;
                            quadElement.Dirty();
                            break;
                        }
                    case GameCommandDrawText drawTextCmd:
                        {
                            if (!TextPrefab) break;

                            var textElement = GetOrCreateFreeTextElement();

                            textElement.text = drawTextCmd.Message.ConvertRCStringToRichString();
                            textElement.color = drawTextCmd.Color;

                            float width = drawTextCmd.Width;
                            float height = drawTextCmd.Height;

                            var scale = Canvas.GetRatchetScreenSpaceToUnityScreenSpaceRatio();
                            var rectTransform = textElement.GetComponent<RectTransform>();

                            switch (drawTextCmd.Alignment % 3)
                            {
                                case 0:
                                    //tmp.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Left;
                                    //rectTransform.pivot = new Vector2(0, 1);
                                    break;
                                case 1:
                                    //tmp.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
                                    drawTextCmd.X -= width * 0.5f;
                                    //rectTransform.pivot = new Vector2(0.5f, 1);
                                    break;
                                case 2:
                                    //tmp.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Right;
                                    drawTextCmd.X -= width;
                                    //rectTransform.pivot = new Vector2(1f, 1);
                                    break;
                            }

                            switch (drawTextCmd.Alignment / 3)
                            {
                                case 1: textElement.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle; drawTextCmd.Y -= drawTextCmd.Height * 0.5f; break;
                                case 2: textElement.verticalAlignment = TMPro.VerticalAlignmentOptions.Bottom; drawTextCmd.Y -= drawTextCmd.Height; break;
                            }

                            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * scale.x);
                            //rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * scale);
                            rectTransform.anchoredPosition = new Vector2(drawTextCmd.X * scale.x * PostScale.x, drawTextCmd.Y * -scale.y * PostScale.y); //Canvas.RatchetScreenSpaceToUnityScreenSpace(drawTextCmd.X, drawTextCmd.Y);
                            rectTransform.localScale = new Vector3(drawTextCmd.ScaleX, drawTextCmd.ScaleY, 1);
                            break;
                        }
                }
            }

            ClearUnusedElements();
        }


        private TMP_Text GetOrCreateFreeTextElement()
        {
            TMP_Text elem = null;

            if (currentTextElementIdx < textElements.Count)
            {
                elem = textElements[currentTextElementIdx++];
                elem.transform.SetAsLastSibling();
                return elem;
            }

            elem = Instantiate(TextPrefab);
            elem.transform.SetParent(this.transform, false);
            textElements.Add(elem);
            currentTextElementIdx = textElements.Count;
            return elem;
        }

        private QuadElement GetOrCreateFreeQuadElement()
        {
            QuadElement elem = null;

            if (currentQuadElementIdx < quadElements.Count)
            {
                elem = quadElements[currentQuadElementIdx++];
                elem.transform.SetAsLastSibling();
                return elem;
            }

            elem = Instantiate(QuadPrefab);
            elem.transform.SetParent(this.transform, false);
            quadElements.Add(elem);
            currentQuadElementIdx = quadElements.Count;
            return elem;
        }

        private PolyElement GetOrCreateFreeWidget2DElement()
        {
            PolyElement elem = null;

            if (currentWidget2DElementIdx < widget2DElements.Count)
            {
                elem = widget2DElements[currentWidget2DElementIdx++];
                elem.transform.SetAsLastSibling();
            }
            else
            {
                elem = Instantiate(Widget2DPrefab);
                elem.transform.SetParent(this.transform, false);
                widget2DElements.Add(elem);
                currentWidget2DElementIdx = widget2DElements.Count;
            }

            elem.m_Vertices.Clear();
            elem.m_UVs.Clear();
            elem.m_Colors.Clear();
            elem.m_Indices.Clear();
            return elem;
        }

        private void ClearUnusedElements()
        {
            while (currentTextElementIdx < textElements.Count)
            {
                Destroy(textElements[currentTextElementIdx].gameObject);
                textElements.RemoveAt(currentTextElementIdx);
            }

            while (currentQuadElementIdx < quadElements.Count)
            {
                Destroy(quadElements[currentQuadElementIdx].gameObject);
                quadElements.RemoveAt(currentQuadElementIdx);
            }

            while (currentWidget2DElementIdx < widget2DElements.Count)
            {
                Destroy(widget2DElements[currentWidget2DElementIdx].gameObject);
                widget2DElements.RemoveAt(currentWidget2DElementIdx);
            }
        }

        private void ResetElementCounters()
        {
            currentTextElementIdx = 0;
            currentQuadElementIdx = 0;
            currentWidget2DElementIdx = 0;
        }

    }
}
