using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PanelVisibleSwitcher : MonoBehaviour
{
    [SerializeField] private RectTransform[] _panels;

    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (_panels == null || _panels.Length == 0)
            return;

        foreach (var panel in _panels)
            panel.gameObject.SetActive(!panel.gameObject.activeSelf);
    }
}
