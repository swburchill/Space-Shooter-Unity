using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioVolume : MonoBehaviour 
{
	private float _audioVolume;
	private GameObject _volumeSliderObject;
	private Slider _volumeSlider;

	// Use this for initialization
	void Start () 
	{
		_audioVolume = PlayerPrefs.GetFloat("Audio Volume");
		if(_audioVolume < 0.0f || _audioVolume > 1.0f) 
		{
			_audioVolume = 1.0f;
		}
		AudioListener.volume = _audioVolume;
		_volumeSliderObject = GameObject.Find("VolumeSlider");
		_volumeSlider = _volumeSliderObject.GetComponentInChildren<Slider>();
		_volumeSlider.value = _audioVolume;
	}

	public void ChangeAudioVolume(float volume)
	{
		_audioVolume = volume;
		PlayerPrefs.SetFloat("Audio Volume", _audioVolume);
		AudioListener.volume = _audioVolume;
	}
}
