using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyNoAttack : AIController
{
    protected override void AttackEnemy()
    {
        Debug.Log("Enemy is attacking!");
    }
}
