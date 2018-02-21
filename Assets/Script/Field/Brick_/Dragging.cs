using System;
using UnityEngine;
using DG.Tweening;

namespace Game.Field.Brick_ {
	using Utility;

    public class Dragging {
        public event Action<Vector3, Vector3> OnDragEvent;
        private Vector3 position;
        private bool hasDraged;
        private Collider collider;

        public Dragging(Collider collider) {
            this.collider = collider;
        }

        public void Drag(Vector3 position, bool isDown) {
            if (!isDown) {
                this.hasDraged = false;
            }
            else if (!this.hasDraged && this.collider.Pointcast(position)) {
                this.hasDraged = true;
            }
            else if (this.hasDraged && this.OnDragEvent != null){
                this.OnDragEvent(this.position, position);
            }

            if (this.hasDraged) {
                this.position = position;
            }
        }
    }
}