using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Game.Component.UI {
	using Utility;

	public class ViceButton : MonoBehaviour {
		private static Vector3 MIRAGE_SCALE = new Vector3 (3, 3, 3);
		private const float SHOW_TIME = 0.3f;

		private static void SetAlpha (Graphic graphic, float value) {
			Color color = graphic.color;
			color.a = value;
			graphic.color = color;
		}

		[SerializeField]
		private AudioClip clickClip;
		[SerializeField]
		private AudioClip showClip;
		[SerializeField]
		private GameObject next;
		[SerializeField]
		private Slot[] onClickSlots;

		private Button button;
		private Image image;
		private Text text;

		protected void Awake () {
			this.image = this.GetComponent<Image> ();
			this.text = this.GetComponentInChildren<Text> ();

			Color imageColor = this.image.color;
			Color textColor = this.text.color;

			ViceButton.SetAlpha (this.image, 0);
			ViceButton.SetAlpha (this.text, 0);

			this.image.DOColor (imageColor, SHOW_TIME)
				.SetEase(Ease.Linear)
				.OnComplete (this.OnShow);
			this.text.DOColor (textColor, SHOW_TIME)
				.SetEase (Ease.Linear);

			this.button = this.GetComponent<Button> ();
			this.button.onClick.AddListener (this.OnClick);
			this.button.enabled = false;

			Sound.Play (this.showClip);
		}

		private void OnShow () {
			this.button.enabled = true;

			if (this.next != null) {
				GameObject.Instantiate (this.next, this.transform.parent);
			}
		}

		private void OnClick () {
			Sound.Play (this.clickClip);
			Mirage.New (this.transform, this.transform.parent, this.image, MIRAGE_SCALE, 0.5f);

			for (int i = 0; i < this.onClickSlots.Length; i++) {
				this.onClickSlots [i].Run (this.gameObject);
			}
		}
	}
}