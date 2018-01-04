using System;
using UnityEngine;
using DG.Tweening;

namespace Game.Utility {
	public static class Lib {
		public static Tweener Shake (Transform transform, Vector3 power, float time) {
			return transform.DOPunchPosition (power, time)
				.SetEase(Ease.InOutElastic);
		}

		public static Tweener Shake (Transform transform, Vector4 value) {
			return Lib.Shake (transform, value, value.w);
		}
	}
}

