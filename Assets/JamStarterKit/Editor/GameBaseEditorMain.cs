using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameBase.Editor
{
    public partial class GameBaseEditor
    {
        private Vector2 scrollPosition;

        void ApplyFilter<T>(List<T> prefablist, ref List<T> list, ref bool filterBool) where T : MonoBehaviour
        {
            if (filterBool)
                list = prefablist.FindAll(item => IsHighlighted(item));
            if (filterBool == false || list.Count == 0)
            {
                list = prefablist;
                filterBool = false;
            }
        }

        void LayoutListHeader(string name, ref bool show, ref bool filter, Rect rect)
        {
            show = EditorGUI.Foldout(new Rect(rect) { width = rect.width - 60 }, show, name, true);
            if (GUI.Button(new Rect(rect) { x = rect.width - 60, width = 60 }, filter ? "Highlighted" : "All",
                    new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleRight }))
            {
                filter = !filter;
                CreateMainGuiLists(); // Refresh lists
            }
        }

        void CreateMainGuiLists()
        {
            //
            // Create reorderable lists
            //

            // Filters
        }

        private void OnGUIMain()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Space(5);

            EditorGUILayout.EndScrollView();
        }
    }
}