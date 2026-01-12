using UnityEngine;
using System.Runtime.InteropServices;

/// <summary>
/// OpenLink
/// ------------------------------------------------------------
/// Utility MonoBehaviour for opening external URLs in a new
/// browser tab when running a Unity WebGL build.
///
/// This uses a JavaScript bridge via DllImport("__Internal").
/// The corresponding JavaScript implementation must exist
/// inside a `.jslib` file placed under:
///
/// Assets/Plugins/
///
/// NOTE:
/// - This will ONLY work in WebGL builds.
/// - This will NOT execute in the Unity Editor.
/// - Browsers may block pop-ups unless triggered by
///   direct user interaction (e.g., button click).
/// ------------------------------------------------------------
/// </summary>
public class OpenLink : MonoBehaviour
{
    /// <summary>
    /// JavaScript function defined in a .jslib file.
    /// This function should open the provided URL
    /// in a new browser tab.
    ///
    /// Example JS implementation:
    /// ---------------------------------------------
    /// mergeInto(LibraryManager.library, {
    ///     OpenTab: function (url) {
    ///         window.open(UTF8ToString(url), "_blank");
    ///     }
    /// });
    /// ---------------------------------------------
    /// </summary>
    /// 
    [DllImport("__Internal")]
    private static extern void OpenTab(string url);


    /// <summary>
    /// Opens the given URL in a new browser tab.
    /// This method is compiled ONLY for WebGL builds
    /// and safely ignored in the Editor or other platforms.
    /// </summary>
    /// <param name="url">The full URL to open.</param>
    private static void OpenURL(string url)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        OpenTab(url);
#endif
    }

    /// <summary>
    /// Example public method to open Google.
    /// Intended to be called from a UI Button OnClick event.
    /// </summary>
    public void OpenGoogle() => OpenURL("https://www.google.com/");
}
