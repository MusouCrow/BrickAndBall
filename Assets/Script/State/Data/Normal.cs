using System;
using UnityEngine;

namespace Game.State.Data {
	using Utility;

	[CreateAssetMenuAttribute(menuName="Game/State/Normal")]
	public class Normal : State.Data {
		[SerializeField]
		protected float _coolDownTime;
		[SerializeField]
		protected AudioClip _clip;

		public float coolDownTime {
			get { return this._coolDownTime;}
		}

		public AudioClip clip {
			get { return this._clip;}
		}
	}
}

