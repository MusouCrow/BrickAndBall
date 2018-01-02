using System;
using UnityEngine;

namespace Game.Slot {
	using Component;
	using Component.UI;

	[CreateAssetMenuAttribute(menuName="Game/Slot/StartGame")]
	public class StartGame : Utility.Slot {
		[SerializeField]
		private float wattingTime = 0.3f;
		[SerializeField]
		private float movingTime = 3;

		public override void Run (GameObject gameObject) {
			ViceCamera.Move (true, this.wattingTime, this.movingTime);
			Interface.Clear (this.wattingTime);
			Sound.PlayMusic ();
		}
	}
}

