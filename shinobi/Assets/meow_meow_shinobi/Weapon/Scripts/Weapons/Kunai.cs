using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow_Moew_Shinobi.Weapon
{
    public class Kunai : WeaponBase
    {
        protected override void OnInit()
        {
            
        }

        protected override void OnUpdate()
        {
            float speed = WeaponData.MoveSpeed * Time.deltaTime;
            transform.position += transform.up * speed;
        }

        protected override void OnFire(Vector3 targetPos)
        {
            Vector2 direction = targetPos - transform.position;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }

        protected override void OnHit(IHitReceiver hitReceiver)
        {
            // if (WeaponData.KnockbackForce > 0)
            //     hitReceiver.ApplyKnockback(WeaponData.KnockbackForce);
        }

        protected override void OnFireTarget(List<Transform> enemiesTF, int index)
        {
            Transform target = enemiesTF[0];
            float angle = 1f;

            if (index == 0)
            {
                Fire(target.position);
                return;
            }

            int randomAxis = Random.Range(0, 2) == 0 ? 1 : -1;

            Vector3 pos = target.position;
            pos.x += index % 2 == 0 ? angle * index * randomAxis: -angle * index * randomAxis;
            Fire(pos);
        }
    }
}