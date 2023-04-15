using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedAttackRadius : AttackRadius
{
    public NavMeshAgent Agent;
    public Bullet BulletPrefab;
    public Vector3 BulletSpawnOffset = new Vector3(0, 1, 0);
    public LayerMask Mask;
    private ObjectPool BulletPool;
    [SerializeField] private float SpherecastRadius = 0.1f;
    private RaycastHit Hit;
    private IDamageable targetDamageable;
    private Bullet bullet;

    public bool canAttack;

    public void CreateBulletPool()
    {
        canAttack = false;
        BulletPool = ObjectPool.CreateInstance(BulletPrefab, Mathf.CeilToInt((1 / AttackDelay) * BulletPrefab.AutoDestroyTime));
    }

    protected override IEnumerator Attack()
    {
        WaitForSeconds wait = new WaitForSeconds(AttackDelay);

        yield return wait;

        while (Damageables.Count > 0)
        {
            for (int i = 0; i < Damageables.Count; i++)
            {
                if (HasLineOfSightTo(Damageables[i].GetTransform()))
                {
                    targetDamageable = Damageables[i];
                    OnAttack?.Invoke(Damageables[i]);
                    Agent.Stop();
                    break;
                }
            }

            if (targetDamageable != null && canAttack)
            {
                PoolableObject poolableObject = BulletPool.GetObject();
                if (poolableObject != null)
                {
                    bullet = poolableObject.GetComponent<Bullet>();

                    bullet.Damage = Damage;
                    bullet.transform.position = transform.position + BulletSpawnOffset;
                    bullet.transform.rotation = Agent.transform.rotation;
                    bullet.Rigidbody.AddForce(Agent.transform.forward * BulletPrefab.MoveSpeed, ForceMode.VelocityChange);
                    canAttack = false;
                    yield return wait;
                }
            }
            else
            {
                Agent.enabled = true; //No target in line of sight, keep trying to get closer
            }

            yield return wait;

            if (targetDamageable == null || !HasLineOfSightTo(targetDamageable.GetTransform()))
            {
                Agent.enabled = true;
            }

            Damageables.RemoveAll(DisableDamageables);
        }

        Agent.enabled = true;
        AttackCoroutine = null;
    }

    private bool HasLineOfSightTo(Transform target)
    {
        if (Physics.SphereCast(transform.position + BulletSpawnOffset, SpherecastRadius, ((target.position + BulletSpawnOffset) - (transform.position + BulletSpawnOffset)).normalized, out Hit, Collider.radius, Mask))
        {
            IDamageable damageable;
            if (Hit.collider.TryGetComponent<IDamageable>(out damageable))
            {
                return damageable.GetTransform() == target;
            }
        }

        return false;
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (AttackCoroutine == null)
        {
            Agent.enabled = true;
        }
    }
}
