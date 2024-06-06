using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XrPlayerControl : MonoBehaviour
{
    [SerializeField] GrabMoveProvider[] grabMovers;

    [SerializeField] Collider[] grabColliders;

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < grabMovers.Length; i++)
        {
            if (grabColliders[i] == other)
            {
                SetGrabMovers(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < grabMovers.Length; i++)
        {
            if (grabColliders[i] == other)
            {
                SetGrabMovers(false);
            }
        }
    }

    void SetGrabMovers(bool isActive)
    {
        for (int i = 0; i < grabMovers.Length; i++)
        {
            grabMovers[i].enabled = isActive;
        }
    }

}
