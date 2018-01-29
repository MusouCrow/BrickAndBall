using UnityEngine;

namespace Game.Component.Network {
    public class LockBehaviour : MonoBehaviour {
        protected void Awake() {
            Client.LockUpdateEvent += this.LockUpdate;
        }
        
        protected void OnDestroy() {
            Client.LockUpdateEvent -= this.LockUpdate;
        }

        protected virtual void LockUpdate() {}
    }
}