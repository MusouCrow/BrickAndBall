using System;
using System.Collections.Generic;
using UnityEngine;
using Jitter.LinearMath;
using Jitter.Collision;
using Jitter.Collision.Shapes;

using UCollider = UnityEngine.Collider;
using JMaterial = Jitter.Dynamics.Material;
using Rigidbody = Game.Utility.Rigidbody;

namespace Game.Component {
    using Utility;
    using Network;

    public class Collider : LockBehaviour {
        public delegate void Delegate(Collider collider);
        private enum CollisionState {
            Enter,
            Stay,
            Exit
        }

        [SerializeField]
        private bool isStatic;

        public event Delegate CollisionEnterEvent;
        public event Delegate CollisionStayEvent;
        public event Delegate CollisionExitEvent;
        private Dictionary<Collider, CollisionState> collisionMap;
        private List<Collider> collisionList;
        private Vector3 size;
        private Shape shape;
        private Rigidbody body;

        public Vector3 Position {
            get {
                return this.body.Position.ToVector3();
            }
            set {
                this.body.position = value.ToJVector();
                this.AdjustPosition();
            }
        }

        public Vector3 Velocity {
            get {
                return this.body.LinearVelocity.ToVector3();
            }
            set {
                this.body.LinearVelocity = value.ToFixed().ToJVector();
            }
        }

        public Vector3 Scale {
            set {
                value = value.ToFixed();
                this.transform.localScale = value;

                if (this.body.Shape is SphereShape) {
                    var sphereShape = this.body.Shape as SphereShape;
                    sphereShape.Radius = this.size.x * value.x;
                }
                else {
                    var boxShape = this.body.Shape as BoxShape;
                    boxShape.Size = new JVector(value.x * this.size.x, value.y * this.size.y, value.z * this.size.z);
                }
            }
        }

        public bool IsParticle {
            set {
                this.body.IsParticle = value;
            }
        }

        protected new void Awake() {
            this.orderType = OrderType.Late;
            base.Awake();

            this.collisionMap = new Dictionary<Collider, CollisionState>();
            this.collisionList = new List<Collider>();

            var collider = this.GetComponent<UCollider>();

            if (collider is BoxCollider) {
                var boxCollider = collider as BoxCollider;
                var size = boxCollider.size;

                for (int i = 0; i < 3; i++) {
                    size[i] *= this.transform.lossyScale[i] / this.transform.localScale[i];
                }

                this.shape = new BoxShape(size.ToJVector());
                this.size = size;
            }
            else if (collider is SphereCollider) {
                var sphereCollider = collider as SphereCollider;
                this.shape = new SphereShape(sphereCollider.radius);
                this.size = new Vector3(sphereCollider.radius, sphereCollider.radius, sphereCollider.radius);
            }
            else {
                var size = collider.bounds.size;
                size.x /= this.transform.localScale.x;
                size.y /= this.transform.localScale.y;
                size.z /= this.transform.localScale.z;

                this.shape = new BoxShape(size.ToJVector());
                this.size = size;
            }

            Destroy(collider);
            this.size.ToFixed();
        }
        
        protected override void LockUpdate() {
            if (!this.body.IsStaticOrInactive) {
                this.AdjustPosition();
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

        protected void OnDrawGizmos() {
            if (!Application.isPlaying) {
                return;
            }

            var size = (this.body.Shape.BoundingBox.Max - this.body.Shape.BoundingBox.Min).ToVector3();
            Gizmos.DrawWireCube(this.body.Position.ToVector3(), size);
        }

        protected void OnEnable() {
            this.body = new Rigidbody(this, this.shape);
            this.body.IsStatic = this.isStatic;
            this.body.Position = this.transform.position.ToJVector();
            this.Scale = this.transform.localScale;
            World.AddBody(this.body);
        }

        protected void OnDisable() {
            World.RemoveBody(this.body);
            this.body = null;
        }

        public void AddForce(Vector3 power) {
            this.body.AddForce(power.ToJVector());
        }

        public void AdjustPosition() {
            this.body.Position = this.body.Position.ToFixed();
            this.transform.position = this.body.Position.ToVector3();
        }

        public void CollisionDetected(Collider collider) {
            if (this.CollisionEnterEvent == null && this.CollisionStayEvent == null && this.CollisionExitEvent == null) {
                return;
            }
            
            if (!this.collisionMap.ContainsKey(collider)) {
                this.collisionMap[collider] = CollisionState.Enter;
                this.collisionList.Add(collider);
            }
            else {
                this.collisionMap[collider] = CollisionState.Stay;
            }
        }

        public bool Pointcast(Vector3 point) {
            var jPoint = point.ToJVector();
            
            return GJKCollide.Pointcast(this.body.Shape, ref this.body.orientation, ref this.body.position, ref jPoint);
        }
    }
}