using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class ViceCamera : MonoBehaviour {
		public delegate void OnEndDelegate(bool isGame);
		public static event OnEndDelegate OnEndEvent;

		private static ViceCamera INSTANCE;
		private static AudioClip CLIP;
		private static Vector3 GAME_POS = new Vector3 (0, 10, 0);
		private static Vector3 GAME_ROT = new Vector3 (90, 90, 0);
		private static Vector3 OP_POS = new Vector3 (-25, 10, 0);
		private static Vector3 OP_ROT = new Vector3 (10, 90, 0);
		
		public static void Shake(Vector3 power, float time) {
			INSTANCE.shaking.Enter (power, time);
			INSTANCE.position = INSTANCE.transform.localPosition;
		}

		public static void Shake(Vector4 value) {
			ViceCamera.Shake (value, value.w);
		}

		public static void Move(bool isGame, float wattingTime, float movingTime) {
			INSTANCE.isGame = isGame;

			if (INSTANCE.isGame) {
				INSTANCE.targetPos = GAME_POS;
				INSTANCE.targetRot = GAME_ROT;
			} else {
				INSTANCE.targetPos = OP_POS;
				INSTANCE.targetRot = OP_ROT;
			}

			INSTANCE.timer.Enter (wattingTime);
			INSTANCE.state = 1;
			INSTANCE.movingTime = movingTime;
		}

		public static Vector3 ScreenToWorldPoint(Vector3 pos) {
			pos.z = 10;
			return INSTANCE.camera.ScreenToWorldPoint (pos);
		}

		public Transform target;
		public float speed;

		private Camera camera;
		private Shaking shaking;
		private Timer timer;
		private bool isGame;
		private Vector3 position;
		private Vector3 targetPos;
		private Vector3 targetRot;
		private float movingTime;
		private int state;

		private void Awake () {
			INSTANCE = this;

			if (CLIP == null) {
				CLIP = Resources.Load ("Sound/Fight") as AudioClip;
			}

			this.camera = this.GetComponent<Camera> ();
			this.shaking = new Shaking ();
			this.timer = new Timer ();
		}

		private void FixedUpdate () {
			if (this.timer.IsRunning ()) {
				this.timer.Update (Time.fixedDeltaTime);

				if (this.state == 2) {
					float process = this.timer.GetProcess ();
					this.transform.localPosition = Vector3.Lerp (this.transform.localPosition, this.targetPos, process);
					this.transform.localEulerAngles = Vector3.Lerp (this.transform.localEulerAngles, this.targetRot, process);
				}

				if (!this.timer.IsRunning ()) {
					if (this.state == 1) {
						this.timer.Enter (this.movingTime);
						Sound.Play (CLIP);
					}
					else if (this.state == 2 && ViceCamera.OnEndEvent != null) {
						ViceCamera.OnEndEvent (this.isGame);
					}

					this.state += 1;
				}
			} else if (!this.isGame && this.target) {
				this.transform.RotateAround (this.target.position, Vector3.up, this.speed);
			} else if (this.shaking.IsRunning ()) {
				this.shaking.Update (Time.fixedDeltaTime);
				this.transform.localPosition = this.position + this.shaking.GetPosition ();
			}
		}
	}
}