using System;
using UnityEngine;

namespace Game {
	using Component;

	public static class Lib {
		private static ViceCamera camera;

		public static void BindCamera (ViceCamera camera) {
			Lib.camera = camera;
		}

		public static void Shake (Vector3 power, float time) {
			if (Lib.camera != null) {
				Lib.camera.Shake (power, time);
			}
		}

		public static void Shake (Vector4 value) {
			Lib.Shake (value, value.w);
		}
	}
}

