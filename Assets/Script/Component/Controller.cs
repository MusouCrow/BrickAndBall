using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Controller : MonoBehaviour {
	public delegate void ResetDelegate ();

	public Color targetColor;
	public int direction;
	public bool canControll;

	[System.NonSerialized]
	public Color originColor;
	protected MeshRenderer renderer;
	public event ResetDelegate ResetEvent;

	protected void Awake () {
		this.renderer = this.GetComponent<MeshRenderer> ();
		this.originColor = this.renderer.material.color;
	}

	public void Reset () {
		if (this.ResetEvent != null) {
			this.ResetEvent ();
		}
	}

	public Tweener MoveColor (Color value, float t) {
		return this.renderer.material.DOColor (value, t)
			.SetEase (Ease.Linear);
	}
}
