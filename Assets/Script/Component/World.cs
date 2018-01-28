using System.Collections.Generic;
using UnityEngine;
using Jitter.Collision;
using Jitter.LinearMath;

using JWorld = Jitter.World;
using JRigidbody = Jitter.Dynamics.RigidBody;
using Rigidbody = Game.Utility.Rigidbody;

namespace Game.Component {
    public class World : MonoBehaviour {
        private static World INSTANCE;

        public static void AddBody(Rigidbody rigidbody) {
            INSTANCE.world.AddBody(rigidbody);
        }

        private JWorld world;

        protected void Awake() {
            INSTANCE = this;

            this.world = new JWorld(new CollisionSystemSAP());
            this.world.CollisionSystem.CollisionDetected += this.CollisionDetected;
        }

        protected void FixedUpdate() {
            this.world.Step(0.017f, false);
        }

        private void CollisionDetected(JRigidbody body1, JRigidbody body2, JVector point1, JVector point2, JVector normal, float penetration) {
            var b1 = body1 as Rigidbody;
            var b2 = body2 as Rigidbody;
            
            b1.collider.CollisionDetected(b2.collider);
        }
    }
}