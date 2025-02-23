using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
	[SerializeField] Vector2 size = Vector2.one;
	public Vector2 Size => size;

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		var bound = new Rect(size / -2f, size);
		for (int i = 0; i < 4; i++)
		{
			Gizmos.DrawLine(new Vector3(bound.xMin, 0, bound.yMax), new Vector3(bound.xMax, 0, bound.yMax));
			Gizmos.DrawLine(new Vector3(bound.xMax, 0, bound.yMax), new Vector3(bound.xMax, 0, bound.yMin));
			Gizmos.DrawLine(new Vector3(bound.xMax, 0, bound.yMin), new Vector3(bound.xMin, 0, bound.yMin));
			Gizmos.DrawLine(new Vector3(bound.xMin, 0, bound.yMin), new Vector3(bound.xMin, 0, bound.yMax));
		}
	}
#endif
}
