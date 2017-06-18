using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderTriggers : MonoBehaviour {
    [Range(0, 5)]
    public float xTileScale = 1;
    [Range(0, 5)]
    public float yTileScale = 1;

    void Awake () {
        Material material = GetComponent<MeshRenderer>().material;
        material.mainTextureScale = new Vector2(transform.localScale.x * xTileScale, transform.localScale.y * yTileScale);
    }

    void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<InputController>().IsUseLadder = true;
            other.GetComponent<InputController>().IsUseGravity = false;
            other.GetComponent<InputController>().ladderUseOnly++;
        }
    }

    void OnTriggerStay (Collider other) {
        if (other.CompareTag("Root") == false) return;
        other.isTrigger = true;
    }

    void OnTriggerExit (Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<InputController>().ladderUseOnly--;
        }
    }
}
