using System;
using UnityEngine;

namespace Game.State.Data {
	using Utility;

	[CreateAssetMenuAttribute(menuName="Game/State/Brick_Elast")]
	public class Brick_Elast : State.Data {
		[SerializeField]
		protected Vector2 _scaling;
		[SerializeField]
		protected Vector2 _positioning;
		[SerializeField]
		protected float _time;
		[SerializeField]
		protected float _power;
		[SerializeField]
		protected AudioClip _clip;
		[SerializeField]
		protected string _nextState;

		public Vector2 scaling {
			get {return this._scaling;}
		}

		public Vector2 positioning {
			get {return this._positioning;}
		}

		public float time {
			get {return this._time;}
		}

		public float power {
			get {return this._power;}
		}

		public AudioClip clip {
			get {return this._clip;}
		}

		public string nextState {
			get {return this._nextState;}
		}
	}
}

