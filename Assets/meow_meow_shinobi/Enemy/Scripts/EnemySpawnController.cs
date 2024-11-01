using System;
using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Pool;
using UnityEngine;

namespace Meow_Moew_Shinobi.Enemy
{
    public interface ISpawnable
    {
        void Init();
        EnemyBase Spawn(EEnemyType type, Vector2 spawnPos, Quaternion spawnRot);
        void Despawn(EnemyBase enemy);
    }

    public class EnemySpawnController : MonoBehaviour, ISpawnable
    {
        private const string ENEMY_PATH = "Enemy/Model/";

        private PoolBase<EnemyBase> _pool;

        public void Init()
        {
            _pool = new PoolBase<EnemyBase>(transform);
            
            for (int i = 0, size = Enum.GetNames(typeof(EEnemyType)).Length; i < size; i++)
            {
                string      enemyType   = Enum.GetName(typeof(EEnemyType), i);
                string      path        = ENEMY_PATH + enemyType;
                EnemyBase   resource    = Resources.Load<EnemyBase>(path);

                if(resource == null)
                {
                    Debug.LogError($"{path} 경로에 {(EEnemyType)i} 의 프리펩이 없습니다");
                }

                _pool.Generator(resource, enemyType, 10);
            }
            
            DontDestroyOnLoad(gameObject);
        }

        public EnemyBase Spawn(EEnemyType type, Vector2 spawnPos, Quaternion spawnRot)
        {
            return _pool.Spawn(type.ToString(), spawnPos, spawnRot);
        }

        public void Despawn(EnemyBase enemy)
        {
            _pool.Despawn(enemy, enemy.EnemyType.ToString());   
        }        
    }
}