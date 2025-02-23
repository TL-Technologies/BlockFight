using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoScreenshot : MonoBehaviour
{
#if UNITY_EDITOR
	[SerializeField] bool showPlayer = false;
	[SerializeField] GameObject[] levels;

	private void Start()
	{
		if(Profile.Instance.Level > levels.Length)
		{
			return;
		}

		StartCoroutine(IEShot());
		
	}

	IEnumerator IEShot()
	{
		var levelPrefab = levels[Profile.Instance.Level - 1];
		var level = Instantiate(levelPrefab, transform);
		
		yield return new WaitForSeconds(1f);
		ScreenCapture.CaptureScreenshot($"Screenshots/{levelPrefab.gameObject.name}.png");
		Profile.Instance.Level++;
		yield return new WaitForEndOfFrame();
		level.gameObject.SetActive(false);
		var scene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(scene.name);
	}
#endif
}
