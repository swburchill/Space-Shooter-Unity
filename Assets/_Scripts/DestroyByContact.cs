using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour 
{
	public GameObject explosion;
	public GameObject playerExplosion;
	public GameObject playerShield;
	public GameObject upgradeMain;
	public GameObject upgradeSide;
	public GameObject upgradeMissile;
	public int scoreValue;
	private GameController _gameController;
	private PlayerController _playerController;
	private bool _destroyed;

	void Start()
	{
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
		if (gameControllerObject != null) 
		{
			_gameController = gameControllerObject.GetComponent<GameController>();
		}
		if (_gameController == null) 
		{
			Debug.Log("Cannot find 'GameController' script");
		}

		GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
		if (playerObject != null) 
		{
			_playerController = playerObject.GetComponent<PlayerController>();
		}
		if (_playerController == null) 
		{
			Debug.Log("Cannot find 'PlayerController' script");
			_gameController.GameOver();
		}
		_destroyed = false;
	}

	private void SpawnUpgrade()
	{
		//have a 10% chance to generate an upgrade for the player
		float chance = Random.Range(0.0f, 10.0f);
		if(chance > 9.0f)
		{
			Quaternion spawnRotation = Quaternion.identity;
			float upgradesAvailable = 1.0f;
			if(_gameController.Wave() >= 4)
			{
				upgradesAvailable = 2.0f;
			}
			if(_gameController.Wave() >= 7)
			{
				upgradesAvailable = 3.0f;
			}
			float upgradeChoice = Random.Range(0, upgradesAvailable);
			if(upgradeChoice <= 1.0f)
			{
				Instantiate(upgradeMain, transform.position, spawnRotation);
			}
			if(upgradeChoice > 1.0f && upgradeChoice <= 2.0f)
			{
				Instantiate(upgradeSide, transform.position, spawnRotation);
			}
			if(upgradeChoice > 2.0f && upgradeChoice <= 3.0f)
			{
				Instantiate(upgradeMissile, transform.position, spawnRotation);
			}
		}
	}

	// Destroy everything that enters the trigger
	void OnTriggerEnter(Collider other) 
	{
		//for double laser, check if collision with object thats destoryed and return so we don't destroy twice
		//ie, let double shots that hit at same time to have 1 shot pass through
		if (_destroyed) 
		{
			return;
		}

		//if we are the player
		if (this.tag == "Player") 
		{
			//ignore player shots on player
			if (other.tag == "PlayerShot") 
			{
				return;
			} 
			
			//let player collect upgrades
			else if (other.tag == "UpgradeMain" || other.tag == "UpgradeSide" || other.tag == "UpgradeMissile") 
			{
				if (other.tag == "UpgradeMain") 
				{
					_playerController.AddUpgradeMain ();
				}
				else if (other.tag == "UpgradeSide") 
				{
					_playerController.AddUpgradeSide ();
				}
				else if (other.tag == "UpgradeMissile") 
				{
					_playerController.AddUpgradeMissile ();
				}
				Destroy(other.gameObject);
			}

			//only explode player to enemy fire or collision with enemy or asteroid
			else if (other.tag == "EnemyShot" || other.tag == "EnemyEasy" || other.tag == "EnemyHard" || other.tag == "Asteroid")  
			{
				if (_playerController.Lives () <= 0) 
				{
					if (playerExplosion != null) 
					{
						Instantiate (playerExplosion, transform.position, transform.rotation);
					}
					_gameController.GameOver();
					//only destroy triggers, let other objects destroy themselves
					if (other.tag == "EnemyShot")
					{
						Destroy(other.gameObject);
					}
					Destroy(gameObject);
					_destroyed = true;
				} 
				else 
				{
					_playerController.UseLife ();
					Transform shield = ((GameObject)Instantiate(playerShield, transform.position, transform.rotation)).transform;
					shield.parent = this.transform;
					//only destroy triggers, let other objects destroy themselves
					if (other.tag == "EnemyShot")
					{
						Destroy(other.gameObject);
					}
				}
			} 
		}

		//we're not the player - if we are an enemy
		else if (this.tag == "EnemyEasy" || this.tag == "EnemyHard") 
		{
			//ignore enemy shots on enemy
			if (other.tag == "EnemyShot")
			{
				return;
			}
			//ignore upgrades collisions with enemy and asteroids
			else if (other.tag == "UpgradeMain" || other.tag == "UpgradeSides" || other.tag == "UpgradeMissile" || other.tag == "Asteroid" || other.tag == "EnemyEasy" || other.tag == "EnemyHard" || other.tag == "Boundary")
			{
				return;
			}
			//only explode to player fire or collision with player
			else if (other.tag == "PlayerShot" || other.tag == "Player")  
			{
				if (explosion != null) 
				{
					Instantiate (explosion, transform.position, transform.rotation);
				}
				Destroy(gameObject);
				if (other.tag == "PlayerShot")
				{
					Destroy(other.gameObject);
				}
				_gameController.AddScore(scoreValue);
				SpawnUpgrade();
				_destroyed = true;
			} 
		}

		//we're not the player or an enemy - we must be an asteroid
		else 
		{
			//ignore all collisions with anything except player and player/enemy fire
			if (other.tag == "Boundary" || other.tag == "Asteroid" || other.tag == "UpgradeMain" || other.tag == "UpgradeSides" || other.tag == "UpgradeMissile" || other.tag == "EnemyEasy" || other.tag == "EnemyHard") 
			{
				return;
			}
			if (explosion != null) 
			{
				Instantiate(explosion, transform.position, transform.rotation);
				Destroy(gameObject);
			}
			//only destroy triggers, let other objects destroy themselves
			if (other.tag == "PlayerShot" || other.tag == "EnemyShot")
			{
				Destroy(other.gameObject);
			}

			_gameController.AddScore (scoreValue);
			SpawnUpgrade();
			_destroyed = true;
		}
	}
}
