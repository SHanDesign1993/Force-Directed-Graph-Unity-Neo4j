using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphLineTextureAnimation : MonoBehaviour {

	public float changeInterval = 0.05F;
	public int Speed = 5;

	void Update() {
		LineRenderer rend = GetComponent<LineRenderer>();

		float index = Time.time * Speed;

		Vector2 textureShift = new Vector2 (-index, 0);
		rend.material.mainTextureScale = new Vector2 ((rend.GetPosition(0) - rend.GetPosition(rend.positionCount - 1)).magnitude, 1);
		rend.material.mainTextureOffset = textureShift;
	}
}