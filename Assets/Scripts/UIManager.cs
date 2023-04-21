using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
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

    [SerializeField] public GameObject PauseMenu = null;
    [SerializeField] public GameObject EndGameMenu = null;
    [SerializeField] public GameObject FirstMenu = null;

    private void Start()
    {
        ShowFirstMenu();
    }
    private void Update()
    {

    }
    public void ShowFirstMenu()
    {
        PauseMenu.SetActive(false);
        FirstMenu.SetActive(true);
        EndGameMenu.SetActive(false);
        Time.timeScale = 0;
    }
    public void ShowEndGameMenu()
    {
        PauseMenu.SetActive(false);
        FirstMenu.SetActive(false);
        EndGameMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void Pause()
    {
        PauseMenu.SetActive(true);
        FirstMenu.SetActive(false);
        EndGameMenu.SetActive(false);
        Time.timeScale = 0;
    }
    public void OnclickResume()
    {
        PauseMenu.SetActive(false);
        FirstMenu.SetActive(false);
        EndGameMenu.SetActive(false);
        Time.timeScale = 1;
        IsPlayGame = true;
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
    }

    internal void UpdateHealth()
    {
        HealthField.text = "HP: " + GameManager.Instance.Player.Health.ToString() + " / " + GameManager.Instance.Player.MaxHealth.ToString();
        float healthAlpha = (float)GameManager.Instance.Player.Health / (float)GameManager.Instance.Player.MaxHealth;
        HealthBar.localScale = new Vector3(healthAlpha, 1.0f, 1.0f);
        HealthBar.GetComponent<RawImage>().color = Color.Lerp(Color.red, Color.green, healthAlpha);
    }

    internal void UpdateGun()
    {
        GunField.text = GameManager.Instance.Player.EquipedWeapon.name + ": " + GameManager.Instance.Player.EquipedWeapon.Ammo.ToString() + " / " + GameManager.Instance.Player.EquipedWeapon.MagazineSize.ToString();
        AmmoBar.localScale = new Vector3((float)GameManager.Instance.Player.EquipedWeapon.Ammo / (float)GameManager.Instance.Player.EquipedWeapon.MagazineSize, 1.0f, 1.0f);
    }

    internal void UpdateAmmo()
    {
        AmmoField.text = GameManager.Instance.Player.UnloadedAmmo.ToString();
    }

    internal void UpdateScore()
    {
        ScoreField.text = ScoreManager.Instance.Score.ToString();
    }

    internal void UpdateTime()
    {
        float timeRemaining = (GameManager.Instance.LevelTimeLimitInMinutes * 60.0f) - GameManager.Instance.GameTime;

        TimeField.text = "Time Left: " + ((int)Mathf.Floor(timeRemaining / 60)).ToString("D2") + ":" + ((int)Mathf.Floor(timeRemaining % 60)).ToString("D2");
    }
}
