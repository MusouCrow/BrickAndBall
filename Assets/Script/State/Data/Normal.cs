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
		protected float _judgingInterval;
		protected float _judgingRange;

		public float coolDownTime {
			get {return _coolDownTime;}
		}

		public AudioClip clip {
			get {return _clip;}
		}

		public float judgingInterval {
			get {return _judgingInterval;}
		}

		public float judgingRange {
			get {return _judgingRange;}
		}
	}
}

