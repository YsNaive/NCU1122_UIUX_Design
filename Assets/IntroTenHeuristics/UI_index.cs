using NaiveAPI.DocumentBuilder;
using NaiveAPI.RuntimeWindowUtils;
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
    void Start()
    {
        RuntimeWindow.ScreenElement = UID.rootVisualElement;

        UID.rootVisualElement.style.flexGrow = 1;

        Button btnCreateNewUser = UID.rootVisualElement.Q<Button>("btn-create-new-user");

        btnCreateNewUser.clicked += () =>
        {
            CreateNewUserWindow window = RuntimeWindow.GetWindow<CreateNewUserWindow>();

            window.SetSizeAnyway(RuntimeWindow.ScreenElement.layout.size);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
