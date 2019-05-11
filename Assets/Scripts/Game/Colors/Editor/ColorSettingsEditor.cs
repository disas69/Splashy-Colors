using Framework.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.Colors.Editor
{
    /*
    [CustomEditor(typeof(ColorSettings))]
    public class ColorSettingsEditor : CustomEditorBase<ColorSettings>
    {
        protected override void DrawInspector()
        {
            base.DrawInspector();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Colors", HeaderStyle);
                if (GUILayout.Button("Add color"))
                {
                    RecordObject("Color Settings Change");
                    Target.Configs.Add(new ColorConfig());
                }

                var settings = serializedObject.FindProperty("Configs");
                var count = settings.arraySize;
                for (var i = 0; i < count; i++)
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        var element = settings.GetArrayElementAtIndex(i);

                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUILayout.PropertyField(element.FindPropertyRelative("Name"));
                            EditorGUILayout.PropertyField(element.FindPropertyRelative("Color"));
                            EditorGUILayout.PropertyField(element.FindPropertyRelative("Material"));
                        }
                        EditorGUILayout.EndVertical();

                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            RecordObject("Color Settings Change");
                            Target.Configs.RemoveAt(i);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
    */
}