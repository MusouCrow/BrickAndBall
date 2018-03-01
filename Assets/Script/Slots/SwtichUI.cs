using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Game.Slots {
	using UI;

	[CreateAssetMenuAttribute(menuName="Game/Slot/SwtichUI")]
	public class SwtichUI : Utility.Slot {
		[SerializeField]
		protected GameObject next;
        [SerializeField]
		protected float wattingTime;

		public override void Run (GameObject gameObject) {
            Interface.Clear(this.wattingTime, this.OnComplete, true);
		}

        protected virtual void OnComplete() {
            Interface.Instantiate(this.next);
        }
	}
}

