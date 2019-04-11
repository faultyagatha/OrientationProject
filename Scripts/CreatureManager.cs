using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.Audio;
using OscJack;

/* use different behaviour for 3 different cases (based on GameObject tags):
case 1 "friendly stone": 
A) change the colour of the stone;
B) play a sound #1.

case 2 "unfriendly stone": 
A) play a particle effect (explosion);
B) change the stone's mesh to inactive;
C) play a sound #2
D) create 15 new blue stars.

case 3: "fatal stone": 
A) play a particle effect (Vincent's explosion);
B) change Vincent's mesh to inactive;
C) create 15 new Vincent's stars;
D) switch to the death scene (change to Playable Director #2).
*/

public class CreatureManager : MonoBehaviour {

  [SerializeField] GameObject blueStarPrefab; //new stars on collision with stones
  [SerializeField] GameObject pinkStarPrefab; //new stars on Vincent's death
  int num = 15; //number of stars

  [SerializeField] AudioClip friendly; //sound on collision with friendly stones
  [SerializeField] AudioClip unfriendly; //sound on collision with unfriendly stones
  [SerializeField] ParticleSystem fatalParticles; //Vincent's explosion
  public Component[] render;
  public PlayableDirector playFinalShot; 

  AudioSource audioSource;
  public AudioMixer mixer;

  Renderer otherColour; //access to the stone's colour
  Color32 newStoneColor = new Color32(0, 59, 66, 255);

  OscClient client;

  private void Awake() {
    audioSource = GetComponent<AudioSource>();
    SetAudioState("Start", 1f);
    client = new OscClient("127.0.0.1", 9000); 
  }

  //manage audio mixer and make sound transitions
  void SetAudioState(string snap, float transTime) {
    AudioMixerSnapshot snapshot = mixer.FindSnapshot(snap);
    snapshot.TransitionTo(transTime);
  }

  void OnTriggerEnter(Collider other) {
    switch(other.gameObject.tag) {

      case "Friendly":
        Debug.Log("Colliding");
        GetComponent<Animator>().Play("AntennaeFlip");
        FriendlyFX(other);
        audioSource.PlayOneShot(friendly);
        break;

      case "Unfriendly":
        Debug.Log("Colliding");
        UnfriendlyFX(other);
        audioSource.PlayOneShot(unfriendly);
        CreateBlueStar();
        break;

      case "Fatal":
        Debug.Log("Colliding");
        CreatePinkStar();

        Invoke("FatalFX", 0.2f);
        Invoke("PlayFinalShot", 0.3f);
        Invoke("ReloadScene", 16f);
        break;
    }
  }

  void FriendlyFX(Collider other) {
    SetAudioState("StoneCollision", 1f);
    otherColour = other.gameObject.GetComponent<Renderer>();
    otherColour.material.color = newStoneColor;
  }

  void UnfriendlyFX(Collider other) {
    SetAudioState("StoneCollision", 1f);
    var explosion = other.gameObject.GetComponentInChildren<ParticleSystem>();
    explosion.Play();
    other.gameObject.GetComponent<Renderer>().enabled = false;
  }

 void FatalFX() {
    SetAudioState("FinalShot", 10f);
    fatalParticles.Play();
    render = GetComponentsInChildren<Renderer>();
    foreach (Renderer rend in render) {
      if (rend.tag != "Particles") {
        rend.enabled = false;
      }
    }
}

  void CreateBlueStar() {
    for (int i = 0; i < num; i++) {
      GameObject newBlueStar = Instantiate(blueStarPrefab, PickRandomPosition(), Quaternion.identity);
    }
  }

  void CreatePinkStar() {
    for (int i = 0; i < num; i++) {
      GameObject newPinkStar = Instantiate(pinkStarPrefab, PickRandomPosition(), Quaternion.identity);
    }
  }

  Vector3 PickRandomPosition() {
    Vector3 random = Random.insideUnitSphere * 500f;
    float clampY = Mathf.Clamp(554f, 100f, 554f); //restrict Y position
    random += new Vector3(0f, clampY, 554f);
    return random;
  }

  void OnTriggerExit(Collider other) {
    SetAudioState("Start", 1f); //transition between sounds
    audioSource.Stop();
  }

  void PlayFinalShot() { //scene change on Vincent death
    playFinalShot.Play();
    GetComponent<Animator>().enabled = false;
  }

  void ReloadScene() {
    FadeOutScreen();
    //StartCoroutine(FadeOutScreen());
    SceneManager.LoadScene(0);
  }

  void FadeOutScreen() { //send OSC to VDMX to fade out the screen 
    float fadeOutStep = 0.001f;
    float fadeOutCount;
    for (int i = 0; i < 1001; i++) { //not the best solution but does the job better than its alternative below
      fadeOutCount = 1.0f - i * fadeOutStep;
      print(fadeOutCount);
      client.Send("/test", fadeOutCount);
    }
    print("fading out");
    client.Dispose();
  }

  //IEnumerator FadeOutScreen() {
  //  float FadeOutStep = 0.05f;
  //  float fadeOutCount;
  //  for (int i = 0; i < 21; i++) {
  //    yield return new WaitForSeconds(0.1f);
  //    fadeOutCount = 1.0f - i * FadeOutStep;
  //    print(fadeOutCount);
  //    client.Send("/test", fadeOutCount);
  //  }
  //  print("fading out");
  //  client.Dispose();
  //}
}