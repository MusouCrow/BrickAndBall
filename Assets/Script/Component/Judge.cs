using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;
	using Component.UI;

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

		public enum GameType {
			NONE,
			PVP,
			PVE,
			HELP
		}

		private const int SCORE_MAX = 5;
		private static Judge INSTANCE;

		public static ViceCamera.TargetType StartGame (GameType gameType) {
			INSTANCE.gameType = gameType;
			ViceCamera.TargetType targetType;

			if (gameType == GameType.NONE) {
				targetType = ViceCamera.TargetType.Opening;
			} else {
				targetType = ViceCamera.TargetType.B;
				INSTANCE.teamA.brick.identity = Brick.Identity.AI;
				INSTANCE.teamB.brick.identity = Brick.Identity.Player;
			}

			return targetType;
		}

		public static void SetRunning (bool isRunning) {
			INSTANCE.isRunning = isRunning;

			if (INSTANCE.isRunning) {
				INSTANCE.Shoot (0.1f);
				Sound.PlayMusic (INSTANCE.music);
			} else {
				Sound.PlayMusic ();
			}

			INSTANCE.teamA.brick.isRunning = isRunning;
			INSTANCE.teamB.brick.isRunning = isRunning;
			INSTANCE.teamA.mark.isRunning = isRunning;
			INSTANCE.teamB.mark.isRunning = isRunning;
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
				INSTANCE.Shoot (INSTANCE.shootingTime);
			}
		}

		public static Vector3 GetBallPosition () {
			if (INSTANCE.ball == null) {
				return Vector3.zero;
			}

			return INSTANCE.ball.transform.localPosition;
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
		private Ball ball;
		private float pitch = 1;
		private bool isRunning = false;
		private GameType gameType;

		protected void Awake() {
			INSTANCE = this;

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
		}

		private void Reset (ViceCamera.TargetType type) {
			if (type == ViceCamera.TargetType.Opening) {
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

		public void Shoot (float time) {
			this.StartCoroutine (this.TickShoot (time));
		}

		private IEnumerator TickShoot (float time) {
			yield return new WaitForSeconds (time);

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