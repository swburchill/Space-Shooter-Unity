using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighScore : MonoBehaviour 
{
	private int _highScore;
	private Text _text;
	private GameController _gameController;

	public void Awake()
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

		_highScore = PlayerPrefs.GetInt("High Score", _highScore);
		_text = GetComponent<Text>();
		_text.text = "High Score: " + _highScore;		
	}

	public void ResetHighScore()
	{
		_highScore = 0;
		PlayerPrefs.SetInt("High Score", _highScore);
		_text = GetComponent<Text>();
		_text.text = "High Score: " + _highScore;
		_gameController.HighScoreReset();
	}
}
