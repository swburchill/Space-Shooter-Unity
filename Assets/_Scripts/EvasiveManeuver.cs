using UnityEngine;
using System.Collections;

public class EvasiveManeuver : MonoBehaviour
{
	public Boundary boundary;
	public float tilt;
	public float dodge;
	public float smoothing;
	public Vector2 startWait;
	public Vector2 maneuverTime;
	public Vector2 maneuverWait;
	
	private Transform _playerPostion;
	
	private float currentSpeed;
	private float targetManeuver;

	void Start ()
	{
		GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
		if (playerObject != null) 
		{
			_playerPostion = playerObject.GetComponent<Transform>();
		}
		if (_playerPostion == null) 
		{
			Debug.Log("Cannot find 'Player' Object");
		}
		currentSpeed = GetComponent<Rigidbody>().velocity.z;
		StartCoroutine(Evade());
	}
	
	IEnumerator Evade()
	{
		yield return new WaitForSeconds(Random.Range (startWait.x, startWait.y));
		while(true)
		{
			targetManeuver = Random.Range(1, dodge) * -Mathf.Sign(transform.position.x);
			yield return new WaitForSeconds(Random.Range(maneuverTime.x, maneuverTime.y));
			targetManeuver = 0;
			yield return new WaitForSeconds(Random.Range(maneuverWait.x, maneuverWait.y));
		}
	}
	
	void FixedUpdate ()
	{
		float newManeuver = Mathf.MoveTowards(GetComponent<Rigidbody>().velocity.x, targetManeuver, smoothing * Time.deltaTime);
		GetComponent<Rigidbody>().velocity = new Vector3(newManeuver, 0.0f, currentSpeed);
		GetComponent<Rigidbody>().position = new Vector3
			(
				Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 
				0.0f, 
				Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
			);
		if (this.tag == "EnemyEasy") 
		{
			GetComponent<Rigidbody>().rotation = Quaternion.Euler(0, 180, GetComponent<Rigidbody>().velocity.x * tilt);
		} 
		//make the hard enemy rotate to always face the player
		else if (_playerPostion != null)
		{
			transform.rotation = Quaternion.LookRotation(_playerPostion.position - transform.position);
		}
	}
}
