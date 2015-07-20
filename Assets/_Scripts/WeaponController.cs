using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
	public GameObject mainCannonShot;
	public Transform mainCannonPosition;
	public GameObject sideCannonShot;
	public Transform sideCannon1Position;
	public Transform sideCannon2Position;
	public float mainCannonFireRate;
	public float sideCannonFireRate;
	public float mainCannonDelay;
	public float SideCannonDelay;

	//for intro
	public bool isIntro;
	public float animationTime;
	public float animationLoopTime;
	private float _animationStartTime;

	private GameController _gameController;

	void Start()
	{
		_animationStartTime = Time.time;
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if(gameControllerObject != null) 
		{
			_gameController = gameControllerObject.GetComponent<GameController>();
		}
		if(_gameController == null) 
		{
			Debug.Log ("Cannot find 'GameController' script");
			if (isIntro) 
			{
				InvokeRepeating ("FireMain", mainCannonDelay, mainCannonFireRate);		
			}
		} 
		else 
		{
			if (_gameController.Wave () <= 6 || _gameController.Wave () >= 15) 
			{
				InvokeRepeating ("FireMain", mainCannonDelay, mainCannonFireRate);
			}
			if (_gameController.Wave () >= 7) 
			{
				InvokeRepeating ("FireSide", SideCannonDelay, sideCannonFireRate);
			}
		}
	}

	void FireMain()
	{
		Instantiate(mainCannonShot, mainCannonPosition.position, mainCannonPosition.rotation);
	}

	void FireSide()
	{
		Instantiate(sideCannonShot, sideCannon1Position.position, sideCannon1Position.rotation);
		Instantiate(sideCannonShot, sideCannon2Position.position, sideCannon2Position.rotation);
	}

	void Update()
	{
		if (isIntro) 
		{
			if(Time.time > (_animationStartTime + animationLoopTime))
			{
				_animationStartTime = Time.time;
				InvokeRepeating ("FireMain", mainCannonDelay, mainCannonFireRate);	
			}
			if(Time.time > (_animationStartTime + animationTime))
			{
				CancelInvoke();
			}
		}
	}
}
