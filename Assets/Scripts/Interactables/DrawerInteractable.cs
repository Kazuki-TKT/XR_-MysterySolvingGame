using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class DrawerInteractable : XRGrabInteractable
{
    public UnityEvent OnDrawerDetach;

    [SerializeField]
    Transform drawerTransform; // �����o����Transform

    Transform parentTransform; // �����o���̐eTransform

    //--
    [SerializeField]
    XRSocketInteractor keySocket; // ���p�̃\�P�b�g�C���^���N�^�[
    public XRSocketInteractor GetKeySocket => keySocket;
    //--

    //--
    [SerializeField] XrPhysicsButtonInteractable physicsButton;
    public XrPhysicsButtonInteractable GetPhysicsButton => physicsButton;
    //--

    [SerializeField]
    GameObject keyIndicatorLightGameObj;//�|�C���g���C�g�̃Q�[���I�u�W�F�N�g

    [SerializeField]
    bool isLocked; // �����o�������b�N����Ă��邩�ǂ����̃t���O

    bool isDetachable; // 

    bool isDetached;

    bool isGrabbed; // �����o�����O���u����Ă��邩�ǂ����̃t���O

    Vector3 limitPositions; // �����o���̈ʒu����

    float drawerLimitZ = 0.8f; // �����o����Z������

    const string Default_Layer = "Default"; // �f�t�H���g���C���[

    const string Grab_Layer = "Grab"; // �O���u�p���C���[

    [SerializeField]
    Vector3 limitDistances = new Vector3(.02f, .02f, 0); // �ʒu��������

    //--
    [SerializeField] AudioClip drawerMoveClip;
    public AudioClip GetDrawerMoveClip => drawerMoveClip;
    //--

    //--
    [SerializeField] AudioClip socketedClip;
    public AudioClip GetSocketedClip => socketedClip;
    //--

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // keySocket��Null�ł͂Ȃ��ꍇ
        if (keySocket != null)
        {
            // �����o���̃��b�N�����C�x���g�Ƀ��X�i�[��ǉ�
            keySocket.selectEntered.AddListener(OnDrawerUnLocked);
            // �����o���̃��b�N�C�x���g�Ƀ��X�i�[��ǉ�
            keySocket.selectExited.AddListener(OnDrawerLocked);
        }

        // �e��Transform���擾
        parentTransform = transform.parent.transform;

        // �����ʒu��ݒ�
        limitPositions = drawerTransform.localPosition;

        if (physicsButton != null)
        {
            physicsButton.OnBaseEnter.AddListener(OnIsDetachable);
            physicsButton.OnBaseExit.AddListener(OnIsNotDetachable);
        }
    }

    private void OnIsNotDetachable()
    {
        isDetachable = false;
    }

    private void OnIsDetachable()
    {
        isDetachable = true;
    }

    void Update()
    {
        if (!isDetached)
        { // �����o�����O���u����Ă���ꍇ
            if (isGrabbed && drawerTransform != null)
            {
                // Z���̈ʒu���X�V
                drawerTransform.localPosition = new Vector3(
                    drawerTransform.localPosition.x,
                    drawerTransform.localPosition.y,
                    transform.localPosition.z);

                // �ʒu�������`�F�b�N
                CheckLimits();
            }
        }
    }

    private void OnDrawerLocked(SelectExitEventArgs arg0)
    {
        // �����o�������b�N
        isLocked = true;
        Debug.Log("****DRAWER LOCKED****");
    }

    private void OnDrawerUnLocked(SelectEnterEventArgs arg0)
    {
        // �����o���̃��b�N������
        isLocked = false;

        // ���C�g�p�I�u�W�F�N�g�̕\���I�t
        if (keyIndicatorLightGameObj != null) keyIndicatorLightGameObj.SetActive(false);
        Debug.Log("****DRAWER UNLOCKED****");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        // �����o�������b�N����Ă��Ȃ��ꍇ
        if (!isLocked)
        {
            // �e���Đݒ�
            transform.SetParent(parentTransform);
            // �O���u�t���O��ݒ�
            isGrabbed = true;
        }
        else
        {
            // ���b�N����Ă���ꍇ�̓f�t�H���g���C���[�ɕύX
            ChangeLayerMask(Default_Layer);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (!isDetached)
        {
            // ���C���[���O���u���C���[�ɕύX
            ChangeLayerMask(Grab_Layer);
            // �O���u�t���O������
            isGrabbed = false;
            // �ʒu�����ɖ߂�
            transform.localPosition = drawerTransform.localPosition;
        }
        else
        {
            rb.isKinematic = false;
        }
    }

    private void CheckLimits()
    {
        if (transform.localPosition.x >= limitPositions.x + limitDistances.x ||
           transform.localPosition.x <= limitPositions.x - limitDistances.x)
        {
            ChangeLayerMask(Default_Layer);
        }
        else if (transform.localPosition.y >= limitPositions.y + limitDistances.y ||
            transform.localPosition.y <= limitPositions.y - limitDistances.y)
        {
            ChangeLayerMask(Default_Layer);
        }
        else if (drawerTransform.localPosition.z <= limitPositions.z - limitDistances.z)
        {
            isGrabbed = false;
            drawerTransform.localPosition = limitPositions;
            ChangeLayerMask(Default_Layer);
        }
        else if (drawerTransform.localPosition.z >= drawerLimitZ + limitDistances.z)
        {
            if (!isDetachable)
            {
                isGrabbed = false;
                drawerTransform.localPosition = new Vector3(
                    drawerTransform.localPosition.x,
                    drawerTransform.localPosition.y,
                    drawerLimitZ);
                ChangeLayerMask(Default_Layer);
            }
            else
            {
                DetachDrawer();
            }
        }
    }

    void DetachDrawer()
    {
        isDetached = true;
        drawerTransform.SetParent(this.transform);
        OnDrawerDetach?.Invoke();
    }

    private void ChangeLayerMask(string mask)
    {
        // �C���^���N�V�������C���[�}�X�N��ύX
        interactionLayers = InteractionLayerMask.GetMask(mask);
    }
}
