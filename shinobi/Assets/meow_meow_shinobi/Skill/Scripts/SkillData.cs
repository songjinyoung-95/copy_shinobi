using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Weapon;
using UnityEngine;

namespace Meow_Moew_Shinobi.Skill
{
    [CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable/SkillData", order = 1)]
    public class SkillData : ScriptableObject
    {
        public int Level { get; private set; }
        public IWeaponAbility Ability { get; private set; }

        public ESkillType   SkillType   => _skillType;
        public EAbility[]   Abilities   => _abilitys;
        public float[]      Increase    => _increase;

        public StatusEffectData StatusEffectData => _instanceStatusEffect;


        public string       Name        => _skillName;
        public string       Info        => _skillInfo;
        public Sprite       Thumnail    => _skillThumnail;

        

        [SerializeField] private ESkillType     _skillType;
        [SerializeField] private EAbility[]     _abilitys;
        [SerializeField] private float[]        _increase;

        [SerializeField] private StatusEffectData  _statusEffect;
        
        private StatusEffectData _instanceStatusEffect;


        [Space(50)]
        [SerializeField] private string         _skillName;
        [TextArea]
        [SerializeField] private string         _skillInfo;
        [SerializeField] private Sprite         _skillThumnail;

        public void Init()
        {
            Level   = 0;
            Ability = new WeaponAbility();

            if(_statusEffect != null)
            {
                StatusEffectData statusEffectData = Instantiate(_statusEffect);
                _instanceStatusEffect = statusEffectData;
            }
        }

        public void SetLevel(int level)
        {
            Level = level;
        }
    }
}