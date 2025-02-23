using UnityEngine;
using System.Collections;

public class ScreenShot : MonoBehaviour
{
    public static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/Screenshots/screen_{1}x{2}_{3}.png",
                             System.IO.Path.Combine(Application.dataPath, "../"),
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    static Camera Camera;
    [UnityEditor.MenuItem("Tools/TakeScreenShot")]
    public static void TakeScreenShot()
    {
        if(Camera == null)
		{
            Camera = Camera.main;
        }
       
        var resWidth = Screen.width;
        var resHeight = Screen.height;
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        Camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGBA32, false);
        Camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        Camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotName(resWidth, resHeight);
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.LogError(string.Format("Took screenshot to: {0}", filename));
    }
}