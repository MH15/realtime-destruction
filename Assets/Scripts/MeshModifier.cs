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

    }

    void drawSites(List<Vector2> sites, Vector3 vec3center) {
        var size = GetComponent<MeshRenderer>().bounds.size;
        foreach (var site in sites) {
            Vector3 pos = new Vector3(site.x, site.y, -1.5f);
            pos -= size / 2 - vec3center;
            Instantiate(debugSphere, pos, Quaternion.identity, this.transform);
        }
    }

    void Fracture(Vector2 center) {
        var vec3center = new Vector3(center.x, center.y);
        print("Generating sites around " + center.ToString("F3"));
        var sites = VoronoiHelpers.GenerateSites(canvasSize, 30, center);
        drawSites(sites, vec3center);

        var points = new List<VoronoiLib.Structures.FortuneSite>();

        foreach (var site in sites) {
            // print(site.ToString("F4"));
            // move all input points to Quadrant 1
            var offsetSite = site;
            // offsetSite.x -= (canvasSize.x / 2);
            // offsetSite.y -= (canvasSize.y / 2);
            points.Add(new VoronoiLib.Structures.FortuneSite(offsetSite.x, offsetSite.y));
        }

        //FortunesAlgorithm.Run(points, min x, min y, max x, max y)
        LinkedList<VoronoiLib.Structures.VEdge> cuttingEdges = VoronoiLib.FortunesAlgorithm.Run(points, 0, 0, 800, 800);

        print(cuttingEdges.Count);
        foreach (var edge in cuttingEdges) {
            var start = new Vector3((float)edge.Start.X, (float)edge.Start.Y, -1.5f);
            var end = new Vector3((float)edge.End.X, (float)edge.End.Y, -1.5f);

            // var startSpot = start;
            // startSpot.y -= transform.position.y;
            // var endSpot = end;
            // endSpot.y -= transform.position.y;

            // offset input points by the object dimensions
            // TODO (done?)
            start -= size / 2 - vec3center;
            end -= size / 2 - vec3center;
            Debug.DrawLine(start, end, Color.black, 1000);
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