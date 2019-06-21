using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lockstep.Game
{
    public class UnityCoroutineHelper : SingletonMono<UnityCoroutineHelper>
    {
        public static Coroutine Run(IEnumerator function)
        {
            if (Application.isPlaying)
            {
                return Instance.StartCoroutine(function);
            }
            else
            {
#if UNITY_EDITOR
                //处理编辑器没有运行的情况
                EditorCoroutineRunner.StartEditorCoroutine(function);
#endif
                return null;
            }
        }

        public static void Stop(IEnumerator function)
        {
            if (Application.isPlaying)
            {
                Instance.StopCoroutine(function);
            }
            else
            {
#if UNITY_EDITOR
                EditorCoroutineRunner.StopEditorCoroutine(function);
#endif
            }
        }

        public static void StopAll() {
            if (Application.isPlaying)
            {
                Instance.StopAllCoroutines();
            }
            else
            {
#if UNITY_EDITOR
                EditorCoroutineRunner.StopAllEditorCoroutine();
#endif
            }
        }
    }
}
