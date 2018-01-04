using System;
using UnityEngine;

namespace Game.Slot {
	using Component;

	[CreateAssetMenuAttribute(menuName="Game/Slot/PlayMusic")]
	public class PlayMusic : Utility.Slot {
		[SerializeField]
		private AudioClip music;

		public override void Run (GameObject gameObject) {
			Sound.PlayMusic (this.music);
		}
	}
}

