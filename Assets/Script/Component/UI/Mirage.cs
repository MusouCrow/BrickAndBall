using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Component.UI {
	using Utility;

	public class Mirage : MonoBehaviour {
		private static GameObject PREFAB;

		public static GameObject New<T>(Transform transform, Transform parent, T component, Vector3 targetScale, float time)
			where T : Graphic
		{
			if (PREFAB == null) {
				PREFAB = Resources.Load ("Prefab/UI/Mirage") as GameObject;
			}

			GameObject obj = GameObject.Instantiate (PREFAB, parent) as GameObject;

			obj.transform.localPosition = transform.localPosition;
			obj.transform.localScale = transform.localScale;
			obj.transform.localRotation = transform.localRotation;
			obj.GetComponent<Mirage> ().Init<T> (component, targetScale, time);

			return obj;
		}

		private Vector3 targetScale;
		private Graphic graphic;
		private Timer timer;
		private Color targetColor;

		public void Init<T>(T graphic, Vector3 targetScale, float time) where T : Graphic {
			this.graphic = this.gameObject.AddComponent<T> ();

			this.graphic.color = graphic.color;
			this.targetColor = this.graphic.color;
			this.targetColor.a = 0;

			Type type = typeof(T);

			if (type == typeof(Image)) {
				Image imageWrap = this.graphic as Image;
				Image image = graphic as Image;

				imageWrap.sprite = image.sprite;
				imageWrap.SetNativeSize ();
			} else if (type == typeof(Text)) {
				Text textWrap = this.graphic as Text;
				Text text = graphic as Text;

				textWrap.font = text.font;
				textWrap.fontSize = text.fontSize;
				textWrap.fontStyle = text.fontStyle;
				textWrap.text = text.text;
				textWrap.rectTransform.sizeDelta = new Vector2 (text.preferredWidth, text.preferredHeight);
			}

			this.targetScale = targetScale;
			this.timer.Enter (time);
		}

		protected void Awake () {
			this.timer = new Timer ();
		}

		protected void FixedUpdate () {
			if (!this.timer.IsRunning ()) {
				return;
			}

			this.timer.Update (Time.fixedDeltaTime);

			float process = this.timer.GetProcess ();
			this.transform.localScale = Vector3.Lerp (this.transform.localScale, this.targetScale, process);
			this.graphic.color = Color.Lerp (this.graphic.color, this.targetColor, process);

			if (!this.timer.IsRunning ()) {
				Destroy (this.gameObject);
			}
		}
	}
}