using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WeaponSelectionUI : MonoBehaviour
{
    // Lớp chứa thông tin của mỗi nhân vật
    [System.Serializable]
    public class Character
    {
        public Sprite characterImage;
        public string characterName;
        [TextArea]
        public string characterDescription;
        public WeaponType weaponType;
    }

    // Danh sách các nhân vật
    public List<Character> characters = new List<Character>();

    // Chỉ số của nhân vật hiện tại
    private int currentCharacterIndex = 0;

    // Tham chiếu đến các thành phần UI
    public Image characterImageUI;
    public TextMeshProUGUI characterNameUI;
    public TextMeshProUGUI characterDescriptionUI;

    // Nút Next
    public Button nextButton;

    // **Thêm nút Start**
    public Button startButton;

    void Start()
    {
        // Kiểm tra xem danh sách nhân vật có rỗng không
        if (characters.Count == 0)
        {
            Debug.LogError("Danh sách nhân vật trống!");
            return;
        }

        // Hiển thị thông tin nhân vật đầu tiên
        UpdateCharacterUI();

        // Gắn sự kiện cho nút Next
        nextButton.onClick.AddListener(OnNextButtonClicked);

        // **Gắn sự kiện cho nút Start**
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    void UpdateCharacterUI()
    {
        // Lấy nhân vật hiện tại
        Character currentCharacter = characters[currentCharacterIndex];

        // Cập nhật các thành phần UI
        characterImageUI.sprite = currentCharacter.characterImage;
        characterNameUI.text = currentCharacter.characterName;
        characterDescriptionUI.text = currentCharacter.characterDescription;
    }

    void OnNextButtonClicked()
    {
        // Tăng chỉ số nhân vật hiện tại
        currentCharacterIndex++;

        // Kiểm tra nếu đã đến cuối danh sách thì quay lại đầu
        if (currentCharacterIndex >= characters.Count)
        {
            currentCharacterIndex = 0;
        }

        // Cập nhật UI
        UpdateCharacterUI();
    }

    // **Hàm xử lý khi nhấn nút Start**
    void OnStartButtonClicked()
    {
        // Lấy nhân vật hiện tại
        Character selectedCharacter = characters[currentCharacterIndex];

        // Gán weaponType của nhân vật được chọn vào GameManager.Instance.currentWeapon
        GameManager.Instance.playerChooseWeapon = selectedCharacter.weaponType;

        SceneManager.LoadScene("TraningWeaponMode");
    }
}
