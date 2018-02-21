using System;
using System.Collections;
using UnityEngine;

namespace Game.Slots {
	using UI;
	using Network;

	[CreateAssetMenuAttribute(menuName="Game/Slot/Match")]
	public class Match : Utility.Slot {
		[SerializeField]
		private bool isStart;
		[SerializeField]
		private GameObject next;
		[SerializeField]
		private float wattingTime;

		public override void Run(GameObject gameObject) {
			Interface.Clear(this.wattingTime, this.OnComplete, true);
		}

		private void OnComplete() {
			if (this.isStart) {
				Networkmgr.Connect();
			}
			else {
				Networkmgr.Disconnect();
			}
			
			Interface.Instantiate(this.next);
		}
	}
}

