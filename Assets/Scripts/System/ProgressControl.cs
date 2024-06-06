using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using System;

public class ProgressControl : MonoBehaviour
{
    public UnityEvent<string> OnStartGame;

    public UnityEvent<string> OnChallengeComplete;

    [Header("Start Button")]

    [SerializeField] XRButtonInteractable startButton; // 開始ボタンのインタラクター

    [SerializeField] GameObject keyIndicatorLight; // 鍵のインジケータライト

    [Header("Drawer Interactables")]

    [SerializeField] DrawerInteractable drawer;

    XRSocketInteractor drawerSocket;

    [Header("Combo Lock")]

    [SerializeField] CombinationLock comboLock;

    [Header("The Wall")]

    [SerializeField] TheWall wall;

    XRSocketInteractor wallSocket;

    [SerializeField] GameObject teleportationAreas;

    [Header("Library")]

    [SerializeField] SimpleSliderControl librarySlider;

    [Header("The Robot")]

    [SerializeField] NavMeshRobot robot;

    [Header("Challenge Settings")]

    [SerializeField, TextArea] string startGameStrings; // ゲーム開始文字列

    [SerializeField, TextArea] string endGameString;

    [SerializeField, TextArea] string[] challengeStrings; // メッセージ文字列の配列

    [SerializeField] int wallCubeToDestroy;

    int wallCubesDestoryed;

    bool startGameBool;

    bool challengesCompletedBool;

    [SerializeField] int challengeNumber;

    void Start()
    {
        // startButtonがNullでない場合
        if (startButton != null)
        {
            // ボタンが押されたときのリスナーを追加
            startButton.selectEntered.AddListener(StartButtonPressed);
        }
        OnStartGame?.Invoke(startGameStrings);
        SetDrawerInteractable();
        if (comboLock != null)
        {
            comboLock.UnlockAction += OnComboUnlocked;
        }
        if (wall != null)
        {
            SetWall();
        }
        if (librarySlider != null)
        {
            librarySlider.OnSliderActive.AddListener(LibrarySliderActive);
        }
        if (robot != null)
        {
            robot.OnDestroyWallCube.AddListener(OnDestroyWallCube);
        }
    }

    void ChallengeComplete()
    {
        challengeNumber++;
        if (challengeNumber < challengeStrings.Length)
        {
            OnChallengeComplete?.Invoke(challengeStrings[challengeNumber]);
        }
        else if (challengeNumber >= challengeStrings.Length)
        {
            OnChallengeComplete?.Invoke(endGameString);
        }
    }

    private void StartButtonPressed(SelectEnterEventArgs arg0)
    {
        if (!startGameBool)
        {
            startGameBool = true;
            if (keyIndicatorLight != null)
            {
                keyIndicatorLight.SetActive(true);
            }
            if (challengeNumber < challengeStrings.Length && challengeNumber == 0)
            {
                OnStartGame?.Invoke(challengeStrings[challengeNumber]);
            }
        }
    }


    //--Challenge0
    private void OnDrawerSocketed(SelectEnterEventArgs arg0)
    {
        if (challengeNumber == 0)
        {
            ChallengeComplete();
        }
    }
    //--Challenge1
    private void OnDrawerDetach()
    {
        if (challengeNumber == 1)
        {
            ChallengeComplete();
        }
    }
    //--Challenge2
    private void OnComboUnlocked()
    {
        if (challengeNumber == 2)
        {
            ChallengeComplete();
        }
    }
    //--Challenge3
    private void OnWallSocketed(SelectEnterEventArgs arg0)
    {
        if (challengeNumber == 3)
        {
            ChallengeComplete();
        }
    }
    //--Challenge4
    private void OnDestroyWall()
    {
        if (challengeNumber == 4)
        {
            ChallengeComplete();
        }
        if (teleportationAreas != null) teleportationAreas.SetActive(true);
    }
    //--Challenge5
    private void LibrarySliderActive()
    {
        if (challengeNumber == 5)
        {
            ChallengeComplete();
        }
    }
    //--Challenge6
    private void OnDestroyWallCube()
    {
        wallCubesDestoryed++;
        if (wallCubesDestoryed >= wallCubeToDestroy
            && !challengesCompletedBool)
        {
            challengesCompletedBool = true;
            if (challengeNumber == 6)
            {
                ChallengeComplete();
            }
        }
    }

    void SetDrawerInteractable()
    {
        if (drawer != null)
        {
            drawer.OnDrawerDetach.AddListener(OnDrawerDetach);
            drawerSocket = drawer.GetKeySocket;
            if (drawerSocket != null)
            {
                drawerSocket.selectEntered.AddListener(OnDrawerSocketed);
            }
        }
    }

    private void SetWall()
    {
        wall.OnDestroy.AddListener(OnDestroyWall);
        //--
        wallSocket = wall.GetWallSocket;
        if (wallSocket != null)
        {
            wallSocket.selectEntered.AddListener(OnWallSocketed);
        }
    }

}
