﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInProgress : MonoBehaviour {
	private static GameInProgress _instance;

	public static GameInProgress Instance => _instance;

	private const string SHOW_ON_SCREEN_CONTROLS = "showOnScreenControls";

	[field: SerializeField]
	public int WinnerId { get; set; }

	public bool ShowOnScreenControls {
		get => PlayerPrefs.HasKey(SHOW_ON_SCREEN_CONTROLS) && PlayerPrefs.GetInt(SHOW_ON_SCREEN_CONTROLS) != 0;
		set => PlayerPrefs.SetInt(SHOW_ON_SCREEN_CONTROLS, value ? 1 : 0);
	}

	[field: SerializeField]
	public int PlayerCount { get; set; } = 1;

	[field: SerializeField]
	public PlayerOptionsConfig AllPlayers { get; set; }

	[field: SerializeField]
	public PlayerConfig LeftPlayer { get; set; }

	[field: SerializeField]
	public PlayerConfig RightPlayer { get; set; }

	public InputController LeftInput { get; set; }

	public InputController RightInput { get; set; }

	void Awake() {
		if( _instance == null ) {
			_instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else {
			Destroy(gameObject);
		}
	}

	private void Start() {
#if UNITY_ANDROID || UNITY_IOS || DEBUG_MOBILE
		ShowOnScreenControls = true;
#endif
	}

	public void LoadScene(string name) {
		if( LeftInput != null )
			LeftInput.IsActive = false;
		if( RightInput != null )
			RightInput.IsActive = false;
		StartCoroutine(DoLoadScene(name));
		if( LeftInput != null )
			LeftInput.IsActive = true;
		if( RightInput != null )
			RightInput.IsActive = true;
	}

	private IEnumerator DoLoadScene(string name) {
		SceneManager.LoadScene(name);
		yield return new WaitForSeconds(0.5f);
	}
}