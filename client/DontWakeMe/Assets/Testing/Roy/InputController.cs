using System.Collections;
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
    public bool IsUseLadder { get; set; }

    public bool IsUseGravity { get; set; }

    private DWM.MapContainer mapContainer;


    // Use this for initialization
    void Start () {
        IsUseGravity = playerType != PlayerType.Player_02;
        mapContainer = FindObjectOfType<DWM.MapContainer>();
    }

    // Update is called once per frame
    void Update () {
        UpdateInteractionPoint();
        Move();
        DoPee();
    }

    private void UpdateInteractionPoint() {
        if (isWater)
        {
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
                    Debug.Log("Pee");
                    mapContainer.Water(interactionPoint.position);
                }
            }
        }
        else {
            
        }
    }

    private void Move() {
        float x, y;
        if (playerType == PlayerType.Player_01) {
            x = Input.GetAxis("LeftAnalogHorizontal");
            y = Input.GetAxis("LeftAnalogVertical");
        }
        else {
            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");
        }


        if (x < 0 && y < 0) { // 左上
            transform.eulerAngles = new Vector3(0, 0, 45f);
        }
        else if (x < 0 && y > 0) { // 左下
            transform.eulerAngles = new Vector3(0, 0, -45f);
        }
        else if (x > 0 && y < 0) { // 左下
            transform.eulerAngles = new Vector3(0, 0, -215f);
        }
        else if (x > 0 && y > 0) { // 左下
            transform.eulerAngles = new Vector3(0, 0, -135f);
        }

        else if (x < 0) { // 左
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (x > 0) { // 右
            transform.eulerAngles = new Vector3(0, 180f, 0);
        }
        else if (y < 0) { // 上
            transform.eulerAngles = new Vector3(0, 0, 90f);
        }
        else if (y > 0) { // 下
            transform.eulerAngles = new Vector3(0, 0, 270f);
        }

        var g = gravity;

        if (IsUseGravity == false) g = 0f;
        GetComponent<CharacterController>().Move(new Vector3(x * speed * Time.deltaTime, -g * Time.deltaTime, 0f));

        if (IsUseLadder || playerType == PlayerType.Player_02) {
            GetComponent<CharacterController>().Move(new Vector3(0f, y * speed * Time.deltaTime, 0f));
        }
    }
}
