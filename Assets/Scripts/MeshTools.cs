using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MeshTools {
    /// <summary>Get the MeshFilter on a GameObject.</summary>
    public static Mesh Get(GameObject g) {
        return g.GetComponent<MeshFilter>().mesh;
    }

    /// <summary>Make a copy of the Mesh.</summary>
    public static Mesh Clone(Mesh originalMesh) {
        var clonedMesh = new Mesh();

        clonedMesh.name = "clone";
        clonedMesh.vertices = originalMesh.vertices;
        clonedMesh.triangles = originalMesh.triangles;
        clonedMesh.normals = originalMesh.normals;
        clonedMesh.uv = originalMesh.uv;

        return clonedMesh;
    }

    /// <summary>Cut mesh along bisector line and keep only the half containing the center point.</summary> 
    public static TempMesh Cut(Mesh mesh, Line bisector, Vector2 center) {
        var initialArraySize = 256;
        TempMesh tempMesh = new TempMesh(initialArraySize);
        TempMesh trashMesh = new TempMesh(initialArraySize);
        var intersect = new Intersections();

        var addedPairs = new List<Vector3>(initialArraySize);
        var ogVertices = new List<Vector3>(initialArraySize);
        var ogNormals = new List<Vector3>(initialArraySize);
        var ogUvs = new List<Vector2>(initialArraySize);
        var ogTriangles = new List<int>(initialArraySize * 3);
        var intersectPair = new Vector3[2];

        var tempTriangle = new Vector3[3];

        // Let's always fill the vertices array so that we can access it even if the mesh didn't intersect
        mesh.GetVertices(ogVertices);
        mesh.GetTriangles(ogTriangles, 0);
        mesh.GetNormals(ogNormals);
        mesh.GetUVs(0, ogUvs);

        tempMesh.Clear();
        trashMesh.Clear();

        for (int i = 0; i < ogVertices.Count; ++i) {
            var test = bisector.isLeft(ogVertices[i]);
            // Debug.Log(test);
            if (test) {
                tempMesh.AddVertex(ogVertices, ogNormals, ogUvs, i);
            } else {
                trashMesh.AddVertex(ogVertices, ogNormals, ogUvs, i);
            }
        }

        Plane slice = bisector.GetPlane();

        // 3. Separate triangles and cut those that intersect the plane
        for (int i = 0; i < ogTriangles.Count; i += 3) {
            if (intersect.TrianglePlaneIntersect(ogVertices, ogUvs, ogTriangles, i, ref slice, tempMesh, trashMesh, intersectPair)) {

                addedPairs.AddRange(intersectPair);
            }
        }

        if (addedPairs.Count > 0) {
            //FillBoundaryGeneral(addedPairs);
            MeshTools.FillBoundaryFace(tempMesh, addedPairs, ref tempTriangle);
            return tempMesh;
        } else {
            throw new UnityException("Error: if added pairs is empty, we should have returned false earlier");
        }

        return tempMesh;
    }

    /// <summary>Fill all empty holes in a mesh.</summary> 
    public static void FillHoles(Mesh mesh) {
        // TODO
    }

    /// <summary> Save the Mesh as a new GameObject</summary>
    public static GameObject Export(TempMesh tempMesh, string name) {
        var g = new GameObject(name);

        var meshFilter = g.AddComponent<MeshFilter>();
        meshFilter.mesh.Clear();
        meshFilter.mesh.SetVertices(tempMesh.vertices);
        meshFilter.mesh.SetTriangles(tempMesh.triangles, 0);
        meshFilter.mesh.SetNormals(tempMesh.normals);
        meshFilter.mesh.SetUVs(0, tempMesh.uvs);

        //mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateTangents();

        // var rigidbody = g.AddComponent<Rigidbody>();
        // rigidbody.useGravity = false;

        // var collider = g.AddComponent<MeshCollider>();
        // collider.convex = true;
        // collider.sharedMesh = mesh;

        return g;
    }

    /// <summary>
    /// Replace the mesh with tempMesh.
    /// </summary>
    public static void ReplaceMesh(Mesh mesh, TempMesh tempMesh) {
        mesh.Clear();
        mesh.SetVertices(tempMesh.vertices);
        mesh.SetTriangles(tempMesh.triangles, 0);
        mesh.SetNormals(tempMesh.normals);
        mesh.SetUVs(0, tempMesh.uvs);

        //mesh.RecalculateNormals();
        mesh.RecalculateTangents();

    }

    // helpers from https://github.com/hugoscurti/mesh-cutter

    /// <summary>
    /// Extract polygon from the pairs of vertices.
    /// Per example, two vectors that are colinear is redundant and only forms one side of the polygon
    /// </summary>
    static private List<Vector3> FindRealPolygon(List<Vector3> pairs) {
        List<Vector3> vertices = new List<Vector3>();
        Vector3 edge1, edge2;

        // List should be ordered in the correct way
        for (int i = 0; i < pairs.Count; i += 2) {
            edge1 = (pairs[i + 1] - pairs[i]);
            if (i == pairs.Count - 2)
                edge2 = pairs[1] - pairs[0];
            else
                edge2 = pairs[i + 3] - pairs[i + 2];

            // Normalize edges
            edge1.Normalize();
            edge2.Normalize();

            float threshold = 1e-6f;

            if (Vector3.Angle(edge1, edge2) > threshold)
                // This is a corner
                vertices.Add(pairs[i + 1]);
        }

        return vertices;
    }

    static private void AddTriangle(TempMesh PositiveMesh, ref Vector3[] tempTriangle, List<Vector3> face, int t1, int t2, int t3) {
        tempTriangle[0] = face[t1];
        tempTriangle[1] = face[t2];
        tempTriangle[2] = face[t3];
        PositiveMesh.AddTriangle(tempTriangle);

        tempTriangle[1] = face[t3];
        tempTriangle[2] = face[t2];
        // NegativeMesh.AddTriangle(tempTriangle);
    }

    static private void FillBoundaryFace(TempMesh tempMesh, List<Vector3> added, ref Vector3[] tempTriangle) {
        // 1. Reorder added so in order ot their occurence along the perimeter.
        MeshUtils.ReorderList(added);

        // 2. Find actual face vertices
        var face = FindRealPolygon(added);

        // 3. Create triangle fans
        int t_fwd = 0,
            t_bwd = face.Count - 1,
            t_new = 1;
        bool incr_fwd = true;

        while (t_new != t_fwd && t_new != t_bwd) {
            MeshTools.AddTriangle(tempMesh, ref tempTriangle, face, t_bwd, t_fwd, t_new);

            if (incr_fwd)t_fwd = t_new;
            else t_bwd = t_new;

            incr_fwd = !incr_fwd;
            t_new = incr_fwd ? t_fwd + 1 : t_bwd - 1;
        }
    }
}