using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class Judge : MonoBehaviour {
		private const int SCORE_MAX = 5;
		private const float SHOT_TIME = 2;

		public Wall wallA;
		public Wall wallB;
		public Shooter shooterA;
		public Shooter shooterB;

		private bool aShooted;
		private Timer timer;
		private int scoreA;
		private int scoreB;

		void Awake() {
			this.timer = new Timer();
			this.timer.Enter (SHOT_TIME);
		}

		void FixedUpdate() {
			if (!this.timer.isRunning) {
				return;
			}

			this.timer.Update (Time.fixedDeltaTime);

			if (!this.timer.isRunning) {
				GameObject obj;

				if (this.aShooted) {
					obj = this.shooterA.Shoot ();
				} else {
					obj = this.shooterB.Shoot ();
				}

				Ball ball = obj.GetComponent<Ball> ();
				ball.OnDestroyEvent += ReadyBall;

				this.aShooted = !this.aShooted;
			}
		}

		private void ReadyBall(Vector3 position) {
			this.timer.Enter (SHOT_TIME);

			if (position.x > 0) {
				this.scoreB += 1;
				this.wallB.SetLength ((float)this.scoreB / (float)SCORE_MAX);
			} else {
				this.scoreA += 1;
				this.wallA.SetLength ((float)this.scoreA / (float)SCORE_MAX);
			}
		}
	}
}