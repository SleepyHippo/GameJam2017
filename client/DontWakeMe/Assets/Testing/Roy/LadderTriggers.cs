using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderTriggers : MonoBehaviour {
    private bool mIsPlayerUseLadder;

    void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<InputController>().IsUseLadder = true;
            other.GetComponent<InputController>().IsUseGravity = false;
        }
        else {
            mIsPlayerUseLadder = true;
            other.GetComponent<Collider>().isTrigger = mIsPlayerUseLadder;
        }

    }

//    void OnTriggerStay (Collider other) {
//        if (other.CompareTag("Player") || other.CompareTag("Root")) return;
//    }

    void OnTriggerExit (Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<InputController>().IsUseLadder = false;
            other.GetComponent<InputController>().IsUseGravity = true;
        }
        else {
            mIsPlayerUseLadder = false;
            other.GetComponent<Collider>().isTrigger = mIsPlayerUseLadder;
        }
    }
}
