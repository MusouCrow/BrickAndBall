using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using DG.Tweening;

namespace Game.Component {
	using Utility;
	using Network;

	public class Ball : LockBehaviour {
		private class Stretch {
			private static Vector3 SCALE = new Vector3(1, 1, 1);

			private float rate;
			private float time;
			private bool hasCollded = false;
			private Transform transform;
			private Collider collider;
			private Sequence sequence;

			public Stretch(float rate, float time, Transform transform, Collider collider) {
				this.rate = rate;
				this.time = time;
				this.transform = transform;
				this.collider = collider;
			}

			public void Update(ref Vector3 velocity) {
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

					this.sequence = DOTween.Sequence();
					var t1 = this.transform.DOScale(new Vector3(x, SCALE.y, z), this.time);
					var t2 = this.transform.DOScale(SCALE, this.time);
					this.sequence.Append(t1);
					this.sequence.Append(t2);
					this.hasCollded = false;
				}
			}

			public void OnCollide() {
				if (this.sequence == null || !this.sequence.IsPlaying()) {
					this.hasCollded = true;
				}
			}
		}

		private const float HORI_BORDER = 3.6f;
		private const float VERT_BORDER = 8;
		public delegate bool Delegate(int type, float pos, float point, float velocity);

		[SerializeField]
		private AudioClip clip;
		[SerializeField]
		private GameObject effect;
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

		private Vector3 velocity;
		private float rate = 1;
		private new Collider collider;
		private Stretch stretch;
		private bool hasDown;
		private Vector3 latePosition;

		public float Rate {
			get {
				return this.rate;
			}
			set {
				this.rate = value.ToFixed();
			}
		}

		public Vector3 Velocity {
			get {
				if (this.collider == null) {
					return Vector3.zero;
				}

				return this.collider.Velocity;
			}
			set {
				this.collider.Velocity = value;
			}
		}

		protected new void Awake() {
			base.Awake();
			
			this.collider = this.GetComponent<Collider>();
			this.collider.CollisionEnterEvent += this.OnCollide;
			this.stretch = new Stretch(this.stretchRate, this.stretchTime, this.transform, this.collider);
		}

		protected override void LockUpdate() {
			if (!this.hasDown) {
				return;
			}
			
			var pos = this.collider.Position;

			if (Mathf.Abs(pos.x) > VERT_BORDER) {
				Judge.Gain(pos);
				this.gameObject.SetActive(false);
				return;
			}
			else if (Mathf.Abs(pos.z) > HORI_BORDER) {
				if (pos.z > 0) {
					pos.z = HORI_BORDER;
				}
				else {
					pos.z = -HORI_BORDER;
				}
				this.collider.Position = pos;
			}
			
			if (this.latePosition == pos) {
				pos.x += 0.5f * this.collider.Position.x.ToDirection();
				this.collider.Position = pos;
			}

			this.latePosition = pos;
			this.stretch.Update(ref this.velocity);
			this.velocity = this.collider.Velocity;
		}

		protected void OnEnable() {
			this.hasDown = false;
			this.velocity = Vector3.zero;
			this.NewEffect(this.transform);
		}

		protected void OnDisable() {
			this.transform.localScale = Vector3.one;
		}

		private void OnCollide(Collider collider, Vector3 point) {
			Sound.Play(this.clip);
			var obj = this.NewEffect(this.transform.parent);
			var psr = obj.GetComponent<ParticleSystemRenderer>();
			var mr = collider.GetComponent<MeshRenderer>();

			if (psr && mr) {
				psr.material.color = mr.material.color;
			}

			if (!this.hasDown) {
				this.collider.IsParticle = true;
				var pos = this.collider.Position;
				pos.y = 0.287f;
				this.collider.Position = pos;
				this.hasDown = true;
			}

			ViceCamera.Shake(this.collider.Velocity * this.shakingRate, this.shakingTime);
			this.stretch.OnCollide();
		}

		private GameObject NewEffect(Transform parent) {
			return GameObject.Instantiate(this.effect, this.transform.localPosition, this.transform.localRotation, parent);
		}

		public void Move(float x, float y, float z) {
			this.collider.Velocity += new Vector3 (x, y, z);
		}

		public void Rebound(Vector3 point, Delegate CheckedFunc) {
			var pos = this.collider.Position;
			var velocity = this.collider.Velocity;
			var hasChanged = false;
			
			for (int i = 0; i < 3; i = i + 2) {
				if (CheckedFunc(i, pos[i], point[i], velocity[i])) {
					velocity[i] = -velocity[i];
					hasChanged = true;
				}
			}

			if (hasChanged) {
				this.collider.Velocity = velocity;
			}
		}
	}
}