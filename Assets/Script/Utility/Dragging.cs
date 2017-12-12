using System;
using UnityEngine;

namespace Game.Utility {
	public class Dragging {
		private Camera camera;
		private float depth;
		private Vector3 position;

		public Dragging (Camera camera=null, float depth=0) {
			if (camera == null) {
				camera = Camera.main;
			}
			if (depth == 0) {
				depth = camera.farClipPlane - 0.1f;
			}

			this.camera = camera;
			this.depth = depth;
		}

		public void Update(Vector3 pos) {
			this.position = this.camera.ScreenToWorldPoint (new Vector3 (pos.x, pos.y, this.depth));
		}

		public Vector3 GetPosition() {
			return this.position;
		}
	}
}

