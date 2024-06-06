using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class XRButtonInteractable : XRSimpleInteractable
{
    [SerializeField]
    Image buttonImage; // ボタンのImageコンポーネント

    [SerializeField]
    Color normalColor, highlightedColor, pressedColor, selectedColor; // ボタンの各状態のカラー

    bool isPressed = false; // ボタンが押されているかどうかのフラグ

    void Start()
    {
        // ボタンの色を初期状態にリセット
        ResetColor();
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        // ボタンが押されていない状態に設定
        isPressed = false;
        // ボタンの色をハイライト状態に変更
        buttonImage.color = highlightedColor;
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        // ボタンが押されていない場合
        if (!isPressed)
        {
            // ボタンの色を通常状態に変更
            buttonImage.color = normalColor;
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        // ボタンが押されている状態に設定
        isPressed = true;
        // ボタンの色を押された状態に変更
        buttonImage.color = pressedColor;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        // ボタンが押されていない状態に設定
        isPressed = false;
        // ボタンの色を選択された状態に変更
        buttonImage.color = selectedColor;
    }

    public void ResetColor()
    {
        // ボタンの色を通常状態にリセット
        buttonImage.color = normalColor;
    }
}
