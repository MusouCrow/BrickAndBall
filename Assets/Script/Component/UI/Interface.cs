using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.Component.UI {
	using Utility;

	public class Interface : MonoBehaviour {
		private static Interface INSTANCE;
		public delegate void Delegate();

		public static void Clear (float time=0) {
			INSTANCE.eventSystem.SetActive (false);
			INSTANCE.timer.Enter (time);
			INSTANCE.TimerEvent = INSTANCE.OnClearEnd;
		}

		public static void Result (bool isVictory, float time=0) {
			Sound.PlayMusic (INSTANCE.endingMusic);

			INSTANCE.timer.Enter (time);
			INSTANCE.isVictory = isVictory;
			INSTANCE.TimerEvent = INSTANCE.ShowResult;
		}

		[SerializeField]
		private GameObject eventSystem;
		[SerializeField]
		private GameObject count;
		[SerializeField]
		private GameObject logo;
		[SerializeField]
		private GameObject victory;
		[SerializeField]
		private GameObject failed;
		[SerializeField]
		private AudioClip endingMusic;

		private Timer timer;
		private bool isVictory;
		private Delegate TimerEvent;

		protected void Awake () {
			INSTANCE = this;

			DOTween.defaultEaseType = Ease.OutExpo;
			DOTween.defaultUpdateType = UpdateType.Fixed;

			this.timer = new Timer ();
			ViceCamera.OnEndEvent += OnCameraEnd;

			//Interface.Result (true);
			this.Instantiate (this.logo);
		}

		protected void FixedUpdate () {
			if (this.timer.IsRunning ()) {
				this.timer.Update (Time.fixedDeltaTime);

				if (!this.timer.IsRunning ()) {
					if (this.TimerEvent != null) {
						this.TimerEvent ();
					}
				}
			}
		}

		private void OnClearEnd () {
			Transform transform = INSTANCE.transform;

			for (int i = 0; i < transform.childCount; i++) {
				Transform child = transform.GetChild (i);

				if (child.name != "FPS") {
					Destroy (child.gameObject);
				}
			}

			this.eventSystem.SetActive (true);
		}

		private void ShowResult () {
			if (this.isVictory) {
				this.Instantiate (this.victory);
			} else {
				this.Instantiate (this.failed);
			}
		}

		private void Instantiate (GameObject gameObject) {
			GameObject.Instantiate (gameObject, this.transform);
		}

		private void OnCameraEnd(bool isGame) {
			if (isGame) {
				this.Instantiate (this.count);
			} else {
				this.Instantiate (this.logo);
			}
		}
	}
}