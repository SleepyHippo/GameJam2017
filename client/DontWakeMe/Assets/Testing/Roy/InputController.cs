﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    public enum PlayerType {
        Player_01,  //手柄
        Player_02   //键盘
    }
    public PlayerType playerType;
    public bool isWater = true;
    public float speed = 5f;
    public float gravity = 9.8f;
    public float waterDistance = 4;
    public float digDistance = 4;
    public Transform interactionPoint;
    public int interactionSize = 2;
    public bool IsUseLadder { get; set; }

    public bool IsUseGravity { get; set; }

    private DWM.MapContainer mapContainer;
    private EarthsManager earthsManager;
    public ActionManager actionManager;

    public AudioSource audioSource;
    public AudioClip waterSound;
    public AudioClip digSound;

    public PositionType positionType;

    public bool isNowPause = false;

    #region 不要在意细节
    /// <summary>
    /// 和梯子没毛关系的别用
    /// </summary>
    public int ladderUseOnly;
    #endregion


    // Use this for initialization
    void Start () {
        IsUseGravity = playerType != PlayerType.Player_02;
        mapContainer = FindObjectOfType<DWM.MapContainer>();
        earthsManager = FindObjectOfType<EarthsManager>();
        //actionManager = GetComponentInChildren<ActionManager>();
        actionManager.ChangeMaterial(positionType, ActionManager.ActionType.Move, ActionManager.ActionDirection.Left);
    }

    // Update is called once per frame
    void Update () {
        UpdateInteractionPoint();
        Move();
        DoPee();
        CheckLadderUseState();

        actionManager.transform.position = new Vector3(transform.position.x + 1f, transform.position.y + 1f, transform.position.z);
    }

    private void CheckLadderUseState () {
        if (ladderUseOnly == 0) {
            IsUseLadder = false;
        }
    }

    private void UpdateInteractionPoint () {
        if (isWater) {
            interactionPoint.transform.position = transform.position + Vector3.down * waterDistance;
        }
        else {
            float x, y;
            if (playerType == PlayerType.Player_01) {
                x = Input.GetAxis("LeftAnalogHorizontal");
                y = Input.GetAxis("LeftAnalogVertical");
            }
            else {
                x = Input.GetAxis("Horizontal");
                y = Input.GetAxis("Vertical");
            }
            Vector3 dir = new Vector3(x, y, 0).normalized;
            interactionPoint.transform.position = transform.position + dir * digDistance;
        }
    }

    private void DoPee () {
        if (playerType == PlayerType.Player_01) {
            if (Input.GetKey(KeyCode.Joystick1Button0)) {
                if (isWater) {
//                    Debug.Log("P1 Pee");
                    Pee();
                }
                else {
//                    Debug.Log("P1 Dig");
                    Dig();
                }
                actionManager.ChangeMaterial(actionType, ActionManager.ActionType.Attack, direction);
            }
            else {
                HideInteractionObject();
            }
        }
        else {
            if (Input.GetKey(KeyCode.Return)) {
                if (isWater) {
//                    Debug.Log("P2 Pee");
                    Pee();
                }
                else {
//                    Debug.Log("P2 Dig");
                    Dig();
                }
                actionManager.ChangeMaterial(actionType, ActionManager.ActionType.Attack, direction);
            }
            else {
                HideInteractionObject();
            }
        }
    }


    private ActionManager.ActionDirection direction;

    PositionType actionType;
    private void Move () {
        float x, y;
        IsUseGravity = positionType != earthsManager.positionType;

        if (playerType == PlayerType.Player_01) {
            x = Input.GetAxis("LeftAnalogHorizontal");
            y = Input.GetAxis("LeftAnalogVertical");
        }
        else {
            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");
        }


        actionType = IsUseGravity ? PositionType.OnGround : PositionType.UnderGround;


        if (x < 0) { // 左
            transform.eulerAngles = new Vector3(0, 180, 0);
            actionManager.ChangeMaterial(actionType, ActionManager.ActionType.Move, ActionManager.ActionDirection.Left);
            direction = ActionManager.ActionDirection.Left;
        }
        else if (x > 0) { // 右
            transform.eulerAngles = new Vector3(0, 0f, 0);
            actionManager.ChangeMaterial(actionType, ActionManager.ActionType.Move, ActionManager.ActionDirection.Right);
            direction = ActionManager.ActionDirection.Right;
        }


        //if (x < 0 && y < 0) { // 左上
        //    transform.eulerAngles = new Vector3(0, 0, 45f);
        //}
        //else if (x < 0 && y > 0) { // 左下
        //    transform.eulerAngles = new Vector3(0, 0, -45f);
        //}
        //else if (x > 0 && y < 0) { // 左下
        //    transform.eulerAngles = new Vector3(0, 0, -215f);
        //}
        //else if (x > 0 && y > 0) { // 左下
        //    transform.eulerAngles = new Vector3(0, 0, -135f);
        //}

        //else if (x < 0) { // 左
        //    transform.eulerAngles = new Vector3(0, 0, 0);
        //}
        //else if (x > 0) { // 右
        //    transform.eulerAngles = new Vector3(0, 180f, 0);
        //}
        //else if (y < 0) { // 上
        //    transform.eulerAngles = new Vector3(0, 0, 90f);
        //}
        //else if (y > 0) { // 下
        //    transform.eulerAngles = new Vector3(0, 0, 270f);
        //}

        // 别管这里是啥逻辑，先用着
        //if (positionType == PositionType.Up)
        //    IsUseGravity = positionType != earthsManager.positionType;

        //if (positionType == PositionType.Down)
        //    IsUseGravity = positionType != earthsManager.positionType;

        isWater = IsUseGravity;
        if (IsUseGravity) {
            gameObject.layer = isNowPause ? 12 : 8;
        }
        else if (!IsUseGravity) {
            gameObject.layer = isNowPause ? 12 : 9;
        }

        GetComponent<CharacterController>().Move((IsUseGravity == false) || IsUseLadder
            ? new Vector3(x * speed * Time.deltaTime, y * speed * Time.deltaTime, 0f)
            : new Vector3(x * speed * Time.deltaTime, -gravity * Time.deltaTime, 0f));

        //if (IsUseLadder || playerType == PlayerType.Player_02) {
        //    GetComponent<CharacterController>().Move(new Vector3(0f, y * speed * Time.deltaTime, 0f));
        //}
    }

    void Pee () {
        interactionPoint.gameObject.SetActive(true);
        bool done = mapContainer.Water(interactionPoint.position);
        float centerX = interactionPoint.position.x;
        float centerY = interactionPoint.position.y;
        if (!done) {
            for (int i = -interactionSize; i <= interactionSize; ++i) {
                float x = centerX + i;
                for (int j = -interactionSize; j <= interactionSize; ++j) {
                    float y = centerY + j;
                    done = mapContainer.Water(new Vector3(x, y, 0));
                    if (done) {
                        return;
                    }
                }
            }
        }
        audioSource.clip = waterSound;
        audioSource.Play();
    }

    void Dig () {
        interactionPoint.gameObject.SetActive(true);
        bool done = mapContainer.Dig(interactionPoint.position);
        float centerX = interactionPoint.position.x;
        float centerY = interactionPoint.position.y;
        if (!done) {
            for (int i = -interactionSize; i <= interactionSize; ++i) {
                float x = centerX + i;
                for (int j = -interactionSize; j <= interactionSize; ++j) {
                    float y = centerY + j;
                    done = mapContainer.Dig(new Vector3(x, y, 0));
                    if (done) {
                        return;
                    }
                }
            }
        }
        audioSource.clip = digSound;
        audioSource.Play();
    }

    void HideInteractionObject() {
        interactionPoint.gameObject.SetActive(false);
    }

    void OnTriggerEnter (Collider other) {
        if (other.gameObject.layer != 16) return;

        other.gameObject.SetActive(false);
    }
}