using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;
    private float _speedMultiplier = 2;
    float _thrustForce = 2;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private float _fireRate = 0.15f;
    private float _canFire = -1f;
    [SerializeField] private int _lives = 3;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldsActive = false;
    [SerializeField] private float _canThrust = 0.15f;
    private float _thrustDelay = 1f;
    private bool _isThrusting = false;
    [SerializeField] private GameObject _shieldsVisualizer;
    [SerializeField] private GameObject _rightEngine, _leftEngine;
    [SerializeField] private int _score;
    private UI_Manager _uiManager;
    [SerializeField] private AudioClip _laserSoundClip;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Slider _thrustGauge;
    private float _totalFuel = 100f;
    void Start()
    {
        transform.position = new Vector3(0.5f, -3.6f, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UI_Manager>();
        _audioSource = GetComponent<AudioSource>();
        // _thrustGauge = GameObject.Find("ThrusterSlider").GetComponent<Slider>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL!");
        }
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL!");
        }
        if (_audioSource == null)
        {
            Debug.LogError("Player's Audio Source is NULL!");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
    }
    void Update()
    {
        Movement();
        // _thrustGauge.value = _totalFuel;
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        if (_isSpeedBoostActive == true)
        {
            transform.Translate(direction * (_speed * _speedMultiplier) * Time.deltaTime);
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > _canThrust)
        {
            isThrusting();
            _canThrust = Time.time + _thrustDelay;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            stopThrusting();
        }
        else
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }
        //Thrust();
        //RegenFuel();
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.5f, 0));
        if (transform.position.x >= 9.48f)
        {
            transform.position = new Vector3(9.48f, transform.position.y, 0);
        }
        else if (transform.position.x <= -9.48f)
        {
            transform.position = new Vector3(-9.48f, transform.position.y, 0);
        }
    }

    //void RegenFuel()
    //{
    //    if (_isThrusting == false)
    //    {
    //        UpdateThrustGauge(1 * Time.deltaTime * 50);
    //    }
    //}

    //void Thrust()
    //{
    //    if (_isThrusting == false)
    //    {
    //        UpdateThrustGauge(-2 * Time.deltaTime * 50);
    //    }
        
    //}

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }
        _audioSource.Play();

    }
    
    public void Damage()
    {
        if ( _isShieldsActive == true)
        {
            _isShieldsActive = false;
            _shieldsVisualizer.SetActive(false);
            return;
        }
        _lives -= 1;
        if (_lives == 2)
        {
            _rightEngine.SetActive(true);
        }
        else if ( _lives == 1)
        {
            _leftEngine.SetActive(true);
        }
        _uiManager.UpdateLives(_lives);
        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        // _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        // _speed /= _speedMultiplier;
        _isSpeedBoostActive = false;
    }

    public void ShieldsActive()
    {
        _isShieldsActive = true;
        _shieldsVisualizer.SetActive(true);
    }

    public void AddScore(int score)
    {
        _score += score;
        _uiManager.UpdateScore(_score);
    }

    private void isThrusting()
    {
        _isThrusting = true;
        _speed = 8;
    }

    private void stopThrusting()
    {
        _isThrusting = false;
        _speed = 4;
    }

    //public void UpdateThrustGauge(float fuel)
    //{
    //    if (_totalFuel - fuel < 1)
    //    {
    //        stopThrusting();
    //        _speed = 4;
    //    }
    //    _totalFuel += fuel;
    //}
}
