using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]

public class Oscillator : MonoBehaviour
{

    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [Range(0, 1)] [SerializeField] float movementfactor;
    [SerializeField] float period = 2f;


    AudioSource ObstacleSound;
    [SerializeField] AudioClip MovingSound;

    Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position; //sets startingPos to object's pos at beginning
        ObstacleSound = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) { return; } //protect against dividing by 0
        float cycles = Time.time / period;
        const float tau = 2 * Mathf.PI;

        float rawSineWave = Mathf.Sin(cycles * tau);

        movementfactor = rawSineWave / 2f + 0.5f;

        Vector3 offset = movementVector * movementfactor; //calculate current offset
        transform.position = startingPos + offset; //applies offset to startingPos

        if (!ObstacleSound.isPlaying)
        {
            ObstacleSound.PlayOneShot(MovingSound);
        }
    }
}
