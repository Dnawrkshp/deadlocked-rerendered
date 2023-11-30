using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIMenuManager : MonoBehaviour
    {

        public void OnPlayButtonPressed()
        {
            // save emulator before playing always
            Config.Singleton.Emulator.Save();

            PostInterop.StartEmulator();
        }

    }
}
