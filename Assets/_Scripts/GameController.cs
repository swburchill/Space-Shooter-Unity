using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour 
{
	public GameObject playerOne;
	public GameObject[] hazards;
	public Vector3 spawnValues;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;
	public float waveDisplayWait;

	public GUIText waveText;
	private int _waveCount;
	private int _hazardCount;
	private float _spawnWait;
	private float _waveWait;

	public GUIText shieldText;
	public GUIText shieldGivenText;
	public GUIText scoreText;
	public GUIText highScoreText;
	private int _score;
	private int _highScore;
	private float _lastScoreUpdate;
	private float _timeSinceScoreUpdate;

	public GUIText mainUpgradeCount;
	public GUIText sideUpgradeCount;
	public GUIText missileUpgradeCount;
	public GUIText chargedCount;
	
	private bool _gameOver;
	//used to control which enemies are available on starting waves
	private int _hazardMax;

	private MenuManager _menuManager;
	private Menu _gameOverMenu;

	void Awake()
	{
		GameObject canvasObject = GameObject.Find("Canvas");
		if (canvasObject != null) 
		{
			_menuManager = canvasObject.GetComponent<MenuManager>();
		}
		if (_menuManager == null) 
		{
			Debug.Log("Cannot find 'Menu Manager' script");
		}

		GameObject gameOverObject = GameObject.Find("GameOverMenu");
		if (canvasObject != null) 
		{
			_gameOverMenu = gameOverObject.GetComponent<Menu>();;
		}
		if (_gameOverMenu == null) 
		{
			Debug.Log("Cannot find 'GameOverMenu' menu");
		}
	}
	                                                                                                                                                                                                    
	// Use this for initialization
	void Start() 
	{
		waveText.text = "";
		shieldText.text = "";
		shieldGivenText.text = "";
		mainUpgradeCount.text = "";
		sideUpgradeCount.text = "";
		missileUpgradeCount.text = "";
		chargedCount.text = "";
		_waveCount = 0;
		_gameOver = false;
		_hazardCount = hazardCount;
		_spawnWait = spawnWait;
		_waveWait = waveWait;
		_lastScoreUpdate = Time.time;
		_score = 0;
		_highScore = 0;
		_hazardMax = 3;
		if(PlayerPrefs.GetInt("High Score") > _highScore) 
		{
			_highScore = PlayerPrefs.GetInt("High Score");
		}
		UpdateScore();
		SpawnPlayer();
		StartCoroutine(SpawnWaves());
	}

	void SpawnPlayer()
	{
		Quaternion spawnRotation = Quaternion.identity;
		Instantiate (playerOne, Vector3.zero, spawnRotation);
	}
	
	IEnumerator SpawnWaves()
	{
		yield return new WaitForSeconds(startWait);
		while (!_gameOver)
		{
			_waveCount += 1;
			StartCoroutine(DisplayWave());
			for(int i = 0; i < _hazardCount; i++) 
			{
				Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (hazards[Random.Range(0,_hazardMax)], spawnPosition, spawnRotation);
				if(_gameOver)
				{
					break;
				}
				yield return new WaitForSeconds(_spawnWait);
			}
			if(_gameOver)
			{
				break;
			}
			yield return new WaitForSeconds(_waveWait);
			_hazardCount += 10;
			_spawnWait -= 0.075f;
			_waveWait -= 0.2f;
			_hazardMax += 1;
			if (_spawnWait < 0.05f)
			{
				_spawnWait = 0.05f;
			}
			if (_waveWait < 0.1f)
			{
				_waveWait = 0.1f;
			}
			if (_hazardMax > 5)
			{
				_hazardMax = 5;
			}
		}
	}

	IEnumerator DisplayWave()
	{
		waveText.text = "Wave " + _waveCount;
		yield return new WaitForSeconds(waveDisplayWait);
		waveText.text = "";
	}
	
	public void AddScore(int newScoreValue)
	{
		_score += newScoreValue;
		UpdateScore();
	}

	void UpdateScore() 
	{
		if(!_gameOver) 
		{
			if (_score > _highScore) 
			{
				_highScore = _score;
			}
			scoreText.text = "Score: " + _score;
			highScoreText.text = "High Score: " + _highScore;
		}
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			_menuManager.Pause();
		}

		_timeSinceScoreUpdate = Time.time - _lastScoreUpdate;
		int newScoreValue = (int)_timeSinceScoreUpdate;
		if(newScoreValue > 0) 
		{
			AddScore (newScoreValue * 50);
			_lastScoreUpdate = Time.time;
		}
		PlayerPrefs.SetInt("High Score", _highScore);
	}

	public void GameOver()
	{
		_gameOver = true;
		_menuManager.ShowMenu(_gameOverMenu);
	}	

	public int Wave()
	{
		return _waveCount;
	}

	public void HighScoreReset()
	{
		_highScore = 0;
		highScoreText.text = "High Score: " + _highScore;
	}

	public void setShieldText(int shields, int points, bool declareGiven)
	{
		shieldText.text = "Shields Left: " + shields;
		if (declareGiven) 
		{
			StartCoroutine(SetShieldGivenText(points));
		}
	}

	IEnumerator SetShieldGivenText(int points)
	{
		shieldGivenText.text = "Shields Earned for Reaching: " + points + " Points";
		yield return new WaitForSeconds(waveDisplayWait);
		shieldGivenText.text = "";
	}

	public void setMainUpgradeText(int upgradeCount)
	{
		mainUpgradeCount.text = "Main Cannon: " + upgradeCount + "/30";
	}

	public void setSideUpgradeText(int upgradeCount)
	{
		sideUpgradeCount.text = "Side Cannons: " + upgradeCount + "/30";
	}

	public void setMissileUpgradeText(int upgradeCount)
	{
		missileUpgradeCount.text = "Missiles: " + upgradeCount + "/30";
	}

	public void setChargedCountText(int charges)
	{
		chargedCount.text = "Cannon Charges: " + charges + "/5";
	}

	public int getScore()
	{
		return _score;
	}
}
