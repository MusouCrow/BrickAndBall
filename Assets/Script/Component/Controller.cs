using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	public Color targetColor;
	[System.NonSerialized]
	public Color originColor;

	protected MeshRenderer renderer;

	public void Awake () {
		this.renderer = this.GetComponent<MeshRenderer> ();
		this.originColor = this.renderer.material.color;
	}
	
	public void ColorLert (Color a, Color b, float t) {
		this.renderer.material.color = Color.Lerp (a, b, t);
	}
}
