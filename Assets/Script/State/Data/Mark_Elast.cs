using System;
using UnityEngine;

namespace Game.State.Data {
	using Utility;

	[CreateAssetMenuAttribute(menuName="Game/State/Mark_Elast")]
	public class Mark_Elast : State.Data {
		[SerializeField]
		protected string _nextState;

		public string nextState {
			get {return this._nextState;}
		}
	}
}

