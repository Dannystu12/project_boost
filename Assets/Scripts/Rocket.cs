using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{

    [SerializeField] float rcsThrust = 200f;
    [SerializeField] float mainThrust = 200f;

    Rigidbody rigidBody;
    AudioSource sfxThruster;
    State state = State.Alive;
    SceneLoader sceneLoader;

    enum State
    {
        Alive,
        Dying,
        Transcending
    }

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        sfxThruster = GetComponent<AudioSource>();
        sceneLoader = FindObjectOfType<SceneLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state != State.Alive) return;
        Thrust();
        Rotate();
    }


    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            float thrustThisFrame = mainThrust * Time.deltaTime;
            rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
            if (!sfxThruster.isPlaying)
            {
                sfxThruster.Play();
            }
        }
        else if (sfxThruster.isPlaying)
        {
            sfxThruster.Stop();
        }
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame); ;
        }
        rigidBody.freezeRotation = false; // return control to engine
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) return;
        switch(collision.gameObject.tag)
        {
            case "Landing Pad":
                sceneLoader.Invoke("LoadNextScene", 1f);
                state = State.Transcending;
                break;
            case "Friendly":
                break;
            default:
                state = State.Dying;
                sceneLoader.Invoke("ResetLevel", 1f);
                break;
        }

    }
}
 