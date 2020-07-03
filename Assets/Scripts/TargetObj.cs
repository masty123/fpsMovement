using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObj : MonoBehaviour
{
    public float health = 50f;
    public void takeDmg(float amount)
    {
        health -= amount;
        if(health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
