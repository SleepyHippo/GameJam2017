using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    public enum PlayerType {
        Player_01,
        Player_02
    }
    public PlayerType playerType;

    public float speed = 5f;
    public float gravity = 9.8f;
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
        Move();
        DoPee();
    }

    private void DoPee () {
        if (Input.GetKey(KeyCode.Joystick1Button0)) {
            Debug.Log("Pee");
            mapContainer.Water(transform.position);
        }
    }

    private void Move () {
        var x = Input.GetAxis("LeftAnalogHorizontal");
        var y = Input.GetAxis("LeftAnalogVertical");

        if (playerType == PlayerType.Player_02) {
            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");


            if (x < 0 && y < 0) {  // 左上
                transform.eulerAngles = new Vector3(0, 0, 45f);
            }
            else if (x < 0 && y > 0) {  // 左下
                transform.eulerAngles = new Vector3(0, 0, -45f);
            }
            else if (x > 0 && y < 0) {  // 左下
                transform.eulerAngles = new Vector3(0, 0, -215f);
            }
            else if (x > 0 && y > 0) {  // 左下
                transform.eulerAngles = new Vector3(0, 0, -135f);
            }

            else if (x < 0) {       // 左
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (x > 0) {  // 右
                transform.eulerAngles = new Vector3(0, 180f, 0);
            }
            else if (y < 0) {   // 上
                transform.eulerAngles = new Vector3(0, 0, 90f);
            }
            else if (y > 0) {  // 下
                transform.eulerAngles = new Vector3(0, 0, 270f);
            }
        }

        var g = gravity;

        if (IsUseGravity == false) g = 0f;
        GetComponent<CharacterController>().Move(new Vector3(x * speed * Time.deltaTime, -g * Time.deltaTime, 0f));

        if (IsUseLadder || playerType == PlayerType.Player_02) {
            GetComponent<CharacterController>().Move(new Vector3(0f, y * speed * Time.deltaTime, 0f));
        }
    }
}
