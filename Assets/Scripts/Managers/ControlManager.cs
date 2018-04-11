﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class YearData
{
	public int year;
	public string yearLabel;
	public AudioClip audioClip;
}

public enum ControllerType
{
	Right,
	Left
};

public enum ButtonType
{
	Trigger,
	Grip,
	TouchPad,
	ButtonOne,
	ButtonTwo,
	StartMenu
};

public class YearDataMissive : Missive
{
	public YearData data;
}

public class HelpMissive : Missive
{
	public bool state;
}

public class AudioMissive : Missive
{
	//
}

public class CreditsMissive : Missive
{
	//
}

public class WebViewMissive : Missive
{
	public string url = "";
}

public class InputDataMissive : Missive
{
	public ControllerType controllerType;
	public ButtonType buttonType;
}

public class ControlManager : MonoBehaviour
{
	#region Singleton
	private static ControlManager _instance = null;
	public static ControlManager instance
	{
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<ControlManager>();
			return _instance;
		}
	}

	void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
			return;
		}
		else
		{
			_instance = this;
		}
		DontDestroyOnLoad(transform.gameObject);
	}

	void OnApplicationQuit()
	{
		_instance = null;
		DestroyImmediate(gameObject);
	}
	#endregion

	public YearData[] yearData;

	public int currentYear { get; private set; }
	public int currentYearIndex { get; private set; }
	public timeWalkDayNightToggle dayNightRef { get; set; }

	#region mono
	private void Start()
	{
		Missive.AddListener<InputDataMissive>(OnInput);
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.R))
			ToggleMenu();

		if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.Escape))
			ToggleQuit();

		if (!LoadingManager.instance.IsMainSceneActive())
			return;

		if (Input.GetKeyUp(KeyCode.Y))
			ToggleYear();

		if (Input.GetKeyUp(KeyCode.N))
			ToggleNight();

		if (Input.GetKeyUp(KeyCode.H))
			ToggleHelp();

		if (Input.GetKeyUp(KeyCode.C))
			ToggleCredits();
	}

	private void OnDestroy()
	{
		Missive.RemoveListener<InputDataMissive>(OnInput);
	}
	#endregion

	#region logic
	public void SetCurrentTime(YearData data)
	{
		for (int i = 0; i < yearData.Length; i++)
		{
			if (data.year == yearData[i].year)
			{
				currentYear = data.year;
				currentYearIndex = i;
				return;
			}
		}
		// default if not correct year
		currentYear = yearData[0].year;
		currentYearIndex = 0;
	}
	#endregion

	#region toggles
	public void ToggleYear(int year = -1)
	{
		// ++year
		if (year == -1)
		{
			currentYearIndex = (currentYearIndex >= yearData.Length-1) ? 0 : currentYearIndex + 1;
			currentYear = yearData[currentYearIndex].year;
		}
		else
		{
			// set year
			for (int i = 0; i < yearData.Length; i++)
			{
				if (year == yearData[i].year)
				{
					currentYear = year;
					currentYearIndex = i;
					SendYearDataMissive(yearData[currentYearIndex]);
					return;
				}
			}
			// default if not correct year
			currentYear = yearData[0].year;
			currentYearIndex = 0;
		}

		SendYearDataMissive(yearData[currentYearIndex]);
		SendAudioMissive();
	}

	private void ToggleNight()
	{
		if (dayNightRef == null)
			dayNightRef = FindObjectOfType<timeWalkDayNightToggle>();
		if (dayNightRef != null)
			dayNightRef.ToggleDayNight();
	}

	private void ToggleHelp()
	{
		if (LoadingManager.instance.currentControllerType != LoadingManager.ControlTypes.FPS)
			return;

		SendHelpMissive();
	}

	private void ToggleMenu()
	{
		LoadingManager.instance.ToggleLoadingScene(true);
		LoadingManager.instance.ToggleMainScene(false);
	}

	private void ToggleCredits()
	{
		SendCreditsMissive();
	}

	private void ToggleQuit()
	{
		Application.Quit();
	}
	#endregion

	#region inputs
	public void OnInput(InputDataMissive missive)
	{
		if (missive == null) return;
		switch (missive.controllerType)
		{
			case ControllerType.Right:
				switch (missive.buttonType)
				{
					case ButtonType.Trigger:
						break;
					case ButtonType.Grip:
						break;
					case ButtonType.TouchPad:
						break;
					case ButtonType.ButtonOne:
						// Oculus A

						break;
					case ButtonType.ButtonTwo:
						// Oculus B

						break;
					case ButtonType.StartMenu:
						break;
					default:
						break;
				}
				break;
			case ControllerType.Left:
				switch (missive.buttonType)
				{
					case ButtonType.Trigger:
						break;
					case ButtonType.Grip:
						break;
					case ButtonType.TouchPad:
						break;
					case ButtonType.ButtonOne:
						// Oculus X

						break;
					case ButtonType.ButtonTwo:
						// Oculus Y

						break;
					case ButtonType.StartMenu:
						break;
					default:
						break;
				}
				break;
			default:
				break;
		}
		Debug.Log("Input received: " + missive.controllerType.ToString() + " / " + missive.buttonType.ToString());
	}
	#endregion

	#region missives
	private void SendYearDataMissive(YearData data)
	{
		YearDataMissive missive = new YearDataMissive();
		missive.data = data;
		Missive.Send(missive);
	}

	private void SendHelpMissive()
	{
		HelpMissive missive = new HelpMissive();
		Missive.Send(missive);
	}

	private void SendAudioMissive()
	{
		AudioMissive missive = new AudioMissive();
		Missive.Send(missive);
	}

	private void SendCreditsMissive()
	{
		CreditsMissive missive = new CreditsMissive();
		Missive.Send(missive);
	}

	public void SendWebViewMissive(string url)
	{
		WebViewMissive missive = new WebViewMissive();
		missive.url = url;
		Missive.Send(missive);
	}
	#endregion
}