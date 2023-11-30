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
        public Canvas Canvas;
        public List<TMP_FontAsset> Fonts;
        public Vector2 PostScale = Vector2.one;

        private List<TMPro.TMP_Text> textElements = new List<TMPro.TMP_Text>();
        private List<QuadElement> quadElements = new List<QuadElement>();
        private int currentTextElementIdx = 0;
        private int currentQuadElementIdx = 0;
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
                    {
                        commandQueue.Enqueue(cmd);
                        break;
                    }
            }
        }

        private TMP_Text GetOrCreateFreeTextElement()
        {
            TMP_Text textElement = null;

            if (currentTextElementIdx < textElements.Count)
            {
                textElement = textElements[currentTextElementIdx++];
                textElement.transform.SetAsLastSibling();
                return textElement;
            }

            textElement = Instantiate(TextPrefab);
            textElement.transform.SetParent(this.transform, false);
            textElements.Add(textElement);
            currentTextElementIdx = textElements.Count;
            return textElement;
        }

        private QuadElement GetOrCreateFreeQuadElement()
        {
            QuadElement quadElement = null;

            if (currentQuadElementIdx < quadElements.Count)
            {
                quadElement = quadElements[currentQuadElementIdx++];
                quadElement.transform.SetAsLastSibling();
                return quadElement;
            }

            quadElement = Instantiate(QuadPrefab);
            quadElement.transform.SetParent(this.transform, false);
            quadElements.Add(quadElement);
            currentQuadElementIdx = quadElements.Count;
            return quadElement;
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
        }

        private void Swap()
        {
            currentTextElementIdx = 0;
            currentQuadElementIdx = 0;

            // pop
            while (commandQueue.TryDequeue(out var cmd))
            {
                switch (cmd)
                {
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
    }
}
