using System.Collections;
using System.Collections.Generic;
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
    
    public Utilities.GameState gameState = Utilities.GameState.Play;
    
    private AudioBehavior _audioBehavior;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(obj: this);
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
        gameState = Utilities.GameState.Play;
        
        _audioBehavior = GameObject.Find("AudioManager").GetComponent<AudioBehavior>();
        _audioBehavior.gameState = gameState;
        
        _player = GameObject.Find("Player");

        _attackPatternActive = false;

        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SwitchPausePlay();
        }

        if (gameState == Utilities.GameState.Play)
        {
            if (!_attackPatternActive) StartCoroutine(AttackPattern1());
        }

        if (gameState == Utilities.GameState.GameOver || gameState == Utilities.GameState.Pause)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }
        }
    }

    public void SwitchPausePlay()
    {
        if (gameState == Utilities.GameState.Play)
        {
            gameState = Utilities.GameState.Pause;
            Time.timeScale = 0f;
            _pauseScreen.SetActive(true);
            Cursor.visible = true;
        }
        else if (gameState == Utilities.GameState.Pause)
        {
            gameState = Utilities.GameState.Play;
            Time.timeScale = 1.0f;
            _pauseScreen.SetActive(false);
            Cursor.visible = false;
        }
    }

    public void ReturnToTitle()
    {
        StopAllCoroutines();
        gameState = Utilities.GameState.Title;
        SceneManager.LoadScene("Title");
    }

    public void GameOver()
    {
        StopAllCoroutines();
        gameState = Utilities.GameState.GameOver;
        _gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
    }

    public void Restart()
    {
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
    
    private GameObject CreateBullet(Vector3 pos, Quaternion rot, float speed)
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, pos, rot);
        
        bulletInstance.GetComponent<BulletBehavior>().speed = speed;
        
        return bulletInstance;
    }
}
