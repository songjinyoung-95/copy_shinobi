using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow_Moew_Shinobi.Weapon
{
    public static class AbilityHelper
    {
        private static Dictionary<ESkillType, IWeaponAbility> _dict;

        public static void Init()
        {
            _dict = new Dictionary<ESkillType, IWeaponAbility>()
            {

            };
        }

        public static IWeaponAbility GetSkillTypeByAbility(ESkillType skillType)
        {
            if(!_dict.ContainsKey(skillType))
            {
                // Debug.LogError($"{skillType} 타입이 등록되어있지 않습니다");
                return null;
            }
                
            if(!_dict.TryGetValue(skillType, out var ability))
            {
                Debug.LogError($"{skillType}에 {nameof(ability)} 클래스가 등록되어있지 않습니다");
                return null;
            }

            return ability;
        }
    }
}