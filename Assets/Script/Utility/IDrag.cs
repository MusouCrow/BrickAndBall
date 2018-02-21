using System;
using UnityEngine;

public interface IDrag {
    event Action<Vector3, Vector3> OnDragEvent;
    void Drag(Vector3 position, bool isDown);
}