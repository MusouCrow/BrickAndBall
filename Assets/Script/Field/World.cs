using System.Collections.Generic;
using UnityEngine;
using Jitter.Collision;
using Jitter.LinearMath;

using JWorld = Jitter.World;
using JRigidbody = Jitter.Dynamics.RigidBody;

namespace Game.Field {
    using Network;
    using Utility;

    public class World : LockBehaviour {
        private static World INSTANCE;

        public static void AddBody(JRigidbody rigidbody) {
            INSTANCE.world.AddBody(rigidbody);
        }

        public static void RemoveBody(JRigidbody rigidbody) {
            INSTANCE.world.RemoveBody(rigidbody);
        }

        private JWorld world;

        protected new void Awake() {
            base.Awake();

            INSTANCE = this;
            this.world = new JWorld(new CollisionSystemPersistentSAP());
            this.world.CollisionSystem.CollisionDetected += this.CollisionDetected;
        }

        protected override void LockUpdate() {
            this.world.Step(Networkmgr.STDDT, false);
        }

        private void CollisionDetected(JRigidbody body1, JRigidbody body2, JVector point1, JVector point2, JVector normal, float penetration) {
            var b1 = body1 as Collider.Rigidbody;
            var b2 = body2 as Collider.Rigidbody;
            
            b1.collider.CollisionDetected(b2.collider, point1.ToVector3());
            b2.collider.CollisionDetected(b1.collider, point2.ToVector3());
        }
    }
}