using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//玩家控制
[RequireComponent(typeof(AvatarController))]
public class Player : MonoBehaviour
{
    [SerializeField] AvatarController avatar = null;
    [SerializeField] Joystick variableJoystickLeft = null;
    [SerializeField] Joystick variableJoystickRight = null;
    [SerializeField] GameObject runToogleBtn = null;

    public bool isRun = false;

    private void Start() {
#if UNITY_STANDALONE
        variableJoystickLeft.gameObject.SetActive(false);
        variableJoystickRight.gameObject.SetActive(false);
        runToogleBtn.SetActive(false);
#endif
    }

    private void Update() {
        Vector2 rotateInput = Vector2.zero;
        Vector3 moveInput = Vector3.zero;

#if UNITY_STANDALONE
        rotateInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        isRun = Input.GetKey(KeyCode.LeftShift);
#endif
#if UNITY_ANDROID
        rotateInput = new Vector2(variableJoystickRight.Horizontal, variableJoystickRight.Vertical);
        moveInput = new Vector3(variableJoystickLeft.Horizontal, 0, variableJoystickLeft.Vertical).normalized;
#endif

        Vector3 direction = transform.TransformDirection(moveInput);
        avatar.Move(direction, isRun);
        avatar.Rotate(rotateInput.x);
    }

    public void RunToogle() {
        isRun = !isRun;
        //GameManager.Instance.uiManager.RunBtn(isRun);
    }
}
