using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class Sound : MonoBehaviour {
		private static Sound INSTANCE;

		public static void PlayMusic (AudioClip clip=null) {
			INSTANCE.targetClip = clip;
			INSTANCE.audioSource.volume = INSTANCE.volume;
			INSTANCE.timer.Enter (INSTANCE.gradualTime);
			INSTANCE.process = 0;
		}

		public static void SetMusicPitch (float pitch) {
			INSTANCE.audioSource.pitch = pitch;
		}

		public static bool MusicIsPlaying () {
			return INSTANCE.audioSource.isPlaying;
		}

		public static void Play (AudioClip clip, float volume=1) {
			AudioSource.PlayClipAtPoint (clip, Vector3.zero, volume);
		}

		public float gradualTime = 0.5f;
		[Range(0, 1)]
		public float volume = 1;

		private AudioSource audioSource;
		private Timer timer;
		private AudioClip targetClip;
		private int process;

		private void Awake () {
			INSTANCE = this;
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