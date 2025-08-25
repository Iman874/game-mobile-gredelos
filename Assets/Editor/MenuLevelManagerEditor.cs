#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MenuLevelManager))]
public class MenuLevelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //MenuLevelManager manager = (MenuLevelManager)target;

        /*if (GUILayout.Button("Generate Level Items (Manual)"))
        {
            manager.GenerateLevelItems();
        }
        */
    }
}
#endif
