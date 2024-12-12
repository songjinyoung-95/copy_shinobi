using System;
using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Skill;
using Meow_Moew_Shinobi.Weapon;
using Meow_Moew_Shinobi.Weapon.UI;
using UnityEngine;

namespace Meow_Moew_Shinobi.Singleton
{
    public class WeaponManager
    {
        private const string VIEW_PATH = "Weapon/UI/EquipWeaponView";

        private static WeaponManager _instance;
        public static WeaponManager  Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new WeaponManager();

                return _instance;
            }
        }

        private GameObject _parent;
        private readonly IWeaponView _view;
        private readonly Dictionary<EWeaponType, WeaponController> _savedWeaponController;
        private readonly Dictionary<EWeaponType, WeaponController> _equipedWeaponDict;

        public EWeaponType[] EquipWeapons
        {
            get
            {
                EWeaponType[] equipWeapons = new EWeaponType[_equipedWeaponDict.Count];
                int index = 0;
                
                foreach (var item in _equipedWeaponDict)
                {
                    equipWeapons[index] = item.Key;
                    index++;
                }

                return equipWeapons;
            }
        }


        /// <summary>
        /// 생성자
        /// </summary>
        public WeaponManager()
        {
            _equipedWeaponDict = new Dictionary<EWeaponType, WeaponController>();
            _savedWeaponController = new Dictionary<EWeaponType, WeaponController>();

            if (_view == null)
            {
                _view = UnityEngine.Object.Instantiate(Resources.Load<WeaponView>(VIEW_PATH));
                _view.Init(5);
            }

            _parent = new GameObject("Weapon Controllers");
            _parent.transform.position = Vector3.zero;
        }

        public void Release()
        {
            foreach (var item in _equipedWeaponDict)
                item.Value.Release();

            _view.Release();          
            _equipedWeaponDict.Clear();
        }



        /// <summary>
        /// 장착중인 무기인지 리턴하는 함수
        /// </summary>
        public bool IsWeaponEquipped(EWeaponType type)
        {
            return _equipedWeaponDict.ContainsKey(type);
        }


        /// <summary>
        /// 선택한 웨폰을 장착 or 업그레이드
        /// </summary>
        public void SelectWeapon(EWeaponType weaponType, IWeaponAbility ability, SkillData skillData)
        {   
            if(_equipedWeaponDict.ContainsKey(weaponType))
            {
                if(!_equipedWeaponDict.TryGetValue(weaponType, out WeaponController weaponController))
                {
                    Debug.LogError($"{weaponType} / {nameof(WeaponController)}클래스가 없습니다");
                    return;
                }

                if(skillData.StatusEffectData != null)
                    weaponController.AddStatusEffect(skillData.StatusEffectData);

                weaponController.AddAbility(ability, skillData);

                /// TODO : 이미 장착중인 웨폰은 강화
                _view.UpgradeWeaponUI(weaponType);
                return;
            }

            WeaponController controller = TryLoadWeapon(weaponType);

            if (controller == null)
            {
                controller = new GameObject().AddComponent<WeaponController>();

                controller.transform.SetParent(_parent.transform);
                controller.name = $"{weaponType}_Cotnroller";

                controller.transform.position = StageManager.Instance.GetCharacterTransform().position;

                _savedWeaponController.Add(weaponType, controller);
            }

            _equipedWeaponDict.Add(weaponType, controller);

            controller.OnNearEnemy      += EnemyManager.Instance.FindNearestEnemys;
            controller.OnFireCoolTime   += _view.WeaponFireCoolTimeUI;
            controller.OnHitText        += UIPoolManager.Instance.ShowHitText;

            controller.Init(weaponType);
            _view.EquipWeaponUI(weaponType);

            if (skillData.StatusEffectData != null)
                controller.AddStatusEffect(skillData.StatusEffectData);
        }


        private WeaponController TryLoadWeapon(EWeaponType weaponType)
        {
            if(_savedWeaponController.ContainsKey(weaponType))
            {
                if(!_savedWeaponController.TryGetValue(weaponType, out var controller))
                {
                    Debug.LogError($"컨트롤러가 없습니다");
                    return null;
                }

                controller.gameObject.SetActive(true);
                return controller;
            }

            return null;
        }
    }
}