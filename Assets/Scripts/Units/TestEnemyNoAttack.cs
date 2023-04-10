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

        if (attackTimer > TriggerDelay)
        {
            attackTimer = 0.0f;

            if (!_equipedWeapon.Fire(this))
            {
                _equipedWeapon.Reload(UnloadedAmmo);
            }
        }
    }
}
