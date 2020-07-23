using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MeshTools {
    // Get the MeshFilter on a GameObject
    public static Mesh GetMesh(GameObject g) {
        return g.GetComponent<MeshFilter>().mesh;
    }

    public static Mesh Clone(Mesh originalMesh) {
        var clonedMesh = new Mesh();

        clonedMesh.name = "clone";
        clonedMesh.vertices = originalMesh.vertices;
        clonedMesh.triangles = originalMesh.triangles;
        clonedMesh.normals = originalMesh.normals;
        clonedMesh.uv = originalMesh.uv;

        return clonedMesh;

    }
}