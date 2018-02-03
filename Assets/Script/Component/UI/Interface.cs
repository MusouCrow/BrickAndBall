using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.Component.UI {
	using Utility;
	using Network;

	public class Interface : LockBehaviour {
		private static Interface INSTANCE;
		public delegate void Delegate();

		public static void Clear(float time=0, Delegate OnComplete=null, bool exceptPoster=false) {
			INSTANCE.eventSystem.SetActive(false);
			INSTANCE.timer.Enter(time, () => INSTANCE.TickClear(OnComplete, exceptPoster));
		}

		public static void Result(bool isVictory, float time=0) {
			Sound.PlayMusic(INSTANCE.endingMusic);
			INSTANCE.timer.Enter(time, () => INSTANCE.TickResult(isVictory));
		}

		public static void Instantiate(GameObject gameObject) {
			GameObject.Instantiate(gameObject, INSTANCE.transform);
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

		protected new void Awake() {
			base.Awake();

			INSTANCE = this;
			ViceCamera.OnEndEvent += this.OnCameraEnd;
			Interface.Instantiate(this.logo);
			Screen.SetResolution(225, 400, false);

			this.timer = new Timer();
		}

		protected override void LockUpdate() {
			this.timer.Update();
		}

		private void TickClear(Delegate OnComplete=null, bool exceptPoster=false) {
			for (int i = 0; i < this.transform.childCount; i++) {
				Transform child = this.transform.GetChild(i);

				if (child.name == "FPS" || child.GetComponent<Mirage>() != null) {
					continue;
				}

				if (exceptPoster) {
					if (child.GetComponent<Poster>() == null) {
						Destroy(child.gameObject);
					}
				} else {
					Destroy(child.gameObject);
				}
			}

			this.eventSystem.SetActive(true);
			OnComplete();
		}

		private void TickResult(bool isVictory) {
			if (isVictory) {
				Interface.Instantiate(this.victory);
			} else {
				Interface.Instantiate(this.failed);
			}
		}

		private void OnCameraEnd(ViceCamera.TargetType type) {
			if (type != ViceCamera.TargetType.Opening) {
				Interface.Instantiate(this.count);
			} else {
				Interface.Instantiate(this.logo);
			}
		}
	}
}