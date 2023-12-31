using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

[RequireComponent(typeof(CodelessIAPButton))]
public class RemoveADSButton : MonoBehaviour
{
    private CodelessIAPButton _button;

    private void Start()
    {
        var data = GameDataIO.LoadData();

        if (!data.ShowAds)
            Destroy(gameObject);

        _button = GetComponent<CodelessIAPButton>();
        _button.onPurchaseComplete.AddListener(RemoveAds);
    }

    private void OnDestroy()
    {
        _button.onPurchaseComplete.RemoveListener(RemoveAds);
    }

    private void RemoveAds(Product product)
    {
        var data = GameDataIO.LoadData();
        data.ShowAds = false;
        GameDataIO.SaveData(data);
        gameObject.GetComponent<Button>().interactable = false;
    }
}
