using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Enemy;
using UnityEngine;

namespace Meow_Moew_Shinobi.Stage
{
    [CreateAssetMenu(fileName = "StageData", menuName = "Scriptable/StageData", order = 1)]
    public class StageData : ScriptableObject
    {
        private const int LINEAR_INCREASE_EXP = 2;

        public StageWaveInfo GetWaveInfo(int level)
        {
            if(_waveInfo.Length <= level)
                return null;

            return _waveInfo[level];
        }

        public void NextEXP()
        {
            _needEXP += LINEAR_INCREASE_EXP;
        }

        public int      NexusHelath  => _nexusHealth;
        public string   StageName => _stageName;
        public int      MaxWaveCount => _maxWaveCount;
        public int      NeedEXP      => _needEXP;

        [SerializeField] private int _nexusHealth;
        [SerializeField] private int _maxWaveCount;
        [SerializeField] private string _stageName;
        [SerializeField] private int _needEXP;

        [SerializeField] private StageWaveInfo[] _waveInfo;


        [System.Serializable]
        public class StageWaveInfo
        {
            public EnemyWaveInfo EnemyWaveInfo => _enemyInfos;

            [SerializeField] private EnemyWaveInfo _enemyInfos;
        }

        [System.Serializable]
        public class EnemyWaveInfo
        {
            public EnemyScriptableData[]    EnemyDatas => _enemyDatas;
            public EEnemyType[]             EnemyTypes => _enemyTypes;
            public int[]                    SpawnCounts => _spawnCount;
            public int                      MaxSpawnCount => _maxSpawnCount;



            [SerializeField] private EnemyScriptableData[] _enemyDatas;
            [SerializeField] private EEnemyType[] _enemyTypes;
            [SerializeField] private int[] _spawnCount;
            [SerializeField] private int _maxSpawnCount;
        }
    }
}