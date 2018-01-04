using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Game.Component.UI {
	using Utility;

	public class Mirage : MonoBehaviour {
		private static GameObject PREFAB;

		public static GameObject New (Transform transform, Transform parent, Image image, Vector3 targetScale, float time) {
			if (PREFAB == null) {
				PREFAB = Resources.Load ("Prefab/UI/Mirage") as GameObject;
			}

			GameObject obj = GameObject.Instantiate (PREFAB, parent) as GameObject;

			obj.transform.localPosition = transform.localPosition;
			obj.transform.localScale = transform.localScale;
			obj.transform.localRotation = transform.localRotation;
			obj.GetComponent<Mirage> ().Init (image, targetScale, time);

			return obj;
		}

		public void Init (Image image, Vector3 targetScale, float time) {
			Image newImage = this.gameObject.AddComponent<Image> ();
			newImage.color = image.color;
			newImage.sprite = image.sprite;
			newImage.SetNativeSize ();

			Color targetColor = image.color;
			targetColor.a = 0;

			newImage.DOColor (targetColor, time);
			this.transform.DOScale (targetScale, time)
				.OnComplete (this.DestroyObj);
		}

		private void DestroyObj () {
			Destroy (this.gameObject);
		}
	}
}