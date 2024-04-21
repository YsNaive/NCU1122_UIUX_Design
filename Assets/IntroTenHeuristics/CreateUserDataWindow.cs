using NaiveAPI.UITK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateNewUserWindow : RSRuntimeWindow
{
    public Action OnSaveOrClear;
    public bool IsEditing = false;
    public UserData UserData => UserDataDrawer.value;
    private UserData originalData;
    private bool dataChange = false;

    public UserDataDrawer UserDataDrawer => userDataDrawer;
    private UserDataDrawer userDataDrawer;

    private RSTextElement statusElement;
    private RSButton btnCancel, btnPrev, btnNext;

    private string[] progressNames = { "�򥻸��", "�p�����", "�S����" };
    private List<RSTextElement> progresses = new List<RSTextElement>();

    public CreateNewUserWindow()
    {
        PopupOnClick = false;
        Resizable = false;

        VisualElement progressBar = new VisualElement();
        progressBar.style.flexDirection = FlexDirection.Row;

        Texture2D texture = Resources.Load<Texture2D>("Image/arrow");

        float fontSize = RSTheme.Current.MainText.size * 1.5f;
        float paddingSize = RSTheme.Current.MainText.size * 1.8f;

        statusElement = new RSTextElement("���b�s�W�ϥΪ�");
        statusElement.style.unityTextAlign = TextAnchor.MiddleCenter;
        statusElement.style.fontSize = fontSize;
        statusElement.style.left = 50;
        statusElement.style.right = StyleKeyword.Auto;
        statusElement.style.marginRight = StyleKeyword.Auto;

        progressBar.Add(statusElement);

        for (int i = 0;i < progressNames.Length; i++)
        {
            RSTextElement textElement = new RSTextElement(progressNames[i]);

            textElement.style.backgroundImage = texture;
            textElement.style.unityBackgroundImageTintColor = RSTheme.Current.BackgroundColor2;
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

                btnNext.text = userDataDrawer.IsLastPage() ? "����" : "�U�@��";
            });

            textElement.RegisterCallback<PointerLeaveEvent>((evt) =>
            {
                if (textElement != progresses[userDataDrawer.PageIndex])
                {
                    textElement.style.unityBackgroundImageTintColor = RSTheme.Current.BackgroundColor2;
                }
            });

            progresses.Add(textElement);
            progressBar.Add(textElement);
        }

        progresses[0].style.unityBackgroundImageTintColor = RSTheme.Current.FrontgroundColor2;
        progresses[0].style.color = RSTheme.Current.BackgroundColor;

        progressBar.style.justifyContent = Justify.Center;
        progressBar.style.paddingTop = 10;

        Add(progressBar);

        userDataDrawer = new UserDataDrawer();
        userDataDrawer.value ??= new UserData();
        userDataDrawer.style.paddingTop = 50;
        userDataDrawer.OnMemberValueChanged += (_) =>
        {
            if (IsEditing)
            {
                dataChange = !userDataDrawer.value.Equals(originalData);
                btnCancel.text = dataChange ? "�˱��ܧ�" : "�����s��";
            }
        };
        //var sc = new RSScrollView();
        //sc.style.flexGrow = 1;
        //sc.contentContainer.style.flexGrow = 1;
        Add(userDataDrawer);

        VisualElement controlPanel = new VisualElement();
        controlPanel.style.flexDirection = FlexDirection.Row;

        btnPrev = new RSButton("�W�@��", RSTheme.Current.SuccessColorSet, () => toPrevPage());

        btnNext = new RSButton("�U�@��", RSTheme.Current.SuccessColorSet, () => toNextPageOrFinish());

        btnCancel = new RSButton("�M��", RSTheme.Current.DangerColorSet, () => {
            if (!IsEditing)
            {
                DoubleCheckWindow.Open(new Rect(20, 30, 60, 40), "�Ӱʧ@�|�M���Ҧ����A�T�w�n�R���ܡH", (confirm) =>
                {
                    if (confirm)
                        userDataDrawer.Reset();
                });
            }
            else
            {
                if (dataChange)
                {
                    DoubleCheckWindow.Open(new Rect(20, 30, 60, 40), "�O�_�n�˱�Ҧ��ܧ�A�^��s��e����ơH", (confirm) =>
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
            if (userDataDrawer.IsAllValid())
            {
                if (IsEditing && UserData.Name != originalData.Name)
                {
                    if (DataHandler.FileExists(DataHandler.UserDataDir, UserData.Name + ".json"))
                    {
                        HintWindow.Open(new Rect(35, 80, 30, 10), $"�ק異�ѡA'{UserData.Name}'�w�g�s�b");
                    }
                    else
                    {
                        DataHandler.DeleteData(DataHandler.UserDataDir, originalData.Name + ".json");
                        DataHandler.SaveData(DataHandler.UserDataDir, UserData.Name + ".json", JsonUtility.ToJson(UserData), false);

                        HintWindow.Open(new Rect(35, 80, 30, 10), $"���\�ק�'{UserData.Name}'");
                    }
                }
                else
                {
                    if (DataHandler.SaveData(DataHandler.UserDataDir, UserData.Name + ".json", JsonUtility.ToJson(UserData), IsEditing))
                    {
                        if (IsEditing)
                        {
                            HintWindow.Open(new Rect(35, 80, 30, 10), $"���\�ק�'{UserData.Name}'");
                        }
                        else
                        {
                            HintWindow.Open(new Rect(35, 80, 30, 10), $"���\�s�W'{UserData.Name}'");
                        }
                    }
                    else
                    {
                        HintWindow.Open(new Rect(35, 80, 30, 10), $"�s�W���ѡA'{UserData.Name}'�w�g�s�b");
                    }
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
        else if(userDataDrawer.IsPageValid())
        {
            btnPrev.visible = true;

            userDataDrawer.NextPage();
            if (userDataDrawer.IsLastPage())
            {
                btnNext.text = "����";
            }

            highlightCurrentPage();
        }
        else
        {
            userDataDrawer.ShowPageInvalidMessage();
        }
    }

    private void highlightCurrentPage()
    {
        foreach (TextElement progress in progresses)
        {
            progress.style.unityBackgroundImageTintColor = RSTheme.Current.BackgroundColor2;
            progress.style.color = RSTheme.Current.MainText.color;
        }

        int index = userDataDrawer.PageIndex;

        progresses[index].style.unityBackgroundImageTintColor = RSTheme.Current.FrontgroundColor2;
        progresses[index].style.color = RSTheme.Current.BackgroundColor;
    }

    private void toPrevPage()
    {
        if (userDataDrawer.IsLastPage())
        {
            btnNext.text = "�U�@��";
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

        window.InitLayoutAsPercent(LayoutPercent.FullScreen);

        //window.style.width = Length.Percent(100);
        //window.style.height = Length.Percent(100);

        if (editData != null)
        {
            window.IsEditing = true;

            UserData copy = new UserData(editData);
            window.originalData = editData;
            window.UserDataDrawer.value = copy;
            window.UserDataDrawer.ShowAllInvalidMessage();
            window.statusElement.text = $"���b�s�� '{copy.Name}'";
            window.btnCancel.text = "�����s��";
        }
        //else
        //{
        //    window.statusElement.text = $"���b�s�W�ϥΪ�";
        //    window.btnCancel.text = "�M��";
        //}

        return window;
    }
}