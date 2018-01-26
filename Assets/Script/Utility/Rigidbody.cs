using Jitter.Collision.Shapes;
using JRigidbody = Jitter.Dynamics.RigidBody;

namespace Game.Utility {
    using Component;

    public class Rigidbody : JRigidbody {
        public Collider collider;

        public Rigidbody(Collider collider, Shape shape) : base(shape) {
            this.collider = collider;
        }
    }
}
