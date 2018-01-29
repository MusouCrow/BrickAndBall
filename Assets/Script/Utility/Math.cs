using System;
using System.Text;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using Jitter.LinearMath;

namespace Game.Utility {
    public static class Math {
        public static Vector3 ToVector3(this JVector vector) {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static JVector ToJVector(this Vector3 vector) {
            return new JVector(vector.x, vector.y, vector.z);
        }

        public static float ToFixed(this float value) {
            int direction = value >= 0 ? 1 : -1;

            return Mathf.Floor(Mathf.Abs(value * 1000)) * 0.001f * direction;
        }

        public static Vector3 ToFixed(this Vector3 value) {
            value.x = value.x.ToFixed();
            value.y = value.y.ToFixed();
            value.z = value.z.ToFixed();

            return value;
        }

        public static JVector ToFixed(this JVector value) {
            value.X = value.X.ToFixed();
            value.Y = value.Y.ToFixed();
            value.Z = value.Z.ToFixed();

            return value;
        }

        public static Tweener MoveFixedFloat(DOSetter<float> setter, float startValue, float endValue, float duration) {
            float v = startValue;
            
            return DOTween.To(() => v, (float x) => {v = x.ToFixed(); setter(v);}, endValue, duration);
        }

        public static string BytesToStr(byte[] bytes) {
            var sb = new StringBuilder(); 

            for (int i = 0; i < bytes.Length; i++) {
                sb.Append(bytes[i]);
            }

            return sb.ToString();
        }

        public static string ToBinStr(this float value) {
            return BytesToStr(BitConverter.GetBytes(value));
        }
    }
}