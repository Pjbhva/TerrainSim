using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour {

    public float panSpeed = 50f;
    public Vector2 panLimit;

    public float scrollSpeed = 2f;
    public float maxY = 20f;
    public float minY = 120f;

	// Update is called once per frame
	void Update () {

        Vector3 pos = transform.position;

		if(Input.GetKey("w"))
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        // scroll = Input.GetAxis("Mouse ScrollWheel");
        //pos.y -= scroll * scrollSpeed * 0.01f * Time.deltaTime;


        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
        //pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
	}
}
