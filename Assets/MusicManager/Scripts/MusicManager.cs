using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource source;
    public AudioClip firstClip;
    public List<AudioClip> clip;


    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = firstClip;

        source.Play();
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

        if (clip[clipToPlay] != source.clip)
        {
            source.clip = clip[clipToPlay];
            source.Play();
            StartCoroutine(WaitAndPlayNext());
        }
        else
        {
            PlaySomeMusic();
        }
    }
    IEnumerator WaitAndPlayNext()
    {
        float duration = source.clip.length;
        yield return new WaitForSeconds(duration);
        PlaySomeMusic();
    }
}
