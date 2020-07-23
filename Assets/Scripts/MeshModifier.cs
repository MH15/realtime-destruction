﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoronoiLib;

public class MeshModifier : MonoBehaviour {
    private UnityEngine.Mesh mesh;
    private System.Random rng;
    public GameObject debugSphere;

    private Vector3 size;
    private Vector2 canvasSize;
    private Vector3 vec3center;
    // Start is called before the first frame update
    void Start() {
        rng = new System.Random();
        mesh = GetComponent<MeshFilter>().mesh;

        size = GetComponent<MeshRenderer>().bounds.size;
        canvasSize = new Vector2(size.x, size.y);
        print(canvasSize);

        // MeshHelper.Subdivide(mesh, 6); // divides a single quad into 6x6 quads

        // Vector3[] vertices = mesh.vertices;
        // Vector3[] normals = mesh.normals;

        // for (var i = 0; i < vertices.Length; i++) {
        //     vertices[i] += normals[i] * Mathf.Sin((float)i) / 4f;
        // }
        // mesh.vertices = vertices;

        Fracture(new Vector2(0, 0));
    }

    void Fracture(Vector2 center) {
        vec3center = new Vector3(center.x, center.y);
        print("Generating sites around " + center.ToString("F3"));
        var sites = VoronoiHelpers.GenerateSites(canvasSize, 30, center);
        drawSites(sites);

        var points = new List<VoronoiLib.Structures.FortuneSite>();

        foreach (var site in sites) {
            // move all input points to Quadrant 1
            var offsetSite = site;
            points.Add(new VoronoiLib.Structures.FortuneSite(offsetSite.x, offsetSite.y));
        }

        //FortunesAlgorithm.Run(points, min x, min y, max x, max y)
        LinkedList<VoronoiLib.Structures.VEdge> cuttingEdges = VoronoiLib.FortunesAlgorithm.Run(points, 0, 0, 800, 800);

        // HERE !@!@$@!%$@!%
        var a = points[0];
        print(string.Format("x: {0}, y: {1}", a.X, a.Y));

        var delaunay = VoronoiHelpers.GenerateDelaunay(points);

        StartCoroutine(DrawVoronoiCoroutine(cuttingEdges));
        StartCoroutine(DrawDelaunayCoroutine(delaunay));

        // Sutherland–Hodgman algorithm for slicing concave shapes
        foreach (var point in points) {
            // Duplicate the base mesh
            var piece = duplicateMesh(original);

            foreach (var neighbor in point.Neighbors) {
                // Find the perpendicular bisector and slice
                var bisector = findBisector(point, neighbor);
                // Extend the bisector clipping line "infinitely"
                var extendedBisector = extendLine(bisector);

                // Keep vertices from this side of the bisector line
                MeshHelper.cutMesh(piece, extendedBisector);
                // Fill the hole
                MeshHelper.fillMesh(piece);
            }

            // Now we have the final chunk

        }

        print("length: " + points.Count);
        // getEdges(points[5]);
        // getEdges(points[10]);
        // getEdges(points[20]);
        getEdges(points[25]);

    }

    void getEdges(VoronoiLib.Structures.FortuneSite point) {
        Vector3 pos = new Vector3((float)point.X, (float)point.Y, -1.5f);
        var a = Instantiate(debugSphere, pos, Quaternion.identity);
        a.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

    void drawSites(List<Vector2> sites) {
        var size = GetComponent<MeshRenderer>().bounds.size;
        int i = 0;
        foreach (var site in sites) {
            Vector3 pos = new Vector3(site.x, site.y, -1.5f);
            pos -= size / 2 - vec3center;
            var a = Instantiate(debugSphere, pos, Quaternion.identity);
            a.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            a.name = "Site " + i;
            i++;
        }
    }

    IEnumerator DrawDelaunayCoroutine(List<Tuple<Vector2, Vector2>> edges) {
        foreach (var edge in edges) {
            var start = new Vector3((float)edge.Item1.x, (float)edge.Item1.y, -1.5f);
            var end = new Vector3((float)edge.Item2.x, (float)edge.Item2.y, -1.5f);

            // offset input points by the object dimensions
            start -= size / 2 - vec3center;
            end -= size / 2 - vec3center;

            // Draw cell borders
            Debug.DrawLine(start, end, Color.green, 1000);
            //yield on a new YieldInstruction that waits for 5 seconds.
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator DrawVoronoiCoroutine(LinkedList<VoronoiLib.Structures.VEdge> cuttingEdges) {
        Color wallColor = new Color(0f, 0f, 0f, 1f);

        foreach (var edge in cuttingEdges) {
            var start = new Vector3((float)edge.Start.X, (float)edge.Start.Y, -1.5f);
            var end = new Vector3((float)edge.End.X, (float)edge.End.Y, -1.5f);

            // offset input points by the object dimensions
            start -= size / 2 - vec3center;
            end -= size / 2 - vec3center;

            wallColor.r += .1f;

            // Draw cell borders
            Debug.DrawLine(start, end, wallColor, 1000);
            //yield on a new YieldInstruction that waits for 5 seconds.
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Update is called once per frame
    void Update() {
        // Vector3[] vertices = mesh.vertices;
        // Vector3[] normals = mesh.normals;

        // for (var i = 0; i < vertices.Length; i++) {
        //     print(vertices[i]);
        // }

        // mesh.vertices = vertices;

    }

    void OnCollisionEnter(Collision collision) {
        foreach (ContactPoint contact in collision.contacts) {
            if (collision.gameObject.name == "Bullet(Clone)") {
                Fracture(contact.point);

            }
            Debug.DrawRay(contact.point, contact.normal * 5, Color.red, 4);
        }
    }

}