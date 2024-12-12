using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Enemy;
using UnityEngine;

namespace Meow_Moew_Shinobi.FSM
{
    public class EnemyDieState : StateBase<EnemyBase>
    {
        public EnemyDieState(EnemyBase owner) : base(owner) { }

        public override void Enter()
        {
            // Owner.DespawnEvent?.Invoke(Owner);
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }

        public override void FixedUpdate()
        {
            
        }

        public override void Update()
        {
            
        }
    }
}