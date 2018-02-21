using System;
using UnityEngine;

namespace Game.Slots {
	using Field;
	using UI;
	using TargetType = ViceCamera.TargetType;

	[CreateAssetMenuAttribute(menuName="Game/Slot/SwitchScene")]
	public class SwitchScene : Utility.Slot {
		[SerializeField]
		private GameType gameType;
		[SerializeField]
		private float wattingTime = 0.3f;
		[SerializeField]
		private float movingTime = 3;

		public override void Run(GameObject gameObject) {
			Interface.Clear(this.wattingTime, this.OnComplete);
			Sound.PlayMusic();
		}

		private void OnComplete() {
			Judge.GameType = this.gameType;
			TargetType targetType = TargetType.Opening;

			if (this.gameType == GameType.NONE) {
				targetType = TargetType.Opening;
			}
			else if (this.gameType == GameType.PVE) {
				targetType = TargetType.B;
				Judge.PlayerType = PlayerType.B;
			}
			else if (this.gameType == GameType.PVP) {
				targetType = Judge.PlayerType == PlayerType.A ? TargetType.A : TargetType.B;
			}

			ViceCamera.Move(targetType, this.movingTime);
		}
	}
}

