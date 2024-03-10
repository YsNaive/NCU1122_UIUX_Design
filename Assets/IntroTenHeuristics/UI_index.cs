using NaiveAPI.DocumentBuilder;
using NaiveAPI.RuntimeWindowUtils;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateNewUserWindow : DSRuntimeWindow
{
    public UserData UserData = new UserData();

    public CreateNewUserWindow()
    {
        PopupOnClick = false;

        UserDataDrawer userDataDrawer = new UserDataDrawer();
        userDataDrawer.value = UserData;
        Add(userDataDrawer);

        VisualElement visualElement = new VisualElement();
        visualElement.style.flexDirection = FlexDirection.Row;

        DSButton btnSave = new DSButton("儲存", DocStyle.Current.SuccessColor, () =>
        {
            if (userDataDrawer.IsAllValid())
            {
                DataHandler.SaveData(DataHandler.UserDataDir, UserData.Name + ".json", JsonUtility.ToJson(UserData), false);

                UserDataHandler.LoadAll();

                Destory();
            }
            else
            {
                userDataDrawer.ShowAllInvalidMessage();
            }
        });

        DSButton btnBack = new DSButton("返回", DocStyle.Current.HintColor, () => Close());

        DSButton btnCancel = new DSButton("取消", DocStyle.Current.DangerColor, () => DoubleCheckWindow.Open(new Rect(20, 30, 60, 40), "該動作會撤銷之前的所有更改，確定要離開嗎？", (wantLeave) =>
        {
            if (wantLeave) Destory();
        }));

        btnSave.style.marginRight = 50;
        btnBack.style.marginLeft = 50;
        btnBack.style.marginRight = 50;
        btnCancel.style.marginLeft = 50;
        btnSave.style.width = Length.Percent(20);
        btnBack.style.width = Length.Percent(20);
        btnCancel.style.width = Length.Percent(20);

        visualElement.Add(btnSave);

        visualElement.Add(btnBack);

        visualElement.Add(btnCancel);

        visualElement.style.flexGrow = 1;
        visualElement.style.alignItems = Align.Center;
        visualElement.style.justifyContent = Justify.Center;
        visualElement.style.top = StyleKeyword.Auto;
        visualElement.style.bottom = 0;
        visualElement.style.marginTop = StyleKeyword.Auto;

        Add(visualElement);
    }
}

public class UI_index : MonoBehaviour
{
    public UIDocument UID;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        RuntimeWindow.ScreenElement = UID.rootVisualElement;

        UID.rootVisualElement.style.flexGrow = 1;

        ScrollView rightContainer = UID.rootVisualElement.Q<ScrollView>("right-container");
        rightContainer.style.flexGrow = 1;

        Button btnUserList = UID.rootVisualElement.Q<Button>("btn-user-list");
        btnUserList.style.SetIS_Style(DocStyle.Current.ButtonTextStyle);

        btnUserList.clicked += () =>
        {
            rightContainer.Clear();

            foreach (UserData data in UserDataHandler.Datas)
            {
                SimpleUserDataVisual simpleUserDataVisual = new SimpleUserDataVisual(data);
                simpleUserDataVisual.RegisterCallback<PointerDownEvent>((evt) => DetailedUserDataWindow.Open(data));
                rightContainer.Add(simpleUserDataVisual);
            }
        };

        Button btnCreateNewUser = UID.rootVisualElement.Q<Button>("btn-create-new-user");

        btnCreateNewUser.clicked += () =>
        {
            CreateNewUserWindow window = RuntimeWindow.GetWindow<CreateNewUserWindow>();

            window.SetSizeAnyway(RuntimeWindow.ScreenElement.layout.size);
        };

        DetailedUserDataWindow window = RuntimeWindow.GetWindow<DetailedUserDataWindow>();

        yield return null;

        window.SetLayoutPercentAnyway(new Rect(0, 0, 1, 1));

        window.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class SimpleUserDataVisual : VisualElement
{
    private Texture2D texture;
    public SimpleUserDataVisual(UserData data)
    {
        style.backgroundColor = DocStyle.Current.BackgroundColor;
        style.SetIS_Style(new ISBorder(Color.black, 5));
        style.paddingLeft = 20;
        style.flexDirection = FlexDirection.Row;
        style.alignItems = Align.Center;

        VisualElement preview = new VisualElement();
        preview.style.marginRight = 20;

        texture = new Texture2D(1, 1);
        texture.LoadImage(Convert.FromBase64String(data.Base64Icon));

        preview.style.backgroundImage = new StyleBackground(texture);

        preview.style.width = 100;
        preview.style.height = 100;

        VisualElement rightContainer = new VisualElement();
        rightContainer.style.flexGrow = 1;

        DSLabel nameElement = new DSLabel(data.Name);
        DSLabel contactElement = new DSLabel("聯絡方式：" + data.Contact);
        DSLabel researchTopicElement = new DSLabel("研究主題：" + data.ResearchTopic);

        rightContainer.Add(nameElement);
        rightContainer.Add(contactElement);
        rightContainer.Add(researchTopicElement);

        Add(preview);
        Add(rightContainer);
    }
}
