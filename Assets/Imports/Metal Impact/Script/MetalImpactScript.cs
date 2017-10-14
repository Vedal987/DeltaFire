using UnityEngine;
using System.Collections;

public class MetalImpactScript : MonoBehaviour {
	
	public float despawnTime;
	public float lightDuration;
	
	public Light lightObject;
	
	void Start () {
		lightObject.GetComponent< Light > ().enabled = true;
		StartCoroutine(DespawnTimer());
		StartCoroutine(LightFlash ());
	}
	
	IEnumerator DespawnTimer () {
		yield return new WaitForSeconds(despawnTime);
		Destroy(gameObject);
	}
	
	IEnumerator LightFlash () {
		yield return new WaitForSeconds(lightDuration);
		lightObject.GetComponent< Light > ().enabled = false;
	}
}