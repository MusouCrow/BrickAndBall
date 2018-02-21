using System;
using UnityEngine;
using DG.Tweening;

namespace Game.Slots {
	using Component;

	[CreateAssetMenuAttribute(menuName="Game/Slot/Shake")]
	public class Shake : Utility.Slot {
		[SerializeField]
		private Vector3 power;
		[SerializeField]
		private float time;

		public override void Run (GameObject gameObject) {
			gameObject.transform.DOPunchPosition (this.power, this.time)
				.SetEase (Ease.InOutElastic);
		}
	}
}

