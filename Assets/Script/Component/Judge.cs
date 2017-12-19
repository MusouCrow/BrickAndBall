using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class Judge : MonoBehaviour {
		private const int SCORE_MAX = 5;
		private static Judge instance;

		public static void SetRunning (bool isRunning) {
			instance.isRunning = isRunning;

			if (instance.isRunning) {
				instance.timer.Enter (0.1f);
			}
		}

		public static void Gain (Vector3 position) {
			instance.timer.Enter (instance.shootingTime);

			if (position.x > 0) {
				instance.scoreB += 1;
				instance.wallB.SetLength ((float)instance.scoreB / (float)SCORE_MAX);
			} else {
				instance.scoreA += 1;
				instance.wallA.SetLength ((float)instance.scoreA / (float)SCORE_MAX);
			}

			for (int i = 0; i < instance.sounds.Length; i++) {
				Sound.Play (instance.sounds [i], instance.volume);
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
			Judge.instance = this;
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

					if (!Sound.MusicIsPlaying ()) {
						Sound.PlayMusic (this.music);
					}

					this.ball = obj.GetComponent<Ball> ();
					this.aShooted = !this.aShooted;
				}
			}
		}
	}
}