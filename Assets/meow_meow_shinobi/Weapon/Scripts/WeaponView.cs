using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Util;
using UnityEngine;

namespace Meow_Moew_Shinobi.Weapon.UI
{
    public interface IWeaponView
    {
        void Init(int slotSize);
        void Release();
        void EquipWeaponUI(EWeaponType weaponType);
        void UpgradeWeaponUI(EWeaponType weaponType);
        void WeaponFireCoolTimeUI(EWeaponType weaponType, float coolTime);
    }

    public class WeaponView : BehaviourBase, IWeaponView
    {
        private const string SLOT_PATH = "Weapon/UI/Slot";

        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _slotRTF;

        private WeaponSlot[] _slots;


        private void AddSlot(int count)
        {
            _slots = new WeaponSlot[count];

            for (int i = 0; i < count; i++)
            {
                _slots[i] = Instantiate(Resources.Load<WeaponSlot>(SLOT_PATH), _slotRTF);
                _slots[i].Init();
            }
        }

        /// --------------------
        /// Interface
        /// --------------------

        public void EquipWeaponUI(EWeaponType weaponType)
        {
            WeaponSlot weaponSlot = null;
            foreach (var slot in _slots)
            {
                if(slot.EquipWeaponType != EWeaponType.None)
                    continue;

                weaponSlot = slot;
                break;
            }

            if(weaponSlot == null)
            {
                Debug.LogError($"{nameof(WeaponView)} 빈 슬롯이 없습니다");
                return;
            }

            weaponSlot.EquipWeapon(weaponType, WeaponSpriteMapper.GetWeaponUISprite(weaponType));
        }

        public void WeaponFireCoolTimeUI(EWeaponType weaponType, float coolTime)
        {
            FindWeaponTypeBySlot(weaponType).WeaponFireCoolTime(coolTime);
        }


        public void UpgradeWeaponUI(EWeaponType weaponType)
        {
            FindWeaponTypeBySlot(weaponType).UpgradeWeapon();
        }

        
        private WeaponSlot FindWeaponTypeBySlot(EWeaponType weaponType)
        {
            foreach (var slot in _slots)
            {
                if(slot.EquipWeaponType != weaponType)
                    continue;

                return slot;
            }

            Debug.LogError($"{nameof(WeaponView)} {weaponType} 타입의 슬롯이 없습니다");
            return null;
        }

        public void Init(int slotSize)
        {
            _canvas.worldCamera = Camera.main;

            AddSlot(slotSize);
            DontDestroyOnLoad(gameObject);
        }

        public void Release()
        {
            foreach (var slot in _slots)
                slot.Init();
        }


#if UNITY_EDITOR
        protected override void OnBindSerialzedField()
        {
            _slotRTF = transform.Find("RTF_Slot").GetComponent<RectTransform>();
        }
#endif
    }
}