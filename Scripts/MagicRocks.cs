using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* set random tags for 'magic' stones: 'friendly', 'unfriendly', 'fatal'
 */

public class MagicRocks : MonoBehaviour {

  void Start() {
    SetRandomTags();
  }

  void SetRandomTags() {
    foreach (Transform child in transform) {
      child.tag = (Random.Range(0, 2)) == 1 ? "Friendly" : "Unfriendly"; // ? true; : false
    }
    int deadlyIndex = Random.Range(0, transform.childCount);
    transform.GetChild(deadlyIndex).tag = "Fatal";
  }
}
