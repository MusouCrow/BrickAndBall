using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	public class Fight : Poster {
		public Vector4 shakingValue;

		void OnDestroy() {
			ViceCamera.Shake (this.shakingValue);
			Judge.SetRunning (true);
		}
	}
}