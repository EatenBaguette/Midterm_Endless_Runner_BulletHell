using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBehavior : MonoBehaviour
{
    //Setup
    public static GameBehavior Instance;
    
    //References
    private GameObject _pauseScreen;
    private GameObject _gameOverScreen;
    
    [SerializeField] private GameObject _player;

    [SerializeField] private GameObject bulletPrefab;
    
    //Variable State Assignments
    public float timeBetweenWaves = 3.0f;

    private bool _attackPatternActive;

    private GameState gameState;

    private ScoreBehavior _score;
    
    private AudioBehavior _audioBehavior;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        Time.timeScale = 1;
        _pauseScreen = GameObject.Find("PauseScreen");
        _pauseScreen.SetActive(false);
        _gameOverScreen = GameObject.Find("GameOverScreen");
        _gameOverScreen.SetActive(false);
        
        _score = GameObject.Find("Player").GetComponent<ScoreBehavior>();
        
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        
        gameState.Play();
        
        _audioBehavior = GameObject.Find("AudioManager").GetComponent<AudioBehavior>();
        
        _player = GameObject.Find("Player");

        _attackPatternActive = false;
        
        StartCoroutine(ChangeTimeBetweenWaves());

        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SwitchPausePlay();
        }

        if (gameState.gameState == Utilities.GameState.Play)
        {
            if (!_attackPatternActive) StartCoroutine(AttackPattern1());
        }

        if (gameState.gameState == Utilities.GameState.GameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }
        }
    }

    public void SwitchPausePlay()
    {
        if (gameState.gameState == Utilities.GameState.Play)
        {
            gameState.Pause();
            Time.timeScale = 0f;
            _pauseScreen.SetActive(true);
            Cursor.visible = true;
        }
        else if (gameState.gameState == Utilities.GameState.Pause)
        {
            gameState.Play();
            Time.timeScale = 1.0f;
            _pauseScreen.SetActive(false);
            Cursor.visible = false;
        }
    }

    public void ReturnToTitle()
    {
        StopAllCoroutines();
        gameState.Title();
        Destroy(_audioBehavior.gameObject);
        Destroy(gameState.gameObject);
        SceneManager.LoadScene("Title");
    }

    public void GameOver()
    {
        gameState.GameOver();
        Time.timeScale = 0f;
        _gameOverScreen.SetActive(true);
        Cursor.visible = true;
    }

    public void Restart()
    {
        gameState.GameOver();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // Enemies
    private IEnumerator AttackPattern1()
    {
        _attackPatternActive = true;
        yield return new WaitForSeconds(timeBetweenWaves);

        // add GUI warning here

        StartCoroutine(StraightAttack(
            3,
            0.25f,
            4,
            -6f,
            5f,
            3,
            3,
            225f,
            3f));
        
        yield return new WaitForSeconds(0.5f);
        
        StartCoroutine(StraightAttack(
            3,
            0.25f,
            4,
            6f,
            5f,
            -3,
            3,
            135f,
            2f));
        
        yield return new WaitForSeconds(0.5f);
        
        StartCoroutine(StraightAttack(
            3,
            0.25f,
            10,
            -5f,
            10f,
            3,
            0,
            180f,
            3f));

        yield return new WaitForSeconds(1.0f);

        StartCoroutine(StraightAttack(
            Random.Range(1,10),
            Random.Range(0.5f,2f),
            Random.Range(5,10),
            (Random.Range(0, 1) == 0 ? -1 : 1) * Random.Range(0,3),
            (Random.Range(0, 1) == 0 ? -1 : 1) * Random.Range(4,10),
            3,
            3,
            Random.Range(0,360),
            Random.Range(0.5f,6f)
            ));
        
        yield return new WaitForSeconds(timeBetweenWaves);
        
        _attackPatternActive = false;
    }

    private IEnumerator StraightAttack(
        int numOfAttacks, 
        float timeBetweenAttacks, 
        int numOfBulletsPerAttack, 
        float xPosRelative, 
        float yPosRelative,
        float xChange,
        float yChange,
        float angle, 
        float speed)
    {
        for (int i = 0; i < numOfAttacks; i++)
        {
            for (int j = 0; j < numOfBulletsPerAttack; j++)
            {
                CreateBullet(
                    new Vector3(
                        _player.transform.position.x + xPosRelative + (xChange * j),
                        _player.transform.position.y + yPosRelative + (yChange * j),
                        _player.transform.position.z),
                    
                    Quaternion.Euler(0, 0, angle),
                    
                    speed
                );
            }
            
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    private IEnumerator ChangeTimeBetweenWaves()
    {
        WaitForSeconds oneSec = new WaitForSeconds(1);
        
        while (timeBetweenWaves > 0)
        {
            yield return oneSec;
            timeBetweenWaves -= (1.0f / 30f);
        }
    }
    
    private GameObject CreateBullet(Vector3 pos, Quaternion rot, float speed)
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, pos, rot);
        
        bulletInstance.GetComponent<BulletBehavior>().speed = speed;
        
        return bulletInstance;
    }
}
