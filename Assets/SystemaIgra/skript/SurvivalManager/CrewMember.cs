 
using UnityEngine;
[System.Serializable]

public class CrewMember
{
    public string name;
    public float health = 100f;
    public bool IsAlive => health > 0;

    public void TakeDamage(float amount)
    {
        if (!IsAlive) return;
        health -= amount;
        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"☠️ {name} погиб.");
    }
}
