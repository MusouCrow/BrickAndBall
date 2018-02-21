using System;
using UnityEngine;
using DG.Tweening;

namespace Game.Slots {
	[CreateAssetMenuAttribute(menuName="Game/Slot/Scale")]
	public class Scale : Utility.Slot {
		[SerializeField]
		private Ease easeType = Ease.OutExpo;
		[SerializeField]
		private Vector3 target;
		[SerializeField]
		private float time;

		public override void Run (GameObject gameObject) {
			gameObject.transform.DOScale (this.target, this.time)
				.SetEase (this.easeType);
		}
	}
}

