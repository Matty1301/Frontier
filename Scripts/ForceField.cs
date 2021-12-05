using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip forceFieldStart, forceFieldEnd;
    private float forceFieldDuration;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        forceFieldDuration = GetComponent<ParticleSystem>().main.startLifetimeMultiplier;
    }

    private void OnEnable()
    {
        audioSource.PlayOneShot(forceFieldStart);
        StartCoroutine(QueueForceFieldEnd());
    }

    private IEnumerator QueueForceFieldEnd()
    {
        yield return new WaitForSeconds(forceFieldDuration - forceFieldEnd.length);
        audioSource.PlayOneShot(forceFieldEnd);
    }
}
