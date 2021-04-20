using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    public float speed = 1.0f;

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.x > 5) { this.transform.position = new Vector3(-5, this.transform.position.y, this.transform.position.z); }
        else if (this.transform.position.x < -5) { this.transform.position = new Vector3(5, this.transform.position.y, this.transform.position.z); }

        if (Input.GetKey(KeyCode.A) == true)
        {
            this.transform.position += new Vector3(-speed, 0.0f, 0.0f) * Time.deltaTime;
        }

        this.transform.position += new Vector3(-speed, 0.0f, 0.0f) * Time.deltaTime;
    }
}
