using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Game.Slot {
	using Component.UI;
	using MirageCP = Game.Component.UI.Mirage;

	[CreateAssetMenuAttribute(menuName="Game/Slot/Mirage")]
	public class Mirage : Utility.Slot {
		[SerializeField]
		private float wattingTime;
		[SerializeField]
		private float runningTime;
		[SerializeField]
		private Vector3 targetScale;

		public override void Run (GameObject gameObject) {
			Image image = gameObject.GetComponent<Image> ();
			Transform transform = gameObject.transform;

			DOTween.Sequence()
				.AppendInterval(this.wattingTime)
				.OnComplete(() => {
					MirageCP.New (transform, transform.parent, image, this.targetScale, this.runningTime);
				});
		}
	}
}

