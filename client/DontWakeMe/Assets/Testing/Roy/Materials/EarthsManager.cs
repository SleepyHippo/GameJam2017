using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthsManager : MonoBehaviour {
    public GameObject group_01;
    //public GameObject group_02;

    public Transform topRoot;
    public Transform up2DownRoot;
    public Transform down2BottomRoot;
    public Transform top2UpRoot;

    public PositionType positionType = PositionType.Down;

    void Start () {
    }



    void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
            SwitchMode();
        }
    }


    #region 第一第二交替使用
    private bool switch_1;
    private void SwitchMode () {
        switch_1 = !switch_1;
        //if (switch_1)
        //Moving_Mode_01();

        positionType = switch_1 ? PositionType.Up : PositionType.Down;

        group_01.GetComponent<EarthMotionController>()
            .MoveEarths(switch_1 ? top2UpRoot.position.y : up2DownRoot.position.y);


        //else
        //    Moving_Mode_02();
    }


    /// <summary>
    /// 第一次移动场地
    /// </summary>
    private void Moving_Mode_01 () {
        if (switch_1) {
            group_01.GetComponent<EarthMotionController>().MoveEarths(top2UpRoot.position.y);
            //group_02.GetComponent<EarthMotionController>().MoveEarths(top2UpRoot.position.y);
        }
        else {
            //group_02.GetComponent<EarthMotionController>().MoveEarths(down2BottomRoot.position.y);
            group_01.GetComponent<EarthMotionController>().MoveEarths(up2DownRoot.position.y);
        }

        //StartCoroutine(ResetPosition2Top(group_01.GetComponent<EarthMotionController>()));
    }

    ///// <summary>
    ///// 第二次移动场地
    ///// </summary>
    //private void Moving_Mode_02 () {
    //    group_02.GetComponent<EarthMotionController>().MoveEarths(up2DownRoot.position.y);
    //}
    #endregion


    //private IEnumerator ResetPosition2Top (EarthMotionController _earthMotionController) {
    //    yield return new WaitForSeconds(10f);
    //    _earthMotionController.InitPos(topRoot.position.y);
    //}


}


/// <summary>
/// 土块在屏幕中的位置，也用来切换玩家重力控制开关
/// </summary>
public enum PositionType {
    Up,
    Down
}