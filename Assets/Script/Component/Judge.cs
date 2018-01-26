using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;
	using Component.UI;
	using Identity = Controller.Identity;

	public class Judge : MonoBehaviour {
		[System.Serializable]
		public class Team {
			public Wall wall;
			public Shooter shooter;
			public Brick brick;
			public Mark mark;
			[System.NonSerialized]
			public int score;

			public void SetRunning (bool isRunning) {
				this.brick.isRunning = isRunning;
				this.mark.isRunning = isRunning;
			}

			public void AddScore () {
				this.score += 1;
				this.wall.SetLength ((float)this.score / (float)INSTANCE.scoreMax);
			}

			public void Reset () {
				this.score = 0;
				this.wall.Reset ();
				this.brick.Reset ();
				this.mark.Reset ();
			}
		}

		public enum GameType {
			NONE,
			PVP,
			PVE,
			HELP
		}

		private static Judge INSTANCE;

		public static ViceCamera.TargetType StartGame (GameType gameType) {
			INSTANCE.gameType = gameType;
			ViceCamera.TargetType targetType = ViceCamera.TargetType.Opening;

			if (gameType == GameType.PVP) {
				
			} else if (gameType == GameType.PVE) {
				targetType = ViceCamera.TargetType.B;
				INSTANCE.teamA.brick.identity = Identity.AI;
				INSTANCE.teamB.brick.identity = Identity.Player;
				INSTANCE.teamA.mark.identity = Identity.AI;
				INSTANCE.teamB.mark.identity = Identity.Player;
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

			INSTANCE.teamA.SetRunning (isRunning);
			INSTANCE.teamB.SetRunning (isRunning);
		}

		public static void Gain (Vector3 position) {
			if (position.x < 0) {
				INSTANCE.teamA.AddScore ();
			} else {
				INSTANCE.teamB.AddScore ();
			}

			for (int i = 0; i < INSTANCE.sounds.Length; i++) {
				Sound.Play (INSTANCE.sounds [i], INSTANCE.volume);
			}

			if (INSTANCE.teamA.score == INSTANCE.scoreMax || INSTANCE.teamB.score == INSTANCE.scoreMax) {
				Judge.SetRunning (false);
				Team team = INSTANCE.teamA.brick.identity == Identity.Player ? INSTANCE.teamA : INSTANCE.teamB;
				UI.Interface.Result (team.score == INSTANCE.scoreMax, 0.5f);
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

		public static GameType GetGameType () {
			return INSTANCE.gameType;
		}

		[SerializeField]
		private Team teamA;
		[SerializeField]
		private Team teamB;
		[SerializeField]
		private AudioClip[] sounds;
		[SerializeField]
		private AudioClip music;
		[SerializeField]
		private float volume = 0.5f;
		[SerializeField]
		private float acceleration = 0.00005f;
		[SerializeField]
		private float shootingTime = 2;
		[SerializeField]
		private int scoreMax = 5;

		private bool aShooted;
		private Ball ball;
		private float pitch = 1;
		private bool isRunning = false;
		private GameType gameType;

		protected void Awake() {
			INSTANCE = this;

			ViceCamera.OnEndEvent += this.Reset;
			Shooter.OnShootEvent += this.ReceiveBall;
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
				this.teamA.Reset ();
				this.teamB.Reset ();
			}
		}

		public void Shoot (float time) {
			this.StartCoroutine (this.TickShoot (time));
		}

		private IEnumerator TickShoot (float time) {
			yield return new WaitForSeconds (time);

			if (this.aShooted) {
				this.teamA.shooter.Shoot ();
			} else {
				this.teamB.shooter.Shoot ();
			}

			this.aShooted = !this.aShooted;
		}

		private void ReceiveBall (GameObject obj) {
			this.ball = obj.GetComponent<Ball> ();
		}
	}
}