using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using OscJack;


/* randomly create 10 usual stars on start
 * send OSC to VDMX on start
 */

public class SceneSetting : MonoBehaviour {

  OscClient client;
  public GameObject starPrefab;
  int num = 10;

  void Start() {
    client = new OscClient("127.0.0.1", 9000);
    CreateStar();
    FadeInScreen();
  }

  private void CreateStar() { 
    for (int i = 0; i < num; i++) {
      GameObject star = Instantiate(starPrefab, PickRandomPosition(), Quaternion.identity);
      star.GetComponent<ParticleSystem>().startLifetime = Random.Range(0.5f, 3f);
    }
  }

  Vector3 PickRandomPosition() {
    Vector3 random = Random.insideUnitSphere * 500f; //for the optimal spread of the stars
    float clampY = Mathf.Clamp(554f, 100f, 554f); //restrict Y position
    random += new Vector3(0f, clampY, 550f);
    return random;
   }

  private void FadeInScreen() {
    float FadeInStep = 0.001f;
    float fadeInCount;
    for (int i = 0; i < 100; i++) {
      fadeInCount = i * FadeInStep;
      client.Send("/test", fadeInCount);
    }
    print("fading in");
    client.Dispose();
  }
}

