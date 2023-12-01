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
        public GameObject ZClearPrefab;

        public Canvas Canvas;
        public List<TMP_FontAsset> Fonts;
        public Vector2 PostScale = Vector2.one;

        private List<TMPro.TMP_Text> textElements = new List<TMPro.TMP_Text>();
        private List<QuadElement> quadElements = new List<QuadElement>();
        private List<PolyElement> widget2DElements = new List<PolyElement>();
        private List<GameObject> zClearElements = new List<GameObject>();
        private int currentTextElementIdx = 0;
        private int currentQuadElementIdx = 0;
        private int currentWidget2DElementIdx = 0;
        private int currentZClearElementIdx = 0;
        private bool lastQuadWasZWrite = false;
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
                            if (!Widget2DPrefab) break;

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

                            if (drawQuadCmd.ZWrite && !lastQuadWasZWrite)
                            {
                                GetOrCreateFreeZClearElement();
                            }

                            var quadElement = GetOrCreateFreeQuadElement();
                            quadElement.PopulateFrom(Canvas, drawQuadCmd);

                            lastQuadWasZWrite = quadElement.ZWrite;
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

            DisableUnusedElements();
        }


        private TMP_Text GetOrCreateFreeTextElement()
        {
            TMP_Text elem = null;

            if (currentTextElementIdx < textElements.Count)
            {
                elem = textElements[currentTextElementIdx++];
                elem.gameObject.SetActive(true);
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
                elem.gameObject.SetActive(true);
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
                elem.gameObject.SetActive(true);
                elem.transform.SetAsLastSibling();
                elem.m_Vertices.Clear();
                elem.m_UVs.Clear();
                elem.m_Colors.Clear();
                elem.m_Indices.Clear();
                return elem;
            }

            elem = Instantiate(Widget2DPrefab);
            elem.transform.SetParent(this.transform, false);
            widget2DElements.Add(elem);
            currentWidget2DElementIdx = widget2DElements.Count;
            return elem;
        }

        private GameObject GetOrCreateFreeZClearElement()
        {
            GameObject elem = null;

            if (currentZClearElementIdx < zClearElements.Count)
            {
                elem = zClearElements[currentZClearElementIdx++];
                elem.SetActive(true);
                elem.transform.SetAsLastSibling();
                return elem;
            }

            elem = Instantiate(ZClearPrefab);
            elem.transform.SetParent(this.transform, false);
            zClearElements.Add(elem);
            currentZClearElementIdx = zClearElements.Count;
            return elem;
        }

        private void DisableUnusedElements()
        {
            while (currentTextElementIdx < textElements.Count)
            {
                textElements[currentTextElementIdx++].gameObject.SetActive(false);
            }

            while (currentQuadElementIdx < quadElements.Count)
            {
                quadElements[currentQuadElementIdx++].gameObject.SetActive(false);
            }

            while (currentWidget2DElementIdx < widget2DElements.Count)
            {
                widget2DElements[currentWidget2DElementIdx++].gameObject.SetActive(false);
            }

            while (currentZClearElementIdx < zClearElements.Count)
            {
                zClearElements[currentZClearElementIdx++].gameObject.SetActive(false);
            }
        }

        private void ResetElementCounters()
        {
            currentTextElementIdx = 0;
            currentQuadElementIdx = 0;
            currentWidget2DElementIdx = 0;
            currentZClearElementIdx = 0;
        }

    }
}
