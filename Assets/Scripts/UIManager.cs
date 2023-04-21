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
    public bool IsPlayGame = false;
    public float PlayTime = 1f;
    private float CurentTime = 0;

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
    
    public TextMeshProUGUI TimeCount;
    public TextMeshProUGUI HealthField;
    public RectTransform HealthBar;
    public TextMeshProUGUI GunField;
    public TextMeshProUGUI AmmoField;
    public RectTransform AmmoBar;
    public TextMeshProUGUI ScoreField;

    public GameObject PauseMenu = null;
    public GameObject EndGameMenu = null;
    public GameObject FirstMenu = null;

    private void Start()
    {
        ShowFirstMenu();
    }
    private void Update()
    {
        CurentTime += Time.deltaTime;
        if (PlayTime - CurentTime > 0)
        {
            TimeCount.text = "Time Remaining : " + Mathf.Floor((PlayTime - CurentTime) / 60) + "m " + Mathf.Floor((PlayTime - CurentTime) % 60) + "s";
        }
        else
            ShowEndGameMenu();

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
        HealthField.text = "Health: " + GameManager.Instance.Player.Health.ToString() + " / " + GameManager.Instance.Player.MaxHealth.ToString();
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
}
