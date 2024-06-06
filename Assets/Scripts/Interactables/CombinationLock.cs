using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class CombinationLock : MonoBehaviour
{
    //--
    public UnityAction UnlockAction;
    void OnUnlocked() => UnlockAction?.Invoke();
    //--

    //--
    public UnityAction LockAction;
    void OnLocked() => LockAction?.Invoke();
    //--

    //--
    public UnityAction ComboButtonPressed;
    void OnComboButtonPressed() => ComboButtonPressed?.Invoke();
    //--

    [SerializeField] TMP_Text userInputText; // ユーザーの入力を表示するテキスト

    [SerializeField] TMP_Text infoText; // 情報を表示するテキスト

    const string Start_String = "Enter 3 Digit Combo";

    const string Reset_String = "Enter 3 Digits to Reset Combo";

    [SerializeField] XRButtonInteractable[] comboButtons; // 組み合わせボタンの配列

    [SerializeField] Image lockedPanelImg; // ロック状態を示すパネルのイメージ

    [SerializeField] Color lockedColor; // ロック時の色

    [SerializeField] Color unLockedColor; // アンロック時の色

    [SerializeField] TMP_Text lockedText; // ロック状態を表示するテキスト

    const string UnLocked_String = "UnLocked"; // アンロック状態を示す文字列

    const string Locked_String = "Locked"; // ロック状態を示す文字列

    [SerializeField] bool isLocked; // ロックされているかどうかのフラグ

    [SerializeField] bool isResettable;// リセットされているかどうかのフラグ

    bool resetCombo;

    [SerializeField] int[] comboValues = new int[3]; // 正しい組み合わせの値

    [SerializeField] int[] inputValues; // ユーザーが入力した値

    //--
    [SerializeField] AudioClip lockComboClip;
    public AudioClip GetLockClip => lockComboClip;
    //--

    //--
    [SerializeField] AudioClip unlockComboClip;
    public AudioClip GetUnlockClip => unlockComboClip;
    //--

    //--
    [SerializeField] AudioClip comboButtonPressedClip;
    public AudioClip GetComboPressedClip => comboButtonPressedClip;
    //--

    int maxButtonPresses; // 最大ボタン押下数

    int buttonPresses; // 現在のボタン押下数

    void Start()
    {
        // 最大ボタン押下数を正しい組み合わせの値の数に設定
        maxButtonPresses = comboValues.Length;
        // ユーザー入力値をリセット
        ResetUserValues();
        // 各ボタンにリスナーを追加
        for (int i = 0; i < comboButtons.Length; i++)
        {
            comboButtons[i].selectEntered.AddListener(OnComboButtonPressed);
        }
    }

    private void OnComboButtonPressed(SelectEnterEventArgs arg0)
    {
        // 最大ボタン押下数を超えた場合
        if (buttonPresses >= maxButtonPresses)
        {

        }
        else
        {
            // 各ボタンについて処理
            for (int i = 0; i < comboButtons.Length; i++)
            {
                // 押されたボタンが自分自身の場合
                if (arg0.interactableObject.transform.name == comboButtons[i].transform.name)
                {
                    // ユーザー入力テキストにボタンのインデックスを追加
                    userInputText.text += i.ToString();
                    // ユーザー入力値にボタンのインデックスを記録
                    inputValues[buttonPresses] = i;
                }
                else // 別のボタンが押された場合
                {
                    // ボタンの色をリセット
                    comboButtons[i].ResetColor();
                }
            }
            // ボタン押下数をカウント
            buttonPresses++;
            // 全てのボタンが押された場合
            if (buttonPresses == maxButtonPresses)
            {
                CheckCombo(); // 組み合わせをチェック
            }
            else
            {
                OnComboButtonPressed();
            }
        }
    }

    void CheckCombo()
    {
        if (resetCombo)
        {
            resetCombo = false;
            LockCombo();
            return;
        }

        int matches = 0; // 一致する組み合わせの数

        // 各ボタンについて正しい組み合わせをチェック
        for (int i = 0; i < maxButtonPresses; i++)
        {
            if (inputValues[i] == comboValues[i])
            {
                matches++;
            }
        }
        // 全ての組み合わせが一致した場合
        if (matches == maxButtonPresses)
        {
            UnLockCombo();
        }
        else // 組み合わせが一致しない場合
        {
            ResetUserValues(); // ユーザー入力値をリセット
        }
    }

    void UnLockCombo()
    {
        isLocked = false; // ロックを解除
        OnUnlocked();
        lockedPanelImg.color = unLockedColor; // アンロック色に変更
        lockedText.text = UnLocked_String; // アンロック文字列を設定
        if (isResettable)
        {
            ResetCombo();
        }
    }

    void LockCombo()
    {
        isLocked = true; // ロック
        OnLocked();
        lockedPanelImg.color = lockedColor; // ロック色に変更
        lockedText.text = Locked_String; // ロック文字列を設定
        infoText.text = Start_String;
        for (int i = 0; i < maxButtonPresses; i++)
        {
            comboValues[i] = inputValues[i];
        }
        ResetUserValues();
    }

    void ResetCombo()
    {
        infoText.text = Reset_String;
        ResetUserValues();
        resetCombo = true;
    }

    void ResetUserValues()
    {
        if (isLocked)
        {
            OnLocked();
        }

        // ユーザー入力値を初期化
        inputValues = new int[maxButtonPresses];
        // ユーザー入力テキストをクリア
        userInputText.text = "";
        // ボタン押下数をリセット
        buttonPresses = 0;
    }
}
