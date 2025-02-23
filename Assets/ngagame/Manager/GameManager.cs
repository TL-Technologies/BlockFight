using UnityEngine;

public class GameManager: MonoBehaviour
{
	public static bool Initialized = false;
	private void Awake()
	{
		Application.targetFrameRate = 60;
		DontDestroyOnLoad(gameObject);
		Init();
	}

	public void Init()
	{
		Initialized = true;
	}
	
}
