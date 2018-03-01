using System;
using System.Collections;
using UnityEngine;

namespace Game.Slots {
	using UI;
	using Network;

	[CreateAssetMenuAttribute(menuName="Game/Slot/Match")]
	public class Match : SwtichUI {
		[SerializeField]
		private bool isStart;

		protected override void OnComplete() {
			if (this.isStart) {
				Networkmgr.Connect();
			}
			else {
				Networkmgr.Disconnect();
			}

			base.OnComplete();
		}
	}
}

