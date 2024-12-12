using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meow_Moew_Shinobi.Weapon.UI
{
    public class WeaponSlot : BehaviourBase
    {
        private readonly static Color32 NONE_WEAPON_COLOR   = new Color32(0,0,0,0);
        private readonly static Color32 EQUIP_WEAPON_COLOR  = new Color32(255,255,255,255);

        public EWeaponType EquipWeaponType { get; private set; }

        [SerializeField] private Image _fill;
        [SerializeField] private Image _thumnail;
        [SerializeField] private TextMeshProUGUI _level_TMP;
        

        private Coroutine _coCoolTime;
        
        public void Init()
        {
            if (_coCoolTime != null)
            {
                StopCoroutine(_coCoolTime);
                _coCoolTime = null;
            }

            EquipWeaponType     = EWeaponType.None;
            _fill.fillAmount    = 0;
            _thumnail.color     = NONE_WEAPON_COLOR;
            _level_TMP.text     = "";
        }


        public void EquipWeapon(EWeaponType weaponType, Sprite thumnail)
        {
            EquipWeaponType     = weaponType;
            _thumnail.color     = EQUIP_WEAPON_COLOR;
            _thumnail.sprite    = thumnail;
            _level_TMP.text     = "1";
        }


        public void UpgradeWeapon()
        {
            int parseInt = int.Parse(_level_TMP.text);
            parseInt++;

            _level_TMP.text = parseInt.ToString();
        }

        public void WeaponFireCoolTime(float coolTime)
        {
            if(_coCoolTime != null)
            {
                StopCoroutine(_coCoolTime);
                _coCoolTime = null;
            }

            _fill.fillAmount = 0;
            _coCoolTime = StartCoroutine(Co_CoolTime());

            IEnumerator Co_CoolTime()
            {
                float curTime = 0;
                while(curTime <= coolTime)
                {
                    curTime += Time.deltaTime;
                    _fill.fillAmount = curTime / coolTime;
                    yield return null;
                }

                _fill.fillAmount = 0;
                _coCoolTime = null;
            }
        }


#if UNITY_EDITOR
        protected override void OnBindSerialzedField()
        {
            _fill       = transform.Find("IMG_Fill").GetComponent<Image>();
            _thumnail   = transform.Find("IMG_Thumnail").GetComponent<Image>();
            _level_TMP  = transform.Find("TMP_Level").GetComponent<TextMeshProUGUI>();

            _level_TMP.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Maplestory OTF Bold SDF");
        }
#endif
    }
}