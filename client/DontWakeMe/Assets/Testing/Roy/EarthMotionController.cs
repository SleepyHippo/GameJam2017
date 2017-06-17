using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class EarthMotionController : EarthMotionBase {
    public List<Transform> earthsList;

    public float[] timer;

    public Transform rootPos;

    //private bool isAllArrived;

    //public float speed;
    // Use this for initialization
    void Awake () {
        Init();
    }

    void Start()
    {
        StartCoroutine(MoveEarthsUp2Down());
    }

    // Update is called once per frame
    void Update () {

    }

    private void Init () {
        timer = new float[earthsList.Count];
        //InitEarthsList();
    }

    private void InitEarthsList () {
        foreach (var t in earthsList) {
            t.position = new Vector3(t.position.x, t.position.y + 200f, t.position.z);
        }
    }

    private IEnumerator ArrangeEarthsList (int _index, Vector3 _targetPos) {
        yield return new WaitForSeconds(Random.Range(0, 2f));
        //yield return new WaitForSeconds(3f);


        while (Math.Abs(Vector3.Distance(earthsList[_index].position, _targetPos)) > 0) {
            timer[_index] += 1 / costTime * Time.deltaTime;
            timer[_index] = Mathf.Clamp01(timer[_index]);
            earthsList[_index].position = Vector3.Lerp(earthsList[_index].position, _targetPos + new Vector3(0, (3f * _index), 0), timer[_index]);
            yield return null;
        }
    }

    public IEnumerator MoveEarthsUp2Down () {
        for (var i = 0; i < earthsList.Count; i++) {
            yield return new WaitForSeconds(0.01f);
            var targetPos = new Vector3(transform.position.x, rootPos.position.y, transform.position.z);
            StartCoroutine(ArrangeEarthsList(i, targetPos));
        }
    }
}
