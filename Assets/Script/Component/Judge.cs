using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class Judge : MonoBehaviour {
		[System.Serializable]
		private struct Team {
			public Wall wall;
			public Shooter shooter;
			public Brick brick;
			public Mark mark;
			[System.NonSerialized]
			public int score;
		}

		private const int SCORE_MAX = 1;
		private static Judge INSTANCE;

		public static void SetRunning (bool isRunning) {
			INSTANCE.isRunning = isRunning;

			if (INSTANCE.isRunning) {
				INSTANCE.timer.Enter (0.1f);
				Sound.PlayMusic (INSTANCE.music);
			} else {
				Sound.PlayMusic ();
			}

			INSTANCE.teamA.brick.canControll = isRunning;
			INSTANCE.teamB.brick.canControll = isRunning;
			INSTANCE.teamA.mark.canControll = isRunning;
			INSTANCE.teamB.mark.canControll = isRunning;
		}

		public static void Gain (Vector3 position) {
			if (position.x < 0) {
				INSTANCE.teamA.score += 1;
				INSTANCE.teamA.wall.SetLength ((float)INSTANCE.teamA.score / (float)SCORE_MAX);
			} else {
				INSTANCE.teamB.score += 1;
				INSTANCE.teamB.wall.SetLength ((float)INSTANCE.teamB.score / (float)SCORE_MAX);
			}

			for (int i = 0; i < INSTANCE.sounds.Length; i++) {
				Sound.Play (INSTANCE.sounds [i], INSTANCE.volume);
			}

			if (INSTANCE.teamA.score == SCORE_MAX || INSTANCE.teamB.score == SCORE_MAX) {
				Judge.SetRunning (false);
				UI.Interface.Result (INSTANCE.teamB.score == SCORE_MAX, 0.5f);
			} else {
				INSTANCE.timer.Enter (INSTANCE.shootingTime);
			}
		}

		[SerializeField]
		private Team teamA;
		[SerializeField]
		private Team teamB;
		[SerializeField]
		private AudioClip[] sounds;
		public AudioClip music;
		public float volume = 0.5f;
		public float acceleration = 0.00005f;
		public float shootingTime = 2;

		private bool aShooted;
		private Timer timer;
		private Ball ball;
		private float pitch = 1;
		private bool isRunning = false;

		protected void Awake() {
			INSTANCE = this;

			this.timer = new Timer();
			ViceCamera.OnEndEvent += this.Reset;
		}

		protected void FixedUpdate() {
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
						obj = this.teamA.shooter.Shoot ();
					} else {
						obj = this.teamB.shooter.Shoot ();
					}

					this.ball = obj.GetComponent<Ball> ();
					this.aShooted = !this.aShooted;
				}
			}
		}

		private void Reset (bool isGame) {
			if (!isGame) {
				this.aShooted = false;
				this.pitch = 1;
				this.teamA.score = 0;
				this.teamB.score = 0;
				this.teamA.wall.Reset ();
				this.teamB.wall.Reset ();
				this.teamA.brick.Reset ();
				this.teamB.brick.Reset ();
				this.teamA.mark.Reset ();
				this.teamB.mark.Reset ();
			}
		}
	}
}