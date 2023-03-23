using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyNoAttack : AIController
{
    protected override void Start()
    {
        base.Start();

        UnitFaction = Faction.enemy;
    }

    protected override void AttackEnemy()
    {
        Debug.Log("Enemy is attacking!");
    }
}
