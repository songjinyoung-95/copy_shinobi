using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow_Moew_Shinobi.Enemy
{

    [CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable/EnemyData", order = 1)]
    public class EnemyScriptableData : ScriptableObject
    {
        ///----------------------
        /// Get, Set
        /// ---------------------
        public int      MaxHelath           => _maxHealth;
        public int      Power               => _power;
        public float    MoveSpeed           => _moveSpeed;
        public int      ExperienceEXP       => _experienceEXP;
        public float    AttackRange         => _attackRange;
        public float    AttackDelay         => _attackDelay;

        ///----------------------
        /// SerializedField 
        /// ---------------------
        [SerializeField] private int    _maxHealth;
        [SerializeField] private int    _power;
        [SerializeField] private float  _moveSpeed;
        [SerializeField] private int    _experienceEXP;
        [SerializeField] private float  _attackRange;
        [SerializeField] private float  _attackDelay;
    }
}