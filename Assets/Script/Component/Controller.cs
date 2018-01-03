using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Controller : MonoBehaviour {
	public Color targetColor;
	public int direction;
	public bool canControll;

	[System.NonSerialized]
	public Color originColor;
	protected new MeshRenderer renderer;

	protected void Awake () {
		this.renderer = this.GetComponent<MeshRenderer> ();
		this.originColor = this.renderer.material.color;
	}

	public void ColorLert (Color a, Color b, float t) {
		this.renderer.material.color = Color.Lerp (a, b, t);
	}

	public Tweener MoveColor (Color value, float t) {
		return this.renderer.material.DOColor (value, t).SetUpdate (UpdateType.Fixed);
	}
}
