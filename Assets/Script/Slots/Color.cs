using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Game.Slots {
	using Component;
	using UColor = UnityEngine.Color;

	[CreateAssetMenuAttribute(menuName="Game/Slot/Color")]
	public class Color : Utility.Slot {
		[SerializeField]
		private Ease easeType = Ease.Linear;
		[SerializeField]
		private UColor target;
		[SerializeField]
		private float time;

		public override void Run (GameObject gameObject) {
			gameObject.GetComponent<Graphic> ().DOColor (this.target, this.time)
				.SetEase (this.easeType);
		}
	}
}

