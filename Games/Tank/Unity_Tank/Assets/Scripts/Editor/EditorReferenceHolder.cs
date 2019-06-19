using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Reflection;
using Lockstep.Game.UI;
using Lockstep.Math;
using System.Linq;
using Lockstep.Game;

[CustomPropertyDrawer(typeof(RefData))]
public class RefDataDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){ }
}

[CustomEditor(typeof(ReferenceHolder))]
public class EditorReferenceHolder : Editor {
    private ReferenceHolder m_RefHolder;
    private ReorderableList m_ReorderLst;
    [SerializeField] private string searchName = "";

    void OnEnable(){
        m_RefHolder = (ReferenceHolder) target;
        m_ReorderLst = new ReorderableList(serializedObject, serializedObject.FindProperty("Datas"));
        m_ReorderLst.drawElementCallback = DrawChild;
        m_ReorderLst.onAddCallback = AddButton;
        m_ReorderLst.drawHeaderCallback = DrawHeader;
        m_ReorderLst.onRemoveCallback = RemoveButton;
    }

    public Rect[] GetRects(Rect r){
        Rect[] rects = new Rect[6];
        float remainingWidth = r.width;
        float orderWidth = 50;
        float renameWidth = 50;
        float delWidth = 40;
        float offset = 4;
        float contentWidth = Mathf.FloorToInt((remainingWidth - orderWidth - renameWidth - delWidth - offset * 5) / 3);
        int cols = 0;
        float x = r.x;
        rects[cols++] = new Rect(x, r.y, orderWidth, r.height);
        x += orderWidth + offset; //序号
        rects[cols++] = new Rect(x, r.y, contentWidth, r.height);
        x += contentWidth + offset; //名称
        rects[cols++] = new Rect(x, r.y, renameWidth, r.height);
        x += renameWidth + offset; //改名
        rects[cols++] = new Rect(x, r.y, contentWidth, r.height);
        x += contentWidth + offset; //内容
        rects[cols++] = new Rect(x, r.y, contentWidth, r.height);
        x += contentWidth + offset; //类型
        rects[cols++] = new Rect(x, r.y, delWidth, r.height); //删除
        return rects;
    }


    private void DrawChild(Rect r, int index, bool selected, bool focused){
        if (index >= m_ReorderLst.serializedProperty.arraySize) {
            return;
        }

        var data = m_RefHolder.Datas[index];
        if (string.IsNullOrEmpty(data.TypeName)) {
            return;
        }

        var element = m_ReorderLst.serializedProperty.GetArrayElementAtIndex(index);
        r.y++;
        r.height = 16;
        Rect[] rects = GetRects(r);
        GUI.color = Color.white;
        if (data.hasVal) {
            GUI.color = Color.red;
        }
        else if (m_SearchMatchedData.Contains(data)) {
            GUI.color = Color.yellow;
        }

        if (GUI.Button(rects[0], index.ToString())) { }

        data.name = EditorGUI.TextField(rects[1], data.name);
        if (GUI.Button(rects[2], "→")) {
            if (data.GetGameObject() != null) {
                data.GetGameObject().name = data.name;
                serializedObject.ApplyModifiedProperties();
            }
        }

        EComponentType type = (EComponentType) Enum.Parse(typeof(EComponentType), data.TypeName);
        if (type <= EComponentType.GameObject) {
            var obj = EditorGUI.ObjectField(rects[3], data.bindObj, typeof(UnityEngine.Object),
                true);
            if (data.bindObj != obj) {
                data.bindObj = obj;
                data = GetData(data.GetGameObject());
                serializedObject.ApplyModifiedProperties();
            }
        }
        else if (type == EComponentType.Number) {
            if (data.bindVal.GetType() != typeof(LFloat))
                data.bindVal = LFloat.zero;
            var fVal = EditorGUI.FloatField(rects[3], ((LFloat) data.bindVal).ToFloat());
            data.bindVal = new LFloat(true, (int) fVal * LFloat.Precision);
            ;
        }
        else if (type == EComponentType.Color) {
            if (data.bindVal.GetType() != typeof(Color))
                data.bindVal = Color.white;
            data.bindVal = EditorGUI.ColorField(rects[3], (Color) data.bindVal);
        }

        if (!string.IsNullOrEmpty(data.TypeName)) {
            var comStr = EditorGUI.EnumPopup(rects[4],
                (EComponentType) Enum.Parse(typeof(EComponentType), data.TypeName)).ToString();
            if (data.TypeName != comStr) {
                data.TypeName = comStr;
                serializedObject.ApplyModifiedProperties();
            }
        }

        if (GUI.Button(rects[5], "×")) {
            this.m_ReorderLst.serializedProperty.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
        }

        ;
    }

    private void DrawHeader(Rect headerRect){
        headerRect.xMin += 14; // 忽略拖拽按钮的宽度
        headerRect.y++;
        headerRect.height = 15;
        Rect[] rects = GetRects(headerRect);
        int col = 0;
        string[] names = {
            "order",
            "name",
            "rename",
            "content",
            "type",
            "delete",
        };
        for (int i = 0; i < rects.Length; i++) {
            GUI.Label(rects[col], names[i], EditorStyles.label);
            col++;
        }
    }

    private void RemoveButton(ReorderableList list){
        this.m_ReorderLst.serializedProperty.DeleteArrayElementAtIndex(list.index);
        serializedObject.ApplyModifiedProperties();
    }

    public void AddButton(ReorderableList list){
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Add Component Param"), false, AddComponentData);
        menu.AddItem(new GUIContent("Add Number Param"), false, AddNumberData);
        menu.AddItem(new GUIContent("Add Color Param"), false, AddColorData);
        menu.ShowAsContext();
    }

    public void AddComponentData(){
        m_RefHolder.Datas.Add(new RefData("New GameObejct", EComponentType.GameObject));
        serializedObject.ApplyModifiedProperties();
    }

    public void AddNumberData(){
        m_RefHolder.Datas.Add(new RefData("New Number", EComponentType.Number, LFloat.zero));
        serializedObject.ApplyModifiedProperties();
    }

    public void AddColorData(){
        m_RefHolder.Datas.Add(new RefData("New Color", EComponentType.Color, Color.white));
        serializedObject.ApplyModifiedProperties();
    }


    public override void OnInspectorGUI(){
        EditorGUILayout.BeginHorizontal();
        ShowSearchTool();
        EditorGUILayout.EndHorizontal();
        m_ReorderLst.DoLayoutList();
        GUI.color = Color.white;
        ShowAutoImport();
        EditorGUILayout.BeginHorizontal();
        ShowAutoRename();
        ShowClearAll();
        ShowAutoDetect();
        ShowAutoBinding();
        EditorGUILayout.EndHorizontal();
    }

    private void ShowSearchTool(){
        GUILayout.Label("Search", GUILayout.Width(50f));
        searchName = EditorGUILayout.TextField(searchName);
        if (GUILayout.Button("×", GUILayout.Width(50))) {
            searchName = string.Empty;
        }

        SearchDatas(searchName);
    }

    private HashSet<RefData> m_SearchMatchedData = new HashSet<RefData>();

    public void SearchDatas(string searchName){
        m_SearchMatchedData.Clear();
        if (string.IsNullOrEmpty(searchName)) {
            return;
        }

        for (int i = 0; i < m_RefHolder.Datas.Count; i++) {
            if (m_RefHolder.Datas[i].name.Contains(searchName)) {
                m_SearchMatchedData.Add(m_RefHolder.Datas[i]);
            }
        }
    }


    private void ShowAutoImport(){
        GameObject temp =
            EditorGUILayout.ObjectField(null, typeof(GameObject), true, GUILayout.Height(50)) as GameObject;
        if (temp != null) {
            if (temp.transform.childCount > 0) {
                if (EditorUtility.DisplayDialog("Warning:",
                    "Should import children nodes\r\n\r\ncurrent node:" + temp.name, "Yes", "No")) {
                    var traList = temp.GetComponentsInChildren<Transform>(true);
                    foreach (var t in traList) {
                        if (t != temp.transform) {
                            AddGo(t.gameObject);
                        }
                    }
                }
            }

            AddGo(temp);
        }
    }

    private void ShowAutoRename(){
        if (GUILayout.Button("Auto rename")) {
            RectTransform[] array = m_RefHolder.gameObject.GetComponentsInChildren<RectTransform>(true);
            int BG = 0;
            for (int i = 0; i < array.Length; i++) {
                if (array[i].GetComponent<Image>() != null) {
                    array[i].gameObject.name = "Image_" + BG;
                    BG++;
                }
            }
        }
    }

    private void ShowClearAll(){
        if (GUILayout.Button("ClearAll")) {
            m_ReorderLst.serializedProperty.ClearArray();
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void ShowAutoDetect(){
        if (GUILayout.Button("Auto Detect")) {
            for (int i = 0; i < m_RefHolder.Datas.Count; i++) {
                for (int j = 0; j < m_RefHolder.Datas.Count; j++) {
                    if (i != j && m_RefHolder.Datas[i].name == m_RefHolder.Datas[j].name) {
                        m_RefHolder.Datas[i].hasVal = true;
                    }
                }
            }

            m_RefHolder.Datas.Sort(ListSort);
        }
    }

    private void ShowAutoBinding(){
        string errorMsg = "";
        if (GUILayout.Button("AutoBind")) {
            var owner = m_RefHolder.transform;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type[] types = null;
            {
                types = assemblies.SelectMany((Assembly assembly) => assembly.GetTypes())
                    .Where((Type tt) =>
                        typeof(UIBaseWindow).IsAssignableFrom(tt) ||
                        typeof(UIBaseItem).IsAssignableFrom(tt) && !tt.IsAbstract).ToArray();
            }
            Type targetType = null;
            foreach (var type in types) {
                if (type.Name == owner.name) {
                    targetType = type;
                    break;
                }
            }

            if (targetType == null) {
                Debug.Log("No type " + owner.name);
            }
            else {
                m_RefHolder.Datas.Clear();
                HashSet<Type> targetTypes = new HashSet<Type>() {
                    typeof(Button),
                    typeof(Text),
                    typeof(Image),
                    typeof(Toggle),
                    typeof(Slider),
                    typeof(InputField),
                    typeof(LayoutGroup),
                    typeof(Dropdown),
                    typeof(GameObject),
                    typeof(RawImage),
                    typeof(Transform),
                };
                var fields = targetType
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where((tt) => targetTypes.Contains(tt.PropertyType)).ToArray();
                foreach (var field in fields) {
                    var name = field.Name;
                    var child = RecursiveFindChild(name, owner, field.PropertyType);
                    if (child != null) {
                        AddGo(child.gameObject, field.PropertyType);
                    }
                }

                m_ReorderLst.serializedProperty.ClearArray();
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

    Transform RecursiveFindChild(string name, Transform parent, Type targetType){
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(parent);
        while (queue.Count > 0) {
            var trans = queue.Dequeue();
            if (trans.name == name) {
                if (targetType == typeof(GameObject)
                    || targetType == typeof(Transform))
                    return trans;
                if (trans.gameObject.GetComponent(targetType) != null)
                    return trans;
            }

            foreach (Transform childTran in trans) {
                queue.Enqueue(childTran);
            }
        }

        return null;
    }

    private int ListSort(RefData x, RefData y){
        if (x.hasVal && !y.hasVal) {
            return 1;
        }
        else if (!x.hasVal && y.hasVal) {
            return -1;
        }
        else {
            return 0;
        }
    }

    private void Move(int i, int type){
        if (type == 1) {
            if (i != 0) {
                RefData temp = m_RefHolder.Datas[i];
                m_RefHolder.Datas[i] = m_RefHolder.Datas[i - 1];
                m_RefHolder.Datas[i - 1] = temp;
            }
        }
        else if (type == 2) {
            if (i != m_RefHolder.Datas.Count - 1) {
                RefData temp = m_RefHolder.Datas[i];
                m_RefHolder.Datas[i] = m_RefHolder.Datas[i + 1];
                m_RefHolder.Datas[i + 1] = temp;
            }
        }
    }

    private RefData GetData(GameObject go){
        if (go != null) {
            RefData refData = null;
            for (int i = 0; i < (int) EComponentType.EnumCount; i++) {
                var typeComp = (EComponentType) i;
                if (typeComp != EComponentType.GameObject) {
                    var comp = go.GetComponent(typeComp.ToString());
                    if (comp != null) {
                        refData = new RefData(go.name, typeComp, comp);
                        break;
                    }
                }
            }

            if (refData == null) {
                refData = new RefData(go.name, EComponentType.GameObject, go);
            }

            return refData;
        }

        return null;
    }

    private void AddGo(GameObject go){
        var data = GetData(go);
        if (data != null) {
            m_RefHolder.Datas.Add(data);
        }
    }

    private void AddGo(GameObject go, Type type){
        var data = GetData(go);
        if (data != null) {
            if (type == typeof(GameObject)) {
                data.bindObj = go;
            }
            else if (type == typeof(Transform)) {
                data.bindObj = go.transform;
            }
            else {
                data.bindObj = go.GetComponent(type);
            }

            data.TypeName = type.Name;
            m_RefHolder.Datas.Add(data);
        }
    }
}