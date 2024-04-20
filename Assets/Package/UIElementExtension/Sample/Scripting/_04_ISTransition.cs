using UnityEngine.UIElements;

namespace NaiveAPI.UITK.Sample
{
    /* #1 Intro */
    /*--------------------------------------------------------------------------------|
    | RSTransition can help you to make simple animation for VisualElement in Runtime |
    | Preset in Editor is not in plan now, but may implement in future update.        |
    |--------------------------------------------------------------------------------*/
    public class _04_RSTransition
    {
        /*---------------------------------------------------------------|
        | If you really need to, you can modify it. But in normal,       |
        | you don't need to change most of them while using RSTransition |
        |---------------------------------------------------------------*/
        void BasicProperties()
        {
            RSTransition transition = new RSTransition();
            // how transition play
            // - Forward
            // - Backward
            // - PingPong
            // - Loop
            _ = transition.AnimationMode;

            // keyFrame as RSStyle List
            _ = transition.Properties;

            // keyFrame durations as float List
            // the first item will ignore
            _ = transition.Durations;
        }

        VisualElement visualElement;
        void Usage()
        {
            RSTransition transition = new();

            // Add keyframe
            // Durations for 1st state will not affact on result
            // So you can ignore it
            transition.AddTransition(0, new RSStyle() { Transform = new RSTransform { x = 0, y = 0 } });
            // than create our transition, transition below make element move like a rect.
            // and run from begin to end in 4 sec.
            transition.AddTransition(1, new RSStyle() { Transform = new RSTransform { x = 100, y = 0 } });
            transition.AddTransition(1, new RSStyle() { Transform = new RSTransform { x = 100, y = 100 } });
            transition.AddTransition(1, new RSStyle() { Transform = new RSTransform { x = 0, y = 100 } });
            // close path by add transition to 1st state
            transition.AddTransition(1, transition.Properties[0]);


            // Than set play mode you want
            transition.AnimationMode = RSAnimationMode.Loop;


            // Finally we can make a player
            // You can create it by Ref or Copy
            // Ref use data from src of transition
            // Copy will copy all info from transition

            /*----------------------  Importent  -------------------------|
            | Remember to valid its keyframe before create player         |
            | If you want to know why we need this, please check pdf docs |
            |------------------------------------------------------------*/
            transition.ValidKeyframe();

            var refPlayer = transition.MakePlayerByReference(visualElement);
            var newPlayer = transition.MakePlayerByCopy(visualElement);

            // for example, modify below will affect on "refPlayer" but not "newPlayer"
            transition.Properties[0].Transform.x = -50;
            transition.Properties[0].Transform.y = -50;

            // than start playing
            refPlayer.Start();
            // other operate
            refPlayer.Pause();
            refPlayer.Resume();
            refPlayer.Mode = RSAnimationMode.PingPong;
            // you can bind it to other element;
            refPlayer.MakeSchedule(visualElement);
            // delete scheduled itme
            refPlayer.Unschedule();

            // you can check if a Transition is playing or not
            _ = refPlayer.IsPlaying;
        }
    }
}
