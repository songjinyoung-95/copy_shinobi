using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Meow_Moew_Shinobi.Enemy
{
    public class Splitting_Enemy : EnemyBase
    {
        private readonly static Vector3 SPLIT_SCALE = Vector3.one * 0.75f;
        public bool IsSplit { get; private set; }

        protected override void OnInit()
        {

        }

        protected override void OnSpawn()
        {
            IsSplit = false;
            transform.localScale = Vector3.one;
        }

        protected override void OnDie()
        {
            
        }

        public void SetSplit()
        {
            transform.localScale = SPLIT_SCALE;
            HalfHelath();
            IsSplit = true;
        }
    }
}