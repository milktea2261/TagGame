using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//玩家控制
[RequireComponent(typeof(Rigidbody))]
public class PlayerCtrl : AvatarCtrl
{
    [SerializeField] UIManager uiManager = null;
    [SerializeField] Rigidbody rig = null;

    [Header("Mobile Component")]
    [SerializeField] Joystick variableJoystickLeft = null;
    [SerializeField] Joystick variableJoystickRight = null;
    [SerializeField] GameObject runToogleBtn = null;
    private bool useRun = false;

    protected void Start() {
#if UNITY_ANDROID//開啟虛擬搖桿
        if(variableJoystickLeft != null) {
            variableJoystickLeft.gameObject.SetActive(true);
        }
        if(variableJoystickRight != null) {
            variableJoystickRight.gameObject.SetActive(true);
        }
        if(runToogleBtn != null) {
            runToogleBtn.SetActive(true);
        }
#endif
    }

    protected override void Update() {
        base.Update();

        Vector2 rotateInput = Vector2.zero;
        Vector3 moveInput = Vector3.zero;

#if UNITY_STANDALONE
        rotateInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        useRun = Input.GetKey(KeyCode.LeftShift);
#endif
#if UNITY_ANDROID
        rotateInput = new Vector2(variableJoystickRight.Horizontal, variableJoystickRight.Vertical);
        moveInput = new Vector3(variableJoystickLeft.Horizontal, 0, variableJoystickLeft.Vertical).normalized;
#endif

        Vector3 direction = transform.TransformDirection(moveInput);
        Move(direction, useRun);
        Rotate(rotateInput.x);
    }

    public void RunToogle() {
        useRun = !useRun;
        uiManager.RunBtn(useRun);
    }

    void Move(Vector3 direction, bool isRun) {
        if(IsFreeze) { return; }

        isMoving = direction.magnitude > 0;
        rig.velocity = new Vector3(direction.x, rig.velocity.y, direction.z) * MoveSpeed(isRun);
    }

    void Rotate(float offset) {
        Quaternion rot = Quaternion.Euler(new Vector3(0, offset, 0) * rotSpeed * Time.deltaTime);
        rig.MoveRotation(rig.rotation * rot);
    }
}
