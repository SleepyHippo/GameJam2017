using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderTriggers : MonoBehaviour {
    private bool mIsPlayerUseLadder;

    void OnTriggerEnter (Collider other) {
        if (other.tag != "Player") return;
        other.GetComponent<InputController>().IsUseLadder = true;
        other.GetComponent<InputController>().IsUseGravity = false;
        mIsPlayerUseLadder = true;
    }

    void OnTriggerStay (Collider other) {
        if (other.tag == "Player") return;
        other.GetComponent<Collider>().isTrigger = mIsPlayerUseLadder;
    }

    void OnTriggerExit (Collider other) {
        if (other.tag != "Player") return;
        other.GetComponent<InputController>().IsUseLadder = false;
        other.GetComponent<InputController>().IsUseGravity = true;
        mIsPlayerUseLadder = false;
    }
}
