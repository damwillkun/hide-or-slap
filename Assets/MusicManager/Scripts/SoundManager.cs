using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip firstClip;
    public List<AudioClip> clip;
    [Header("Sounds")]
    public AudioClip SlapSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = firstClip;

        audioSource.Play();
        StartCoroutine(WaitAndPlayNext());
    }

    int Shuffle()
    {
        int pick = Random.Range(0, clip.Count);
        return pick;
    }

    void PlaySomeMusic()
    {
        int clipToPlay = Shuffle();

        if (clip[clipToPlay] != audioSource.clip)
        {
            audioSource.clip = clip[clipToPlay];
            audioSource.Play();
            StartCoroutine(WaitAndPlayNext());
        }
        else
        {
            PlaySomeMusic();
        }
    }
    IEnumerator WaitAndPlayNext()
    {
        float duration = audioSource.clip.length;
        yield return new WaitForSeconds(duration);
        PlaySomeMusic();
    }

    public void PlaySlapSound()
    {
        audioSource.PlayOneShot(SlapSound);
    }
}
