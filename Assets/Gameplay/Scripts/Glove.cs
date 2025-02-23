using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glove : MonoBehaviour
{
	[SerializeField] Renderer render;
	[SerializeField] Material red;
	[SerializeField] Material blue;
	[SerializeField] Side side;
	public Side HandSide => side;

	public void SetColor(Color color)
	{
		render.sharedMaterial = color == Color.Blue ? blue : red;
	}

	public enum Color
	{
		Red,
		Blue
	}

	public enum Side
	{
		Right,
		Left
	}
}
