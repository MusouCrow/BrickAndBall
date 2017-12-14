using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	public class Ball : MonoBehaviour {
		private const float RANGE = 3.5f;
		private const float DEPTH = 10;
		private const float SPEED = 6;

		private Rigidbody rigidbody;

		[SerializeField]
		private AudioClip clip;
		public float rate = 1;

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
			float speed = SPEED * this.rate;

			if (velocity.x < 0 && velocity.x > -speed) {
				velocity.x = -speed;
			} else if (velocity.x > 0 && velocity.x < speed) {
				velocity.x = speed;
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

		public void Move(float x, float y, float z) {
			x *= this.rate;
			y *= this.rate;
			z *= this.rate;

			this.rigidbody.velocity += new Vector3 (x, y, z);
		}

	}
}