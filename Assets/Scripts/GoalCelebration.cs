using UnityEngine;
using System.Collections;

public class GoalCelebration : MonoBehaviour
{
    [SerializeField]
    ParticleSystem particleSystem;

    float timeCreated;

    // Use this for initialization
    void Start()
    {
        timeCreated = Time.time;
        particleSystem.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timeCreated > 1)
        {
            Destroy(gameObject);
        }
    }
}