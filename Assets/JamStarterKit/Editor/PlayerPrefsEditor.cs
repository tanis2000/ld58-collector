using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GameBase.Editor
{
    public class PlayerPrefsEditor : EditorWindow
    {
        private string display = "No property selected";
        private string propertyName;
        private Vector2 scrollPos;

        [MenuItem("Window/PlayerPrefs Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<PlayerPrefsEditor>();
            window.titleContent = new GUIContent("PlayerPrefs Editor");
            window.Show();
        }

        private void OnEnable()
        {
            if (EditorPrefs.HasKey("PreviewedProperty"))
            {
                propertyName = EditorPrefs.GetString("PreviewedProperty");
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            if (!PlayerPrefs.HasKey(propertyName))
            {
                display = "No property found!";
                return;
            }
            
            display = PlayerPrefs.GetString(propertyName);

            EditorPrefs.SetString("PreviewedProperty", propertyName);
        }

        private void OnGUI()
        {
            var style = new GUIStyle
            {
                wordWrap = true,
                normal = new GUIStyleState
                {
                    textColor = Color.white,
                }
            };
            
            EditorGUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(10, 10, 10, 10) });
            
            EditorGUILayout.BeginHorizontal();

            propertyName = EditorGUILayout.TextField(propertyName);
            
            if (GUILayout.Button("Show"))
            {
                UpdateDisplay();
            }
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
            EditorGUILayout.LabelField(display, style);
            if (EditorPrefs.HasKey("PreviewedProperty"))
            {
                if (GUILayout.Button("Remove"))
                {
                    PlayerPrefs.DeleteKey(propertyName);
                    UpdateDisplay();
                }
            }
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();   
        }
    }
}