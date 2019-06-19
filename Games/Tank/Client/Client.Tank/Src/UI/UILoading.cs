using System.Collections;
using System.Collections.Generic;
using Lockstep.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game.UI {
    public class UILoading : UIBaseWindow {
        private Text TextInfo => GetRef<Text>("TextInfo");
        private Slider SliderProgress => GetRef<Slider>("SliderProgress");

        void OnEvent_OnLoadingProgress(object param){
            var iprogress = param as byte[];
            var progress = (iprogress?[0] / 100f) ?? 0;
            ShowProgress(progress);
        }

        void OnEvent_OnAllPlayerFinishedLoad(object param){
            EndLoading();
        }

        void OnEvent_ReconnectLoadProgress(object param){
            ShowProgress((float) param);
        }

        void OnEvent_ReconnectLoadDone(object param){
            EndLoading();
        }

        void OnEvent_VideoLoadProgress(object param){
            ShowProgress((float) param);
        }

        void OnEvent_VideoLoadDone(object param){
            EndLoading();
        }

        void ShowProgress(float progress){
            SliderProgress.value = progress;
            TextInfo.text = $"Loading {((int) (SliderProgress.value * 10000)) / 100f}%";
        }

        void EndLoading(){
            SliderProgress.value = 1;
            OpenWindow(UIDefine.UIGameStatus);
            Close();
        }
    }
}