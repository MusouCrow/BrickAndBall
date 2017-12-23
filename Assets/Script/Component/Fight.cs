using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	public class Fight : Poster {
		protected void Awake () {
			base.Awake ();

			this.OnEndEvent += this.OnEnd;
		}

		private void OnEnd() {
			Judge.SetRunning (true);
		}
	}
}