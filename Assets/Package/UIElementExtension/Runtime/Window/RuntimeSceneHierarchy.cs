using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RuntimeSceneHierarchy : RSRuntimeWindow
    {
        public static event Action<GameObject> OnSelectObject;
        RSScrollView scrollView;
        Scene currentScene;
        public RuntimeSceneHierarchy()
        {
            scrollView = new RSScrollView();
            Add(scrollView);
            SceneManager.activeSceneChanged += (_, scene) => { currentScene = scene; UpdateHierarchy(); };
            currentScene = SceneManager.GetActiveScene();
            schedule.Execute(UpdateHierarchy).Every(500);
            TabName = "Scene Hierarchy";
        }
        public void UpdateHierarchy()
        {
            foreach(var obj in currentScene.GetRootGameObjects())
            {
                if (!scrollView.Children().Cast<MenuItem>().Any(MenuItem => MenuItem.TargetObject == obj))
                    scrollView.Add(new MenuItem(obj));
            }
        }
        class MenuItem : FoldoutDrawer<GameObject>
        {
            public GameObject TargetObject;
            public void solveLayout()
            {
                if (childCount == 0)
                {
                    LayoutInline();
                    iconElement.style.opacity = 0;
                    UnregisterFoldoutStateCallback(iconElement);
                }
                else
                {
                    LayoutExpand();
                    labelElement.style.backgroundColor = Color.clear;
                    RegisterFoldoutStateCallback(iconElement);
                    UnregisterFoldoutStateCallback(labelElement);
                }
            }
            public MenuItem(GameObject targetObject)
                : base()
            {
                TargetObject = targetObject;
                label = targetObject.name;
                foreach (Transform transform in targetObject.transform)
                    Add(new MenuItem(transform.gameObject));
                solveLayout();
                labelElement.RegisterCallback<PointerEnterEvent>(evt =>
                {
                    labelElement.style.backgroundColor = RSTheme.Current.BackgroundColor2;
                });
                labelElement.RegisterCallback<PointerDownEvent>(evt =>
                {
                    OnSelectObject?.Invoke(TargetObject);
                });
                labelElement.RegisterCallback<PointerLeaveEvent>(evt =>
                {
                    labelElement.style.backgroundColor = Color.clear;
                });

                schedule.Execute(() =>
                {
                    if (TargetObject == null)
                    {
                        parent?.schedule.Execute((parent as MenuItem).solveLayout).ExecuteLater(1);
                        parent?.Remove(this);
                        return;
                    }
                    foreach (Transform transform in targetObject.transform)
                    {
                        if (!Children().Cast<MenuItem>().Any(MenuItem => MenuItem.TargetObject == transform.gameObject))
                        {
                            Add(new MenuItem(transform.gameObject));
                            solveLayout();
                        }
                    }
                }).Every(500);
            }

            protected override void CreateGUI()
            {
            }
            public override void RepaintDrawer()
            {
            }

        }
    }
}