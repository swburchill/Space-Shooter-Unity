using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour 
{
	public float speed;
	public float tilt;
	public Boundary boundary;
	ParticleSystem[] emitters;

	public GameObject mainCannonShot;
	public Transform mainCannonPosition;
	public GameObject sideCannon1Shot;
	public GameObject sideCannon2Shot;
	public Transform sideCannon1Position;
	public Transform sideCannon2Position;
	public GameObject missileShot;
	public Transform missilePosition;
	public float mainCannonFireRate;
	public float sideCannonFireRate;
	public float missileFireRate;
	private float _nextMainCannonFire;
	private float _nextSideCannonFire;
	private float _nextMissileFire;
	
	private bool _canFireMain;
	private bool _canFireSides;
	private bool _canFireMissiles;
	public int storeShots;
	private int _storedShots;

	public int lives;
	private int _scoreSinceLastShield;
	private int _scoreShieldEarned;
	public int scoreForShieldRegen;

	private int _mainUpgradeCount;
	private int _sideUpgradeCount;
	private int _missileUpgradeCount;

	private GameController _gameController;

	//run once when created
	void Awake()
	{
		emitters = this.GetComponentsInChildren<ParticleSystem>();
		_canFireMain = true;
		_canFireSides = false;
		_canFireMissiles = false;
		_storedShots = storeShots;
		_scoreSinceLastShield = 0;
		_scoreShieldEarned = 0;
		_mainUpgradeCount = 0;
		_sideUpgradeCount = 0;
		_missileUpgradeCount = 0;

		GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
		if (gameControllerObject != null) 
		{
			_gameController = gameControllerObject.GetComponent<GameController>();
		}
		if (_gameController == null) 
		{
			Debug.Log("Cannot find 'GameController' script");
		}
		_gameController.setShieldText(lives, _scoreShieldEarned, false);
		_gameController.setMainUpgradeText (_mainUpgradeCount);
		_gameController.setSideUpgradeText (_sideUpgradeCount);
		_gameController.setMissileUpgradeText (_missileUpgradeCount);
	}

	//run every frame
	void Update()
	{
		// Instantiates a projectile if the Fire1 button (default is Ctrl) is pressed.
		if(_canFireMain && Input.GetButton("Fire1") && Time.time > _nextMainCannonFire) 
		{
			if(_storedShots > 0)
			{
				_nextMainCannonFire = Time.time + Mathf.Min(mainCannonFireRate, 0.05f);
				_storedShots -= 1;
			}
			else
			{
				_nextMainCannonFire = Time.time + mainCannonFireRate;
			}
			Instantiate(mainCannonShot, mainCannonPosition.position, Quaternion.Euler (0.0f, 0.0f, 0.0f));
		}
		if (_canFireSides && Input.GetButton ("Fire1") && Time.time > _nextSideCannonFire) 
		{
			if (_storedShots > 0) 
			{
				_nextSideCannonFire = Time.time + Mathf.Min(sideCannonFireRate, 0.05f);
				_storedShots -= 1;
			} 
			else
			{
				_nextSideCannonFire = Time.time + sideCannonFireRate;
			}
			Instantiate(sideCannon1Shot, sideCannon1Position.position, Quaternion.Euler (0.0f, 0.0f, 0.0f));
			Instantiate(sideCannon2Shot, sideCannon2Position.position, Quaternion.Euler (0.0f, 0.0f, 0.0f));
		}
		//store shots to reward for not holding trigger
		if ((!Input.GetButton ("Fire1")) && ((Time.time > (_nextSideCannonFire + sideCannonFireRate)) || (Time.time > (_nextMainCannonFire + mainCannonFireRate)))) 
		{
			_storedShots += 1;
			if (_storedShots > storeShots)
			{
				_storedShots = storeShots;
			}
			_nextMainCannonFire = Time.time;
			_nextSideCannonFire = Time.time;
		}
		if(_canFireMissiles && Input.GetButton("Fire1") && Time.time > _nextMissileFire) 
		{
			_nextMissileFire = Time.time + missileFireRate;
			Instantiate(missileShot, missilePosition.position, Quaternion.Euler (0.0f, 0.0f, 0.0f));
		}

		if((_gameController.getScore() - _scoreSinceLastShield) > scoreForShieldRegen) 
		{
			AddLife();
			_scoreSinceLastShield = _gameController.getScore();
		}
		_gameController.setChargedCountText (_storedShots);
	}

	//run once every 0.02 seconds
	void FixedUpdate()
	{
		//get input for movement
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		if(moveVertical < 0) //falling back so turn off engine particles and reduce speed(penalty)
		{
			foreach (ParticleSystem emit in emitters) 
			{
				emit.Stop();
				emit.Clear();
				moveHorizontal = moveHorizontal * 0.9f;
				moveVertical = moveVertical * 0.8f;
			}
		} 
		else if(moveVertical > 0) //moving forward so use both effects to simulate boost
		{
			foreach (ParticleSystem emit in emitters) 
			{
				moveHorizontal = moveHorizontal * 1.1f;
				emit.Play();
			}
		}
		else //staying still so only use one effect
		{
			emitters[0].Play();
			emitters[1].Stop();
			emitters[1].Clear();
		}

		//set movement
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		GetComponent<Rigidbody>().velocity = movement * speed;
		
		//clip player to view
		GetComponent<Rigidbody>().position = new Vector3
		(
			Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 
			0.0f, 
			Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
		);

		//change particle effects if movement was clamped - will keep tilt as a means of dodging(feature)
		if(GetComponent<Rigidbody> ().position.z <= boundary.zMin && moveVertical < 0) 
		{
			//turn flare back on to indicate no longer falling passed bottom of screen
			emitters[0].Play();
			GetComponent<Rigidbody>().velocity = new Vector3 (GetComponent<Rigidbody>().velocity.x, 0.0f, 0.0f);
		}

		if(GetComponent<Rigidbody> ().position.z >= boundary.zMax && moveVertical > 0) 
		{
			//turn core back off to indicate no longer accelerating passed top limit
			emitters[1].Stop();
			emitters[1].Clear();
			GetComponent<Rigidbody>().velocity = new Vector3 (GetComponent<Rigidbody>().velocity.x, 0.0f, 0.0f);
		}
		
		//have object rotate when turning
		GetComponent<Rigidbody>().rotation = Quaternion.Euler (0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
	}

	public void AddUpgradeMain()
	{
		//takes 30 upgrades to rach max fire rate from 0.5
		mainCannonFireRate = mainCannonFireRate * 0.95f;
		_mainUpgradeCount += 1;
		if(mainCannonFireRate < 0.115f)
		{
			_mainUpgradeCount -= 1;
			mainCannonFireRate = 0.115f;
			_gameController.AddScore(100);
		}
		_gameController.setMainUpgradeText (_mainUpgradeCount);
	}

	public void AddUpgradeSide()
	{
		//takes 30 upgrades to rach max fire rate from 0.66
		_canFireSides = true;
		sideCannonFireRate = sideCannonFireRate * 0.95f;
		_sideUpgradeCount += 1;
		if(sideCannonFireRate < 0.14f)
		{
			_sideUpgradeCount -= 1;
			sideCannonFireRate = 0.14f;
			_gameController.AddScore(100);
		}
		_gameController.setSideUpgradeText (_sideUpgradeCount);
	}

	public void AddUpgradeMissile()
	{
		//takes 30 upgrades to reach max fire rate from 2
		_canFireMissiles = true;
		missileFireRate = missileFireRate * 0.95f;
		_missileUpgradeCount += 1;
		if(missileFireRate < 0.42f)
		{
			_missileUpgradeCount -= 1;
			missileFireRate = 0.42f;
			_gameController.AddScore(100);
		}
		_gameController.setMissileUpgradeText (_missileUpgradeCount);
	}

	public int Lives()
	{
		return lives;
	}

	public void UseLife()
	{
		lives -= 1;
		_gameController.setShieldText(lives, _scoreShieldEarned, false);
	}

	private void AddLife()
	{
		lives += 1;
		_scoreShieldEarned += scoreForShieldRegen;
		_gameController.setShieldText(lives, _scoreShieldEarned, true);
	}
}
