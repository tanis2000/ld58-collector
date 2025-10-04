using GameBase.Utils;
using UnityEditor;
using UnityEngine;

namespace GameBase.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxRange))]
    public class MinMaxRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            //EditorGUIUtility.LookLikeControls ();

            EditorGUI.BeginProperty(pos, label, prop);

            SerializedProperty min = prop.FindPropertyRelative("m_min");
            SerializedProperty max = prop.FindPropertyRelative("m_max");
            SerializedProperty hasMax = prop.FindPropertyRelative("m_hasMax");
            SerializedProperty hasValue = prop.FindPropertyRelative("m_hasValue");

            float toWidth = 16;
            float maxLblWidth = 55;
            float buttonWidth = 20;

            float rectWidth = 0; //(pos.width-toWidth)*0.4f;		
            pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);
            //EditorGUI.LabelField( new Rect(posX, pos.y,rectWidth, pos.height), prop.name);
            //posX += rectWidth;
            float posX = pos.x;

            // Don't make child fields be indented
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (min != null && max != null && hasMax != null)
            {
                bool hasMaxVal = hasMax.boolValue;

                if (hasMaxVal == false)
                {
                    rectWidth = pos.width - (maxLblWidth + 4 + buttonWidth);
                }
                else
                {
                    rectWidth = (pos.width - (toWidth + 2 + buttonWidth + 2)) * 0.5f;
                }

                // EditorGUI.IndentedRect(new Rect(posX, pos.y,rectWidth, pos.height));
                EditorGUI.BeginChangeCheck();
                float minVal = EditorGUI.FloatField(new Rect(posX, pos.y, rectWidth, pos.height), min.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    min.floatValue = minVal;
                    if (max.floatValue < minVal)
                    {
                        max.floatValue = minVal;
                    }

                    hasValue.boolValue = false;
                }

                if (hasMaxVal)
                {
                    posX += rectWidth + 2;

                    EditorGUI.LabelField(new Rect(posX, pos.y, toWidth, pos.height), "to");
                    posX += toWidth;

                    // EditorGUI.IndentedRect(new Rect(posX, pos.y,rectWidth, pos.height));
                    EditorGUI.BeginChangeCheck();
                    float maxVal = EditorGUI.FloatField(new Rect(posX, pos.y, rectWidth, pos.height), max.floatValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        max.floatValue = maxVal;
                        hasValue.boolValue = false;
                    }

                    posX += rectWidth + 2;
                }
                else
                {
                    posX += rectWidth + 4;
                    EditorGUI.LabelField(new Rect(posX, pos.y, maxLblWidth, pos.height), "Is Range");
                    posX += maxLblWidth;
                }

                // button
                EditorGUI.BeginChangeCheck();
                hasMaxVal = EditorGUI.Toggle(new Rect(posX, pos.y, buttonWidth, pos.height), hasMaxVal);

                if (EditorGUI.EndChangeCheck())
                {
                    hasMax.boolValue = hasMaxVal;
                    hasValue.boolValue = false;
                }
            }
            else
            {
                Debug.LogWarning("Min or max properties null");
            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();

            //EditorGUILayout.LabelField("Yo dawg!");
        }
    }
}