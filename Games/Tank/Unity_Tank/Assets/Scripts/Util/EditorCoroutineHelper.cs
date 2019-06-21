#if UNITY_EDITOR
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Lockstep.Game
{
    public static class EditorCoroutineRunner
    {
        private class EditorCoroutine : IEnumerator
        {
            private Stack<IEnumerator> _executionStack;

            public EditorCoroutine(IEnumerator iterator)
            {
                this._executionStack = new Stack<IEnumerator>();
                this._executionStack.Push(iterator);
            }

            public bool MoveNext()
            {
                if (_executionStack.Count == 0)
                {
                    return false;
                }

                IEnumerator i = this._executionStack.Peek();

                if (i.MoveNext())
                {
                    object result = i.Current;
                    if (result != null && result is IEnumerator)
                    {
                        this._executionStack.Push(result as IEnumerator);
                    }
                    return true;
                }
                else
                {
                    if (this._executionStack.Count > 1)
                    {
                        this._executionStack.Pop();
                        return true;
                    }
                }
                return false;
            }

            public void Reset()
            {
                throw new System.NotSupportedException("This Operation Is Not Supported.");
            }

            public void End()
            {
                _executionStack.Clear();
            }

            public object Current
            {
                get { return this._executionStack.Peek().Current; }
            }

            public bool Find(IEnumerator iterator)
            {
                return this._executionStack.Contains(iterator);
            }
        }

        private static List<EditorCoroutine> _editorCoroutineList;
        private static List<IEnumerator> _buffer;

        public static IEnumerator StartEditorCoroutine(IEnumerator iterator)
        {
            if (_editorCoroutineList == null)
            {
                _editorCoroutineList = new List<EditorCoroutine>();
            }
            if (_buffer == null)
            {
                _buffer = new List<IEnumerator>();
            }
            if (_editorCoroutineList.Count == 0)
            {
                EditorApplication.update += Update;
            }

            // add iterator to buffer first
            _buffer.Add(iterator);

            return iterator;
        }

        public static void StopEditorCoroutine(IEnumerator iterator)
        {
            EditorCoroutine ec = Find(iterator);
            if (ec != null)
            {
                ec.End();
            }
            else if (_buffer != null && _buffer.Count != 0 && _buffer.Contains(iterator))
            {
                _buffer.Remove(iterator);
            }
        }

        public static void StopAllEditorCoroutine()
        {
            _buffer.Clear();
            _editorCoroutineList.Clear();
        }

        private static EditorCoroutine Find(IEnumerator iterator)
        {
            for (int i = 0; i < _editorCoroutineList.Count; i++)
            {
                EditorCoroutine editorCoroutine = _editorCoroutineList[i];
                if (editorCoroutine.Find(iterator))
                {
                    return editorCoroutine;
                }
            }
            return null;
        }

        private static void Update()
        {
            // EditorCoroutine execution may append new iterators to buffer
            // Therefore we should run EditorCoroutine first
            _editorCoroutineList.RemoveAll
            (
                coroutine => { return coroutine.MoveNext() == false; }
            );

            // If we have iterators in buffer
            if (_buffer.Count > 0)
            {
                for (int i = 0; i < _buffer.Count; i++)
                {
                    IEnumerator iterator = _buffer[i];
                    // If this iterators not exists
                    if (Find(iterator) == null)
                    {
                        // Added this as new EditorCoroutine
                        _editorCoroutineList.Add(new EditorCoroutine(iterator));
                    }
                }

                // Clear buffer
                _buffer.Clear();
            }

            // If we have no running EditorCoroutine
            // Stop calling update anymore
            if (_editorCoroutineList.Count == 0)
            {
                EditorApplication.update -= Update;
            }
        }
    }
}
#endif