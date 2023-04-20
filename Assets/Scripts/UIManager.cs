using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public TextMeshProUGUI HealthField;
    public RectTransform HealthBar;
    public TextMeshProUGUI GunField;
    public TextMeshProUGUI AmmoField;
    public RectTransform AmmoBar;

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
}
