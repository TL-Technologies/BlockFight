using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureSkin : MonoBehaviour
{
    [SerializeField] string savePath;
    [SerializeField] Camera cam;
    void Start()
    {
        StartCoroutine(AutoCapture());
    }

    IEnumerator AutoCapture()
	{
        for(int i = 0; i < transform.childCount; i++)
		{
            transform.GetChild(i).gameObject.SetActive(false);
		}

        for (int i = 0; i < transform.childCount; i++)
        {
            var item = transform.GetChild(i);
            item.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(1f);
            Texture2D tex = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.RGBA32, false);
            RenderTexture.active = cam.targetTexture;
            tex.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes($"{Application.dataPath}/{savePath}/{item.gameObject.name}.png", bytes);
            item.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
        }
	}
}
