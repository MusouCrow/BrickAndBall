using System;
using UnityEngine;

namespace Game.Utility {
	[Serializable]
	public class ValueRound {
		public float min;
		public float max;
		public float value;
		public float speed;
		public int direction;

		public void Update () {
			this.value += this.speed * this.direction;

			if (this.direction == 1 && this.value >= this.max) {
				this.value = this.max;
				this.direction = -this.direction;
			} else if (this.direction == -1 && this.value <= this.min) {
				this.value = this.min;
				this.direction = -this.direction;
			}
		}
	}
}
