using System.Collections.Generic;
using UnityEngine;

namespace Creotly.FizzFazz
{
    public class GamesManager : MonoBehaviour
    {
        public static GamesManager Instance { get; private set; }
        public int CurrentLevel { get; private set; }

        Dictionary<string, LevelData> Levels;
        [SerializeField] private LevelData DefaultLevel;
        [SerializeField] private LevelList _allLevels;

        private void Awake()
        {
            // ----- Singleton -----
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;

            // ----- Setup -----
            CurrentLevel = 1; // Placeholder

            Levels = new Dictionary<string, LevelData>();

            foreach (var item in _allLevels.Levels)
            {
                Levels[item.LevelName] = item;
            }
        }

        public LevelData GetLevel()
        {

            string levelName = "Level" + CurrentLevel.ToString();

            if (Levels.ContainsKey(levelName))
            {
                return Levels[levelName];
            }

            return DefaultLevel;
        }
        
        public int GetLevelElementsCount()
        {
            return DefaultLevel.Edges.Count;
        }
    }

}