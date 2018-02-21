using System;
using UnityEngine;

namespace Game.Slots {
	using Component;
	using Component.Network;

	[CreateAssetMenuAttribute(menuName="Game/Slot/SetRunning")]
	public class SetRunning : Utility.Slot {
		[SerializeField]
		private bool isRunning;

		public override void Run(GameObject gameObject) {
			Judge.IsRunning = this.isRunning;
		}
	}
}

