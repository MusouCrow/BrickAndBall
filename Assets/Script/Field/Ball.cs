using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using DG.Tweening;

namespace Game.Field {
	using Utility;
	using Network;

	public class Ball : LockBehaviour {
		private static Ball INSTANCE;
		private const float HORI_BORDER = 3.6f;
		private const float VERT_BORDER = 8;
		private const float FLOOR_Y = 0.287f;
		public delegate bool Delegate(int type, float pos, float point, float velocity);

		public static Vector3 Position {
			get {
				if (INSTANCE == null) {
					return Vector3.zero;
				}

				return INSTANCE.transform.localPosition;
			}
		}

		public static Vector3 Velocity {
			get {
				if (INSTANCE == null) {
					return Vector3.zero;
				}
				else if (INSTANCE.collider == null) {
					return Vector3.zero;
				}

				return INSTANCE.collider.Velocity;
			}
			set {
				INSTANCE.collider.Velocity = value;
			}
		}

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

		private new Collider collider;
		private bool hasDown;
		private Vector3 latePosition;
		private int collidingCount;
		private Sequence stretchSequence;

		protected new void Awake() {
			base.Awake();
			INSTANCE = this;
			
			this.collider = this.GetComponent<Collider>();
			this.collider.CollisionEnterEvent += this.OnCollide;
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
		}

		protected void OnEnable() {
			this.hasDown = false;
			this.collidingCount = 0;
			this.NewEffect(this.transform);
		}

		protected void OnDisable() {
			this.stretchSequence.Kill();
			this.stretchSequence = null;
			this.transform.localScale = Vector3.one;
		}

		private void OnCollide(Collider collider, Vector3 point) {
			Sound.Play(this.clip);
			var obj = this.NewEffect(this.transform.parent);
			var psr = obj.GetComponent<ParticleSystemRenderer>();
			var mr = collider.GetComponent<MeshRenderer>();

			this.collidingCount++;
			var velocity = this.collider.Velocity;
			int direction = velocity.x > 0 ? 1 : -1;
			velocity.x += this.collidingCount * 0.05f * direction;
			this.collider.Velocity = velocity;

			if (psr && mr) {
				psr.material.color = mr.material.color;
			}

			if (!this.hasDown) {
				this.collider.IsParticle = true;
				var pos = this.collider.Position;
				pos.y = FLOOR_Y;
				this.collider.Position = pos;
				this.hasDown = true;
			}

			ViceCamera.Shake(this.collider.Velocity * this.shakingRate, this.shakingTime);
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
			var scale = Vector3.one;
			
			for (int i = 0; i < 3; i = i + 2) {
				if (CheckedFunc(i, pos[i], point[i], velocity[i])) {
					velocity[i] = -velocity[i];
					
					if ((this.stretchSequence == null || !this.stretchSequence.IsPlaying()) && i != 1) {
						var value = (velocity[i] * this.stretchRate).ToFixed();
						var other = i == 0 ? 2 : 0;
						scale[i] += value * 2;
						scale[other] -= value;
					}

					hasChanged = true;
				}
			}

			if (scale != Vector3.one) {
				this.stretchSequence = DOTween.Sequence();
				var t1 = this.transform.DOScale(scale, this.stretchTime);
				var t2 = this.transform.DOScale(Vector3.one, this.stretchTime);
				this.stretchSequence.Append(t1);
				this.stretchSequence.Append(t2);
			}

			if (hasChanged) {
				this.collider.Velocity = velocity;
			}
		}
	}
}