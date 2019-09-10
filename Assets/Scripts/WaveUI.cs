using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveUI : MonoBehaviour
{
    [SerializeField]
    WaveSpawner spawner;

    [SerializeField]
    Animator waveAnimator;

    [SerializeField]
    Text waveCountdownText;

    [SerializeField]
    Text waveCountText;

    private WaveSpawner.SpawnState previousState;

    // Start is called before the first frame update
    void Start()
    {
        if(spawner == null)
        {
            Debug.LogError("No spawner referenced!");
            this.enabled = false;
        }

        if (waveAnimator == null)
        {
            Debug.LogError("No waveAnimator referenced!");
            this.enabled = false;
        }

        if (waveCountdownText == null)
        {
            Debug.LogError("No waveCountdownText referenced!");
            this.enabled = false;
        }

        if (waveCountText == null)
        {
            Debug.LogError("No waveCountText referenced!");
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (spawner.state)
        {
            case WaveSpawner.SpawnState.COUNTING:
                UpdateCountdownUI();
                break;
            case WaveSpawner.SpawnState.SPAWNING:
                UpdateIncomingUI();
                break;
        }

        previousState = spawner.State;
    }

    void UpdateCountdownUI()
    {
        if(previousState != WaveSpawner.SpawnState.COUNTING)
        {
            waveAnimator.SetBool("WaveIncoming", false);
            waveAnimator.SetBool("WaveCountdown", true);
            Debug.Log("Counting");
        }
        waveCountdownText.text = ((int) spawner.WaveCountdown).ToString();
    }

    void UpdateIncomingUI()
    {
        if (previousState != WaveSpawner.SpawnState.SPAWNING)
        {
            
            waveAnimator.SetBool("WaveCountdown", false);
            waveAnimator.SetBool("WaveIncoming", true);
            waveCountText.text = ((int) spawner.NextWave).ToString();
            Debug.Log("Spawning");
        }
    }
}
