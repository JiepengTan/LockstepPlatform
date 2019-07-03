using System;

class EditorTableFileToBytes {
    public void GenCodeAndData(string dir){
        void FuncDealFile(string path){ }
        EditorUtil.WalkWithProcessBar(dir, "*.xls", FuncDealFile);
    }
}