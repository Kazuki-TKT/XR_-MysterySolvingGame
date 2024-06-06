using UnityEngine;
using UnityEngine.Events;

public class DoorInteractable : SimpleHingeInteractable
{
    public UnityEvent OnOpen;

    [SerializeField] CombinationLock combinationLock;

    [SerializeField] Transform doorObj;

    [SerializeField] Vector3 rotationLimits;

    [SerializeField] Collider closedCollider;

    bool isClosed;

    Vector3 startRotation;

    [SerializeField] Collider openCollider;

    bool isOpen;

    [SerializeField] Vector3 endRotation;

    float startAngleX;

    protected override void Start()
    {
        base.Start();

        startRotation = transform.eulerAngles;
        startAngleX = GetAngle(startRotation.x);

        if (combinationLock != null)
        {
            combinationLock.UnlockAction += OnUnLocked;
            combinationLock.LockAction += OnLocked;
        }
    }

    private void OnLocked()
    {
        LockHinge();
    }

    private void OnUnLocked()
    {
        UnLockHinge();
    }

    protected override void Update()
    {
        base.Update();
        if (doorObj != null)
        {
            doorObj.localEulerAngles = new Vector3(
                doorObj.localEulerAngles.x,
                transform.localEulerAngles.y,
                doorObj.localEulerAngles.z
            );
        }

        if (isSelected)
        {
            CheckLimits();
        }

    }

    void CheckLimits()
    {
        isClosed = false;
        isOpen = false;

        float localAnglesX = GetAngle(transform.localEulerAngles.x);

        if (localAnglesX >= startAngleX + rotationLimits.x ||
            localAnglesX <= startAngleX - rotationLimits.x)
        {
            ReleaseHinge();
        }
    }

    float GetAngle(float angle)
    {
        if (angle >= 180)
        {
            angle -= 360;
        }
        return angle;
    }

    protected override void ResetHinge()
    {
        if (isClosed)
        {
            transform.localEulerAngles = startRotation;
        }
        else if (isOpen)
        {
            transform.localEulerAngles = endRotation;
            OnOpen?.Invoke();
        }
        else
        {
            transform.localEulerAngles = new Vector3(
                startAngleX,
                transform.localEulerAngles.y,
                transform.localEulerAngles.z
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other = closedCollider)
        {
            isClosed = true;
            ReleaseHinge();
        }
        else if (other = openCollider)
        {
            isOpen = true;
            ReleaseHinge();
        }
    }
}
