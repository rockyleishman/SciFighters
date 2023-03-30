using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyNoAttack : AIController
{
    private float attackTimer;

    protected override void Start()
    {
        base.Start();

        UnitFaction = Faction.enemy;

        attackTimer = 0.0f;
    }

    protected override void AttackEnemy()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer > 1.0f)
        {
            attackTimer = 0.0f;
            FireLaser(1, 15.0f, Color.red);
        }


        
    }
}
