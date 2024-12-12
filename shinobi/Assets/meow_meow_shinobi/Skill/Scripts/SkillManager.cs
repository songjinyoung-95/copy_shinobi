using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Character;
using Meow_Moew_Shinobi.Skill;
using Meow_Moew_Shinobi.Weapon;
using UnityEngine;

namespace Meow_Moew_Shinobi.Singleton
{
    public class SkillManager
    {
        private const string VIEW_PATH = "Skill/UI/SelectSkillView";
        private const string DATA_PATH = "Skill/Data/WeaponSkillData";

        private const int SELECT_SLOT_COUNT = 3;

        private static SkillManager _instance;
        public static SkillManager  Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new SkillManager();

                return _instance;
            }
        }


        private WeaponSkillDataContainer _weaponSkillData;
        private ISkillView _view;

        /// <summary>
        /// 생성자
        /// </summary>
        public SkillManager()
        {
            WeaponSkillDataContainer weaponSkillDataContainer = Resources.Load<WeaponSkillDataContainer>(DATA_PATH);

            if (weaponSkillDataContainer == null)
            {
                Debug.LogError($"{DATA_PATH} 경로에 스크립터블 데이터가 존재하지 않습니다 ");
                return;
            }
            
            _weaponSkillData = weaponSkillDataContainer;
            _weaponSkillData.Init();


            SelectSkillView view = Resources.Load<SelectSkillView>(VIEW_PATH);
            
            if (view == null)
            {
                Debug.LogError($"{VIEW_PATH} 경로에 프리펩이 존재하지 않습니다 ");
                return;
            }

            _view = Object.Instantiate(view);
            _view.OnSelectedSkill += SelectedSkill;
            _view.Init(ShowSkillView);
        }

        private void SelectedSkill(SkillData data)
        {
            EWeaponType weaponType = _weaponSkillData.FindWeaponTypeBySkill(data);

            WeaponInfoRecorder.RecordWeaponLevel(weaponType);
            WeaponManager.Instance.SelectWeapon(weaponType, data.Ability, data);
        }

        private SkillData RandomSkill()
        {
            List<EWeaponType> equipSkills = CharacterManager.Instance.EquipWeapons;

            EWeaponType weaponType  = equipSkills[Random.Range(0, equipSkills.Count)];
            SkillData   equipWeapon = _weaponSkillData.GetWeaponSkill(weaponType, ESkillType.Equip);

            if (!WeaponManager.Instance.IsWeaponEquipped(weaponType))
            {
                return equipWeapon;
            }

            List<SkillData> weaponDatas = _weaponSkillData.GetWeaponSkills(weaponType);
            // weaponDatas.Remove(equipWeapon);

            SkillData randomSkill = weaponDatas[Random.Range(1, weaponDatas.Count)];
            randomSkill.SetLevel(WeaponInfoRecorder.GetWeaponLevel(weaponType));
            return randomSkill;
        }

        public void CharacterEquipWeapon(EWeaponType weaponType)
        {
            SkillData skill = _weaponSkillData.GetWeaponSkill(weaponType, ESkillType.Equip);
            SelectedSkill(skill);
        }

        /// <summary>
        /// 스킬 선택 뷰 활성화
        /// </summary>
        public void ShowSkillView()
        {
            if(StageManager.Instance.StageState == EStageState.End)
                return;

            SkillData[] skillDatas = new SkillData[SELECT_SLOT_COUNT];
            HashSet<SkillData> selectedSkills = new HashSet<SkillData>();

            for (int i = 0; i < skillDatas.Length; i++)
            {
                SkillData randomSkill;

                do randomSkill = RandomSkill();
                while (selectedSkills.Contains(randomSkill));

                skillDatas[i] = randomSkill;
                selectedSkills.Add(randomSkill);
            }

            _view.ShowSkillView(skillDatas);
        }

        /// <summary>
        /// 스킬 선택 뷰 비활성화
        /// </summary>
        public void HideSkillView()
        {
            _view.HideSkillView();
        }

        public void Release()
        {
            _weaponSkillData.Init();
        }
    }
}