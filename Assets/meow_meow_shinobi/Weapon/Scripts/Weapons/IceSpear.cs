using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow_Moew_Shinobi.Weapon
{
    public class IceSpear : WeaponBase
    {
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
            
        }

        protected override void OnInit()
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