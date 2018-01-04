using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using DG.Tweening;

namespace Game.Component {
	using Utility;

	public class Ball : MonoBehaviour {
		private class Stretch {
			private const float VALUE = 0.01f;
			private const float TIME = 0.075f;
			private static Vector3 SCALE = new Vector3 (1, 1, 1);

			private bool hasCollded = false;
			private Transform transform;
			private Rigidbody rigidbody;
			private Sequence sequence;

			public Stretch (Transform transform, Rigidbody rigidbody) {
				this.transform = transform;
				this.rigidbody = rigidbody;
			}

			public void Update (ref Vector3 velocity) {
				if (this.hasCollded) {
					float x = 1;
					float z = 1;

					if (this.rigidbody.velocity.x > 0 != velocity.x > 0) {
						float value = this.rigidbody.velocity.x * VALUE;
						x += value * 2;
						z -= value;
					}

					if (this.rigidbody.velocity.z > 0 != velocity.z > 0) {
						float value = this.rigidbody.velocity.z * VALUE;
						x -= value;
						z += value * 2;
					}

					this.sequence = DOTween.Sequence ();
					Tweener t1 = this.transform.DOScale (new Vector3 (x, 1, z), TIME);
					Tweener t2 = this.transform.DOScale (SCALE, TIME);
					this.sequence.Append (t1);
					this.sequence.Append (t2);

					this.hasCollded = false;
				}
			}

			public void OnCollisionEnter () {
				if (this.sequence == null || !this.sequence.IsPlaying ()) {
					this.hasCollded = true;
				}
			}
		}

		private const float RANGE = 3.5f;
		private const float DEPTH = 10;
		private const float SPEED = 6;

		public AudioClip clip;
		public GameObject effect;
		public float rate = 1;
		public float shakingTime = 0.1f;

		private new Rigidbody rigidbody;
		private Stretch stretch;
		private bool hasDown = false;
		private Vector3 velocity;

		protected void Awake () {
			this.rigidbody = this.GetComponent<Rigidbody> ();
			this.rigidbody.sleepThreshold = 0.001f;
			this.stretch = new Stretch (this.transform, this.rigidbody);

			this.NewEffect (this.transform);
		}

		protected void FixedUpdate () {
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

			if (this.hasDown) {
				this.stretch.Update (ref this.velocity);
				this.velocity = this.rigidbody.velocity;

				float speed = SPEED * this.rate;

				if (this.velocity.x < 0 && this.velocity.x > -speed) {
					this.velocity.x = -speed;
				} else if (this.velocity.x > 0 && this.velocity.x < speed) {
					this.velocity.x = speed;
				}

				this.rigidbody.velocity = this.velocity;
			}
		}

		protected void OnCollisionEnter(Collision collision) {
			Sound.Play (this.clip);
			GameObject obj = this.NewEffect (this.transform.parent);
			ParticleSystemRenderer psr = obj.GetComponent<ParticleSystemRenderer>();
			MeshRenderer mr = collision.gameObject.GetComponent<MeshRenderer> ();

			if (psr && mr) {
				psr.material.color = collision.gameObject.GetComponent<MeshRenderer> ().material.color;
			}

			ViceCamera.Shake (this.rigidbody.velocity * 0.01f, this.shakingTime);
			this.stretch.OnCollisionEnter ();
			this.hasDown = true;
		}

		private GameObject NewEffect(Transform parent) {
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