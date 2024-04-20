namespace NaiveAPI.UITK
{
    public class RSLocalizationLanguageDrawer : StringDropdownDrawer
    {
        public bool LinkSettings
        {
            get => m_LinkSettings;
            set
            {
                if (m_LinkSettings != value)
                {
                    m_LinkSettings = value;
                    if (value)
                        OnValueChanged += applyOnSetting;
                    else
                        OnValueChanged -= applyOnSetting;
                }
            }
        }
        private bool m_LinkSettings = false;

        public RSLocalizationLanguageDrawer()
        {
            SetChoices(RSLocalization.LanguageKeys);
            value = RSLocalization.CurrentLanguage;
        }

        void applyOnSetting()
        {
            RSLocalization.CurrentLanguage = value;
        }
    }
}
