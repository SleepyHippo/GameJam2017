using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthsGroup : EarthMotionBase {
    public List<Transform> earthsList;
    public float[] timer;

    void Awake () {
        Init();
    }

    private void Init () {
        timer = new float[earthsList.Count];
    }

    private IEnumerator ArrangeEarthsList (int _index, Vector3 _targetPos) {
        timer[_index] = 0f;
        yield return new WaitForSeconds(Random.Range(0, 1f));

        while (timer[_index] < 1f) {
            timer[_index] += 1 / costTime * Time.deltaTime;
            timer[_index] = Mathf.Clamp01(timer[_index]);
            earthsList[_index].position = Vector3.Lerp(earthsList[_index].position, _targetPos + new Vector3(0, (3f * _index), 0), timer[_index]);
            yield return null;
        }
    }

    public IEnumerator MoveEarths (float _y) {
        for (var i = 0; i < earthsList.Count; i++) {
            yield return new WaitForSeconds(0.01f);
            var targetPos = new Vector3(transform.position.x, _y, transform.position.z);
            StartCoroutine(ArrangeEarthsList(i, targetPos));
        }
    }

    public void InitPos (float _y) {
        for (var i = 0; i < earthsList.Count; i++) {
            var targetPos = new Vector3(earthsList[i].position.x, _y + (3f * i), earthsList[i].position.z);
            earthsList[i].position = targetPos;
        }
    }
}
