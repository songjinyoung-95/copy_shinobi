using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Meow_Moew_Shinobi.Pool;
using Meow_Moew_Shinobi.UI.EffectText;
using Meow_Moew_Shinobi.Skill;

namespace Meow_Moew_Shinobi.Weapon
{
    public delegate List<Transform> FindNearestEnemy(int count, Vector3 pos);
    public delegate HitText HitDamageText(ETextType textType, Vector3 pos);

    public class WeaponController : MonoBehaviour
    {
        ///----------------------
        /// const, readonly
        /// ---------------------        
        private const string WEAPON_POOL_PATH   = "Weapon/Model/";
        private const string DATA_PATH          = "Weapon/Data/";

        private const int DEFAULT_POOL_COUNT = 5;
        private readonly static Vector3 DEFAULT_WEAPON_SIZE = Vector3.one;
        private readonly static Vector3 SPLIT_WEAPON_SIZE   = Vector3.one * 0.7f;


        ///----------------------
        /// private
        /// ---------------------
        private string      _weaponName;
        private EWeaponType _weaponType;
        private WeaponData  _data;
        
        private PoolBase<WeaponBase>    _weaponPool;

        private List<IWeaponAbility>    _abilities;
        private List<StatusEffectData>  _statusEffects;

        private WaitForSeconds  _fireDelay;
        private WaitForSeconds  _serial_FireDelay;

        private Coroutine _coFire;

        ///----------------------
        /// evnets
        /// ---------------------
        public event HitDamageText OnHitText;
        public event Action<EWeaponType, float> OnFireCoolTime;
        public event FindNearestEnemy OnNearEnemy;

        public void Init(EWeaponType type)
        {
            _abilities      = new List<IWeaponAbility>();
            _statusEffects  = new List<StatusEffectData>();

            _weaponType     = type;
            _weaponName     = _weaponType.ToString();

            SetData();
            WeaponGenerator();
            CheckFireWeapon();
        }

        public void Release()
        {
            OnFireCoolTime  -= OnFireCoolTime;
            OnNearEnemy     -= OnNearEnemy;

            if(_coFire != null)
            {
                StopCoroutine(_coFire);
                _coFire = null;
            }

            gameObject.SetActive(false);
        }

        public void AddAbility(IWeaponAbility ability, SkillData skillData)
        {
            _abilities.Add(ability);
            ability.AddAbility(_data, skillData);
        }

        public void AddStatusEffect(StatusEffectData statusEffectData)
        {
            foreach (var effect in _statusEffects)
            {
                if(effect.EffectType == statusEffectData.EffectType)
                {
                    effect.IncreaseValue(statusEffectData.Duration, statusEffectData.Intensity);
                    return;
                }
            }
            
            _statusEffects.Add(statusEffectData);
        }


        private void SetData()
        {
            string      path = DATA_PATH + _weaponType;
            WeaponData  data = Resources.Load<WeaponData>(path);

            if(data == null)
            {
                Debug.LogError($"{DATA_PATH}에 해당하는 {_weaponType}의 데이터 스크립터블이 존재하지 않습니다");
                return;                
            }

            _data = Instantiate(data);
            _data.Init();

            _fireDelay          = new WaitForSeconds(_data.FireDelay);
            _serial_FireDelay   = new WaitForSeconds(_data.Serial_FireDelay);
        }

        private void WeaponGenerator()
        {
            _weaponPool = new PoolBase<WeaponBase>(transform);

            string      path    = WEAPON_POOL_PATH + _weaponType;
            WeaponBase  weapon  = Resources.Load<WeaponBase>(path);

            if(weapon == null)
            {
                Debug.LogError($"{WEAPON_POOL_PATH}에 해당하는 {weapon} 오브젝트가 존재하지 않습니다");
                return;
            }

            _weaponPool.Generator(weapon, _weaponName, DEFAULT_POOL_COUNT);
        }



        private void CheckFireWeapon()
        {
            List<Transform> enemiesTF = OnNearEnemy?.Invoke(_data.FireCount, transform.position);

            if (enemiesTF.Count <= 0)
            {
                _coFire = StartCoroutine(Co_FindTarget());
                return;
            }

            _coFire = StartCoroutine(Co_Fire(enemiesTF));
        }



        private IEnumerator Co_FindTarget()
        {
            List<Transform> enemiesTF = OnNearEnemy?.Invoke(_data.FireCount, transform.position);
            while (enemiesTF.Count <= 0)
            {
                enemiesTF = OnNearEnemy?.Invoke(_data.FireCount, transform.position);
                yield return null;
            }

            _coFire = StartCoroutine(Co_Fire(enemiesTF));
        }



        private IEnumerator Co_Fire(List<Transform> enemiesTF)
        {
            Transform firstEnemy = enemiesTF[0];

            float dir = float.MaxValue;
            while (dir >= _data.Range)
            {
                dir = Mathf.Abs(firstEnemy.position.y - transform.position.y);
                yield return null;
            }

            RemoveNullEnemies(enemiesTF);

            for (int i = 0; i < _data.FireCount; i++)
            {
                WeaponBase weapon = SpawnWeapon(transform.position);
                weapon.SetWeaponSize(DEFAULT_WEAPON_SIZE);
                weapon.FireTarget(enemiesTF, i);

                if (_data.SplitCount > 0)
                    weapon.OnHitSplit += WeaponHitSplit;

                yield return _serial_FireDelay;
            }

            OnFireCoolTime?.Invoke(_weaponType, _data.FireDelay);
            yield return new WaitForSeconds(_data.FireDelay);

            CheckFireWeapon();
        }

        private WeaponBase SpawnWeapon(Vector3 spawnPos)
        {
            WeaponBase weapon = _weaponPool.Spawn(_weaponName, spawnPos, Quaternion.identity);

            weapon.OnHitEffect += DamageText;
            weapon.Init(_data, _statusEffects, _weaponPool.Despawn);

            return weapon;
        }

        private void WeaponHitSplit(Vector3 spawnPos)
        {
            float spawnDistance = 0.4f;
            for (int i = 0; i < _data.SplitCount; i++)
            {
                float   randomAngle     = UnityEngine.Random.Range(0f, 180);
                float   radians         = randomAngle * Mathf.Deg2Rad;
                Vector3 randomDirection = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians));

                Vector3 offset          = randomDirection.normalized * spawnDistance;
                Vector3 spawnPosition   = spawnPos + offset;

                WeaponBase weapon = SpawnWeapon(spawnPosition);
                weapon.SetWeaponSize(SPLIT_WEAPON_SIZE);
                weapon.Fire(randomDirection + spawnPos);
            }
        }


        private HitText DamageText(ETextType textType, Vector3 pos)
        {
             return OnHitText?.Invoke(textType, pos);
        }


        private List<Transform> RemoveNullEnemies(List<Transform> enemies)
        {
            float dir = float.MaxValue;
            for (int i = enemies.Count - 1; i < 0; i--)
            {
                dir = Mathf.Abs(enemies[i].position.y - transform.position.y);

                if(dir < _data.Range)
                    enemies.Remove(enemies[i]);
            }

            return enemies;
        }



#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _data.Range);
        }
#endif
    }
}