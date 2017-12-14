using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	public class Ball : MonoBehaviour {
		private const float RANGE = 3.5f;
		private const float DEPTH = 10;

		private Rigidbody rigidbody;

		[SerializeField]
		private AudioClip clip;
		public float speed;

		public delegate void OnDestroyDelegate (Vector3 position);
		public event OnDestroyDelegate OnDestroyEvent;

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

			if (velocity.x < 0 && velocity.x > -this.speed) {
				velocity.x = -this.speed;
			} else if (velocity.x > 0 && velocity.x < this.speed) {
				velocity.x = this.speed;
			}

			this.rigidbody.velocity = velocity;
		}

		void OnDestroy() {
			if (this.OnDestroyEvent != null) {
				this.OnDestroyEvent (this.transform.localPosition);
			}
		}

		void OnCollisionEnter(Collision collision) {
			AudioSource.PlayClipAtPoint (this.clip, this.transform.position);
		}
	}
}