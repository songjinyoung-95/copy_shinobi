using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Stage;
using UnityEngine;

namespace Meow_Moew_Shinobi.Weapon
{
    public class Log : WeaponBase
    {
        protected override void OnInit()
        {
            // WeaponData.ApplyHitCount = int.MaxValue;
        }

        protected override void OnUpdate()
        {
            float speed = WeaponData.MoveSpeed * Time.deltaTime;
            transform.position += transform.up * speed;
        }

        protected override void OnFire(Vector3 targetPos)
        {
            transform.position = targetPos;
        }

        protected override void OnHit(IHitReceiver hitReceiver)
        {
            // hitReceiver.ApplyKnockback(WeaponData.KnockbackForce);
        }

        protected override void OnFireTarget(List<Transform> enemiesTF, int index)
        {
            Vector2 minArea = StageHelper.StageArea.minArea;
            Vector2 maxArea = StageHelper.StageArea.maxArea;

            Vector2 randomPos = new Vector2(Random.Range(minArea.x, maxArea.x), transform.position.y);

            Fire(randomPos);
        }
    }
}