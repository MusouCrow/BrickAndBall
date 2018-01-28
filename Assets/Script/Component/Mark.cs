using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class Mark : Controller {
		[SerializeField]
		private Protector protector;

		protected new void FixedUpdate() {
			base.FixedUpdate();

			if (Input.GetKeyDown(KeyCode.Space)) {
				var statemgr = this.GetComponent<Statemgr>();
				statemgr.Play("Elast");
			}
		}

		public void Play() {
			this.protector.SetActive(true);
		}

		public float GetTime() {
			return this.protector.time;
		}
	}
}