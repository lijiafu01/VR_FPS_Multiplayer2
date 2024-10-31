using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using TraningMode;
using UnityEngine.SceneManagement;

public class WeaponSelectionUI : MonoBehaviour
{
    public WeaponType _WeaponType = WeaponType.Pistol;
    public TextMeshProUGUI weaponNameText;
    public RawImage weaponRawImage;
    public Texture bowtt;
    public Texture Pistoltt;

    private void Start()
    {
        GameManager.Instance.playerChooseWeapon = WeaponType.Pistol;

    }
    //public List<Texture> weaponTextures = new List<Texture>();
    /*public RawImage weaponRawImage; // Link this in the Unity Inspector
    public List<Texture> weaponTextures = new List<Texture>(); // This list should be populated with weapon textures in the same order as the weapons list

    public TextMeshProUGUI weaponNameText;  // Sử dụng TextMeshProUGUI
    public Button nextButton;    // Link this in the Unity Inspector
    public Button previousButton;  // Link this in the Unity Inspector

    private int currentWeaponIndex = 0;
    private List<WeaponType> weapons = new List<WeaponType> { WeaponType.Pistol, WeaponType.Grenade, WeaponType.Bow };

    private void Start()
    {
        UpdateWeaponDisplay();
        nextButton.onClick.AddListener(NextWeapon);
        previousButton.onClick.AddListener(PreviousWeapon);
    }

    IEnumerator FadeWeaponImage(Texture newTexture)
    {
        float duration = 0.3f; // Duration of the fade
        float currentTime = 0f;

        // Fade out
        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, currentTime / duration);
            weaponRawImage.color = new Color(1f, 1f, 1f, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }

        weaponRawImage.texture = newTexture; // Change the texture

        // Fade in
        currentTime = 0f;
        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, currentTime / duration);
            weaponRawImage.color = new Color(1f, 1f, 1f, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    void UpdateWeaponDisplay()
    {
        if (weapons.Count > 0 && weaponTextures.Count > 0)
        {
            weaponNameText.text = weapons[currentWeaponIndex].ToString();
            StartCoroutine(FadeWeaponImage(weaponTextures[currentWeaponIndex])); // Use coroutine to fade texture
            GameManager.Instance.playerChooseWeapon = weapons[currentWeaponIndex];
        }
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            PreviousWeapon();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            NextWeapon();
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            NextTraningScene();
        }
    }
    public void NextTraningScene()
    {
        SceneManager.LoadScene("TraningWeaponMode");
    }*/
    public void NextTraningScene()
    {
        SceneManager.LoadScene("TraningWeaponMode");
    }
    public void NextWeapon()
    {
        if (_WeaponType == WeaponType.Pistol)
        {
            _WeaponType = WeaponType.Bow;
            weaponNameText.text = _WeaponType.ToString();
            weaponRawImage.texture = bowtt;
        }
        else
        {
            _WeaponType = WeaponType.Pistol;
            weaponNameText.text = _WeaponType.ToString();
            weaponRawImage.texture = Pistoltt;

        }
        GameManager.Instance.playerChooseWeapon = _WeaponType;
    }

    public void PreviousWeapon()
    {
        if (_WeaponType == WeaponType.Bow)
        {
            _WeaponType = WeaponType.Pistol;
            weaponNameText.text = _WeaponType.ToString();
            weaponRawImage.texture = Pistoltt;



        }
        else
        {
            _WeaponType = WeaponType.Bow;
            weaponNameText.text = _WeaponType.ToString();
            weaponRawImage.texture = bowtt;

        }
        GameManager.Instance.playerChooseWeapon = _WeaponType;
    }

}
