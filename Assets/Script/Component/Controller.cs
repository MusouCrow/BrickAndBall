using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	public Color targetColor;
	[System.NonSerialized]
	public Color originColor;
	public int direction;
	public bool canControll;

	protected new MeshRenderer renderer;

	protected void Awake () {
		this.renderer = this.GetComponent<MeshRenderer> ();
		this.originColor = this.renderer.material.color;
	}
	
	public void ColorLert (Color a, Color b, float t) {
		this.renderer.material.color = Color.Lerp (a, b, t);
	}
}
