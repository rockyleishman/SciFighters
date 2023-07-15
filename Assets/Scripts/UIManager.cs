using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider = null;

    //[SerializeField] private TextMeshProUGUI volumeTextUI = null;
    [SerializeField] private GameObject _gameObject;
    private AudioSource AS;

    public void ChangeVolume()
    {
        AS.volume = volumeSlider.value;
    }

    private static UIManager _instance;

    internal static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<UIManager>();
            }

            return _instance;
        }
    }

    internal bool IsPlayGame = false;

    [SerializeField] public TextMeshProUGUI TimeField;
    [SerializeField] public TextMeshProUGUI HealthField;
    [SerializeField] public RectTransform HealthBar;
    [SerializeField] public TextMeshProUGUI GunField;
    [SerializeField] public TextMeshProUGUI AmmoField;
    [SerializeField] public RectTransform AmmoBar;
    [SerializeField] public TextMeshProUGUI ScoreField;
    [SerializeField] public TextMeshProUGUI FinalScoreField;
    public Transform EndGameMenuScoreTF;
    public TextMeshProUGUI EndGameMenuScoreTMP;
    [SerializeField] public GameObject PauseMenu = null;
    [SerializeField] public GameObject EndGameMenu = null;
    [SerializeField] public GameObject FirstMenu = null;
    [SerializeField] public GameObject NameInputMenu;

    private void Start()
    {
        ShowFirstMenu();
        AS = _gameObject.GetComponent<AudioSource>();
        EndGameMenuScoreTF = EndGameMenu.transform.Find("txtScore");
        EndGameMenuScoreTMP = EndGameMenuScoreTF.GetComponent<TextMeshProUGUI>();
        volumeSlider.value = 0.5f;
        NameInputMenu.SetActive(true);
    }

    private void Update()
    {
        if (PauseMenu.activeSelf == true)
        {
            ChangeVolume();
        }
    }

    public void CloseNameInputCavase()
    {
        NameInputMenu.SetActive(false);
    }

    public void ShowFirstMenu()
    {
        PauseMenu.SetActive(false);
        FirstMenu.SetActive(true);
        EndGameMenu.SetActive(false);
        //pause game
        Time.timeScale = 0;
        GameManager.Instance.PausePlayer();
        //show cursor
        Cursor.lockState = CursorLockMode.None;
    }

    public void ShowEndGameMenu()
    {
        var highScoreBoard = EndGameMenu.transform.Find("ScorePanel").GetComponent<HighScoreBoard>();
        highScoreBoard.SavePlayerScoreData();
        highScoreBoard.UpdateHighScoreLeaderboard();
        PauseMenu.SetActive(false);
        FirstMenu.SetActive(false);
        EndGameMenu.SetActive(true);
        
        EndGameMenuScoreTMP.text = "Score: " + ScoreManager.Instance.Score.ToString();
        //pause game
        Time.timeScale = 0;
        GameManager.Instance.PausePlayer();
        PlayerPrefs.SetInt(GameManager.Instance.currentPlayer, ScoreManager.Instance.Score);
        //show cursor
        Cursor.lockState = CursorLockMode.None;
    }

    public void Pause()
    {
        PauseMenu.SetActive(true);
        FirstMenu.SetActive(false);
        EndGameMenu.SetActive(false);
        //pause game
        Time.timeScale = 0;
        GameManager.Instance.PausePlayer();
        //show cursor
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnclickResume()
    {
        PauseMenu.SetActive(false);
        FirstMenu.SetActive(false);
        EndGameMenu.SetActive(false);
        //unpause game
        Time.timeScale = 1;
        GameManager.Instance.UnpausePlayer();
        IsPlayGame = true;
        //lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        //refresh HUD
        UpdateUI();
    }

    public void OnclickRestart()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnclickQuit()
    {
        EditorApplication.ExitPlaymode();
    }

    internal void UpdateUI()
    {
        UpdateHealth();
        UpdateGun();
        UpdateAmmo();
        UpdateScore();
        UpdateTime();
    }

    internal void UpdateHealth()
    {
        HealthField.text = "HP: " + GameManager.Instance.Player.Health.ToString() + " / " +
                           GameManager.Instance.Player.MaxHealth.ToString();
        float healthAlpha = (float)GameManager.Instance.Player.Health / (float)GameManager.Instance.Player.MaxHealth;
        if (healthAlpha >= 0.0f && healthAlpha <= 1.0f)
        {
            HealthBar.localScale = new Vector3(healthAlpha, 1.0f, 1.0f);
            HealthBar.GetComponent<RawImage>().color = Color.Lerp(Color.red, Color.green, healthAlpha);
        }
        else
        {
            healthAlpha = 0.0f;
        }
    }

    internal void UpdateGun()
    {
        try
        {
            GunField.text = GameManager.Instance.Player.EquipedWeapon.name + ": " +
                            GameManager.Instance.Player.EquipedWeapon.Ammo.ToString() + " / " +
                            GameManager.Instance.Player.EquipedWeapon.MagazineSize.ToString();
            AmmoBar.localScale =
                new Vector3(
                    (float)GameManager.Instance.Player.EquipedWeapon.Ammo /
                    (float)GameManager.Instance.Player.EquipedWeapon.MagazineSize, 1.0f, 1.0f);
        }
        catch
        {
            //wait for next update
        }
    }

    internal void UpdateAmmo()
    {
        AmmoField.text = GameManager.Instance.Player.UnloadedAmmo.ToString();
    }

    internal void UpdateScore()
    {
        ScoreField.text = "Score: " + ScoreManager.Instance.Score.ToString();
    }

    internal void UpdateTime()
    {
        float timeRemaining = (GameManager.Instance.LevelTimeLimitInMinutes * 60.0f) - GameManager.Instance.GameTime;

        TimeField.text = "Time Left: " + ((int)Mathf.Floor(timeRemaining / 60)).ToString("D2") + ":" +
                         ((int)Mathf.Floor(timeRemaining % 60)).ToString("D2");
    }
}