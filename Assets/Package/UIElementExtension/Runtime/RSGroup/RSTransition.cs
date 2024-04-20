using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public enum RSAnimationMode
    {
        Forward,
        Backward,
        Loop,
        PingPong,
    }

    public class RSTransition
    {
        public RSAnimationMode AnimationMode = RSAnimationMode.Forward;
        public List<float> Durations = new();
        public List<RSStyle> Properties = new();

        public void AddTransition(float durationSec, RSStyle transTarget)
        {
            Durations.Add(durationSec);
            Properties.Add(transTarget);
        }
        public void AddWait(float durationSec)
        {
            if (Properties.Count == 0)
                throw new System.Exception("You need to add at least 1 transition before add wait");
            Durations.Add(durationSec);
            Properties.Add(Properties[Properties.Count - 1]);
        }

        public RSTransitionPlayer MakePlayerByReference(VisualElement element)
        {
            var player = RSTransitionPlayer.CreateByReference(this);
            player.MakeSchedule(element, element);
            return player;
        }
        public RSTransitionPlayer MakePlayerByCopy(VisualElement element)
        {
            var player = RSTransitionPlayer.CreateByCopy(this);
            player.MakeSchedule(element, element);
            return player;
        }
        public void ValidKeyframe()
        {
            int enable = 0;
            foreach (var style in Properties)
                enable |= style.SetUnsetFlag;
            foreach (var style in Properties)
                style.SetUnsetFlag = enable;
            var first = Properties[0];
            RSStyleComponent[] toValid = new RSStyleComponent[Properties.Count];
            foreach(var component in first.VisitActiveStyle())
            {
                toValid[0] = component;
                int validFlag = component.SetUnsetFlag;
                for (int i = 1; i < Properties.Count; i++)
                {
                    toValid[i] = Properties[i].GetStyleComponent(component.StyleFlag);
                    validFlag |= Properties[i].SetUnsetFlag;
                }

                // valid first
                for(int i=1; i < toValid.Length; i++)
                {
                    if (toValid[0].SetUnsetFlag == validFlag)
                        break;
                    toValid[0].LoadFromIfUnset(toValid[i]);
                }
                // valid end
                for (int i = toValid.Length - 2; i >= 0; i--) 
                {
                    if (toValid[toValid.Length - 1].SetUnsetFlag == validFlag)
                        break;
                    toValid[toValid.Length-1].LoadFromIfUnset(toValid[i]);
                }

                for (int i = 1, imax = toValid.Length - 1; i < imax; i++)
                {
                    for(int j = i + 1; j < toValid.Length; j++)
                    {
                        if (toValid[i].SetUnsetFlag == validFlag)
                            break;
                        if (toValid[i] == toValid[j])
                            continue;
                        var rate = Durations[i];
                        var sum = rate;
                        for (int k = i + 1; k <= j; k++)
                            sum += Durations[k];
                        rate /= sum;
                        toValid[i].LoadFromLerpIfUnset(toValid[i-1], toValid[j], rate);
                        //Debug.Log(j+" "+(toValid[j] as RSSize).height);
                    }
                }
            }
        }
    }

    public class RSTransitionPlayer
    {
        private RSTransitionPlayer() { }
        public static RSTransitionPlayer CreateByReference(RSTransition src)
        {
            RSTransitionPlayer ret = new RSTransitionPlayer();
            ret.Mode = src.AnimationMode;
            ret.Durations = src.Durations;
            ret.Properties = src.Properties;
            return ret;
        }
        public static RSTransitionPlayer CreateByCopy(RSTransition src)
        {
            RSTransitionPlayer ret = new RSTransitionPlayer();
            ret.Mode = src.AnimationMode;
            ret.Durations = new List<float>(src.Durations);
            ret.Properties = new();
            foreach (var prop in src.Properties)
                ret.Properties.Add(RSStyle.CreateFrom(prop));
            return ret;
        }

        public event Action OnTransitionEnd;
        public bool IsPlaying => !isEnd;
        public RSAnimationMode Mode;
        private IVisualElementScheduledItem item;
        private List<float> Durations;
        private List<RSStyle> Properties;
        private int currentTo = -1;
        private float currentDuration;
        private float beginTime;
        private float lastTime;
        private bool isForward;
        private bool isEnd;
        private RSStyle from, to;

        public void Pause()
        {
            item?.Pause();
        }
        public void Resume()
        {
            lastTime = Time.time;
            item.Every(20);
        }
        public void Start()
        {
            getInit();
            item.Every(20);
        }
        public void Unschedule()
        {
            item?.Pause();
            item = null;
        }
        public void MakeSchedule(VisualElement affectElement) { MakeSchedule(affectElement, affectElement); }
        public void MakeSchedule(VisualElement scheduledOn, VisualElement affectElement)
        {
            isForward = Mode != RSAnimationMode.Backward;
            getInit();
            item = scheduledOn.schedule.Execute(() =>
            {
                if(Time.time - beginTime > currentDuration)
                {
                    getNext();
                }
                if (isEnd)
                {
                    to.ApplyOn(affectElement);
                    item.Pause();
                    OnTransitionEnd?.Invoke();
                    return;
                }
                var rate = (Time.time - beginTime) / currentDuration;
                from.ApplyTransitionOn(affectElement.style, to, rate);
            });
        }
        private void getInit()
        {
            if (Properties.Count < 2)
                throw new Exception("RSTransition required at leaset 2 Properties to play.");

            beginTime = Time.time;
            isEnd = false;
            if (isForward)
            {
                from = Properties[0];
                currentTo = 1;
                to = Properties[currentTo];
                currentDuration = Durations[currentTo];
            }
            else
            {
                currentTo = Properties.Count - 2;
                from = Properties[currentTo + 1];
                to = Properties[currentTo];
                currentDuration = Durations[currentTo + 1];
            }
        }
        private void getNext()
        {
            if (Properties.Count < 2)
                throw new Exception("RSTransition required at leaset 2 Properties to play.");
            if(isForward)
            {
                if(currentTo < Properties.Count - 1)
                {
                    from = Properties[currentTo];
                    currentTo++;
                    to = Properties[currentTo];
                    currentDuration = Durations[currentTo];
                }
                else
                {
                    isEnd = true;
                }
            }
            else
            {
                if (currentTo > 0)
                {
                    from = Properties[currentTo];
                    currentTo--;
                    to = Properties[currentTo];
                    currentDuration = Durations[currentTo + 1];
                }
                else
                {
                    isEnd = true;
                }
            }
            if(isEnd && Mode == RSAnimationMode.Loop) { getInit(); }
            if(isEnd && Mode == RSAnimationMode.PingPong) { isForward = !isForward; getInit(); }
        }
    }
}