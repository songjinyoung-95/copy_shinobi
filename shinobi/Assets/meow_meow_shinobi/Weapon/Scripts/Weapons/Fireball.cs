using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.UI.EffectText;
using Meow_Moew_Shinobi.Util;
using UnityEngine;

namespace Meow_Moew_Shinobi.Weapon
{
    public class Fireball : WeaponBase
    {
        protected override void OnUpdate()
        {
            float speed = WeaponData.MoveSpeed * Time.deltaTime;
            transform.position += transform.up * speed;            
        }

        protected override void OnInit()
        {
            
        }


        protected override void OnFire(Vector3 targetPos)
        {
            Vector2 direction = targetPos - transform.position;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);            
        }

        protected override void OnHit(IHitReceiver hitReceiver)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, WeaponData.ShockRange, LayerMask);

            foreach (var hit in hits)
            {
                IHitReceiver target = CacheCollider<IHitReceiver>.TryGetComponenet(hit);
                Vector2 targetPos = hit.transform.position;

                if (target == null)
                    continue;

                if (target == hitReceiver)
                    continue;

                int shockDamage = Mathf.RoundToInt(Random.Range(WeaponData.ShockDamage * 0.8f, WeaponData.ShockDamage * 1f));
                hitReceiver.ApplyDamage(shockDamage);

                HitEffect?.Invoke(ETextType.HitText, targetPos).Show(targetPos, shockDamage);
            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, WeaponData.ShockRange);
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