using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameBehavior : MonoBehaviour
{
    //Setup
    public static GameBehavior Instance;
    
    //References
    private GameObject _pauseScreen;
    private GameObject _gameOverScreen;
    
    private AudioSource _audioSource;
    [SerializeField] private AudioMixerGroup _SFXMixer;
    [SerializeField] private AudioClip _spawnEnemy;
    [SerializeField] private AudioClip _pauseSound;
    [SerializeField] private AudioClip _unpauseSound;
    
    [SerializeField] private GameObject _player;

    [SerializeField] private GameObject bulletPrefab;
    
    //Variable State Assignments
    public float timeBetweenWaves = 3.0f;
    public float bulletSpeed = 3.0f;
    public int bulletsAddition = 0;
    public float distanceBetweenSubtractor = 0;

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
        
        _audioSource = GetComponent<AudioSource>(); 
        _audioSource.outputAudioMixerGroup = _SFXMixer;
        
        StartCoroutine(DecreaseDistanceBetweenAttacks());
        StartCoroutine(DecreaseTimeBetweenWaves());
        StartCoroutine(HarderBulletsOverTime());

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
            _audioSource.PlayOneShot(_pauseSound);
            Time.timeScale = 0f;
            _pauseScreen.SetActive(true);
            Cursor.visible = true;
        }
        else if (gameState.gameState == Utilities.GameState.Pause)
        {
            gameState.Play();
            _audioSource.PlayOneShot(_unpauseSound);
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
        _audioBehavior.PressStart();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // Enemies
    private IEnumerator AttackPattern1()
    {
        _attackPatternActive = true;
        yield return new WaitForSeconds(timeBetweenWaves);

        // add GUI warning here
        

        StartCoroutine(StraightAttack(
            3 + bulletsAddition,
            0.25f,
            4,
            -6f,
            5f,
            3 - distanceBetweenSubtractor,
            3 - distanceBetweenSubtractor,
            225f + Random.Range(-25, 25),
            bulletSpeed));

        yield return new WaitForSeconds((1.0f + Random.Range(-0.5f, 0.5f)));
        
        StartCoroutine(StraightAttack(
            3 + bulletsAddition,
            0.25f,
            4,
            6f,
            5f,
            -3,
            3,
            135f + Random.Range(-25, 25),
            bulletSpeed - 1.0f));

        yield return new WaitForSeconds((1.0f + Random.Range(-0.5f, 0.5f)));
        
        StartCoroutine(StraightAttack(
            3 + bulletsAddition,
            0.25f,
            10,
            -5f,
            10f,
            3,
            0,
            180f + Random.Range(-25, 25),
            bulletSpeed));

        yield return new WaitForSeconds((1.5f + Random.Range(-0.5f, 0.5f)));

        StartCoroutine(StraightAttack(
            Random.Range(1,10) + bulletsAddition,
            Random.Range(0.5f,2f),
            Random.Range(5,10),
            (Random.Range(0, 1) == 0 ? -1 : 1) * Random.Range(0,3),
            (Random.Range(0, 1) == 0 ? -1 : 1) * Random.Range(4,10),
            3,
            3,
            Random.Range(0,360),
            Random.Range(0.5f, 4.5f)
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
        _audioSource.PlayOneShot(_spawnEnemy);
        
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

    private IEnumerator DecreaseTimeBetweenWaves()
    {
        WaitForSeconds oneSec = new WaitForSeconds(1);
        
        while (timeBetweenWaves > 0)
        {
            timeBetweenWaves -= (1.0f / 30f);
            
            yield return oneSec;
        }
    }

    private IEnumerator DecreaseDistanceBetweenAttacks()
    {
        WaitForSeconds oneSec = new WaitForSeconds(1);
        
        while (distanceBetweenSubtractor < 2f)
        {
            distanceBetweenSubtractor += (1.0f / 60f);
            yield return oneSec;
        }
    }

    private IEnumerator HarderBulletsOverTime()
    {
        WaitForSeconds oneSec = new WaitForSeconds(1);
        
        while (true)
        {
            float numBulletsFloat = 0;
            
            bulletSpeed += (1.0f / 45f);
            
            numBulletsFloat += (1.0f / 20f);
            bulletsAddition = Mathf.FloorToInt(numBulletsFloat);
            
            yield return oneSec;
        }
    }
    
    private GameObject CreateBullet(Vector3 pos, Quaternion rot, float speed)
    {
        if (Vector3.Distance(pos, _player.transform.position) <= 3.0f)
        {
            return null;
        }
        else
        {
            GameObject bulletInstance = Instantiate(bulletPrefab, pos, rot);
                    
                    bulletInstance.GetComponent<BulletBehavior>().speed = speed;
                    
                    return bulletInstance;
        }
    }
}
