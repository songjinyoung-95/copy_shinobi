using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Enemy;
using Meow_Moew_Shinobi.Pool;
using UnityEngine;

namespace Meow_Moew_Shinobi.Singleton
{    
    public class EnemyManager
    {
        private static EnemyManager _instance;
        public static EnemyManager  Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new EnemyManager();

                return _instance;
            }
        }

        private List<EnemyBase> _enemies;
        private ISpawnable      _spawner;

        public EnemyManager()
        {
            _enemies = new List<EnemyBase>();

            if (_spawner == null)
            {
                _spawner = new GameObject("EnemySpanwer").AddComponent<EnemySpawnController>();
                _spawner.Init();
            }
        }

        public EnemyBase SpawnEnemy(EEnemyType type, EnemyScriptableData data, Vector2 pos, Quaternion rot)
        {
            EnemyBase enemy = _spawner.Spawn(type, pos, rot);

            // enemy.OnFreezeEvent += () => VFXManager.Instance.ShowVFX(EVfxType.Freeze, enemy.transform.position, enemy.transform);

            if (enemy == null)
            {
                Debug.LogError($"Enemy Null");
                return null;
            };

            enemy.Spawn(data, _spawner.Despawn);
            EventByEnemyType(enemy);
            AddEnemy(enemy);

            return enemy;
        }

        private void AddEnemy(EnemyBase enemy)
        {
            if(_enemies.Contains(enemy))
                return;

            _enemies.Add(enemy);
        }

        public void RemoveEnemy(EnemyBase enemy)
        {
            if(!_enemies.Contains(enemy))
                return;

            _enemies.Remove(enemy);
        }

        public void AllStopEnemies()
        {
            foreach (var enemy in _enemies)
                enemy.Release();
        }

        public void AllReturnEnemies()
        {
            foreach (var enemy in _enemies)
                _spawner.Despawn(enemy);
            
            _enemies.Clear();
        }

        private Transform FindNearestEnemy(List<Transform> duplicates, Vector3 position)
        {
            Transform nearestEnemy  = null;
            float nearestDistance   = float.MaxValue;

            foreach (var enemy in _enemies)
            {
                if(duplicates.Contains(enemy.transform))
                    continue;

                float distance = Vector3.Distance(position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy.transform;
                }
            }

            return nearestEnemy;
        }

        public List<Transform> FindNearestEnemys(int count, Vector3 position)
        {
            List<Transform> enemiesTF = new List<Transform>();
            
            for (int i = 0; i < count; i++)
            {
                Transform enemyTF = FindNearestEnemy(enemiesTF, position);

                if (enemyTF == null)
                    continue;

                enemiesTF.Add(enemyTF);
            }

            return enemiesTF;
        }

        private void EventByEnemyType(EnemyBase enemy)
        {
            enemy.DieEvent += RemoveEnemy;
            enemy.DieEvent += (enemy) => VFXManager.Instance.ShowVFX(EVfxType.Die, enemy.transform.position);

            switch (enemy.EBehavioralType)
            {
                case EBehavioralType.Default:
                    break;

                case EBehavioralType.Die_Splitting:
                    enemy.DieEvent += SplitEnemy;
                    break;
            }
        }

        private void SplitEnemy(EnemyBase enemy)
        {
            float offset = 0.25f;
            Vector3[] vectors = new Vector3[] 
            {
                enemy.transform.position + Vector3.left * offset,
                enemy.transform.position + Vector3.right* offset,
            };

            Splitting_Enemy splitting = enemy as Splitting_Enemy;

            if(splitting == null)
            {
                Debug.LogError($"{enemy.name} : {splitting} 클래스로 캐스팅 할 수 없습니다");
                return;
            }

            if(splitting.IsSplit)
                return;

            ///분열된 객체인지 검사
            for (int i = 0; i < vectors.Length; i++)
            {
                Splitting_Enemy splitting_Enemy = SpawnEnemy(enemy.EnemyType, enemy.Data, vectors[i], enemy.transform.rotation) as Splitting_Enemy;
                splitting_Enemy.SetSplit();
                
                StageManager.Instance.NewSplitSpawnEnemy(splitting_Enemy);
            }
        }
    }
}