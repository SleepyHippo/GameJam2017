using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class EarthMotionController : EarthMotionBase {
    public List<EarthsGroup> earthsList;


    public void MoveEarths (float _y) {
        foreach (var t in earthsList) {
            StartCoroutine(t.MoveEarths(_y));
        }
    }

    public void InitPos (float _y) {
        foreach (var t in earthsList) {
            t.InitPos(_y);
        }
    }
}
