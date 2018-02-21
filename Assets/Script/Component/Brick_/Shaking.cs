using System;
using UnityEngine;
using DG.Tweening;

namespace Game.Component.Brick_ {
	using Utility;

    public class Shaking {
        private static Vector4 POWER = new Vector4(0.1f, 0, 0, 0.2f);
        
        public Vector3 position;
        private Vector3 power;
        private Tweener tweener;
        private Action AdjustPosition;

        public Shaking(int direction, Action Callback) {
            this.power = POWER * direction;
            this.AdjustPosition = Callback;
        }

        public void Collide() {
            if (this.tweener == null || !this.tweener.IsPlaying()) {
                this.tweener = DOTween.Punch(this.GetPosition, this.SetPosition, this.power, POWER.w);
            }
        }

        private Vector3 GetPosition() {
            return this.position;
        }

        private void SetPosition(Vector3 value) {
            this.position = value.ToFixed();
            this.AdjustPosition();
        }
    }
}