using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(NoteAttribute))]
public class NoteDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        NoteAttribute note = (NoteAttribute)attribute;

        // hitung tinggi text tanpa GUI call
        GUIStyle style = EditorStyles.helpBox;
        style.wordWrap = true;
        float height = style.CalcHeight(new GUIContent(note.text), EditorGUIUtility.currentViewWidth - 40);

        return height + 4; // kasih sedikit padding
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        NoteAttribute note = (NoteAttribute)attribute;

        GUIStyle style = EditorStyles.helpBox;
        style.wordWrap = true;

        EditorGUI.LabelField(position, note.text, style);
    }
}
