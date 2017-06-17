using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotionController : MonoBehaviour
{
    public List<Transform> posList;

    public float posLerpTime;

    private float timer;

    //private Vector3 curPos;
    private Vector3 curEulerAngles;

    private int tempCameraCounter;

    void Start()
    {
        //transform.position = posList[0].position;
        //curPos = transform.position;
        curEulerAngles = transform.eulerAngles;
        timer = 0;
        tempCameraCounter = 0;
    }

    void Update()
    {
        if (Vector3.Distance(transform.eulerAngles, posList[tempCameraCounter].eulerAngles) == 0)
        {
            curEulerAngles = transform.eulerAngles;
            timer = 0;

            tempCameraCounter++;

            if (tempCameraCounter > posList.Count - 1)
            {
                tempCameraCounter = 0;
            }

        }
        else
        {
            timer += 1 / posLerpTime * Time.deltaTime;
            //transform.position = Vector3.Lerp(curPos, posList[tempCameraCounter].position, timer);
            transform.eulerAngles = Vector3.Lerp(curEulerAngles, posList[tempCameraCounter].eulerAngles, timer);
        }
    }
}
