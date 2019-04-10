using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * script controls sound fade in 
 */

public class FadeIn : MonoBehaviour {

  [SerializeField] private int fadeInTime = 10;
  AudioSource thisAudio;

  private void Awake() {
    thisAudio = GetComponent<AudioSource>();
  }

	void Update() {
      AudioFadeIn();
  }

  private void AudioFadeIn() {
    if (thisAudio.volume < 1) {
      thisAudio.volume = thisAudio.volume + (Time.deltaTime / (fadeInTime + 1));
    } else {
      Destroy(this);
    }
  }
}
