using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;

namespace Game.Component {
	using Utility;

	public class Ball : NetworkBehaviour {
		private class Stretch {
			private static Vector3 SCALE = new Vector3 (1, 1, 1);

			private float rate;
			private float time;
			private bool hasCollded = false;
			private Transform transform;
			private Rigidbody rigidbody;
			private Sequence sequence;

			public Stretch (float rate, float time, Transform transform, Rigidbody rigidbody) {
				this.rate = rate;
				this.time = time;
				this.transform = transform;
				this.rigidbody = rigidbody;
			}

			public void Update (ref Vector3 velocity) {
				if (this.hasCollded) {
					float x = SCALE.x;
					float z = SCALE.z;

					if (this.rigidbody.velocity.x > 0 != velocity.x > 0) {
						float value = this.rigidbody.velocity.x * this.rate;
						x += value * 2;
						z -= value;
					}

					if (this.rigidbody.velocity.z > 0 != velocity.z > 0) {
						float value = this.rigidbody.velocity.z * this.rate;
						x -= value;
						z += value * 2;
					}

					this.sequence = DOTween.Sequence ();
					Tweener t1 = this.transform.DOScale (new Vector3 (x, SCALE.y, z), this.time);
					Tweener t2 = this.transform.DOScale (SCALE, this.time);
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

		[SerializeField]
		private AudioClip clip;
		[SerializeField]
		private GameObject effect;
		[SerializeField]
		private float rangeZ = 3.5f;
		[SerializeField]
		private float speed = 6;
		[SerializeField]
		private float sleepThreshold = 0.001f;
		[SerializeField]
		private float shakingRate = 0.005f;
		[SerializeField]
		private float shakingTime = 0.05f;
		[SerializeField]
		private float stretchRate = 0.01f;
		[SerializeField]
		private float stretchTime = 0.075f;

		[SyncVar]
		[NonSerialized]
		public float rate = 1;
		private Rigidbody rigidbody;
		private Stretch stretch;
		private bool hasDown = false;
		private Vector3 velocity;

		protected void Awake () {
			this.rigidbody = this.GetComponent<Rigidbody> ();
			this.rigidbody.sleepThreshold = this.sleepThreshold;
			this.stretch = new Stretch (this.stretchRate, this.stretchTime, this.transform, this.rigidbody);

			this.NewEffect (this.transform);
		}

		protected void FixedUpdate () {
			Vector3 pos = this.rigidbody.position;

			if (pos.y < 0) {
				if (Judge.GetGameType () == Judge.GameType.PVP) {
					if (this.isServer) {
						NetworkAgent.Gain (this.transform.localPosition);
						Destroy (this.gameObject);
					}
				} else {
					Judge.Gain (this.transform.localPosition);
					Destroy (this.gameObject);
				}

				return;
			}

			if (pos.z > this.rangeZ) {
				pos.z = this.rangeZ;
			} else if (pos.z < -this.rangeZ) {
				pos.z = -this.rangeZ;
			}

			this.rigidbody.position = pos;

			if (this.hasDown) {
				this.stretch.Update (ref this.velocity);
				this.velocity = this.rigidbody.velocity;

				float speed = this.speed * this.rate;

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

			ViceCamera.Shake (this.rigidbody.velocity * this.shakingRate, this.shakingTime);
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