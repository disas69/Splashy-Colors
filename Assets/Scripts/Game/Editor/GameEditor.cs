using Game.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.Editor
{
    public static class GameEditor
    {
        [MenuItem("Game/Play", priority = 1)]
        public static void PlayGame()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
            EditorApplication.isPlaying = true;
        }

        [MenuItem("Game/Clear Data", priority = 2)]
        public static void ClearGameData()
        {
            FileUtil.DeleteFileOrDirectory(Application.persistentDataPath + "/" + GameData.FileName);
        }
    }
}