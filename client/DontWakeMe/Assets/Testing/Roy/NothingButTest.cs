using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NothingButTest : MonoBehaviour {
    public List<Rigidbody> rigidbodys;
    public float radius = 15.0f;
    public float power = 100.0f;
    public Transform explosionPos;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < rigidbodys.Count; i++) {
            rigidbodys[i].AddExplosionForce(power, explosionPos.position, radius, 3.0f);
        }
    }

}
