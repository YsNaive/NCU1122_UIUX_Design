using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public sealed class RSAlign : RSStyleComponent<RSAlign>
    {
        public const int F_AlignSelf      = 1 << 0;
        public const int F_AlignItems     = 1 << 1;
        public const int F_JustifyContent = 1 << 2;
        public const int F_Any = F_AlignSelf | F_AlignItems | F_JustifyContent;

        [SerializeField] private Align   m_AlignSelf      = Align.Auto;
        [SerializeField] private Align   m_AlignItems     = Align.Stretch;
        [SerializeField] private Justify m_JustifyContent = Justify.FlexStart;
        #region properties get set
        public Align alignSelf
        {
            get => m_AlignSelf;
            set { 
                m_AlignSelf = value; 
                m_flag |= F_AlignSelf; 
            }
        }
        public Align alignItems
        {
            get => m_AlignItems;
            set
            {
                m_AlignItems = value;
                m_flag |= F_AlignItems;
            }
        }
        public Justify justifyContent
        {
            get => m_JustifyContent;
            set
            {
                m_JustifyContent = value;
                m_flag |= F_JustifyContent;
            }
        }
        #endregion

        public override RSStyleFlag StyleFlag => RSStyleFlag.Align;
        public override int PropertyCount => 3;

        public override void SetValueToDefault(int flag)
        {
            if ((flag & F_AlignSelf) == F_AlignSelf) m_AlignSelf = Align.Auto;
            if ((flag & F_AlignItems) == F_AlignItems) m_AlignItems = Align.Stretch;
            if ((flag & F_JustifyContent) == F_JustifyContent) m_JustifyContent = Justify.FlexStart;
        }
        public override void ApplyOn(IStyle style)
        {
            if (GetFlag(F_AlignSelf))      style.alignSelf      = m_AlignSelf;
            if (GetFlag(F_AlignItems))     style.alignItems     = m_AlignItems;
            if (GetFlag(F_JustifyContent)) style.justifyContent = m_JustifyContent;
        }
        public override void LoadFrom(IStyle style)
        {
            m_flag = F_Any;
            m_AlignSelf      = style.alignSelf.value;
            m_AlignItems     = style.alignItems.value;
            m_JustifyContent = style.justifyContent.value;
        }
        public override void LoadFrom(RSAlign other)
        {                
            m_flag           = other.m_flag;
            m_AlignSelf      = other.m_AlignSelf;
            m_AlignItems     = other.m_AlignItems;
            m_JustifyContent = other.m_JustifyContent;
        }
        public override void LoadFromIfUnset(RSAlign other)
        {
            if (!GetFlag(F_AlignSelf)      && other.GetFlag(F_AlignSelf))      m_AlignSelf      = other.m_AlignSelf;
            if (!GetFlag(F_AlignItems)     && other.GetFlag(F_AlignItems))     m_AlignItems     = other.m_AlignItems;
            if (!GetFlag(F_JustifyContent) && other.GetFlag(F_JustifyContent)) m_JustifyContent = other.m_JustifyContent;
            m_flag |= other.SetUnsetFlag;
        }
        public override void LoadFromLerp(RSAlign begin, RSAlign end, float rate)
        {
            int flag = begin.m_flag & end.m_flag;
            var loadTarget = rate >= 1f ? end : begin;

            int temp = loadTarget.m_flag;
            loadTarget.m_flag = flag;
            LoadFrom(loadTarget);
            loadTarget.m_flag = temp;
        }

    }
}
