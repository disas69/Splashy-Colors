using Game.Data;
using Game.Data.Settings;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class GameConfigurationWindow : EditorWindow
    {
        private const string Title = "Game Configuration";

        private SerializedObject _serializedObject;
        private Vector2 _colorsContentPosition;
        private Vector2 _levelsContentPosition;
        private GUIStyle _headerStyle;
        private GUIStyle _labelStyle;

        [MenuItem("Game/Configuration", priority = 3)]
        public static void OpenWindow()
        {
            var window = GetWindow<GameConfigurationWindow>(Title);
            window.minSize = new Vector2(650f, 450f);
            window.Show();
        }

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(GameConfiguration.Instance);

            _headerStyle = new GUIStyle
            {
                normal = {textColor = Color.gray},
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            _labelStyle = new GUIStyle
            {
                normal = {textColor = Color.gray},
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter
            };
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            DrawWindowLayout();

            _serializedObject.ApplyModifiedProperties();
            _serializedObject.Update();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(GameConfiguration.Instance);
            }
        }

        private void DrawWindowLayout()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(285f), GUILayout.ExpandHeight(true));
                {
                    DrawBaseSettings();
                    DrawBallSettings();
                    DrawColorSettings();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawLevels();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawBaseSettings()
        {
            EditorGUILayout.LabelField("Base settings", _headerStyle);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.PropertyField(_serializedObject.FindProperty("Lives"));
                EditorGUILayout.PropertyField(_serializedObject.FindProperty("LinesCount"));
                EditorGUILayout.PropertyField(_serializedObject.FindProperty("LinesVisibleRange"));
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawBallSettings()
        {
            EditorGUILayout.LabelField("Ball settings", _headerStyle);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                var ballSettings = _serializedObject.FindProperty("BallSettings");
                EditorGUILayout.PropertyField(ballSettings.FindPropertyRelative("JumpHeight"));
                EditorGUILayout.PropertyField(ballSettings.FindPropertyRelative("MoveSpeed"));
                EditorGUILayout.PropertyField(ballSettings.FindPropertyRelative("SmoothSpeed"));
                EditorGUILayout.PropertyField(ballSettings.FindPropertyRelative("XPositionCap"));
                EditorGUILayout.PropertyField(ballSettings.FindPropertyRelative("InCurve"));
                EditorGUILayout.PropertyField(ballSettings.FindPropertyRelative("OutCurve"));
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawColorSettings()
        {
            EditorGUILayout.LabelField("Colors", _headerStyle);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                if (GUILayout.Button("Add color"))
                {
                    RecordObject("Color Settings Change");
                    GameConfiguration.Instance.Colors.Add(new ColorSettings());
                }

                _colorsContentPosition = EditorGUILayout.BeginScrollView(_colorsContentPosition);
                {
                    var colors = _serializedObject.FindProperty("Colors");
                    var count = colors.arraySize;
                    for (var i = 0; i < count; i++)
                    {
                        EditorGUILayout.BeginHorizontal(GUI.skin.box);
                        {
                            var element = colors.GetArrayElementAtIndex(i);

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
                                GameConfiguration.Instance.Colors.RemoveAt(i);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawLevels()
        {
            EditorGUILayout.LabelField("Levels", _headerStyle);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                if (GUILayout.Button("Add level"))
                {
                    RecordObject("Level Settings Change");
                    GameConfiguration.Instance.Levels.Add(new LevelSettings());
                }

                _levelsContentPosition = EditorGUILayout.BeginScrollView(_levelsContentPosition);
                {
                    var levels = _serializedObject.FindProperty("Levels");
                    var count = levels.arraySize;
                    for (var i = 0; i < count; i++)
                    {
                        var element = levels.GetArrayElementAtIndex(i);
                        var level = element.FindPropertyRelative("Level");
                        var score = element.FindPropertyRelative("Score");
                        
                        element.isExpanded = EditorGUILayout.Foldout(element.isExpanded, $"Level {level.intValue} : {score.intValue}", true);
                        if (element.isExpanded)
                        {
                            EditorGUILayout.BeginHorizontal(GUI.skin.box);
                            {
                                EditorGUILayout.BeginVertical();
                                {
                                    EditorGUILayout.PropertyField(level);
                                    EditorGUILayout.PropertyField(score);

                                    var lineSettings = element.FindPropertyRelative("LineSettings");
                                    EditorGUILayout.LabelField("Line Settings", _labelStyle);
                                    EditorGUILayout.PropertyField(lineSettings.FindPropertyRelative("PlatformWidth"));
                                    EditorGUILayout.PropertyField(lineSettings.FindPropertyRelative("MinPlatformsCount"));
                                    EditorGUILayout.PropertyField(lineSettings.FindPropertyRelative("MaxPlatformsCount"));

                                    var pathSettings = element.FindPropertyRelative("PathSettings");
                                    EditorGUILayout.LabelField("Path Settings", _labelStyle);
                                    EditorGUILayout.PropertyField(pathSettings.FindPropertyRelative("StartSpeed"));
                                    EditorGUILayout.PropertyField(pathSettings.FindPropertyRelative("SpeedMultiplier"));
                                    EditorGUILayout.PropertyField(pathSettings.FindPropertyRelative("SpeedIncreaseTime"));
                                    EditorGUILayout.PropertyField(pathSettings.FindPropertyRelative("MinPlatformDistance"));
                                    EditorGUILayout.PropertyField(pathSettings.FindPropertyRelative("MaxPlatformDistance"));
                                    EditorGUILayout.PropertyField(pathSettings.FindPropertyRelative("MaxXShift"));
                                }
                                EditorGUILayout.EndVertical();

                                if (GUILayout.Button("X", GUILayout.Width(20)))
                                {
                                    RecordObject("Level Settings Change");
                                    GameConfiguration.Instance.Levels.RemoveAt(i);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.Space();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private void RecordObject(string changeDescription = "Game Configuration change")
        {
            Undo.RecordObject(_serializedObject.targetObject, changeDescription);
        }
    }
}