using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Component {
	public class Shooter : NetworkBehaviour {
		public delegate void OnShootDelegate (GameObject obj);
		public static event OnShootDelegate OnShootEvent;

		[SerializeField]
		private GameObject ball;
		[SerializeField]
		private AudioClip clip;
		[SerializeField]
		private Vector4 shakingValue;

		private GameObject NewBall () {
			GameObject obj = GameObject.Instantiate (this.ball, this.transform.localPosition, Quaternion.identity, this.transform.parent) as GameObject;
			obj.GetComponent<Rigidbody> ().AddForce (this.transform.localScale, ForceMode.VelocityChange);
		
			return obj;
		}

		private void Effect () {
			Sound.Play (this.clip);
			ViceCamera.Shake (this.shakingValue);
		}

		public void Shoot () {
			if (Judge.GetGameType () != Judge.GameType.PVP) {
				if (Shooter.OnShootEvent != null) {
					Shooter.OnShootEvent (this.NewBall ());
				}

				this.Effect ();
			} else {
				this.CmdShoot ();
			}
		}

		[Command]
		private void CmdShoot () {
			GameObject obj = this.NewBall ();
			NetworkServer.Spawn (obj);
			this.RpcShoot ();
		}

		[ClientRpc]
		private void RpcShoot () {
			this.Effect ();

			if (Shooter.OnShootEvent != null) {
				Shooter.OnShootEvent (GameObject.FindWithTag("Ball"));
			}
		}
	}
}