﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	[SerializeField]
	private EventSystem _eventSystem;

	[SerializeField]
	private GameObject cursor;

	private GameObject _currentSelection;

	private GameObject CurrentSelection {
		get => _currentSelection;
		set {
			if( value == null )
				_eventSystem.SetSelectedGameObject(_currentSelection);
			else
				_currentSelection = value;
		}
	}

	void Update() {
		UpdateSelection();
	}

	public void Set1Player() {
		GameInProgress.Instance.PlayerCount = 1;
		GameInProgress.Instance.LoadScene("CharSel");
	}

	public void Set2Player() {
		GameInProgress.Instance.PlayerCount = 2;
		GameInProgress.Instance.LoadScene("CharSel");
	}

	public void Controls() {
		GameInProgress.Instance.LoadScene("Controls");
	}

	public void Credits() {
		GameInProgress.Instance.LoadScene("Credits");
	}

	public void Exit() {
		Application.Quit();
	}

	private void UpdateSelection() {
		CurrentSelection = _eventSystem.currentSelectedGameObject;
		cursor.transform.position = new Vector2(
			cursor.transform.position.x,
			CurrentSelection.transform.position.y
		);
	}
}