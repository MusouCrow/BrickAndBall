using System;
using UnityEngine;

namespace Game.Slots {
	using Component;
	using Utility;

	[CreateAssetMenuAttribute(menuName="Game/Slot/PlayMusic")]
	public class PlayMusic : Slot {
		[SerializeField]
		private AudioClip music;

		public override void Run (GameObject gameObject) {
			Sound.PlayMusic (this.music);
		}
	}
}

