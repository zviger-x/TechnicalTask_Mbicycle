using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAudioPlayer : AudioPlayer, IPointerDownHandler
{
    private Button _button;

    private void OnValidate()
    {
        _type = AudioType.Sound;
    }

    private new void Start()
    {
        base.Start();

        _button = GetComponent<Button>();
        _type = AudioType.Sound;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_button.interactable)
            return;

        Play();
    }
}
