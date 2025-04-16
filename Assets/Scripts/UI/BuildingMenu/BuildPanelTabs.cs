using UnityEngine;
using UnityEngine.UI;

public class BuildPanelTabs : MonoBehaviour
{
    [SerializeField] private Button constructionTabButton;
    [SerializeField] private Button bonusTabButton;
    [SerializeField] private GameObject constructionContainer;
    [SerializeField] private GameObject bonusContainer;

    private void Start()
    {
        constructionTabButton.onClick.AddListener(ShowConstruction);
        bonusTabButton.onClick.AddListener(ShowBonuses);

        ShowConstruction(); // Démarre sur l'onglet construction
    }

    private void ShowConstruction()
    {
        constructionContainer.SetActive(true);
        bonusContainer.SetActive(false);
    }

    private void ShowBonuses()
    {
        constructionContainer.SetActive(false);
        bonusContainer.SetActive(true);
    }
}
