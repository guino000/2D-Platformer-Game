using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;

    public static int _remaining_lives = 3;
    public static int RemainingLives
    {
        get { return _remaining_lives; }
    }

    private void Awake()
    {
        if(gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }
    }

    private void Start()
    {
        if(cameraShake == null)
        {
            Debug.LogError("No camera shake referenced on GameMaster!");
        }
    }

    public Transform playerPrefab;
    public Transform spawnPoint;
    public float spawnDelay;
    public GameObject spawnPrefab;

    public CameraShake cameraShake;

    [SerializeField]
    private GameObject gameOverUI;

    public void EndGame()
    {
        Debug.Log("GAME OVER");
        gameOverUI.SetActive(true);
    }

    public IEnumerator RespawnPlayer()
    {
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(spawnDelay);
        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        GameObject clone = (GameObject)Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation);
        Destroy(clone, 3f);
    }

    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        _remaining_lives--;
        if(_remaining_lives <= 0)
        {
            gm.EndGame();
        }
        else
        {
            gm.StartCoroutine(gm.RespawnPlayer());
        }        
    }

    public static void KillEnemy(Enemy enemy)
    {
        gm._KillEnemy(enemy);
    }

    public void _KillEnemy(Enemy _enemy)
    {
        GameObject clone = (GameObject) Instantiate(_enemy.deathParticles, _enemy.transform.position, Quaternion.identity).gameObject;
        Destroy(clone, 5f);
        cameraShake.Shake(_enemy.shakeAmt, _enemy.shakeLenght);
        Destroy(_enemy.gameObject);
    }
}
