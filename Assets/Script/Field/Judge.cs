using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Game.Field {
	using Utility;
	using Network;
	using Brick_;
	using UI;

	public enum GameType {
		NONE,
		PVP,
		PVE,
		HELP
	}

	public enum PlayerType {
		A,
		B
	}

	public class Judge : LockBehaviour {
		[System.Serializable]
		public class Team {
			public Wall wall;
			public Shooter shooter;
			public Brick brick;
			[System.NonSerialized]
			public int score;
			[System.NonSerialized]
			public string fd;

			public void AddScore() {
				this.score += 1;
				this.wall.SetLength((float)this.score / (float)INSTANCE.scoreMax);
			}

			public void Reset() {
				this.score = 0;
				this.wall.Reset();
				this.brick.Reset();
			}
		}

		private static Judge INSTANCE;

		public static Vector3 BallPosition {
			get {
				if (INSTANCE.ball == null) {
					return Vector3.zero;
				}

				return INSTANCE.ball.transform.localPosition;
			}
		}

		public static bool IsRunning {
			set {
				INSTANCE.isRunning = value;

				if (INSTANCE.isRunning) {
					INSTANCE.Shoot(0.1f);
					Sound.PlayMusic(INSTANCE.music);
				} else {
					Sound.PlayMusic();
					Networkmgr.Disconnect();
				}
				
				INSTANCE.teamA.brick.isRunning = value;
				INSTANCE.teamB.brick.isRunning = value;
			}
		}

		public static PlayerType PlayerType {
			set {
				INSTANCE.playerTeam = value == PlayerType.A ? INSTANCE.teamA : INSTANCE.teamB;
				INSTANCE.opponentTeam = value == PlayerType.A ? INSTANCE.teamB : INSTANCE.teamA;
				INSTANCE.playerTeam.brick.isPlayer = true;
				INSTANCE.opponentTeam.brick.isPlayer = false;
			}
			get {
				return INSTANCE.playerTeam == INSTANCE.teamA ? PlayerType.A : PlayerType.B;
			}
		}

		public static GameType GameType {
			set {
				INSTANCE.gameType = value;
			}
			get {
				return INSTANCE.gameType;
			}
		}

		public static string Comparison {
			get {
				var sb = new StringBuilder();
				sb.Append(Judge.BallPosition.x + ",");
				sb.Append(Judge.BallPosition.y + ",");
				sb.Append(Judge.BallPosition.z + ",");
				sb.Append(INSTANCE.ball.Velocity.x + ",");
				sb.Append(INSTANCE.ball.Velocity.y + ",");
				sb.Append(INSTANCE.ball.Velocity.z + ",");
				sb.Append(INSTANCE.teamA.brick.transform.localScale.x + ",");
				sb.Append(INSTANCE.teamA.brick.transform.position.x + ",");
				sb.Append(INSTANCE.teamB.brick.transform.localScale.x + ",");
				sb.Append(INSTANCE.teamB.brick.transform.position.x + ",");
				sb.Append(INSTANCE.teamA.wall.scale.x + ",");
				sb.Append(INSTANCE.teamB.wall.scale.x + ",");
				sb.Append(INSTANCE.teamA.wall.transform.position.z + ",");
				sb.Append(INSTANCE.teamB.wall.transform.position.z + ",");
				
				var md5 = MD5.Create();
				var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
				sb = new StringBuilder();

				for (int i = 0; i < bytes.Length; i++) {
					sb.Append(bytes[i].ToString("x2"));
				}

				return sb.ToString();
			}
		}

		public static void SetFd(string a, string b) {
			INSTANCE.teamA.fd = a;
			INSTANCE.teamB.fd = b;
		}

		public static void Gain(Vector3 position) {
			if (position.x < 0) {
				INSTANCE.teamA.AddScore();
			} else {
				INSTANCE.teamB.AddScore();
			}

			for (int i = 0; i < INSTANCE.sounds.Length; i++) {
				Sound.Play(INSTANCE.sounds [i], INSTANCE.volume);
			}

			if (INSTANCE.teamA.score == INSTANCE.scoreMax || INSTANCE.teamB.score == INSTANCE.scoreMax) {
				Judge.IsRunning = false;
				UI.Interface.Result(INSTANCE.playerTeam.score == INSTANCE.scoreMax, 0.5f);
			} else {
				INSTANCE.Shoot(INSTANCE.shootingTime);
			}
		}

		public static void SetInput(string fd, InputData inputData) {
			var team = INSTANCE.teamA.fd == fd ? INSTANCE.teamA : INSTANCE.teamB;
			team.brick.dragging.Drag(inputData.mousePos.ToVector3(), inputData.isDown);
		}

		[SerializeField]
		private Team teamA;
		[SerializeField]
		private Team teamB;
		[SerializeField]
		private Ball ball;
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
		private float pitch = 1;
		private bool isRunning;
		private GameType gameType;
		private Team playerTeam;
		private Team opponentTeam;
		private Timer timer;

		protected new void Awake() {
			base.Awake();

			INSTANCE = this;
			ViceCamera.OnEndEvent += this.Reset;
			this.timer = new Timer();
		}

		protected override void LockUpdate() {
			if (!this.isRunning) {
				return;
			}

			this.timer.Update();
			this.pitch += this.acceleration;
			Sound.MusicPitch = this.pitch;
			this.ball.Rate = this.pitch;
		}

		public void Shoot(float time) {
			this.timer.Enter(time, this.TickShoot);
		}

		private void Reset(ViceCamera.TargetType type) {
			if (type == ViceCamera.TargetType.Opening) {
				this.aShooted = false;
				this.pitch = 1;
				this.teamA.Reset ();
				this.teamB.Reset ();
			}
		}

		private void TickShoot() {
			if (this.aShooted) {
				this.teamA.shooter.Shoot(this.ball);
			} else {
				this.teamB.shooter.Shoot(this.ball);
			}

			this.aShooted = !this.aShooted;
		}
	}
}