using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class Judge : MonoBehaviour {
		private const int SCORE_MAX = 5;
		private static Judge INSTANCE;

		public static void SetRunning (bool isRunning) {
			INSTANCE.isRunning = isRunning;

			if (INSTANCE.isRunning) {
				INSTANCE.timer.Enter (0.1f);
				Sound.PlayMusic (INSTANCE.music);
			} else {
				Sound.PlayMusic ();
			}
		}

		public static void Gain (Vector3 position) {
			INSTANCE.timer.Enter (INSTANCE.shootingTime);

			if (position.x > 0) {
				INSTANCE.scoreB += 1;
				INSTANCE.wallB.SetLength ((float)INSTANCE.scoreB / (float)SCORE_MAX);
			} else {
				INSTANCE.scoreA += 1;
				INSTANCE.wallA.SetLength ((float)INSTANCE.scoreA / (float)SCORE_MAX);
			}

			for (int i = 0; i < INSTANCE.sounds.Length; i++) {
				Sound.Play (INSTANCE.sounds [i], INSTANCE.volume);
			}
		}

		public Wall wallA;
		public Wall wallB;
		public Shooter shooterA;
		public Shooter shooterB;
		public AudioClip[] sounds;
		public AudioClip music;
		public float volume = 0.5f;
		public float acceleration = 0.00005f;
		public float shootingTime = 2;

		private bool aShooted;
		private Timer timer;
		private int scoreA;
		private int scoreB;
		private Ball ball;
		private float pitch = 1;
		private bool isRunning = false;

		void Awake() {
			INSTANCE = this;
			this.timer = new Timer();
		}

		void FixedUpdate() {
			if (!this.isRunning) {
				return;
			}

			this.pitch += this.acceleration;
			Sound.SetMusicPitch (this.pitch);

			if (this.ball != null) {
				this.ball.rate = this.pitch;
			}

			if (this.timer.IsRunning ()) {
				this.timer.Update (Time.fixedDeltaTime);

				if (!this.timer.IsRunning ()) {
					GameObject obj;

					if (this.aShooted) {
						obj = this.shooterA.Shoot ();
					} else {
						obj = this.shooterB.Shoot ();
					}

					this.ball = obj.GetComponent<Ball> ();
					this.aShooted = !this.aShooted;
				}
			}
		}
	}
}