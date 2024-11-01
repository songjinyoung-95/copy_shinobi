using System;
using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Enemy;
using Meow_Moew_Shinobi.Weapon;
using UnityEngine;

namespace Meow_Moew_Shinobi.Stage
{
    public delegate EnemyBase EnemySpawn(EEnemyType type, EnemyScriptableData data, Vector2 pos, Quaternion rot);
    public delegate void LevelUp();

    public class StageController : MonoBehaviour
    {
        private readonly Vector3 SPAWN_ROTATION = new Vector2(0, 180);

        /// -------------------
        /// get, set
        /// -------------------
        
        public Transform TF_Character => _character_TF;
        public Vector2 StageArea_Min => _spawnAreaMin;
        public Vector2 StageArea_Max => _spawnAreaMax;

        private Vector2 GetRandomSpawnArea => new Vector2(UnityEngine.Random.Range(_spawnAreaMin.x, _spawnAreaMax.x), _spawnAreaMin.y);


        /// -------------------
        /// serializedField
        /// -------------------
        
        [SerializeField] private StageData  _stageData;
        [SerializeField] private Nexus      _nexus;

        [SerializeField] private Vector2 _spawnAreaMin;
        [SerializeField] private Vector2 _spawnAreaMax;

        [SerializeField] private Transform _character_TF;



        /// -------------------
        /// private
        /// -------------------
        
        private IStaeView _view;
        
        private int _currentWave;
        private int _totalEnemyCount;
        private int _gainedExp;

        private Coroutine _coWaveSpawnEnemy;

        /// -------------------
        /// readonly
        /// -------------------        
        private readonly WaitForSeconds _spawnDelay         = new WaitForSeconds(3);
        private readonly WaitForSeconds _oneSecondsDelay    = new WaitForSeconds(1);


        /// -------------------
        /// event
        /// -------------------
        
        public event Action OnEndStage;
        public event EnemySpawn OnSpawnEnemy;
        public event LevelUp    OnLevelUp;

        public void Init(IStaeView view)
        {
            _currentWave        = 0;
            _totalEnemyCount    = 0;
            _gainedExp          = 0;

            _view       = view;
            _stageData  = Instantiate(_stageData);

            OnEndStage += EndStage;
            
            _view.Init(_stageData.StageName);
            _nexus.Init(_stageData.NexusHelath, OnEndStage, null);
        }

        private void AddExp(EnemyBase enemy)
        {
            _totalEnemyCount--;

            int needExp = _stageData.NeedEXP;

            _gainedExp += enemy.Data.ExperienceEXP;
            _view.UpdateLevelPrograss(_gainedExp, needExp);

            if (_gainedExp >= needExp)
            {
                _gainedExp = 0;

                OnLevelUp?.Invoke();
                _stageData.NextEXP();
                
                _view.UpdateLevelPrograss(_gainedExp, _stageData.NeedEXP);
            }                                 
        }

        public void StartWave()
        {
            StageData.StageWaveInfo waveInfo = _stageData.GetWaveInfo(_currentWave);

            if(waveInfo == null)
            {
                Debug.LogError($"{_currentWave}에 해당하는 웨이브 데이터 없습니다");
                return;
            }

            StageData.EnemyWaveInfo enemyWaveInfo = waveInfo.EnemyWaveInfo;

            if(_coWaveSpawnEnemy != null)
                return;

            _coWaveSpawnEnemy = StartCoroutine(Co_WaveSpawnEnemy(enemyWaveInfo));
            _currentWave++;

            _view.UpdateWaveLevelText(_currentWave, _stageData.MaxWaveCount);
        }

        private void EndStage()
        {
            if (_coWaveSpawnEnemy != null)
            {
                StopCoroutine(_coWaveSpawnEnemy);
                _coWaveSpawnEnemy = null;
            }
        }

        public void DestroyStage()
        {
            _nexus  = null;
            _view   = null;

            if (_coWaveSpawnEnemy != null) StopCoroutine(_coWaveSpawnEnemy);

            Destroy(gameObject);
        }
        

        private IEnumerator Co_CheckAllEnemiesDefeated()
        {
            while(_totalEnemyCount > 0)
                yield return null;

            OnEndStage.Invoke();
        }


        private IEnumerator Co_WaveSpawnEnemy(StageData.EnemyWaveInfo enemyWaveInfo)
        {
            int[] counts = enemyWaveInfo.SpawnCounts;

            while (IsAnyEnemyRemaining(counts))
            {
                yield return _spawnDelay;
                
                int maxSpawnCount = enemyWaveInfo.MaxSpawnCount;

                for (int i = 0; i < enemyWaveInfo.EnemyTypes.Length; i++)
                {
                    if (counts[i] <= 0)
                        continue;

                    int spawnCount = Mathf.Min(counts[i], maxSpawnCount);
                    SpawnEnemies(spawnCount, enemyWaveInfo.EnemyTypes[i], enemyWaveInfo.EnemyDatas[i]);

                    counts[i]       -= spawnCount;
                    maxSpawnCount   -= spawnCount;

                    if (maxSpawnCount <= 0)
                        break;
                }
            }
            _coWaveSpawnEnemy = null;
            yield return StartCoroutine(Co_NextWave());
        }

        private IEnumerator Co_NextWave()
        {
            if(_stageData.MaxWaveCount <= _currentWave)
            {
                StartCoroutine(Co_CheckAllEnemiesDefeated());
                yield break;
            }

            int nextWaveTime = 5;
            
            _view.UpdateWaveTimer(nextWaveTime);
            _view.ShowWaveTimer();

            /// UI 오픈
            while (nextWaveTime > 0)
            {
                yield return _oneSecondsDelay;

                nextWaveTime--;
                _view.UpdateWaveTimer(nextWaveTime);
            }

            _view.HideWaveTimer();
            StartWave();
        }

        public void SpawnSplitEnemy(EnemyBase enemy)
        {
            enemy.DieEvent += AddExp;
            _totalEnemyCount++;
        }

        private void SpawnEnemies(int count, EEnemyType enemyType, EnemyScriptableData data)
        {
            for (int i = 0; i < count; i++)
            {
                EnemyBase enemy = OnSpawnEnemy?.Invoke(enemyType, data, GetRandomSpawnArea, Quaternion.Euler(SPAWN_ROTATION));
                enemy.DieEvent += AddExp;
                _totalEnemyCount++;
            }
        }

        private bool IsAnyEnemyRemaining(int[] counts)
        {
            foreach (int count in counts)
            {
                if (count > 0)
                    return true;
            }
            return false;
        }
    }
}