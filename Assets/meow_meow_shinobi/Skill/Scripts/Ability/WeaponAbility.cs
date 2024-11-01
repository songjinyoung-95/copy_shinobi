using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Skill;
using UnityEngine;

namespace Meow_Moew_Shinobi.Weapon
{
    public interface IWeaponAbility
    {
        void AddAbility(WeaponData weaponData, SkillData skillData);
        void AddSkillEffect();
    }

    public class WeaponAbility : IWeaponAbility
    {
        public void AddAbility(WeaponData weaponData, SkillData skillData)
        => weaponData.IncreaseAbility(skillData.Abilities, skillData.Increase);

        public void AddSkillEffect()
        {
            
        }
    }


}