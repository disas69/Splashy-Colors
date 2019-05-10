﻿using System;
using Framework.Tools.Data;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class Data
    {
        public int CurrentScore;
        public int BestScore;
    }

    public static class GameData
    {
        public const string FileName = "GameData";

        private static readonly JsonDataKeeper<Data> DataKeeper;

        public static Data Data { get; private set; }

        static GameData()
        {
            DataKeeper = new JsonDataKeeper<Data>(Application.persistentDataPath + "/" + FileName, true);
        }

        public static void Load()
        {
            Data = DataKeeper.Load();
        }

        public static void Save()
        {
            DataKeeper.Save(Data);
        }
    }
}