using UnityEngine;
using Jitter.Collision;
using JWorld = Jitter.World;
using JRigidbody = Jitter.Dynamics.RigidBody;

namespace Game.Component {
    public class World : MonoBehaviour {
        private static World INSTANCE;

        public static void AddBody(JRigidbody body) {
            INSTANCE.world.AddBody(body);
        }

        private JWorld world;

        protected void Awake() {
            INSTANCE = this;
            this.world = new JWorld(new CollisionSystemSAP());
        }

        protected void LateUpdate() {
            this.world.Step(0.017f, true);
        }
    }
}