using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class Mark : Controller {
		public float Time {
			get {
				return this.protector.time;
			}
		}

		[SerializeField]
		private Protector protector;

		public void Play() {
			this.protector.SetActive(true);
		}
	}
}