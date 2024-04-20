using NaiveAPI.UITK;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.Sample
{
    internal class UIElementExtensionSample : MonoBehaviour
    {
        public UIDocument UID;
        public bool PlayTransition = false;
        public RSAnimationMode AnimationMode = RSAnimationMode.Forward;

        [Header("Run & Modify Style in PlayMode")]
        public RSStyle RSStyle = new();
        [Header("Transistion to ")]
        public RSStyle TransistionStyle = new();
        TextElement previewElement;
        RSTransition RSTransition;

        RSTransitionPlayer player;
        void Start()
        {
            TextField tf = new TextField();
            tf.RegisterValueChangedCallback(evt => { });
            previewElement = new TextElement();
            previewElement.text = "Text Element";
            UID.rootVisualElement.Add(previewElement);
            previewElement.style.SetRS_Style(RSStyle);

            RSTransition = new RSTransition();
            RSTransition.AddTransition(0, RSStyle);
            RSTransition.AddTransition(0.75f, TransistionStyle);
            player = RSTransitionPlayer.CreateByReference(RSTransition);
        }
        void Update()
        {
            //if (PlayTransition)
            //{
            //    PlayTransition = false;
            //    player.Stop();
            //    player.Mode = AnimationMode;
            //    player.MakeSchedule(previewElement, previewElement);
            //}
            previewElement.style.SetRS_Style(RSStyle);
            previewElement.MarkDirtyRepaint();
        }
    }

}