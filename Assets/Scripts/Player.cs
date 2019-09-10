using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [System.Serializable]
    public class PlayerStats
    {
        public int maxHealth = 100;

        private int _curHealth;
        public int curHealth
        {
            get { return _curHealth; }
            set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        public void Init()
        {
            curHealth = maxHealth;
        }
    }

    public PlayerStats playerStats = new PlayerStats();

    [SerializeField]
    private StatusIndicator statusIndicator;

    public int fallBoundary = -20;

    private void Start()
    {
        playerStats.Init();

        if(statusIndicator == null)
        {
            Debug.LogError("No status indicator referenced on player!");
        }
        else
        {
            statusIndicator.SetHealth(playerStats.curHealth, playerStats.maxHealth);
        }
    }

    private void Update()
    {
        if(transform.position.y <= fallBoundary)
        {
            DamagePlayer(999999);
        }
    }

    public void DamagePlayer(int damage)
    {
        playerStats.curHealth -= damage;
        if(playerStats.curHealth <= 0)
        {
            GameMaster.KillPlayer(this);
        }

        if (statusIndicator == null)
        {
            Debug.LogError("No status indicator referenced on player!");
        }
        else
        {
            statusIndicator.SetHealth(playerStats.curHealth, playerStats.maxHealth);
        }
    }

}
