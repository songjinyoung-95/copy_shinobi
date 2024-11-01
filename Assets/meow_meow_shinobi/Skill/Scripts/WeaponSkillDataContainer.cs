using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Weapon;
using UnityEngine;

namespace Meow_Moew_Shinobi.Skill
{
    [CreateAssetMenu(fileName = "WeaponDataContainer", menuName = "Scriptable/WeaponDataContainer", order = 1)]
    public class WeaponSkillDataContainer : ScriptableObject
    {
        [SerializeField] private Skills[] _skills;

        private Dictionary<EWeaponType, List<SkillData>> _weaponSkillsDict;

        public void Init()
        {
            _weaponSkillsDict = new Dictionary<EWeaponType, List<SkillData>>(); 
            
            for (int i = 0; i < _skills.Length; i++)
            {
                _weaponSkillsDict.Add(_skills[i].WeaponType, _skills[i].SkillList);
                
                foreach (var skill in _skills[i].SkillList)
                    skill.Init();
            }
        }

        public List<SkillData> GetWeaponSkills(EWeaponType weaponType)
        {
            if(_weaponSkillsDict.ContainsKey(weaponType))
            {
                return _weaponSkillsDict[weaponType];
            }

            return null;
        }

        public SkillData GetWeaponSkill(EWeaponType weaponType, ESkillType skillType)
        {
            if(!_weaponSkillsDict.ContainsKey(weaponType))
            {
                Debug.LogError($" 데이터 컨테이너에 {weaponType} 의 데이터가 저장되어있지 않습니다");
                return null;
            }

            foreach (var skill in _weaponSkillsDict[weaponType])
            {
                if(skill.SkillType == skillType)
                    return skill;
            }

            Debug.LogWarning($"{_weaponSkillsDict[weaponType]} 해당 무기 스킬 데이터에 {skillType} 이 없습니다");
            return null;
        }

        public EWeaponType FindWeaponTypeBySkill(SkillData data)
        {
            foreach (var item in _weaponSkillsDict)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    if (item.Value[i] == data)
                        return item.Key;
                }
            }

            Debug.LogError($"{data.Name} / {data.SkillType} 에 해당하는 WeaponType이 없습니다");
            return EWeaponType.None;
        }
        

        [System.Serializable]
        private class Skills
        {
            public EWeaponType WeaponType;
            public List<SkillData> SkillList = new List<SkillData>();
        }
    }
}