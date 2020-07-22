using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectorScript : MonoBehaviour {
    public GameObject bullet;
    public float bulletSpeed;
    public float cameraOffest = 2f;
    public float spreadBias = 0.6f;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        // if (Input.GetMouseButtonDown(0)) {
        //     Vector3 mousePosition = Input.mousePosition;
        //     mousePosition.z = cameraOffest;
        //     Vector3 inputPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //     // Vector3 dir = inputPosition - transform.position;
        //     // dir.Normalize();
        //     GameObject projectile = Instantiate(bullet, inputPosition, Quaternion.identity)as GameObject;
        //     projectile.GetComponent<Rigidbody>().velocity = Vector3.forward * bulletSpeed;

        //     Debug.DrawLine(inputPosition, Vector3.forward * 100, Color.cyan, 3.0f);
        // }
    }

    void FixedUpdate() {
        if (Input.GetMouseButtonDown(0)) {
            float aspectRatio = (float)Screen.width / (float)Screen.height;
            float spreadBiasX = spreadBias * aspectRatio * 0.8f;
            float spreadBiasY = spreadBias / aspectRatio;
            // Get the mouse position in world space
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = cameraOffest;
            Vector3 inputPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // Point the bullet out from the player instead of straight forward
            float width = Screen.width;
            float height = Screen.height;
            Vector2 center = new Vector2(width / 2, height / 2);
            Vector2 offset = new Vector2(mousePosition.x, mousePosition.y) - center;

            // print("offset: " + offset.ToString("F3"));

            Vector2 norm = new Vector2(offset.x / width * 2, offset.y / height * 2);

            // print("norm: " + norm.ToString("F3"));
            // Make bullets in the center go relatively forward
            // float p = 5f;
            // dist = new Vector2(Mathf.Pow(dist.x, p), Mathf.Pow(dist.y, p));
            // print(dist);

            Vector3 dir = Vector3.forward + new Vector3(norm.x * spreadBiasX, norm.y * spreadBiasY, 0);

            Debug.DrawRay(inputPosition, transform.TransformDirection(dir) * 1000, Color.white, 2f);
            // Vector3 dir = inputPosition - transform.position;
            // dir.Normalize();
            // print(dir);
            GameObject projectile = Instantiate(bullet, inputPosition, Quaternion.identity)as GameObject;
            projectile.GetComponent<Rigidbody>().velocity = dir * bulletSpeed;

        }

        // RaycastHit hit;
        // // Does the ray intersect any objects excluding the player layer
        // if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity)) {
        //     Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        //     Debug.Log("Did Hit");
        // } else {
        //     Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        //     // Debug.Log("Did not Hit");
        // }
    }
}