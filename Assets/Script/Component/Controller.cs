using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Controller : MonoBehaviour {
	public delegate void ResetDelegate ();
	public enum Identity {
		Player,
		AI,
		Network
	}

	public Color targetColor;
	public int direction;

	[System.NonSerialized]
	public Color originColor;
	protected MeshRenderer renderer;
	public event ResetDelegate ResetEvent;
	[System.NonSerialized]
	public bool isRunning;
	[System.NonSerialized]
	public Identity identity;

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

	public bool CanConroll () {
		return this.isRunning && this.identity == Identity.Player;
	}
}
