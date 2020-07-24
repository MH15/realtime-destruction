using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct Line {
    // public Line(Vector2 origin, float slope) {

    // }
    public Vector2 origin;
    public float slope;
    // public float angle; // degrees

    public bool isLeft(Vector2 c) {

        float angle = Mathf.Atan2(slope, 1);
        // Debug.Log(Mathf.Rad2Deg * angle);

        Vector2 a = new Vector2(
            1000 * Mathf.Sin(angle),
            1000 * Mathf.Cos(angle)
        );
        Vector2 b = new Vector2(-1000 * Mathf.Sin(angle), -1000 * Mathf.Cos(angle));

        return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
    }

    public Plane GetPlane() {
        float angle = Mathf.Atan2(slope, 1);
        // Debug.Log(Mathf.Rad2Deg * angle);

        Vector2 a = new Vector2(
            1000 * Mathf.Sin(angle),
            1000 * Mathf.Cos(angle)
        );
        Vector2 b = new Vector2(-1000 * Mathf.Sin(angle), -1000 * Mathf.Cos(angle));

        Vector3 A = new Vector3(a.x, -100, a.y);
        Vector3 B = new Vector3(b.x, -100, b.y);
        Vector3 C = new Vector3(a.x, 100, a.y);

        return new Plane(A, B, C);
    }
}