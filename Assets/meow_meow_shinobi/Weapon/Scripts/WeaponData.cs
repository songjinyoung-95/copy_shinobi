using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow_Moew_Shinobi.Weapon
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable/WeaponData", order = 1)]
    public class WeaponData : ScriptableObject
    {
        public int FireCount => _fireCount;
        public int SplitCount => _splitCount;
        public int ApplyHitCount => _applyHitCount;
        public int Damage => _damage;
        public int ShockDamage => _shockDamage;
        public float ShockRange => _shockRange;
        public float MoveSpeed => _moveSpeed;
        public float FireDelay => _fireDelay;
        public float Serial_FireDelay => _serial_fireDelay;
        public float KnockbackForce => _knockbackForce;
        public float Range => _range;


        [Header("발사 횟수")]
        [SerializeField] private int    _fireCount = 1;
        [Header("적중 시 분열")]
        [SerializeField] private int    _splitCount = 0;
        [Header("관통력")]
        [SerializeField] private int    _applyHitCount = 1;
        [Header("데미지")]
        [SerializeField] private int    _damage = 1;
        [Header("충격 데미지")]
        [SerializeField] private int    _shockDamage = 0;
        [Header("충격 범위")]
        [SerializeField] private float  _shockRange = 0;
        [Header("발사 속도")]
        [SerializeField] private float  _moveSpeed = 20;
        [Header("발사 딜레이")]
        [SerializeField] private float  _fireDelay = 1;
        [Header("연속 발사 딜레이")]
        [SerializeField] private float  _serial_fireDelay = 0;
        [Header("발사 거리")]
        [SerializeField] private float  _range = 8.5f;
        [Header("넉백 파워")]
        [SerializeField] private float  _knockbackForce = 0;

        private Dictionary<EAbility, Action<float>> _matchAbility;

        public void Init()
        {
            _matchAbility = new Dictionary<EAbility, Action<float>>()
            {
                {EAbility.FireCount,      value => _fireCount           += (int)value},  
                {EAbility.HitCount,       value => _applyHitCount       += (int)value},
                {EAbility.Split,          value => _splitCount          += (int)value}, 
                {EAbility.PowerUp,        value => _damage              += Mathf.RoundToInt(_damage * value)}, 
                {EAbility.ShockPower,     value => _shockDamage         += Mathf.RoundToInt(_shockDamage * value)},
                {EAbility.FireSpeed,      value => _moveSpeed           += Mathf.RoundToInt(_moveSpeed * value)},

                {EAbility.Knockback,      value => _knockbackForce      *= 1 + value}, 
                {EAbility.ShockRange,     value => _shockRange          *= 1 + value}, 
                {EAbility.FireDelay,      value => _fireDelay           *= 1 - value},
            };
        }

        public void IncreaseAbility(EAbility[] abilitys, float[] increase)
        {
            for (int i = 0, size = abilitys.Length; i < size; i++)
            {
                if(!_matchAbility.ContainsKey(abilitys[i]))
                {
                    Debug.LogError($"{abilitys[i]} 등록되지 않았습니다");
                    continue;
                }

                _matchAbility[abilitys[i]](increase[i]);
            }
        }
    }
}