using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoronoiLib;

public class VoronoiHelpers : MonoBehaviour {
    static System.Random rng = new System.Random();

    public static List<Tuple<Vector2, Vector2>> GenerateDelaunay(List<VoronoiLib.Structures.FortuneSite> points) {
        var processed = new HashSet<VoronoiLib.Structures.FortuneSite>();
        var delaunay = new List<Tuple<Vector2, Vector2>>();
        foreach (var site in points) {
            foreach (var neighbor in site.Neighbors) {
                if (!processed.Contains(neighbor)) {
                    delaunay.Add(
                        new Tuple<Vector2, Vector2>(
                            new Vector2((float)site.X, (float)site.Y),
                            new Vector2((float)neighbor.X, (float)neighbor.Y)
                        ));
                }
            }
            processed.Add(site);
        }
        return delaunay;
    }

    // Generate sites randomly across the area
    public static List<Vector2> GenerateSites(Vector2 dimensions, int count) {
        List<Vector2> sites = new List<Vector2>();

        for (int i = 0; i < count; i++) {
            float x = (float)rng.NextDouble() * dimensions.x;
            float y = (float)rng.NextDouble() * dimensions.y;

            sites.Add(new Vector2(x, y));
        }

        return sites;
    }

    // Generate sites biased around a center point
    public static List<Vector2> GenerateSites(Vector2 dimensions, int count, Vector2 center) {
        List<Vector2> sites = new List<Vector2>();

        float sigma = 3f;

        for (int i = 0; i < count; i++) {

            // float x = (float)rng.NextDouble();
            // float y = (float)rng.NextDouble();
            float x = RandomGaussian(0f, 1f, sigma);
            float y = RandomGaussian(0f, 1f, sigma);
            // float x = getNormal(nextFloat());
            // float y = getNormal(nextFloat());
            // print(x);

            // x = Mathf.Log(1 - x) / (-gamma);
            // y = Mathf.Log(1 - y) / (-gamma);

            // x *= dimensions.x / 2;
            // y *= dimensions.y / 2;
            x *= dimensions.x;
            y *= dimensions.y;

            sites.Add(new Vector2(x, y));
        }

        return sites;
    }

    private static float getNormal(float u) {
        return Mathf.Exp(-Mathf.Pow(u, 2f) / 2f) / Mathf.Sqrt(2f * Mathf.PI);
    }

    private static float getNormalRandom(float mean, float stdDev) {
        float u1 = 1.0f - (float)rng.NextDouble(); //uniform(0,1] random doubles
        float u2 = 1.0f - (float)rng.NextDouble();
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
            Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
        float randNormal =
            mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        return randNormal;
    }

    public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f, float sig = 3.0f) {
        float u, v, S;

        do {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / sig;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }

    private static float nextFloat() {
        return (float)rng.NextDouble();
    }

}