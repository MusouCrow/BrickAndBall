using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Game.Component {
	using Utility;

	public class Brick : Controller {
		private const float RANGE = 2.5f;

		public float powerXMin;
		public float powerXMax;
		public float powerZMin;
		public float powerZMax;

		private Transform transform;
		private Statemgr statemgr;
		private Random random;
		private Dragging dragging;
		private Shaking shaking;

		void Awake () {
			base.Awake ();

			this.transform = this.GetComponent<Transform> ();
			this.statemgr = this.GetComponent<Statemgr> ();
			this.random = new Random ();
			this.dragging = new Dragging ();
			this.shaking = new Shaking (this.transform);
		}

		void FixedUpdate() {
			this.shaking.Update (Time.fixedDeltaTime);
		}

		void OnMouseDown() {
			this.dragging.Update (Input.mousePosition);
		}

		void OnMouseDrag () {
			Vector3 oldPos = this.dragging.GetPosition ();
			this.dragging.Update (Input.mousePosition);
			Vector3 newPos = this.dragging.GetPosition ();

			Vector3 pos = this.transform.position;
			pos.z += newPos.z - oldPos.z;

			if (pos.z > RANGE) {
				pos.z = RANGE;
			} else if (pos.z < -RANGE) {
				pos.z = -RANGE;
			}

			this.transform.position = pos;
		}

		void OnCollisionEnter(Collision collision) {
			if (collision.rigidbody != null) {
				Vector3 velocity = collision.rigidbody.velocity;
				float valueX = Mathf.Lerp (this.powerXMin, this.powerXMax, (float)this.random.NextDouble ());
				float valueZ = Mathf.Lerp (this.powerZMin, this.powerZMax, (float)this.random.NextDouble ());

				if (velocity.x > 0) {
					velocity.x += valueX;
				} else {
					velocity.x -= valueX;
				}

				if (this.random.NextDouble () < 0.5) {
					velocity.z += valueZ;
				} else {
					velocity.z -= valueZ;
				}

				collision.rigidbody.velocity = velocity;
			}

			this.shaking.Enter (0, 0.1f, 0.05f);
		}
	}
}