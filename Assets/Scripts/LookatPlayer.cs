using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookatPlayer : MonoBehaviour
{
    Transform player;
    private void Start() {
        player = FindObjectOfType<PlayerCtrl>().transform;
    }
    void Update()
    {
        Vector3 dir = (transform.position - player.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
