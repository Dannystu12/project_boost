using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

    Rigidbody rigidBody;
    AudioSource sfxThruster;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody>();
        sfxThruster = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        ProcessInput();
	}

    private void ProcessInput()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up);
            if(!sfxThruster.isPlaying)
            {
                sfxThruster.Play();
            }
        }
        else if(sfxThruster.isPlaying)
        {
            sfxThruster.Stop();
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward);
        }
        else if(Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward); ;
        }
    }
}
