using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Enemy;
using UnityEngine;

namespace Meow_Moew_Shinobi.FSM
{
    public class EnemyIdleState : StateBase<EnemyBase>
    {
        private readonly int KEY_IDLE = Animator.StringToHash("idle");

        Rigidbody2D _rigidbody;
        public EnemyIdleState(EnemyBase owner) : base(owner) 
        {
            _rigidbody = owner.Rigidbody;
        }

        public override void Enter()
        {

        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }

        public override void FixedUpdate()
        {
            _rigidbody.velocity = Vector2.zero;
        }

        public override void Update()
        {

        }
    }
}