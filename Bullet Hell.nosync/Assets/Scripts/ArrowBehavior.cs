using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class ArrowBehavior : MonoBehaviour
{
    [SerializeField] private float _velocity = 150f;

    public float Speed
    {
        get => _velocity;
    }

    private ScoreBehavior _scoreBehavior;

    [SerializeField] private SpriteRenderer _playerColor;

    private float topYPos = 0;

    private float _orthographicSize;

    private RectMask2D _powerupMeter;
    private int _powerupMeterMin = -120;
    private int _powerupMeterMax = -330;

    public float _powerMeterChargeDelta = 0.05f;

    private float _powerChargeAmount;
    private float PowerChargeAmount
    {
        get => _powerChargeAmount;
        set
        {
            _powerChargeAmount = value;
            var pad = _powerupMeter.padding;
            pad.z = (-1 * _powerChargeAmount * Mathf.Abs(_powerupMeterMax - _powerupMeterMin) + _powerupMeterMin);
            _powerupMeter.padding = pad;
        }
    }
    
    private Vector3 _movement;
    
    [SerializeField] Rigidbody2D rb;
    
    private bool inBoostPowerup = false;    
    [SerializeField] private float boostMultiplier;
    [SerializeField] private float boostDuration;
    private Color _currentPowerupColor;
    [SerializeField] private float _powerupWarningTime;
    [SerializeField] private float _numberOfWarningBlinks;
    [SerializeField] private Color boostColor = new Color(59f/255f, 125f/255f,171f/255f);
    
    
    // Start is called before the first frame update
    void Start()
    { 
        _scoreBehavior = GetComponent<ScoreBehavior>();
        _orthographicSize = GameObject.FindObjectOfType<Camera>().orthographicSize;
        _powerupMeter = GameObject.Find("Powerup Bar Mask").GetComponent<RectMask2D>();
    }

    void Update()
    {
        _movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0).normalized;

        if (PowerChargeAmount >= 1 & Input.GetAxis("Use Powerup") != 0)
        {
            PowerChargeAmount = 0;
            StartCoroutine(BoostPowerup());
        }
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = _movement * (_velocity * Time.fixedDeltaTime);
        if (_movement == Vector3.zero)
        {
            rb.velocity = Vector3.zero;
        }
        
        //Score Updating and YLimit
        
        if (transform.position.y > topYPos)
        {
            topYPos = transform.position.y;
            _scoreBehavior.Score = topYPos;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Bullet") & !inBoostPowerup)
        {
            GameBehavior.Instance.GameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet") & !inBoostPowerup)
        {
            PowerChargeAmount += _powerMeterChargeDelta;
        }
    }

    private IEnumerator BoostPowerup()
    {
        inBoostPowerup = true;
        _currentPowerupColor = boostColor;
        float regularVelocity = _velocity;
        _velocity *= boostMultiplier;
        
        _playerColor.material.color = _currentPowerupColor;
        
        yield return new WaitForSeconds(boostDuration-_powerupWarningTime);

        StartCoroutine(PowerupBlink());
        
        yield return new WaitForSeconds(_powerupWarningTime);

        _playerColor.material.color = Color.white;

        _velocity = regularVelocity;

        inBoostPowerup = false;
    }

    private IEnumerator PowerupBlink()
    {
        for(int i = 0 ; i < _numberOfWarningBlinks ; i++)
        {
            _playerColor.material.color = Color.white;
            yield return new WaitForSeconds(_powerupWarningTime / _numberOfWarningBlinks / 2f);
            _playerColor.material.color = _currentPowerupColor;
            yield return new WaitForSeconds(_powerupWarningTime / _numberOfWarningBlinks / 2f);
        }
    }
}
