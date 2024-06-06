using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.CinemachineFreeLook;

public class NavMeshJoyStick : SimpleHingeInteractable
{
    [SerializeField] NavMeshRobot robot;

    [SerializeField] Transform rotationParentObject;

    [SerializeField] Transform trackedObject;

    [SerializeField] Transform trackingObject;


    protected override void ResetHinge()
    {
        if (robot != null)
        {
            robot.StopAgent();
        }
    }

    protected override void Update()
    {
        base.Update();
        if (isSelected)
        {
            MoveRobot();
        }
    }

    void MoveRobot()
    {
        if (robot != null)
        {
            trackingObject.position = new Vector3
            (trackedObject.position.x,
            trackingObject.position.y,
            trackedObject.position.z);

            rotationParentObject.rotation = Quaternion.identity;

            robot.MoveAgent(trackingObject.localPosition);
        }
    }

}
