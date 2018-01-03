using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.Component {
	using Utility;

	public class ViceCamera : MonoBehaviour {
		public delegate void OnEndDelegate(bool isGame);
		public static event OnEndDelegate OnEndEvent;

		private static ViceCamera INSTANCE;
		private static Vector3 GAME_POS = new Vector3 (0, 10, 0);
		private static Vector3 GAME_ROT = new Vector3 (90, 90, 0);
		private static Vector3 OP_POS = new Vector3 (-25, 10, 0);
		private static Vector3 OP_ROT = new Vector3 (10, 90, 0);
		
		public static void Shake(Vector3 power, float time) {
			Lib.Shake (INSTANCE.transform, power, time);
		}

		public static void Shake(Vector4 value) {
			ViceCamera.Shake (value, value.w);
		}

		public static void Move(bool isGame, float wattingTime, float movingTime) {
			INSTANCE.StartCoroutine (INSTANCE.TickMove (isGame, wattingTime, movingTime));
		}

		public static Vector3 ScreenToWorldPoint(Vector3 pos) {
			pos.z = 10;
			return INSTANCE.camera.ScreenToWorldPoint (pos);
		}

		public Transform target;
		public float speed;
		public AudioClip clip;

		private new Camera camera;
		private bool isGame = false;

		private void Awake () {
			INSTANCE = this;

			this.camera = this.GetComponent<Camera> ();
		}


		private void FixedUpdate () {
			if (!this.isGame && this.target) {
				this.transform.RotateAround (this.target.position, Vector3.up, this.speed);
			}
		}


		private IEnumerator TickMove (bool isGame, float wattingTime, float movingTime) {
			Vector3 targetPos;
			Vector3 targetRot;
			this.isGame = isGame;

			if (isGame) {
				targetPos = GAME_POS;
				targetRot = GAME_ROT;
			} else {
				targetPos = OP_POS;
				targetRot = OP_ROT;
			}

			yield return new WaitForSeconds (wattingTime);

			Sound.Play (this.clip);
			INSTANCE.transform.DOLocalMove (targetPos, movingTime)
				.SetEase (Ease.InOutBack)
				.SetUpdate (UpdateType.Fixed)
				.OnComplete (() => ViceCamera.OnEndEvent (isGame));
			INSTANCE.transform.DOLocalRotate (targetRot, movingTime)
				.SetEase (Ease.InOutBack)
				.SetUpdate (UpdateType.Fixed);
		}

	}
}