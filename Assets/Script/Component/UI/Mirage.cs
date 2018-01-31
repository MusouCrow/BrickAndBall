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

		public static GameObject New(Transform transform, Transform parent, Image image, Vector3 targetScale, float time) {
			if (PREFAB == null) {
				PREFAB = Resources.Load("Prefab/UI/Mirage") as GameObject;
			}

			var obj = GameObject.Instantiate(PREFAB, parent);
			obj.transform.localPosition = transform.localPosition;
			obj.transform.localScale = transform.localScale;
			obj.transform.localRotation = transform.localRotation;

			var newImage = obj.AddComponent<Image>();
			newImage.color = image.color;
			newImage.sprite = image.sprite;
			newImage.SetNativeSize();

			var targetColor = image.color;
			targetColor.a = 0;

			newImage.DOColor(targetColor, time);
			obj.transform.DOScale(targetScale, time)
				.OnComplete(() => Destroy(obj));

			return obj;
		}
	}
}