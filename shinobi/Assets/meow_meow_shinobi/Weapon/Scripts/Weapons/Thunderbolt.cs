using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow_Moew_Shinobi.Weapon
{
    public class Thunderbolt : WeaponBase
    {
        [Space(20)]
        [Header("자식 클래스")]
        [Space(5)]
        [SerializeField] private ParticleSystem _boltParticle;

        private Coroutine _coIsAlive;

        private IEnumerator Co_IsAlive(Action donecallback)
        {
            while (_boltParticle.isPlaying)
                yield return null;

            donecallback?.Invoke();
            _coIsAlive = null;
        }

        protected override void OnFire(Vector3 targetPos)
        {
            transform.position = targetPos;

            _boltParticle.Play();
            _coIsAlive = StartCoroutine(Co_IsAlive(() => HitCollider.enabled = true));
        }

        protected override void OnDisableWeapon()
        {
            _coIsAlive = StartCoroutine(Co_IsAlive(()=>
            {
                if (_coIsAlive != null)
                {
                    StopCoroutine(_coIsAlive);
                    _coIsAlive = null;
                }

                base.OnDisableWeapon();
            }));
        }

        protected override void OnHit(IHitReceiver hitReceiver)
        {

        }

        protected override void OnInit()
        {
            HitCollider.enabled = false;
        }

        protected override void OnUpdate()
        {
            
        }

        protected override void OnFireTarget(List<Transform> enemiesTF, int index)
        {
            Transform targetTF = enemiesTF[0];

            while (enemiesTF.Count <= index)
                index--;

            targetTF = enemiesTF[index];

            Fire(targetTF.position);
        }
    }
}