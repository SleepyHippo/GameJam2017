using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour {
    public float changeCost = 1f;

    public List<Material> type_01_Attack_L;
    public List<Material> type_01_Attack_R;

    public List<Material> type_01_Move_L;
    public List<Material> type_01_Move_R;

    public List<Material> type_02_Attack_L;
    public List<Material> type_02_Attack_R;

    public List<Material> type_02_Move_L;
    public List<Material> type_02_Move_R;


    private float timer;
    private bool switchMaterials;

    private MeshRenderer meshRenderer;

    // Use this for initialization
    void Start () {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update () {
        timer += 1f / changeCost * Time.deltaTime;

        if (timer > 1) {
            switchMaterials = !switchMaterials;
        }


    }

    private int imageIndex;
    public void ChangeMaterial (PositionType _positionType, ActionType _actionType, ActionDirection _actionDirection) {
        timer += 1f / changeCost * Time.deltaTime;

        if (timer > 1)
        {
            switchMaterials = !switchMaterials;
            timer = 0;
        }
        else return;

        switch (_positionType) {
            case PositionType.UnderGround:

                switch (_actionType) {
                    case ActionType.Attack:

                        switch (_actionDirection) {
                            case ActionDirection.Left:
                                meshRenderer.material = type_01_Attack_L[imageIndex];
                                imageIndex++;
                                if (imageIndex > 1) imageIndex = 0;
                                break;
                            case ActionDirection.Right:
                                meshRenderer.material = type_01_Attack_R[imageIndex];
                                imageIndex++;
                                if (imageIndex > 1) imageIndex = 0;
                                break;
                        }


                        break;
                    case ActionType.Move:

                        switch (_actionDirection) {
                            case ActionDirection.Left:
                                meshRenderer.material = type_01_Move_L[imageIndex];
                                imageIndex++;
                                if (imageIndex > 1) imageIndex = 0;
                                break;
                            case ActionDirection.Right:
                                meshRenderer.material = type_01_Move_R[imageIndex];
                                imageIndex++;
                                if (imageIndex > 1) imageIndex = 0;
                                break;
                        }

                        break;
                }




                break;
            case PositionType.OnGround:

                switch (_actionType) {
                    case ActionType.Attack:

                        switch (_actionDirection) {
                            case ActionDirection.Left:
                                meshRenderer.material = type_02_Attack_L[imageIndex];
                                imageIndex++;
                                if (imageIndex > 1) imageIndex = 0;
                                break;
                            case ActionDirection.Right:
                                meshRenderer.material = type_02_Attack_R[imageIndex];
                                imageIndex++;
                                if (imageIndex > 1) imageIndex = 0;
                                break;
                        }


                        break;
                    case ActionType.Move:

                        switch (_actionDirection) {
                            case ActionDirection.Left:
                                meshRenderer.material = type_02_Move_L[imageIndex];
                                imageIndex++;
                                if (imageIndex > 1) imageIndex = 0;
                                break;
                            case ActionDirection.Right:
                                meshRenderer.material = type_02_Move_R[imageIndex];
                                imageIndex++;
                                if (imageIndex > 1) imageIndex = 0;
                                break;
                        }

                        break;
                }

                break;
        }
    }

    public enum ActionType {
        Attack,
        Move
    }

    public enum ActionDirection {
        Left,
        Right
    }
}



