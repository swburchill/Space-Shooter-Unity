using UnityEngine;
using System.Collections;

public class MissileLock : MonoBehaviour 
{
	private Transform _enemyPostion;
	public float speed;
	private float _currentSpeed;

	void Start()
	{
		_enemyPostion = null;
		_currentSpeed = speed;
		FindClosestEnemy();
	}

	void FindClosestEnemy()
	{
		float nearestDistanceSqr = Mathf.Infinity;
		GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("EnemyEasy");
		foreach(GameObject obj in enemyObjects)
		{
			Vector3 objPosition = obj.transform.position;
			float distanceSqr = (objPosition - transform.position).sqrMagnitude;
			
			if(distanceSqr < nearestDistanceSqr)
			{
				_enemyPostion = obj.transform;
				nearestDistanceSqr = distanceSqr;
			}
		}
		enemyObjects = GameObject.FindGameObjectsWithTag("EnemyHard");
		foreach(GameObject obj in enemyObjects)
		{
			Vector3 objPosition = obj.transform.position;
			float distanceSqr = (objPosition - transform.position).sqrMagnitude;
			
			if(distanceSqr < nearestDistanceSqr)
			{
				_enemyPostion = obj.transform;
				nearestDistanceSqr = distanceSqr;
			}
		}
	}

	void Update()
	{
		if (_enemyPostion == null) 
		{
			FindClosestEnemy();
		}
	}
	
	void FixedUpdate ()
	{
		//make the missile rotate to always face the closest enemey
		if (_enemyPostion != null) 
		{
			GetComponent<Rigidbody>().velocity = transform.forward * 0;
			transform.rotation = Quaternion.LookRotation (_enemyPostion.position - transform.position);
			transform.position = Vector3.MoveTowards (transform.position, _enemyPostion.position, _currentSpeed * Time.deltaTime);
		} 
		else 
		{
			GetComponent<Rigidbody>().velocity = transform.forward * _currentSpeed;
		}
	}
}
