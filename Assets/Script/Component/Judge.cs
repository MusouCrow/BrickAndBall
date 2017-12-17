using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class Judge : MonoBehaviour {
		private const int SCORE_MAX = 5;

		[SerializeField]
		private Wall wallA;
		[SerializeField]
		private Wall wallB;
		[SerializeField]
		private Shooter shooterA;
		[SerializeField]
		private Shooter shooterB;
		[SerializeField]
		private AudioClip[] clips;
		public float volume = 0.5f;
		public float acceleration = 0.00005f;
		public float shootingTime = 2;

		private AudioSource audioSource;
		private bool aShooted;
		private Timer timer;
		private int scoreA;
		private int scoreB;
		private Ball ball;
		private bool playSound;

		void Awake() {
			this.audioSource = this.GetComponent<AudioSource> ();

			this.timer = new Timer();
			this.timer.Enter (this.shootingTime);
		}

		void FixedUpdate() {
			this.audioSource.pitch += this.acceleration;

			if (this.ball != null) {
				this.ball.rate = this.audioSource.pitch;
			}

			if (this.playSound) {
				for (int i = 0; i < this.clips.Length; i++) {
					AudioSource.PlayClipAtPoint (this.clips [i], Vector3.zero, this.volume);
				}

				this.playSound = !this.playSound;
			}

			if (!this.timer.IsRunning ()) {
				return;
			}

			this.timer.Update (Time.fixedDeltaTime);
			
			if (!this.timer.IsRunning ()) {
				GameObject obj;

				if (this.aShooted) {
					obj = this.shooterA.Shoot ();
				} else {
					obj = this.shooterB.Shoot ();
				}

				this.ball = obj.GetComponent<Ball> ();
				this.ball.OnDestroyEvent += this.ReadyBall;

				if (!this.audioSource.isPlaying) {
					this.audioSource.Play ();
				}

				this.aShooted = !this.aShooted;
			}
		}

		private void ReadyBall(Vector3 position) {
			this.timer.Enter (this.shootingTime);

			if (position.x > 0) {
				this.scoreB += 1;
				this.wallB.SetLength ((float)this.scoreB / (float)SCORE_MAX);
			} else {
				this.scoreA += 1;
				this.wallA.SetLength ((float)this.scoreA / (float)SCORE_MAX);
			}

			this.playSound = true;
		}
	}
}