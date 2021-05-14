using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    [SerializeField]
    RectTransform uiHandleTransform;
    [SerializeField] 
    Color handleActiveColor;

    Color handleDefaultColor;

    Image handleImage;

    Toggle toggle;

    Vector2 handlePosition;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.isOn = Configurations.quizMode;

        handlePosition = uiHandleTransform.anchoredPosition;
        handleImage = uiHandleTransform.GetComponent<Image>();

        handleDefaultColor = handleImage.color;

        toggle.onValueChanged.AddListener(OnSwitch);

        if (toggle.isOn)
        {
            OnSwitch(true);
        } else
        {
            OnSwitch(false);
        }
    }

    public void OnSwitch(bool on)
    {
        if (on)
        {
            uiHandleTransform.anchoredPosition = handlePosition * -1;
            handleImage.color = handleActiveColor;
            Configurations.quizMode = true;
        } else
        {
            uiHandleTransform.anchoredPosition = handlePosition;
            handleImage.color = handleDefaultColor;
            Configurations.quizMode = false;
        }
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}
