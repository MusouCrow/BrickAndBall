using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Component {
	public class Count : Poster {
		public GameObject next;

		void OnDestroy() {
			if (this.next != null) {
				GameObject.Instantiate (this.next, this.transform.parent);
			}
		}
	}
}