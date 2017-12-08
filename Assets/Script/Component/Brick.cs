using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Game.Component {
	public class Brick : Controller {
		private const float RANGE = 2.5f;
		private const float DEPTH = 10;

		public float powerXMin;
		public float powerXMax;
		public float powerZMin;
		public float powerZMax;

		private Transform transform;
		private Statemgr statemgr;
		private Camera camera;
		private Vector3 mousePos;
		private Random random;

		void Awake () {
			base.Awake ();

			this.transform = this.GetComponent<Transform> ();
			this.statemgr = this.GetComponent<Statemgr> ();
			this.camera = Camera.main;
			this.random = new Random ();
		}

		void OnMouseDown () {
			this.mousePos = this.ToWorldPoint (Input.mousePosition);
		}

		void OnMouseDrag () {
			Vector3 mousePos = this.ToWorldPoint (Input.mousePosition);
			float delta = mousePos.z - this.mousePos.z;
			this.mousePos = mousePos;

			Vector3 pos = this.transform.position;
			pos.z += delta;

			if (pos.z > RANGE) {
				pos.z = RANGE;
			} else if (pos.z < -RANGE) {
				pos.z = -RANGE;
			}

			this.transform.position = pos;
		}

		private Vector3 ToWorldPoint (Vector3 pos) {
			return this.camera.ScreenToWorldPoint (new Vector3 (pos.x, pos.y, DEPTH));
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
		}
	}
}