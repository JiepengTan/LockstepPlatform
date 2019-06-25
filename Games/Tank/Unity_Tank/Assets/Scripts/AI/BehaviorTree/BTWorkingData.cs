using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lockstep.AI {
    public unsafe partial class BTWorkingData : TAny {
        ~BTWorkingData(){
            if (_pDatas != null) {
                NativeHelper.Free(new IntPtr(_pDatas));
                _pDatas = null;
            }
        }

        private byte* _pDatas = null;
        private int[] _dataOffset;
        private int _dataLen = 0;

        public unsafe void* GetContext(int idx){
            var offset = _dataOffset[idx];
            Debug.Assert(offset >= 0 && offset < _dataLen, " out of range");
            return _pDatas + offset;
        }

        public void Init(int[] offsets, int totalMemSize){
            _pDatas = (byte*) (NativeHelper.Alloc(totalMemSize).ToPointer());
            _dataOffset = offsets;
            _dataLen = totalMemSize;
        }

        public BTWorkingData Clone(){
            var ret = new BTWorkingData();
            ret.Init(this._dataOffset, this._dataLen);
            NativeHelper.MemCpy(this._pDatas, ret._pDatas, _dataLen);
            return ret;
        }
    }
}