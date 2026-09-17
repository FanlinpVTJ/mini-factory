using MiniFactory.Production;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ValueSystem.Misc;
using Zenject;

namespace MiniFactory.UI
{
    public sealed class OfflineIncomeView : MonoBehaviour
    {
        [SerializeField, Tooltip("Child panel; keep this component on an active object outside the panel.")]
        private GameObject _panel;
        [SerializeField] private TMP_Text _incomeText;
        [SerializeField] private Button _closeButton;

        private long _displayedIncomeVersion;

        [Inject] private FactoryProduction _production;

        private void Awake()
        {
            _panel.SetActive(false);
        }

        private void OnEnable()
        {
            _production.OnOfflineIncomeApplied += ShowIncome;
            _closeButton.onClick.AddListener(Close);
            ShowPendingIncome();
        }

        private void Start()
        {
            ShowPendingIncome();
        }

        private void OnDisable()
        {
            _production.OnOfflineIncomeApplied -= ShowIncome;
            _closeButton.onClick.RemoveListener(Close);
        }

        private void ShowPendingIncome()
        {
            if (_production.OfflineIncomeVersion > _displayedIncomeVersion && _production.LastOfflineIncome > 0)
            {
                ShowIncome(_production.LastOfflineIncome);
            }
        }

        private void ShowIncome(double income)
        {
            _displayedIncomeVersion = _production.OfflineIncomeVersion;
            string amount = income < 1 ? income.ToString("G3") : TextFormatter.FormatNumber((float)income);
            _incomeText.text = "+" + amount;
            _panel.SetActive(true);
        }

        private void Close()
        {
            _panel.SetActive(false);
        }
    }
}
