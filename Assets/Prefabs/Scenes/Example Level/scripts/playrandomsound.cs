using UnityEngine;
using System.Collections;

public class playrandomsound : MonoBehaviour {
	
	private AudioSource myaudio;
	public AudioClip[] Sounds; 
	public float delay;

	private int ran;

	void Start () {
		myaudio = GetComponent<AudioSource>();
		StartCoroutine (Play ());
	}

	IEnumerator Play()
	{
		yield return new WaitForSeconds (delay);
		ran = Random.Range(1,Sounds.Length);
		myaudio.clip = Sounds[ran];
		myaudio.pitch = 0.9f + 0.1f *Random.value;
		myaudio.PlayOneShot(myaudio.clip);

		Sounds[ran] = Sounds[0];
		Sounds[0] = myaudio.clip;
	}

}
