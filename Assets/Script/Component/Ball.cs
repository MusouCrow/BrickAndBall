using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Game.Component {
	public class Ball : MonoBehaviour {
		private const float RANGE = 3.5f;
		private const float DEPTH = 10;
		private const float SPEED = 6;

		public AudioClip clip;
		public GameObject effect;
		public float rate = 1;
		public float shakingTime = 0.1f;

		private Transform follow;
		private Rigidbody rigidbody;
		private bool isBegin = false;

		// Use this for initialization
		void Awake () {
			this.rigidbody = this.GetComponent<Rigidbody> ();
			this.rigidbody.sleepThreshold = 0.001f;
			this.NewEffect (this.transform);
		}

		// Update is called once per frame
		void FixedUpdate () {
			Vector3 pos = this.rigidbody.position;

			if (pos.y < 0) {
				Judge.Gain (this.transform.localPosition);
				Destroy (this.gameObject);
				return;
			}

			if (pos.z > RANGE) {
				pos.z = RANGE;
			} else if (pos.z < -RANGE) {
				pos.z = -RANGE;
			}

			this.rigidbody.position = pos;

			if (!this.isBegin && pos.y < 0.4f) {
				this.isBegin = true;
			}

			if (this.isBegin) {
				Vector3 velocity = this.rigidbody.velocity;
				float speed = SPEED * this.rate;

				if (velocity.x < 0 && velocity.x > -speed) {
					velocity.x = -speed;
				} else if (velocity.x > 0 && velocity.x < speed) {
					velocity.x = speed;
				}

				this.rigidbody.velocity = velocity;
			}
		}

		void OnCollisionEnter(Collision collision) {
			Sound.Play (this.clip);
			GameObject obj = this.NewEffect (this.transform.parent);
			ParticleSystemRenderer psr = obj.GetComponent<ParticleSystemRenderer>();
			MeshRenderer mr = collision.gameObject.GetComponent<MeshRenderer> ();

			if (psr && mr) {
				psr.material.color = collision.gameObject.GetComponent<MeshRenderer> ().material.color;
			}

			ViceCamera.Shake (this.rigidbody.velocity * 0.01f, this.shakingTime);
		}

		GameObject NewEffect(Transform parent) {
			return GameObject.Instantiate (this.effect, this.transform.localPosition, this.transform.localRotation, parent) as GameObject;
		}

		public void Move(float x, float y, float z) {
			x *= this.rate;
			y *= this.rate;
			z *= this.rate;

			this.rigidbody.velocity += new Vector3 (x, y, z);
		}
	}
}