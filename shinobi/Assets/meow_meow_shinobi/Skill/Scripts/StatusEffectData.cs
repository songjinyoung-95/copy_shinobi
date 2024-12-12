using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow_Moew_Shinobi.Skill
{
    [CreateAssetMenu(fileName = "StatusEffectData", menuName = "Scriptable/StatusEffectData", order = 1)]
    public class StatusEffectData : ScriptableObject
    {
        public EStatusEffect EffectType => _effectType;
        public float Duration => _duration;
        public float Intensity => _intensity;


        [SerializeField] private EStatusEffect   _effectType;
        [SerializeField] private float           _duration;
        [SerializeField] private float           _intensity;

        public void IncreaseValue(float duration, float intensity)
        {
            _duration   += duration;
            _intensity  += intensity;
        }
    }
}