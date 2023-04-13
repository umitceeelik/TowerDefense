using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour, IDamageable
{
    public int Health = 20;

    public Transform GetTransform()
    {
        return transform;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }

}
