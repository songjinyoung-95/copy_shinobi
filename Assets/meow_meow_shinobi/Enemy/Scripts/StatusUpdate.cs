using System;
using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Skill;
using Unity.VisualScripting;
using UnityEngine;

namespace Meow_Moew_Shinobi.Enemy
{
    public class StatusUpdate : MonoBehaviour
    {
        private List<StatusEffectInstance> _statusEffects;
        private Dictionary<EStatusEffect, StatusEffectInstance> _dict;
        private Action _donecallback;

        public event Action OnFreeze;
        public event Action OnStun;
        public event Action<float> OnKnockback;

        public void Init(Action donecallback)
        {
            _statusEffects = new List<StatusEffectInstance>();
            _dict = new Dictionary<EStatusEffect, StatusEffectInstance>()
            {
                {EStatusEffect.Freeze,      new FreezeStatus(OnFreeze)},
                {EStatusEffect.Stun,        new StunStatus(OnStun)},
                {EStatusEffect.Knockback,   new KnockbackStatus(OnKnockback)}
            };
            
            _donecallback = donecallback;
        }

        public void AddStatus(StatusEffectData data)
        {
            if(!_dict.ContainsKey(data.EffectType))
            {
                Debug.LogError($"{data.EffectType} 추가되지 않은 스테이터스 입니다");
                return;
            }

            if(!_dict.TryGetValue(data.EffectType, out var value))
            {
                Debug.LogError($"{value} 클래스를 생성하지 않았습니다");
                return;
            }

            value.Initialize(data);
            value.Execute();

            if(_statusEffects.Contains(value))
                return;
            
            _statusEffects.Add(value);
        }

        public void ClearStatus()
        {
            _statusEffects.Clear();

            foreach (var item in _dict)
                item.Value.StatusData = null;
        }

        private void Update()
        {
            if(_statusEffects.Count <= 0)
                return;

            for (int i = 0, size = _statusEffects.Count - 1; i >= size; i--)
            {
                if (_statusEffects[i].IsRecoverStatus())
                    _statusEffects.Remove(_statusEffects[i]);
            }

            if(_statusEffects.Count <= 0)
                _donecallback?.Invoke();
        }

        private abstract class StatusEffectInstance
        {
            public StatusEffectData StatusData;
            public float Duration;
            public float Intensity;
            public float CurrentTime;

            public void Initialize(StatusEffectData data)
            {
                StatusData  = data;
                Duration    = StatusData.Duration;
                Intensity   = StatusData.Intensity;
                CurrentTime = 0;
            }

            protected abstract void OnExecute();
            public void Execute() => OnExecute();

            public bool IsRecoverStatus()
            {
                CurrentTime += Time.deltaTime;

                if(CurrentTime >= Duration)
                {
                    StatusData  = null;
                    CurrentTime = 0;

                    return true;
                }

                return false;
            }
        }


        private class FreezeStatus : StatusEffectInstance
        {
            private event Action _onFreeze;
            public FreezeStatus(Action freeze)
            {
                _onFreeze = freeze;
            }

            protected override void OnExecute()
            {
                _onFreeze?.Invoke();
            }
        }

        private class KnockbackStatus : StatusEffectInstance
        {
            private event Action<float> _onKnockback;
            public KnockbackStatus(Action<float> knockback)
            {
                _onKnockback = knockback;
            }

            protected override void OnExecute()
            {
                _onKnockback?.Invoke(Intensity);
            }
        }

        private class StunStatus : StatusEffectInstance
        {
            private Action _onStun;
            public StunStatus(Action stun)
            {
                _onStun = stun;
            }

            protected override void OnExecute()
            {
                _onStun?.Invoke();
            }
        }
    }
}