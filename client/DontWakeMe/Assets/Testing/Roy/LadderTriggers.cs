using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderTriggers : MonoBehaviour {
    private bool mIsPlayerUseLadder;

    /// <summary>
    /// 保存和梯子交接的碰撞体信息，当人物进入梯子时关闭碰撞体信息
    /// </summary>
    private Dictionary<int, Collider> roots = new Dictionary<int, Collider>();

    void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Root")) {
            if (roots.ContainsKey(other.GetHashCode()) == false)
                roots.Add(other.GetHashCode(), other);
        }

        if (other.CompareTag("Player")) {
            other.GetComponent<InputController>().IsUseLadder = true;
            other.GetComponent<InputController>().IsUseGravity = false;
            mIsPlayerUseLadder = true;

            foreach (var key in roots.Keys) {
                roots[key].isTrigger = mIsPlayerUseLadder;
            }
        }
    }

    void OnTriggerExit (Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<InputController>().IsUseLadder = false;
            other.GetComponent<InputController>().IsUseGravity = true;
            mIsPlayerUseLadder = false;

            foreach (var key in roots.Keys) {
                roots[key].isTrigger = mIsPlayerUseLadder;
            }
        }
    }
}
