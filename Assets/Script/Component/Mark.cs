using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	public class Mark : Controller {
		public Shooter shooter;

		[SerializeField]
		private Protector protector;

		public void Play() {
			this.protector.SetActive (true);
		}

		public float GetTime() {
			return this.protector.time;
		}

		void OnMouseDown () {
			this.shooter.Shoot ();
		}
	}
}