using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class QuitGame : MonoBehaviour
    {
        private void Update()
        {
            // use this to change how to quit game, maybe with a menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitApp();
            }
        }

        private void QuitApp()
        {
            #if UNITY_EDITOR
                // Set variable if game is running from Unity
                EditorApplication.isPlaying = false;
            #else
                // Will be called if game is running from a build
                Application.Quit();
            #endif
            
        }
    }

