using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class rocket : MonoBehaviour {

    Rigidbody rigidBody;
    AudioSource rocketSound;

    float startingPitch = 0.9f;

    [SerializeField] float rcsThrust = 200f;
    [SerializeField] float mainThrust = 150f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip transcendSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;

    bool collisionsEnabled;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    // Use this for initialization
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        rocketSound = GetComponent<AudioSource>();
        collisionsEnabled = true;
    }

    // Update is called once per frame
    void Update() {
        if (state == State.Alive)
        {
            RespondtoThrustInput();
            RespondtoRotateInput();
        }

        if (Debug.isDebugBuild)
        {
            RespondToDebugInput();
        }

    }

    private void RespondToDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsEnabled = !collisionsEnabled;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !collisionsEnabled) //dying/debug mode
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Fuel":
                print("Fuel ingested.");
                break;
            case "Finish":
                LevelCompleteSequence();
                break;
            default:
                LevelFailedSequence();
                break;
        }
    }


    private void LevelCompleteSequence()
    {
        state = State.Transcending;
        
        Invoke("LoadNextScene", 0.4f);
        rocketSound.Stop();
        mainEngineParticles.Stop();
        rocketSound.PlayOneShot(transcendSound);
    }

    private void LevelFailedSequence()
    {
        state = State.Dying;
        
        Invoke("LoadSceneDeath", 1f);
        rocketSound.Stop();
        mainEngineParticles.Stop();
        rocketSound.PlayOneShot(deathSound);
        deathParticles.Play();
    }

   

    private void LoadSceneDeath()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (CurrentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }
    

    private void RespondtoRotateInput()
    {
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        rigidBody.freezeRotation = true; //freezes world rotation physics

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
            rocketSound.pitch = startingPitch - 0.05F;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate( -Vector3.forward * rotationThisFrame);
            rocketSound.pitch = startingPitch + 0.05F;
        }
        else
        {
            rocketSound.pitch = startingPitch;
        }

        rigidBody.freezeRotation = false; //unfreeze world rotation physics
    }

    private void RespondtoThrustInput()
    {
        if (Input.GetKey(KeyCode.Space)) //can thrust and rotate simultaneously
        {
            ApplyThrust();
        }
        else
        {
            rocketSound.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
        if (!rocketSound.isPlaying)
        {
            rocketSound.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }
}
