using Lockstep.Game;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;


public class EditorUIRecover : UnityEditor.Editor {
    [MenuItem("Tools/RecoverAllUI")]
    static void RecoverAllUI(){
        string Dir = "Game/Resources/UI";
        var font = Resources.Load<Font>("Fonts/OpenSans/OpenSansBold");

        bool CallBackFunc(GameObject insGo){
            bool isNeedReplace = false;
            EditorUtil.WalkAllChild<Text>(insGo.transform, (text) => {
                if (text.font == null) {
                    var name = text.HierarchyName();
                    Debug.Log("Miss Text" + name);
                    text.font = font;
                    isNeedReplace = true;
                }
            });
            return isNeedReplace;
        }

        EditorUtil.WalkAllPrefab(Dir, CallBackFunc);
    }
}