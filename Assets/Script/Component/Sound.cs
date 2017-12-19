using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class Sound : MonoBehaviour {
		private static Sound instance;

		public static void PlayMusic (AudioClip clip) {
			instance.targetClip = clip;
			instance.timer.Enter (instance.gradualTime);
			instance.process = 0;
		}

		public static void SetMusicPitch (float pitch) {
			instance.audioSource.pitch = pitch;
		}

		public static bool MusicIsPlaying () {
			return instance.audioSource.isPlaying;
		}

		public static void Play (AudioClip clip, float volume=1) {
			AudioSource.PlayClipAtPoint (clip, Vector3.zero, volume);
		}

		public float gradualTime = 0.5f;
		[Range(0, 1)]
		public float volume = 0.3f;

		private AudioSource audioSource;
		private Timer timer;
		private AudioClip targetClip;
		private int process;

		private void Awake () {
			Sound.instance = this;
			this.audioSource = this.GetComponent<AudioSource> ();
			this.timer = new Timer ();
		}

		private void FixedUpdate () {
			if (this.timer.IsRunning ()) {
				this.timer.Update (Time.fixedDeltaTime);

				float target = 0;

				if (this.process == 1) {
					target = this.volume;
				}

				this.audioSource.volume = Mathf.Lerp (this.audioSource.volume, target, this.timer.GetProcess ());

				if (!this.timer.IsRunning ()) {
					this.process += 1;

					if (this.process == 1) {
						this.audioSource.clip = this.targetClip;
						this.audioSource.pitch = 1;
						this.audioSource.Play ();
						this.timer.Enter (this.gradualTime);
					}
				}
			}
		}
	}
}