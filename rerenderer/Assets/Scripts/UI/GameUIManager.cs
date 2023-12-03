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
        private bool lastElementHadZWrite = false;
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
            if (!this.enabled) return;

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

                            if (drawWidget2DCmd.Vu1DrawState.ZWrite && !lastElementHadZWrite)
                                GetOrCreateFreeZClearElement();

                            var polyElement = GetOrCreateFreeWidget2DElement();
                            polyElement.PopulateFrom(Canvas, drawWidget2DCmd);

                            lastElementHadZWrite = polyElement.m_ZWrite;
                            break;
                        }
                    case GameCommandDrawQuad drawQuadCmd:
                        {
                            if (!QuadPrefab) break;

                            if (drawQuadCmd.Vu1DrawState.ZWrite && !lastElementHadZWrite)
                                GetOrCreateFreeZClearElement();

                            var quadElement = GetOrCreateFreeQuadElement();
                            quadElement.PopulateFrom(Canvas, drawQuadCmd);

                            lastElementHadZWrite = quadElement.m_ZWrite;
                            break;
                        }
                    case GameCommandDrawText drawTextCmd:
                        {
                            if (!TextPrefab) break;

                            var textElement = GetOrCreateFreeTextElement();
                            textElement.text = drawTextCmd.Message.ConvertRCStringToRichString();
                            textElement.color = drawTextCmd.Color;
                            textElement.SetClippingRectFromSwizzle(drawTextCmd.Vu1DrawState.Scissor);

                            var rectTransform = textElement.GetComponent<RectTransform>();
                            rectTransform.anchoredPosition = Canvas.RatchetScreenSpaceToUnityPixelSpace(drawTextCmd.X, drawTextCmd.Y);
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
