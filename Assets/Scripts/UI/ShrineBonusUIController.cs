using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShrineBonusUIController : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform availableContainer;
    [SerializeField] private Transform activatedContainer;
    [SerializeField] private Button tabAvailableButton;
    [SerializeField] private Button tabActivatedButton;

    [SerializeField] private GameObject availablePanel;
    [SerializeField] private GameObject activatedPanel;

    private void Awake()
    {
        tabAvailableButton.onClick.AddListener(ShowAvailable);
        tabActivatedButton.onClick.AddListener(ShowActivated);
    }

    private void Start()
    {
        RefreshUI();
    }

    private void OnEnable()
    {
        
        ShrineBonusManager.OnBonusListChanged += RefreshUI;
        ResourceManager.OnResourceChanged += HandleResourceChanged;
    }

    private void OnDisable()
    {
        ShrineBonusManager.OnBonusListChanged -= RefreshUI;
        ResourceManager.OnResourceChanged -= HandleResourceChanged;
    }

    private void HandleResourceChanged(ResourceType type, int amount)
    {
        RefreshUI();
    }

    private void ShowAvailable()
    {
        availablePanel.SetActive(true);
        activatedPanel.SetActive(false);
    }

    private void ShowActivated()
    {
        availablePanel.SetActive(false);
        activatedPanel.SetActive(true);
    }

    public void RefreshUI()
    {
        // Clear
        foreach (Transform child in availableContainer)
            Destroy(child.gameObject);

        foreach (Transform child in activatedContainer)
            Destroy(child.gameObject);

        // Available bonuses
        foreach (var bonus in ShrineBonusManager.Instance.GetAvailableBonuses())
        {
            GameObject slot = Instantiate(slotPrefab, availableContainer);
            var text = slot.GetComponentInChildren<TMP_Text>();
            text.text = $"{bonus.bonusName} - {bonus.description}";

            var button = slot.GetComponentInChildren<Button>();
            button.interactable = ShrineBonusManager.Instance.CanActivate(bonus);
            button.onClick.AddListener(() => {
                ShrineBonusManager.Instance.ActivateBonus(bonus);
                RefreshUI();
            });
        }

        // Activated bonuses
        foreach (var bonus in ShrineBonusManager.Instance.GetActivatedBonuses())
        {
            GameObject slot = Instantiate(slotPrefab, activatedContainer);
            var text = slot.GetComponentInChildren<TMP_Text>();
            text.text = $"{bonus.bonusName} - ACTIVÉ";
        }
    }
}
