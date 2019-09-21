using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;

    [SerializeField]
    private int maxLives = 3;
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

    //Caching
    private AudioManager audioManager;

    private void Start()
    {
        _remaining_lives = maxLives;
        if(cameraShake == null)
        {
            Debug.LogError("No camera shake referenced on GameMaster!");
        }

        //Caching
        audioManager = AudioManager.instance;
        if(audioManager == null)
        {
            Debug.LogError("No AudioManager found in the scene!");
        }
    }

    public Transform playerPrefab;
    public Transform spawnPoint;
    public float spawnDelay;
    public GameObject spawnPrefab;
    public string spawnSoundName;

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
        audioManager.PlaySound(spawnSoundName);
        yield return new WaitForSeconds(spawnDelay);
        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        GameObject clone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation);
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
        // Death Sound
        audioManager.PlaySound(_enemy.deathSound);

        // Instantiate Particles
        GameObject clone = (GameObject) Instantiate(_enemy.deathParticles, _enemy.transform.position, Quaternion.identity).gameObject;
        Destroy(clone, 5f);

        // Camera Shake
        cameraShake.Shake(_enemy.shakeAmt, _enemy.shakeLenght);
        Destroy(_enemy.gameObject);
    }
}
