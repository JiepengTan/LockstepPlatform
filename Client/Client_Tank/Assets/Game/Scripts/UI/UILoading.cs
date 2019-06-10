using System.Collections;
using System.Collections.Generic;
using Lockstep.Game;
using UnityEngine;
using UnityEngine.UI;

public class UILoading : UIBaseWindow {
    public Text textInfo;

    public Slider sliderProgress;

    void OnEvent_OnLoadingProgress(object param){
        var progress = param as byte[];
        sliderProgress.value = (progress?[0] / 100f) ?? 0;
        textInfo.text = $"Loading {sliderProgress.value * 100}%";
    }

    void OnEvent_OnAllPlayerFinishedLoad(object param){
        sliderProgress.value = 1;
        OpenWindow(UIDefine.UIGameStatus);
        Close();
    }
}