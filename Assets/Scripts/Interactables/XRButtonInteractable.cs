using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class XRButtonInteractable : XRSimpleInteractable
{
    [SerializeField]
    Image buttonImage; // �{�^����Image�R���|�[�l���g

    [SerializeField]
    Color normalColor, highlightedColor, pressedColor, selectedColor; // �{�^���̊e��Ԃ̃J���[

    bool isPressed = false; // �{�^����������Ă��邩�ǂ����̃t���O

    void Start()
    {
        // �{�^���̐F��������ԂɃ��Z�b�g
        ResetColor();
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        // �{�^����������Ă��Ȃ���Ԃɐݒ�
        isPressed = false;
        // �{�^���̐F���n�C���C�g��ԂɕύX
        buttonImage.color = highlightedColor;
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        // �{�^����������Ă��Ȃ��ꍇ
        if (!isPressed)
        {
            // �{�^���̐F��ʏ��ԂɕύX
            buttonImage.color = normalColor;
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        // �{�^����������Ă����Ԃɐݒ�
        isPressed = true;
        // �{�^���̐F�������ꂽ��ԂɕύX
        buttonImage.color = pressedColor;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        // �{�^����������Ă��Ȃ���Ԃɐݒ�
        isPressed = false;
        // �{�^���̐F��I�����ꂽ��ԂɕύX
        buttonImage.color = selectedColor;
    }

    public void ResetColor()
    {
        // �{�^���̐F��ʏ��ԂɃ��Z�b�g
        buttonImage.color = normalColor;
    }
}
