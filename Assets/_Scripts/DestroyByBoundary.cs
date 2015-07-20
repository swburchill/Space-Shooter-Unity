using UnityEngine;
using System.Collections;

public class DestroyByBoundary : MonoBehaviour 
{
	void OnTriggerExit(Collider other) 
	{
		// Destroy everything that leaves the boundary trigger
		Destroy(other.gameObject);
	}
}
