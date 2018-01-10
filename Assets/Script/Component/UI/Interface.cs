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

		public static void Clear (float time=0, Delegate OnComplete=null, bool exceptPoster=false) {
			INSTANCE.StartCoroutine (INSTANCE.TickClear (time, OnComplete, exceptPoster));
		}

		public static void Result (bool isVictory, float time=0) {
			INSTANCE.StartCoroutine (INSTANCE.TickResult (isVictory, time));
		}

		public static void Instantiate (GameObject gameObject) {
			GameObject.Instantiate (gameObject, INSTANCE.transform);
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

		protected void Awake () {
			INSTANCE = this;

			DOTween.Init ();
			DOTween.defaultEaseType = Ease.OutExpo;
			DOTween.defaultUpdateType = UpdateType.Fixed;

			ViceCamera.OnEndEvent += OnCameraEnd;
			Interface.Instantiate (this.logo);
		}

		private IEnumerator TickClear (float time, Delegate OnComplete=null, bool exceptPoster=false) {
			this.eventSystem.SetActive (false);

			yield return new WaitForSeconds (time);

			for (int i = 0; i < this.transform.childCount; i++) {
				Transform child = this.transform.GetChild (i);

				if (child.name == "FPS" || child.GetComponent<Mirage> () != null) {
					continue;
				}

				if (exceptPoster) {
					if (child.GetComponent<Poster> () == null) {
						Destroy (child.gameObject);
					}
				} else {
					Destroy (child.gameObject);
				}
			}

			this.eventSystem.SetActive (true);
			OnComplete ();
		}

		private IEnumerator TickResult (bool isVictory, float time) {
			Sound.PlayMusic (this.endingMusic);

			yield return new WaitForSeconds (time);

			if (isVictory) {
				Interface.Instantiate (this.victory);
			} else {
				Interface.Instantiate (this.failed);
			}
		}

		private void OnCameraEnd(ViceCamera.TargetType type) {
			if (type != ViceCamera.TargetType.Opening) {
				Interface.Instantiate (this.count);
			} else {
				Interface.Instantiate (this.logo);
			}
		}
	}
}