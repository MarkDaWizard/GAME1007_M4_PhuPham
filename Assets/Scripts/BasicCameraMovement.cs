using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCameraMovement : MonoBehaviour
{
    public Vector3 pos;



    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = pos;
        if (Input.GetKey("a"))
        {
            pos.x -= 50 * Time.deltaTime;
        }
        else if(Input.GetKey("d"))
        {
            pos.x += 50 * Time.deltaTime;
        }

        if (Input.GetKey("s"))
        {
            pos.y -= 50 * Time.deltaTime;
        }
        else if(Input.GetKey("w"))
        {
            pos.y += 50 * Time.deltaTime;
        }

    }
}
