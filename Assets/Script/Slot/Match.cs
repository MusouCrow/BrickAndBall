using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Slot {
	using Component;
	using Component.UI;
	using Component.Network;

	[CreateAssetMenuAttribute(menuName="Game/Slot/Match")]
	public class Match : Utility.Slot {
		[SerializeField]
		private bool isStart;
		[SerializeField]
		private GameObject next;
		[SerializeField]
		private float wattingTime;

		public override void Run(GameObject gameObject) {
			Interface.Clear(this.wattingTime, this.OnComplete, true);
		}

		private void OnComplete() {
			/*
			if (this.isStart) {
				Networkmgr.StartMatch();
			} else {
				Networkmgr.ExitMatch();
			}
			*/
			
			if (this.isStart) {
				if (Application.isEditor) {
					NetworkManager.singleton.StartHost();
				}
				else {
					NetworkManager.singleton.StartClient();
				}
			} else {
				NetworkManager.singleton.StopHost();
			}

			Interface.Instantiate(this.next);
		}
	}
}

