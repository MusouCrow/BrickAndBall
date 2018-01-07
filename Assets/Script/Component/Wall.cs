using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.Component {
	using Utility;

	public class Wall : MonoBehaviour {
		[SerializeField]
		private Transform barTransform;
		[SerializeField]
		private Vector4 shakingValue;

		private Tweener tweener;

		protected void OnCollisionEnter(Collision collision) {
			if (this.tweener == null || !this.tweener.IsPlaying ()) {
				this.tweener = this.transform.DOPunchPosition (this.shakingValue, this.shakingValue.w)
					.SetEase (Ease.InOutElastic);
			}
		}

		public void SetLength(float value) {
			this.barTransform.DOScaleX (value, 1)
				.SetEase (Ease.OutBack);
		}

		public void Reset () {
			this.SetLength (0);
		}
	}
}