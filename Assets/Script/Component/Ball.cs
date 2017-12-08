using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	public class Ball : MonoBehaviour {
		private const float RANGE = 3.5f;
		private const float DEPTH = 10;
		private const float SPEED = 6;

		private Rigidbody rigidbody;

		// Use this for initialization
		void Start () {
			this.rigidbody = this.GetComponent<Rigidbody> ();
			this.rigidbody.sleepThreshold = 0.001f;
		}

		// Update is called once per frame
		void FixedUpdate () {
			Vector3 pos = this.rigidbody.position;

			if (pos.y < 0) {
				Destroy (this.gameObject);
				return;
			}

			if (pos.z > RANGE) {
				pos.z = RANGE;
			} else if (pos.z < -RANGE) {
				pos.z = -RANGE;
			}

			this.rigidbody.position = pos;

			Vector3 velocity = this.rigidbody.velocity;

			if (velocity.x < 0 && velocity.x > -SPEED) {
				velocity.x = -SPEED;
			} else if (velocity.x > 0 && velocity.x < SPEED) {
				velocity.x = SPEED;
			}

			this.rigidbody.velocity = velocity;
		}
	}
}