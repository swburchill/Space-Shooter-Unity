using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour 
{
	public Menu currentMenu;
	
	// Use this for initialization
	public void Start() 
	{
		if (currentMenu.name == "PauseMenu") 
		{
			return;
		}
		ShowMenu(currentMenu);
	}
	
	public void ShowMenu(Menu menu)
	{
		if (currentMenu != null) 
		{
			currentMenu.IsOpen = false;
		}

		currentMenu = menu;
		currentMenu.IsOpen = true;
	}

	public void HideCurrentMenu()
	{
		if (currentMenu != null) 
		{
			currentMenu.IsOpen = false;
		}
	}

	public void SinglePlayer()
	{
		Application.LoadLevel("Main");
	}

	public void Restart()
	{
		Application.LoadLevel(Application.loadedLevel);
	}
	
	public void ReturnMainMenu()
	{
		Time.timeScale = 1.0f;
		Application.LoadLevel("Menu");
	}

	public void Quit()
	{
		Application.Quit();
	}
	
	public void Pause()
	{
		OpenMenu();
		Time.timeScale = 0.0f;
	}

	public void Resume()
	{
		CloseMenu();
		Time.timeScale = 1.0f;
	}

	private void OpenMenu()
	{
		currentMenu.IsOpen = true;
	}

	private void CloseMenu()
	{
		currentMenu.IsOpen = false;
	}
}
