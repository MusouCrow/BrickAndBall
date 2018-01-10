using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Component {
	using Utility;

	public class Player : NetworkBehaviour {
		private static Brick BRICK_A;
		private static Brick BRICK_B;

		private static void Preload () {
			BRICK_A = BRICK_A == null ? GameObject.Find ("Field/BrickA").GetComponent<Brick> () : BRICK_A;
			BRICK_B = BRICK_B == null ? GameObject.Find ("Field/BrickB").GetComponent<Brick> () : BRICK_B;
		}

		public Slot onStartSlot;

		private Brick brick;

		protected void Awake () {
			Player.Preload ();

			this.brick = BRICK_A.GetPlayer () == null ? BRICK_A : BRICK_B;
			this.brick.SetPlayer (this);

			this.transform.localPosition = this.brick.transform.localPosition;
			this.transform.localScale = this.brick.transform.localScale;

			if (this.brick == BRICK_B) {
				this.onStartSlot.Run (this.gameObject);
			}
		}

		protected void OnDestroy () {
			this.brick.SetPlayer ();
		}
	}
}

