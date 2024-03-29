using System;
using System.Diagnostics;
using UnityEngine;

namespace Utils.Core
{
    public static class LogHelper
    {
        /// <summary>
        /// Opens Unity log in the device's preferred text reader, Player.Log in-build or Editor.Log in-editor.
        /// </summary>
        public static void OpenLog()
        {
            string log = Application.consoleLogPath;
            try
            {
                Process.Start(log);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Error trying to open notepad: " + e);
            }
            return;
        }
    }
}