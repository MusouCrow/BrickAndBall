using System;
using System.Collections.Generic;
using UnityEngine;
using Jitter.LinearMath;
using Jitter.Collision.Shapes;
using Rigidbody = Game.Utility.Rigidbody;
using JRigidbody = Jitter.Dynamics.RigidBody;
using UCollider = UnityEngine.Collider;

namespace Game.Component {
    using Utility;

    public class Collider : MonoBehaviour {
        public delegate void Delegate(Rigidbody body);
        private enum CollisionState {
            Enter,
            Stay,
            Exit
        }

        public Delegate CollisionDetectedEvent;
        
        public event Delegate CollisionEnterEvent;
        public event Delegate CollisionStayEvent;
        public event Delegate CollisionExitEvent;

        [SerializeField]
        private bool isStatic;
        private Rigidbody body;
        private Dictionary<Rigidbody, CollisionState> collisionMap;
        private List<Rigidbody> collisionList;

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
            this.collisionMap = new Dictionary<Rigidbody, CollisionState>();
            this.collisionList = new List<Rigidbody>();

            var collider = this.GetComponent<UCollider>();
            Shape shape = null;

            if (collider is SphereCollider) {
                var sphereCollider = collider as SphereCollider;
                shape = new SphereShape(sphereCollider.radius);
            }
            else {
                shape = new BoxShape(collider.bounds.size.ToJVector());
            }

            this.body = new Rigidbody(this, shape);
            this.body.IsStatic = this.isStatic;
            this.body.Position = this.transform.localPosition.ToJVector();
            
            /*
            if (collider.material != null) {
                this.body.Material.kineticFriction = collider.material.dynamicFriction;
                this.body.Material.staticFriction = collider.material.staticFriction;
                this.body.Material.restitution = collider.material.bounciness;
            }
            */

            World.AddBody(this.body);
            World.CollisionSystem.CollisionDetected += this.CollisionDetected;

            //this.CollisionEnterEvent += (Rigidbody body) => print(this.name);
        }
        
        protected void FixedUpdate() {
            if (!this.body.IsStaticOrInactive) {
                this.body.position = this.body.position.ToFixed();
                this.transform.localPosition = this.body.position.ToVector3();
            }

            for (int i = this.collisionList.Count - 1; i > -1; i--) {
                var key = this.collisionList[i];
                var value = this.collisionMap[key];
                
                if (value == CollisionState.Enter) {
                    if (this.CollisionEnterEvent != null) {
                        this.CollisionEnterEvent(key);
                    }
                }
                else if (value == CollisionState.Exit) {
                    if (this.CollisionExitEvent != null) {
                        this.CollisionExitEvent(key);
                    }
                    
                    this.collisionMap.Remove(key);
                    this.collisionList.RemoveAt(i);
                    continue;
                }
                else {
                    if (this.CollisionStayEvent != null) {
                        this.CollisionStayEvent(key);
                    }
                }
                
                this.collisionMap[key] = CollisionState.Exit;
            }
        }

        private void CollisionDetected(JRigidbody body1, JRigidbody body2, JVector point1, JVector point2, JVector normal, float penetration) {
            if (body1 != this.body && body2 != this.body) {
                return;
            }

            var body = body1 == this.body ? body2 as Rigidbody : body1 as Rigidbody;
            
            if (!this.collisionMap.ContainsKey(body)) {
                this.collisionMap[body] = CollisionState.Enter;
                this.collisionList.Add(body);
            }
            else {
                this.collisionMap[body] = CollisionState.Stay;
            }
        }
    }
}