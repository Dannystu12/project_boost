using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{

    [SerializeField] float rcsThrust = 200f;
    [SerializeField] float mainThrust = 200f;
    [SerializeField] float levelLoadDelay = 1f;

    [Header("SFX")]
    [SerializeField] AudioClip sfxThrust;
    [SerializeField] AudioClip sfxDeath;
    [SerializeField] AudioClip sfxWin;

    [Header("VFX")]
    [SerializeField] ParticleSystem particlesThrust;
    [SerializeField] ParticleSystem particlesDeath;
    [SerializeField] ParticleSystem particlesWin;

    [Header("Debug")]
    [SerializeField] bool debugMode = false;

    Rigidbody rigidBody;
    AudioSource audioSource;
    State state = State.Alive;
    SceneLoader sceneLoader;
    bool collisionDetection = true;

    enum State { Alive, Dying, Transcending }

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        sceneLoader = FindObjectOfType<SceneLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state != State.Alive) return;
        if(debugMode) RespondToDebugInput();
        RespondToThrustInput();
        ResponToRotateInput();
    }

    private void RespondToDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            sceneLoader.Invoke("LoadNextScene", levelLoadDelay);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDetection = !collisionDetection;
        }
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            EngageThrust();
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Stop();
            particlesThrust.Stop();
        }
    }

    private void EngageThrust()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
        if (!audioSource.isPlaying || audioSource.clip != sfxThrust)
        {
            PlayAudio(sfxThrust, true);
        }

        if(!particlesThrust.isPlaying)
        {
            particlesThrust.Play();
        }
    }

    private void ResponToRotateInput()
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
        if (state != State.Alive || !collisionDetection) return;
        switch(collision.gameObject.tag)
        {
            case "Landing Pad":
                StartSuccessSequence();
                break;
            case "Friendly":
                break;
            default:
                StartDeathSequence();
                break;
        }

    }

    private void StartDeathSequence()
    {
        if(particlesThrust.isPlaying)
        {
            particlesThrust.Stop();
        }
        state = State.Dying;
        sceneLoader.Invoke("ResetLevel", levelLoadDelay);
        PlayAudio(sfxDeath, false);
        particlesDeath.Play();
    }

    private void StartSuccessSequence()
    {
        if (particlesThrust.isPlaying)
        {
            particlesThrust.Stop();
        }
        sceneLoader.Invoke("LoadNextScene", levelLoadDelay);
        state = State.Transcending;
        PlayAudio(sfxWin, false);
        particlesWin.Play();
    }

    private void PlayAudio(AudioClip clip, bool looping)
    {
        if(audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.clip = clip;
        audioSource.loop = looping;

        audioSource.Play();
    }
}
 