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

    [SerializeField] TMP_Text userInputText; // ���[�U�[�̓��͂�\������e�L�X�g

    [SerializeField] TMP_Text infoText; // ����\������e�L�X�g

    const string Start_String = "Enter 3 Digit Combo";

    const string Reset_String = "Enter 3 Digits to Reset Combo";

    [SerializeField] XRButtonInteractable[] comboButtons; // �g�ݍ��킹�{�^���̔z��

    [SerializeField] Image lockedPanelImg; // ���b�N��Ԃ������p�l���̃C���[�W

    [SerializeField] Color lockedColor; // ���b�N���̐F

    [SerializeField] Color unLockedColor; // �A�����b�N���̐F

    [SerializeField] TMP_Text lockedText; // ���b�N��Ԃ�\������e�L�X�g

    const string UnLocked_String = "UnLocked"; // �A�����b�N��Ԃ�����������

    const string Locked_String = "Locked"; // ���b�N��Ԃ�����������

    [SerializeField] bool isLocked; // ���b�N����Ă��邩�ǂ����̃t���O

    [SerializeField] bool isResettable;// ���Z�b�g����Ă��邩�ǂ����̃t���O

    bool resetCombo;

    [SerializeField] int[] comboValues = new int[3]; // �������g�ݍ��킹�̒l

    [SerializeField] int[] inputValues; // ���[�U�[�����͂����l

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

    int maxButtonPresses; // �ő�{�^��������

    int buttonPresses; // ���݂̃{�^��������

    void Start()
    {
        // �ő�{�^���������𐳂����g�ݍ��킹�̒l�̐��ɐݒ�
        maxButtonPresses = comboValues.Length;
        // ���[�U�[���͒l�����Z�b�g
        ResetUserValues();
        // �e�{�^���Ƀ��X�i�[��ǉ�
        for (int i = 0; i < comboButtons.Length; i++)
        {
            comboButtons[i].selectEntered.AddListener(OnComboButtonPressed);
        }
    }

    private void OnComboButtonPressed(SelectEnterEventArgs arg0)
    {
        // �ő�{�^���������𒴂����ꍇ
        if (buttonPresses >= maxButtonPresses)
        {

        }
        else
        {
            // �e�{�^���ɂ��ď���
            for (int i = 0; i < comboButtons.Length; i++)
            {
                // �����ꂽ�{�^�����������g�̏ꍇ
                if (arg0.interactableObject.transform.name == comboButtons[i].transform.name)
                {
                    // ���[�U�[���̓e�L�X�g�Ƀ{�^���̃C���f�b�N�X��ǉ�
                    userInputText.text += i.ToString();
                    // ���[�U�[���͒l�Ƀ{�^���̃C���f�b�N�X���L�^
                    inputValues[buttonPresses] = i;
                }
                else // �ʂ̃{�^���������ꂽ�ꍇ
                {
                    // �{�^���̐F�����Z�b�g
                    comboButtons[i].ResetColor();
                }
            }
            // �{�^�����������J�E���g
            buttonPresses++;
            // �S�Ẵ{�^���������ꂽ�ꍇ
            if (buttonPresses == maxButtonPresses)
            {
                CheckCombo(); // �g�ݍ��킹���`�F�b�N
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

        int matches = 0; // ��v����g�ݍ��킹�̐�

        // �e�{�^���ɂ��Đ������g�ݍ��킹���`�F�b�N
        for (int i = 0; i < maxButtonPresses; i++)
        {
            if (inputValues[i] == comboValues[i])
            {
                matches++;
            }
        }
        // �S�Ă̑g�ݍ��킹����v�����ꍇ
        if (matches == maxButtonPresses)
        {
            UnLockCombo();
        }
        else // �g�ݍ��킹����v���Ȃ��ꍇ
        {
            ResetUserValues(); // ���[�U�[���͒l�����Z�b�g
        }
    }

    void UnLockCombo()
    {
        isLocked = false; // ���b�N������
        OnUnlocked();
        lockedPanelImg.color = unLockedColor; // �A�����b�N�F�ɕύX
        lockedText.text = UnLocked_String; // �A�����b�N�������ݒ�
        if (isResettable)
        {
            ResetCombo();
        }
    }

    void LockCombo()
    {
        isLocked = true; // ���b�N
        OnLocked();
        lockedPanelImg.color = lockedColor; // ���b�N�F�ɕύX
        lockedText.text = Locked_String; // ���b�N�������ݒ�
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

        // ���[�U�[���͒l��������
        inputValues = new int[maxButtonPresses];
        // ���[�U�[���̓e�L�X�g���N���A
        userInputText.text = "";
        // �{�^�������������Z�b�g
        buttonPresses = 0;
    }
}
