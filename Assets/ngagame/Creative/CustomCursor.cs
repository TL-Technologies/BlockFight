using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
	[SerializeField] Transform customCursor;


	private void Awake()
	{
#if UNITY_EDITOR
		customCursor.gameObject.SetActive(true);
		Cursor.visible = false;
#else
		customCursor.gameObject.SetActive(false);
#endif
	}

	// Update is called once per frame
	void Update()
    {
		customCursor.transform.position = Input.mousePosition;
    }
}
