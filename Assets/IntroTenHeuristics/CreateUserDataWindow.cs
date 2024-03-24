using NaiveAPI.DocumentBuilder;
using NaiveAPI.RuntimeWindowUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateNewUserWindow : DSRuntimeWindow
{
    public Action OnSaveOrClear;
    public bool IsEditing = false;
    public UserData UserData => UserDataDrawer.value;
    private UserData originalData;
    private bool dataChange = false;

    public UserDataDrawer UserDataDrawer => userDataDrawer;
    private UserDataDrawer userDataDrawer;

    private DSTextElement statusElement;
    private DSButton btnCancel, btnPrev, btnNext;

    private string[] progressNames = { "基本資料", "聯絡資料", "特質資料" };
    private List<DSTextElement> progresses = new List<DSTextElement>();

    public CreateNewUserWindow()
    {
        PopupOnClick = false;
        Resizable = false;

        VisualElement progressBar = new VisualElement();
        progressBar.style.flexDirection = FlexDirection.Row;

        Texture2D texture = Resources.Load<Texture2D>("Image/arrow");

        float fontSize = DocStyle.Current.MainTextSize * 1.5f;
        float paddingSize = DocStyle.Current.MainTextSize * 1.8f;

        statusElement = new DSTextElement("正在新增使用者");
        statusElement.style.unityTextAlign = TextAnchor.MiddleCenter;
        statusElement.style.fontSize = fontSize;
        statusElement.style.left = 50;
        statusElement.style.right = StyleKeyword.Auto;
        statusElement.style.marginRight = StyleKeyword.Auto;

        progressBar.Add(statusElement);

        for (int i = 0;i < progressNames.Length; i++)
        {
            DSTextElement textElement = new DSTextElement(progressNames[i]);

            textElement.style.backgroundImage = texture;
            textElement.style.unityBackgroundImageTintColor = DocStyle.Current.SubBackgroundColor;
            textElement.style.fontSize = fontSize;
            textElement.style.paddingTop = paddingSize / 2f;
            textElement.style.paddingBottom = paddingSize / 2f;
            textElement.style.paddingLeft = paddingSize;
            textElement.style.paddingRight = paddingSize;

            textElement.RegisterCallback<PointerEnterEvent>((evt) =>
            {
                if (textElement != progresses[userDataDrawer.PageIndex])
                {
                    textElement.style.unityBackgroundImageTintColor = setColor(textElement.style.unityBackgroundImageTintColor.value);
                }
            });

            textElement.RegisterCallback<PointerDownEvent>((evt) =>
            {
                int index = progresses.IndexOf(textElement);

                userDataDrawer.SetPageIndex(index);

                highlightCurrentPage();

                btnPrev.visible = !userDataDrawer.IsFirstPage();

                btnNext.text = userDataDrawer.IsLastPage() ? "完成" : "下一頁";
            });

            textElement.RegisterCallback<PointerLeaveEvent>((evt) =>
            {
                if (textElement != progresses[userDataDrawer.PageIndex])
                {
                    textElement.style.unityBackgroundImageTintColor = DocStyle.Current.SubBackgroundColor;
                }
            });

            progresses.Add(textElement);
            progressBar.Add(textElement);
        }

        progresses[0].style.unityBackgroundImageTintColor = DocStyle.Current.SubFrontgroundColor;
        progresses[0].style.color = DocStyle.Current.BackgroundColor;

        progressBar.style.justifyContent = Justify.Center;
        progressBar.style.paddingTop = 10;

        Add(progressBar);

        userDataDrawer = new UserDataDrawer();
        userDataDrawer.value ??= new UserData();
        userDataDrawer.style.paddingTop = 50;
        userDataDrawer.OnUserDataChanged += (data) =>
        {
            if (IsEditing)
            {
                dataChange = !data.Equals(originalData);
                btnCancel.text = dataChange ? "捨棄變更" : "取消編輯";
            }
        };
        //var sc = new DSScrollView();
        //sc.style.flexGrow = 1;
        //sc.contentContainer.style.flexGrow = 1;
        Add(userDataDrawer);

        VisualElement controlPanel = new VisualElement();
        controlPanel.style.flexDirection = FlexDirection.Row;

        btnPrev = new DSButton("上一頁", DocStyle.Current.SuccessColor, () => toPrevPage());

        btnNext = new DSButton("下一頁", DocStyle.Current.SuccessColor, () => toNextPageOrFinish());

        btnCancel = new DSButton("清除", DocStyle.Current.DangerColor, () => {
            if (!IsEditing)
            {
                DoubleCheckWindow.Open(new Rect(20, 30, 60, 40), "該動作會清除所有更改，確定要刪除嗎？", (confirm) =>
                {
                    if (confirm)
                        userDataDrawer.Reset();
                });
            }
            else
            {
                if (dataChange)
                {
                    DoubleCheckWindow.Open(new Rect(20, 30, 60, 40), "是否要捨棄所有變更，回到編輯前的資料？", (confirm) =>
                    {
                        if (confirm)
                            userDataDrawer.Reset(new UserData(originalData));
                    });
                }
                else
                {
                    OnSaveOrClear?.Invoke();
                    Destory();
                }
            }
        });

        btnCancel.style.marginRight = 50;
        btnPrev.style.marginLeft = 50;
        btnPrev.style.marginRight = 50;
        btnNext.style.marginLeft = 50;
        btnCancel.style.width = Length.Percent(20);
        btnPrev.style.width = Length.Percent(20);
        btnNext.style.width = Length.Percent(20);

        btnPrev.visible = false;

        //btnSave.style.marginRight = 50;
        //btnCancel.style.marginLeft = 50;
        //btnSave.style.width = Length.Percent(20);
        //btnCancel.style.width = Length.Percent(20);

        controlPanel.Add(btnCancel);
        controlPanel.Add(btnPrev);
        controlPanel.Add(btnNext);
        //visualElement.Add(btnCancel);

        controlPanel.style.justifyContent = Justify.FlexEnd;
        controlPanel.style.top = StyleKeyword.Auto;
        controlPanel.style.bottom = 0;
        controlPanel.style.marginTop = StyleKeyword.Auto;

        Add(controlPanel);
        //Add(sc);
    }

    private Color setColor(Color color)
    {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        v += (v > 0.5f) ? -0.1f : 0.055f;
        return Color.HSVToRGB(h, s, v);
    }

    private void toNextPageOrFinish()
    {
        if (userDataDrawer.IsLastPage())
        {
            saveData();
        }
        else if(userDataDrawer.IsPageValid())
        {
            btnPrev.visible = true;

            userDataDrawer.NextPage();
            if (userDataDrawer.IsLastPage())
            {
                btnNext.text = "完成";
            }

            highlightCurrentPage();
        }
        else
        {
            userDataDrawer.ShowPageInvalidMessage();
        }
    }

    private void saveData()
    {

        if (userDataDrawer.IsAllValid())
        {
            if (IsEditing && UserData.Name != originalData.Name)
            {
                DataHandler.DeleteData(DataHandler.UserDataDir, originalData.Name + ".json");
                DataHandler.SaveData(DataHandler.UserDataDir, UserData.Name + ".json", JsonUtility.ToJson(UserData), false);
            }
            else
            {
                DataHandler.SaveData(DataHandler.UserDataDir, UserData.Name + ".json", JsonUtility.ToJson(UserData), IsEditing);
            }

            UserDataHandler.LoadAll();
            OnSaveOrClear?.Invoke();
            Destory();
        }
        else
        {
            userDataDrawer.ShowAllInvalidMessage();
        }
    }

    private void highlightCurrentPage()
    {
        foreach (TextElement progress in progresses)
        {
            progress.style.unityBackgroundImageTintColor = DocStyle.Current.SubBackgroundColor;
            progress.style.color = DocStyle.Current.MainText.Color;
        }

        int index = userDataDrawer.PageIndex;

        progresses[index].style.unityBackgroundImageTintColor = DocStyle.Current.SubFrontgroundColor;
        progresses[index].style.color = DocStyle.Current.BackgroundColor;
    }

    private void toPrevPage()
    {
        if (userDataDrawer.IsLastPage())
        {
            btnNext.text = "下一頁";
        }

        userDataDrawer.PrevPage();

        highlightCurrentPage();

        if (userDataDrawer.IsFirstPage())
        {
            btnPrev.visible = false;
        }
    }

    public static CreateNewUserWindow Open(Action finish, UserData editData = null)
    {
        CreateNewUserWindow window = GetWindow<CreateNewUserWindow>();

        window.EnableTab = false;
        window.OnSaveOrClear = finish;

        window.style.width = Length.Percent(100);
        window.style.height = Length.Percent(100);

        if (editData != null)
        {
            window.IsEditing = true;

            UserData copy = new UserData(editData);
            window.originalData = editData;
            window.UserDataDrawer.value = copy;
            window.UserDataDrawer.ShowAllInvalidMessage();
            window.statusElement.text = $"正在編輯 '{copy.Name}'";
            window.btnCancel.text = "取消編輯";
        }
        //else
        //{
        //    window.statusElement.text = $"正在新增使用者";
        //    window.btnCancel.text = "清除";
        //}

        return window;
    }
}