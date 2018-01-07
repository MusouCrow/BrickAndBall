using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.Component {
	using Utility;

	public class Sound : MonoBehaviour {
		private static Sound INSTANCE;

		public static void PlayMusic (AudioClip clip=null) {
			INSTANCE.targetClip = clip;
			INSTANCE.audioSource.volume = INSTANCE.volume;

			if (INSTANCE.sequence != null && INSTANCE.sequence.IsPlaying ()) {
				INSTANCE.sequence.Complete ();
			}

			INSTANCE.sequence = DOTween.Sequence ();
			Tweener t1 = INSTANCE.MoveVolume (0);
			Tweener t2 = INSTANCE.MoveVolume (INSTANCE.volume);

			INSTANCE.sequence.Append (t1);
			INSTANCE.sequence.AppendCallback (INSTANCE.StartPlay);
			INSTANCE.sequence.Append (t2);
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

		[SerializeField]
		private float gradualTime = 0.5f;
		[SerializeField]
		[Range(0, 1)]
		private float volume = 1;

		private AudioSource audioSource;
		private AudioClip targetClip;
		private Sequence sequence;

		private void Awake () {
			INSTANCE = this;
			this.audioSource = this.GetComponent<AudioSource> ();
		}

		private Tweener MoveVolume (float target) {
			return DOTween.To (INSTANCE.GetVolume, INSTANCE.SetVolume, target, INSTANCE.gradualTime);
		}

		private float GetVolume () {
			return this.audioSource.volume;
		}

		private void SetVolume (float volume) {
			this.audioSource.volume = volume;
		}

		private void StartPlay () {
			this.audioSource.clip = this.targetClip;
			this.audioSource.pitch = 1;
			this.audioSource.Play ();
		}
	}
}