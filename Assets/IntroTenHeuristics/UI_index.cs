using NaiveAPI.DocumentBuilder;
using NaiveAPI.RuntimeWindowUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateNewUserWindow : DSRuntimeWindow
{
    public UserData UserData = new UserData();

    public VisualElement errorMsg;

    public CreateNewUserWindow()
    {
        Add(RuntimeDrawer.Create(UserData));

        DSButton btnSave = new DSButton("Save", () =>
        {
            if (validCheck())
            {
                DataHandler.SaveData(DataHandler.UserDataDir, UserData.Name + ".json", JsonUtility.ToJson(UserData), false);
            }
        });

        Add(btnSave);

        errorMsg = new VisualElement();

        Add(errorMsg);
    }

    private bool validCheck()
    {
        errorMsg.Clear();

        if (DataHandler.FileExists(DataHandler.UserDataDir, UserData.Name + ".json"))
        {
            errorMsg.Add(new DSTextElement("已存在相同的使用者名稱"));

            return false;
        }

        return true;
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
