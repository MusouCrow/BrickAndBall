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
			private static Vector3 SCALE = new Vector3 (1, 1, 1);

			private float rate;
			private float time;
			private bool hasCollded = false;
			private Transform transform;
			private Collider collider;
			private Sequence sequence;

			public Stretch (float rate, float time, Transform transform, Collider collider) {
				this.rate = rate;
				this.time = time;
				this.transform = transform;
				this.collider = collider;
			}

			public void Update (ref Vector3 velocity) {
				if (this.hasCollded) {
					float x = SCALE.x;
					float z = SCALE.z;
					var colliderVelocity = this.collider.Velocity;

					if (colliderVelocity.x > 0 != velocity.x > 0) {
						float value = colliderVelocity.x * this.rate;
						x += value * 2;
						z -= value;
					}

					if (colliderVelocity.z > 0 != velocity.z > 0) {
						float value = colliderVelocity.z * this.rate;
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

			public void OnCollide () {
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
		private float shakingRate = 0.005f;
		[SerializeField]
		private float shakingTime = 0.05f;
		[SerializeField]
		private float stretchRate = 0.01f;
		[SerializeField]
		private float stretchTime = 0.075f;

		[NonSerialized]
		public float rate = 1;
		[NonSerialized]
		public Vector3 velocity;
		private new Collider collider;
		private Stretch stretch;
		private bool hasDown = false;

		protected void Awake () {
			this.collider = this.GetComponent<Collider> ();
			this.collider.CollisionEnterEvent += this.OnCollide;
			this.stretch = new Stretch (this.stretchRate, this.stretchTime, this.transform, this.collider);
			this.NewEffect (this.transform);
		}

		protected void FixedUpdate () {
			var pos = this.collider.Position;

			if (pos.y < 0) {
				Judge.Gain (pos);
				Destroy (this.gameObject);
				return;
			}
			
			if (pos.z > this.rangeZ) {
				pos.z = this.rangeZ;
				this.collider.Position = pos;
			} else if (pos.z < -this.rangeZ) {
				pos.z = -this.rangeZ;
				this.collider.Position = pos;
			}

			if (this.hasDown) {
				this.stretch.Update (ref this.velocity);
				this.velocity = this.collider.Velocity;
				float speed = this.speed * this.rate;
				
				if (this.velocity.x < 0 && this.velocity.x > -speed) {
					this.velocity.x = -speed;
					this.collider.Velocity = this.velocity;
				} else if (this.velocity.x > 0 && this.velocity.x < speed) {
					this.velocity.x = speed;
					this.collider.Velocity = this.velocity;
				}
			}
		}

		private void OnCollide(Collider collider) {
			Sound.Play (this.clip);
			var obj = this.NewEffect (this.transform.parent);
			var psr = obj.GetComponent<ParticleSystemRenderer>();
			var mr = collider.GetComponent<MeshRenderer> ();

			if (psr && mr) {
				psr.material.color = mr.material.color;
			}

			ViceCamera.Shake (this.collider.Velocity * this.shakingRate, this.shakingTime);
			this.stretch.OnCollide ();

			if (!this.hasDown) {
				this.collider.IsParticle = true;
				this.hasDown = true;
			}
		}

		private GameObject NewEffect(Transform parent) {
			return GameObject.Instantiate (this.effect, this.transform.localPosition, this.transform.localRotation, parent) as GameObject;
		}

		public void Move(float x, float y, float z) {
			x *= this.rate;
			y *= this.rate;
			z *= this.rate;
			this.collider.Velocity += new Vector3 (x, y, z);
		}
	}
}