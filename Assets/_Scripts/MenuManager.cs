using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour 
{
	public Menu currentMenu;
	private Menu _pauseMenu;
	
	// Use this for initialization
	public void Start() 
	{
		//on start the pause menu is the default current menu
		_pauseMenu = currentMenu;
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
		OpenMenu ();
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
		Time.timeScale = 1.0f;
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
		ShowMenu(_pauseMenu);
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
		if (currentMenu.defaultButton != null) 
		{
			EventSystem.current.SetSelectedGameObject(currentMenu.defaultButton);
		}
	}

	private void CloseMenu()
	{
		currentMenu.IsOpen = false;
	}

	public void SetButtonSelected(GameObject selectedButton)
	{
		EventSystem.current.SetSelectedGameObject(selectedButton);
	}
}
