using System;
using UnityEngine;
using Jitter.Collision.Shapes;
using JRigidbody = Jitter.Dynamics.RigidBody;
using UCollider = UnityEngine.Collider;

namespace Game.Component {
    using Utility;

    public class Collider : MonoBehaviour {
        [SerializeField]
        private bool isStatic;
        private JRigidbody body;

        public Vector3 Position {
            get {
                return this.body.Position.ToVector3();
            }
            set {
                this.body.Position = value.ToJVector();
            }
        }

        public Vector3 Velocity {
            get {
                return this.body.LinearVelocity.ToVector3();
            }
            set {
                this.body.LinearVelocity = value.ToJVector();
            }
        }

        public void AddForce(Vector3 power) {
            this.body.AddForce(power.ToJVector());
        }

        protected void Awake() {
            var collider = this.GetComponent<UCollider>();
            Shape shape = null;

            if (collider is BoxCollider) {
                var boxCollider = collider as BoxCollider;
                shape = new BoxShape(boxCollider.size.ToJVector());
            }
            else if (collider is MeshCollider) {
                shape = new BoxShape(collider.bounds.size.ToJVector());
            }
            else if (collider is SphereCollider) {
                var sphereCollider = collider as SphereCollider;
                shape = new SphereShape(sphereCollider.radius);
            }

            this.body = new JRigidbody(shape);
            this.body.IsStatic = this.isStatic;
            this.body.IsParticle = this.isStatic;
            this.body.Position = this.transform.localPosition.ToJVector();

            if (collider.material != null) {
                this.body.Material.kineticFriction = collider.material.dynamicFriction;
                this.body.Material.staticFriction = collider.material.staticFriction;
                this.body.Material.restitution = collider.material.bounciness;
            }

            World.AddBody(this.body);
        }
        
        protected void FixedUpdate() {
            if (!this.body.IsStaticOrInactive) {
                this.body.position = this.body.position.ToFixed();
                this.transform.localPosition = this.body.position.ToVector3();
            }
        }
    }
}