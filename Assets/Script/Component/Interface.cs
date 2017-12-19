using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	public class Interface : MonoBehaviour {
		public GameObject count;

		void Start () {
			GameObject.Instantiate (this.count, this.transform);
		}
	}
}