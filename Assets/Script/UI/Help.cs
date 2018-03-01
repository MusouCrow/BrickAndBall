using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Game.UI {
    using Network;
    using Utility;

    public class Help : LockBehaviour {
		[SerializeField]
		private GameObject next;
        [SerializeField]
        private Vector4 targetScale;
        [SerializeField]
        private AudioClip clip;

        private Tweener tweener;

        protected new void Awake() {
            base.Awake();

            this.transform.localScale = Vector3.zero;
            this.tweener = this.DOScale(this.targetScale, this.targetScale.w);
            Sound.Play(this.clip);
        }

        protected override void LockUpdate() {
            if (Input.GetMouseButtonDown(0)) {
                this.DOScale(Vector3.zero, this.targetScale.w)
                    .OnComplete(this.OnExit);
                Sound.Play(this.clip);
            }
        }

        private Tweener DOScale(Vector3 target, float time) {
            return this.transform.DOScale(target, time)
                .SetEase(Ease.InOutElastic);
        }

        private void OnExit() {
            Interface.Instantiate(this.next);
            Destroy(this.gameObject);
        }
    }
}
