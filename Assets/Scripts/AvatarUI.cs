using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarUI : MonoBehaviour
{
    [SerializeField] AvatarLayout taggerLayout = null;
    [SerializeField] AvatarLayout runnerLayout = null;

    private void Start() {
        taggerLayout.ShowAvatar();
        runnerLayout.ShowAvatar();
    }


}