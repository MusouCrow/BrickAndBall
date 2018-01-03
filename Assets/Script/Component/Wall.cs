using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.Component {
	using Utility;

	public class Wall : MonoBehaviour {
		[SerializeField]
		private Transform barTransform;
		public Vector4 shakingValue;

		private Tweener tweener;

		protected void OnCollisionEnter(Collision collision) {
			if (this.tweener == null || !this.tweener.IsPlaying ()) {
				this.tweener = Lib.Shake (this.transform, this.shakingValue);
			}
		}

		public void SetLength(float value) {
			this.barTransform.DOScaleX (value, 1)
				.SetEase (Ease.OutBack)
				.SetUpdate (UpdateType.Fixed);
		}

		public void Reset () {
			this.SetLength (0);
		}
	}
}