using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class SimpleUIControl : MonoBehaviour
{
    [SerializeField] ProgressControl progressControl;

    [SerializeField] TMP_Text[] msgTexts; // �e�L�X�g�I�u�W�F�N�g�̔z��

    private void OnEnable()
    {
        if (progressControl != null)
        {
            progressControl.OnStartGame.AddListener(StartGame);
            progressControl.OnChallengeComplete.AddListener(ChallengeComplete);
        }
    }

    private void ChallengeComplete(string arg0)
    {
        SetText(arg0);
    }

    private void StartGame(string arg0)
    {
        SetText(arg0);
    }

    public void SetText(string msg)
    {
        // �S�Ẵe�L�X�g�I�u�W�F�N�g�Ƀ��b�Z�[�W��ݒ�
        for (int i = 0; i < msgTexts.Length; i++)
        {
            msgTexts[i].text = msg;
        }
    }
}
