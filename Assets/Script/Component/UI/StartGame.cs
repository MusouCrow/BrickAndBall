using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component.UI {
	using Utility;

	public class StartGame : ViceButton {
		protected void Awake () {
			base.Awake ();

			this.button.onClick.AddListener (this.OnClick);
		}

		private void OnClick() {
			ViceCamera.Move (true, 0.3f, 3);
			Interface.Clear (0.3f);
		}
	}
}