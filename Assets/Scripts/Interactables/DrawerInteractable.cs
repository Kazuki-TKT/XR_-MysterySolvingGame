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
    Transform drawerTransform; // 引き出しのTransform

    Transform parentTransform; // 引き出しの親Transform

    //--
    [SerializeField]
    XRSocketInteractor keySocket; // 鍵用のソケットインタラクター
    public XRSocketInteractor GetKeySocket => keySocket;
    //--

    //--
    [SerializeField] XrPhysicsButtonInteractable physicsButton;
    public XrPhysicsButtonInteractable GetPhysicsButton => physicsButton;
    //--

    [SerializeField]
    GameObject keyIndicatorLightGameObj;//ポイントライトのゲームオブジェクト

    [SerializeField]
    bool isLocked; // 引き出しがロックされているかどうかのフラグ

    bool isDetachable; // 

    bool isDetached;

    bool isGrabbed; // 引き出しがグラブされているかどうかのフラグ

    Vector3 limitPositions; // 引き出しの位置制限

    float drawerLimitZ = 0.8f; // 引き出しのZ軸制限

    const string Default_Layer = "Default"; // デフォルトレイヤー

    const string Grab_Layer = "Grab"; // グラブ用レイヤー

    [SerializeField]
    Vector3 limitDistances = new Vector3(.02f, .02f, 0); // 位置制限距離

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

        // keySocketがNullではない場合
        if (keySocket != null)
        {
            // 引き出しのロック解除イベントにリスナーを追加
            keySocket.selectEntered.AddListener(OnDrawerUnLocked);
            // 引き出しのロックイベントにリスナーを追加
            keySocket.selectExited.AddListener(OnDrawerLocked);
        }

        // 親のTransformを取得
        parentTransform = transform.parent.transform;

        // 初期位置を設定
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
        { // 引き出しがグラブされている場合
            if (isGrabbed && drawerTransform != null)
            {
                // Z軸の位置を更新
                drawerTransform.localPosition = new Vector3(
                    drawerTransform.localPosition.x,
                    drawerTransform.localPosition.y,
                    transform.localPosition.z);

                // 位置制限をチェック
                CheckLimits();
            }
        }
    }

    private void OnDrawerLocked(SelectExitEventArgs arg0)
    {
        // 引き出しをロック
        isLocked = true;
        Debug.Log("****DRAWER LOCKED****");
    }

    private void OnDrawerUnLocked(SelectEnterEventArgs arg0)
    {
        // 引き出しのロックを解除
        isLocked = false;

        // ライト用オブジェクトの表示オフ
        if (keyIndicatorLightGameObj != null) keyIndicatorLightGameObj.SetActive(false);
        Debug.Log("****DRAWER UNLOCKED****");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        // 引き出しがロックされていない場合
        if (!isLocked)
        {
            // 親を再設定
            transform.SetParent(parentTransform);
            // グラブフラグを設定
            isGrabbed = true;
        }
        else
        {
            // ロックされている場合はデフォルトレイヤーに変更
            ChangeLayerMask(Default_Layer);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (!isDetached)
        {
            // レイヤーをグラブレイヤーに変更
            ChangeLayerMask(Grab_Layer);
            // グラブフラグを解除
            isGrabbed = false;
            // 位置を元に戻す
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
        // インタラクションレイヤーマスクを変更
        interactionLayers = InteractionLayerMask.GetMask(mask);
    }
}
