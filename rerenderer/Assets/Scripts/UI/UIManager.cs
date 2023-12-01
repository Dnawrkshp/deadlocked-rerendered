using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Singleton = null;
        public static bool RenderGameUI = false;
        public static bool RenderMenu = true;
        public static bool UIInputEnabled { get; private set; } = true;

        [Header("Screens")]
        public GameUIManager GameUIScreen;
        public UIMenuManager MenuScreen;

        [Header("Camera")]
        public Camera UICamera;
        
        private RenderTexture uiTargetRt;

        private void Awake()
        {
            Singleton = this;
        }

        private void Update()
        {
            if (MenuScreen && MenuScreen.isActiveAndEnabled != RenderMenu)
                MenuScreen.gameObject.SetActive(RenderMenu);
            if (GameUIScreen && GameUIScreen.isActiveAndEnabled != RenderGameUI)
                GameUIScreen.gameObject.SetActive(RenderGameUI);

            UpdateUIRenderTexture();
        }

        public void EnableUIInput(float delay = 0f)
        {
            if (delay <= 0f)
                UIInputEnabled = true;
            else
                StartCoroutine(SetUIInputDelayed(delay, true));
        }

        public void DisableUIInput(float delay = 0f)
        {
            if (delay <= 0f)
                UIInputEnabled = false;
            else
                StartCoroutine(SetUIInputDelayed(delay, false));
        }

        IEnumerator SetUIInputDelayed(float delay, bool value)
        {
            yield return new WaitForSeconds(delay);

            UIInputEnabled = value;
        }

        private void UpdateUIRenderTexture()
        {
            var targetWidth = Screen.width;
            var targetHeight = Screen.height;

            if (!uiTargetRt || uiTargetRt.width != targetWidth || uiTargetRt.height != targetHeight)
            {
                if (uiTargetRt) uiTargetRt.Release();

                uiTargetRt = new RenderTexture(targetWidth, targetHeight, 16, RenderTextureFormat.ARGB32);
                Shader.SetGlobalTexture("_uiTarget", uiTargetRt);
            }

            UICamera.enabled = uiTargetRt;
            UICamera.targetTexture = uiTargetRt;
            UICamera.orthographicSize = targetHeight / 2f;
            UICamera.aspect = targetWidth / targetHeight;
        }
    }

}
