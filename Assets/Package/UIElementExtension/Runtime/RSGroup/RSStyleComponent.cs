using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public abstract class RSStyleComponent
    {
        public abstract RSStyleFlag StyleFlag { get; }
        public abstract int PropertyCount { get; }

        [SerializeField]
        protected int m_flag = 0;
        /// <summary>
        /// <b>Don't modify if you dont know what is this for.</b>
        /// </summary>
        public virtual int SetUnsetFlag
        {
            get => m_flag;
            set => m_flag = value & (int)(uint.MaxValue >> (32 - PropertyCount));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void SetAll() { SetFlag(-1, true); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void UnsetAll() { SetUnsetFlag = 0; }
        public abstract void SetValueToDefault(int flag);
        public void SetAllValueToDefault() { SetValueToDefault(-1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool GetFlag(int mask) { return (m_flag & mask) == mask; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool GetFlag(int mask, int flag) { return (flag & mask) == mask; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool GetFlag(int mask, int flag, int flag2) { return (flag & flag2 & mask) == mask; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlag(int mask, bool value)
        {
            if (value)
                m_flag |= mask;
            else
                m_flag &= ~mask;
            SetUnsetFlag = m_flag;
        }
        public void ApplyOn(VisualElement element) { ApplyOn(element.style); }
        public abstract void ApplyOn(IStyle style);
        public void LoadFrom(VisualElement element) { LoadFrom(element.style); }
        public abstract void LoadFrom(IStyle style);
        public abstract void LoadFrom(RSStyleComponent other);
        public abstract void LoadFromIfUnset(RSStyleComponent other);
        public abstract void LoadFromLerp(RSStyleComponent begin, RSStyleComponent end , float rate);
        public abstract void LoadFromLerpIfUnset(RSStyleComponent begin, RSStyleComponent end , float rate);
    }
    [System.Serializable]
    public abstract class RSStyleComponent<T> : RSStyleComponent
        where T : RSStyleComponent<T>, new()
    {
        public readonly static T Default = new() { SetUnsetFlag = -1 };
        public readonly static T op_temp = new() { SetUnsetFlag = -1};
        public static T CreateFrom(VisualElement element) { return CreateFrom(element.style); }
        public static T CreateFrom(IStyle style) { var ret = new T(); ret.LoadFrom(style); return ret; }
        public static T CreateFrom(T other) { var ret = new T(); ret.LoadFrom(other); return ret; }
        public override void LoadFromLerp(RSStyleComponent begin, RSStyleComponent end, float rate) { LoadFromLerp((T)begin, (T)end, rate); }
        public abstract void LoadFromLerp(T begin, T end, float rate);
        public abstract void LoadFrom(T other);
        public override void LoadFrom(RSStyleComponent other)
        {
            var cast = other as T;
            if(cast != null)
                LoadFrom(cast);
        }
        public void LoadFromIfUnset(VisualElement other) { op_temp.LoadFrom(other); LoadFromIfUnset(op_temp); }
        public void LoadFromIfUnset(IStyle other) { op_temp.LoadFrom(other); LoadFromIfUnset(op_temp); }
        public override void LoadFromIfUnset(RSStyleComponent other) { op_temp.LoadFrom(other); LoadFromIfUnset(op_temp); }
        public override void LoadFromLerpIfUnset(RSStyleComponent begin, RSStyleComponent end, float rate) { op_temp.LoadFromLerp(begin, end, rate); LoadFromIfUnset(op_temp);}
        public abstract void LoadFromIfUnset(T other);
        public T DeepCopy() { T ret = new(); ret.LoadFrom((T)this); return ret; }
        /// <summary>
        /// <br>Make the style of Element = thisStyle*(1-rate) + targetStyle*rate</br>
        /// <br>both self and target should be "set" to active property</br>
        /// </summary>
        public void ApplyTransitionOn(IStyle style, T transTarget, float rate)
        {
            op_temp.LoadFromLerp((T)this, transTarget, rate);
            op_temp.ApplyOn(style);
        }
    }
}
