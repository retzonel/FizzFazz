using System.Collections.Generic;
using UnityEngine;

namespace Creotly.FizzFazz
{
    [CreateAssetMenu(fileName = "Levels", menuName = "Levels/AllLevels")]
    public class LevelList : ScriptableObject
    {
        public List<LevelData> Levels;
    }
}

