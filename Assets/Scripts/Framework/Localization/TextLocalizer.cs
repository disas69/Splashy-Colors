using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Localization
{
    public enum TextLocalizerMode
    {
        NoMode,
        ToUpper,
        ToLower
    }

    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextLocalizer : MonoBehaviour
    {
        private TextMeshProUGUI _textComponent;

        [HideInInspector] public string Key;
        public TextLocalizerMode Mode = TextLocalizerMode.NoMode;

        private void Start()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();

            LocalizationManager.LanguageChanged += Localize;
            Localize();
        }

        private void Localize()
        {
            if (_textComponent != null)
            {
                _textComponent.text = GetString(Key);
            }
        }

        private string GetString(string key)
        {
            var result = LocalizationManager.GetString(key);

            switch (Mode)
            {
                case TextLocalizerMode.ToUpper:
                    result = result.ToUpper();
                    break;
                case TextLocalizerMode.ToLower:
                    result = result.ToLower();
                    break;
            }

            return result;
        }

        private void OnDestroy()
        {
            LocalizationManager.LanguageChanged -= Localize;
        }
    }
}