using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//玩家控制
[RequireComponent(typeof(Rigidbody))]
public class PlayerCtrl : AvatarCtrl
{
    [SerializeField] Rigidbody rig = null;

    [Header("Mobile Component")]
    [SerializeField] Joystick variableJoystickLeft = null;
    [SerializeField] Joystick variableJoystickRight = null;
    [SerializeField] GameObject runToogleBtn = null;
    private bool isRun = false;
    protected override float currentSpeed => rig.velocity.magnitude;

    protected override void Start() {
#if UNITY_STANDALONE
        if(variableJoystickLeft != null) {
            variableJoystickLeft.gameObject.SetActive(false);
        }
        if(variableJoystickRight != null) {
            variableJoystickRight.gameObject.SetActive(false);
        }
        if(runToogleBtn != null) {
            runToogleBtn.SetActive(false);
        }
#endif
        base.Start();
    }

    protected override void Update() {
        base.Update();

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
        Move(direction, isRun);
        Rotate(rotateInput.x);
    }

    public void RunToogle() {
        isRun = !isRun;
        //GameManager.Instance.uiManager.RunBtn(isRun);
    }

    void Move(Vector3 direction, bool isRun) {
        if(IsFreeze) { return; }
        rig.velocity = direction * MoveSpeed(isRun);
        
        //rig.MovePosition(rig.position + direction * MoveSpeed(isRun) * Time.deltaTime);
    }

    void Rotate(float offset) {
        Quaternion rot = Quaternion.Euler(new Vector3(0, offset, 0) * rotSpeed * Time.deltaTime);
        rig.MoveRotation(rig.rotation * rot);
    }
}
