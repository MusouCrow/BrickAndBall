using System;
using UnityEngine;

namespace Game.Slot {
	using Component;
	using Component.UI;

	[CreateAssetMenuAttribute(menuName="Game/Slot/SwitchScene")]
	public class SwitchScene : Utility.Slot {
		[SerializeField]
		private Judge.GameType gameType;
		[SerializeField]
		private float wattingTime = 0.3f;
		[SerializeField]
		private float movingTime = 3;

		public override void Run(GameObject gameObject) {
			Interface.Clear(this.wattingTime, this.OnComplete);
			Sound.PlayMusic();
		}

		private void OnComplete() {
			var targetType = Judge.StartGame (this.gameType);
			ViceCamera.Move(targetType, this.movingTime);
		}
	}
}

