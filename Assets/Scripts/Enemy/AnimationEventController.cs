using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventController : MonoBehaviour
{
    private RangedAttackRadius RangedAttackRadius;

    private void Awake()
    {
        RangedAttackRadius = transform.parent.GetChild(1).GetComponent<RangedAttackRadius>();
        //transform.parent.TryGetComponent<RangedAttackRadius>(out RangedAttackRadius);
    }
    public void ThrowBullet()
    {
        RangedAttackRadius.canAttack = true;
    }

}
