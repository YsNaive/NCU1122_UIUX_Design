using NaiveAPI.UITK;


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class UI_index : MonoBehaviour
{
    public UIDocument UID;
    //public SODocPage DocumentPage;

    private RSScrollView userPageContainer;
    VisualElement rightContainer;
    VisualElement leftToolbar;
    Button btnHome, btnUserList, btnCreateNewUser, btnDocument, btnOnlineHelp;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        RuntimeWindow.ScreenElement = UID.rootVisualElement;

        UID.rootVisualElement.style.flexGrow = 1;

        VisualElement background = UID.rootVisualElement.Q<VisualElement>("background");
        background.style.backgroundColor = RSTheme.Current.BackgroundColor;

        leftToolbar = UID.rootVisualElement.Q<VisualElement>("left-toolbar");
        leftToolbar.style.backgroundColor = RSTheme.Current.BackgroundColor2;

        userPageContainer = new RSScrollView();

        rightContainer = UID.rootVisualElement.Q<VisualElement>("right-container");
        rightContainer.style.backgroundColor = RSTheme.Current.BackgroundColor;
        rightContainer.style.flexGrow = 1;

        btnHome = new RSButton("�D��");
        btnHome.AddToClassList("left-toolbar-btn");
        btnHome.clicked += openMainPage;

        btnUserList = new RSButton("�խ�\n����");
        btnUserList.AddToClassList("left-toolbar-btn");
        btnUserList.clicked += openUserList;

        btnCreateNewUser = new RSButton("�s�W");
        btnCreateNewUser.AddToClassList("left-toolbar-btn");
        btnCreateNewUser.clicked += () => { openCreateNewUser(); };

        VisualElement bottomElement = new VisualElement();
        bottomElement.style.marginTop = StyleKeyword.Auto;
        bottomElement.style.top = StyleKeyword.Auto;
        bottomElement.style.bottom = 0;
        bottomElement.style.marginBottom = 0;

        btnDocument = new RSButton("����");
        btnDocument.AddToClassList("left-toolbar-btn");
        btnDocument.style.SetRS_Style(new RSBorder(Color.clear, 0));
        //btnDocument.wked += openDocument;

        btnOnlineHelp = new RSButton("�u�W\n���U");
        btnOnlineHelp.AddToClassList("left-toolbar-btn");
        btnOnlineHelp.style.SetRS_Style(new RSBorder(Color.clear, 0));
        btnOnlineHelp.clicked += () => DoubleCheckWindow.Open(new Rect(20, 30, 60, 40), "�Y�N����ܥ~�������A�O�_�n�~��H", (confirm) =>
        {
            if (confirm)
                Application.OpenURL("https://ysnaive.github.io/NCU1122_UIUX_Design/");
        });

        leftToolbar.Add(btnHome);
        leftToolbar.Add(btnUserList);
        leftToolbar.Add(btnCreateNewUser);

        bottomElement.Add(btnDocument);
        bottomElement.Add(btnOnlineHelp);

        leftToolbar.Add(bottomElement);

        openMainPage();

        DetailedUserDataWindow window = RuntimeWindow.GetWindow<DetailedUserDataWindow>();

        yield return null;

        window.style.width = Length.Percent(100);
        window.style.height = Length.Percent(100);
        window.Close();
    }
    (int newIndex, UserData userData) getRandData(int prevIndex)
    {
        var dataList = UserDataHandler.Datas;
        if (dataList.Count == 1)
            return (0, dataList[0]);
        if (dataList.Count == 0)
            return (-1, new UserData());
        var resultIndex = prevIndex;
        while (resultIndex ==  prevIndex) {
            resultIndex = Random.Range(0, dataList.Count);
        }
        return (resultIndex, dataList[resultIndex]);
    }

    private void clearPage()
    {
        rightContainer.Clear();
        var border = new RSBorder(Color.clear, 0);
        foreach(var ve in leftToolbar.Children())
            ve.style.SetRS_Style(border);
        btnDocument.style.SetRS_Style(border);
    }
    private void openMainPage() 
    {
        clearPage();
        btnHome.style.SetRS_Style(new RSBorder(RSTheme.Current.FrontgroundColor, 2.5f));
        VisualElement root = new VisualElement();
        root.style.alignItems = Align.Center;
        root.style. flexGrow = 1;
        RSTextElement label = new RSTextElement("�ϥΪ̬ɭ��]�p�M��Ų");
        label.style.fontSize = label.style.fontSize.value.value * 2f;
        label.style.marginTop = RSTheme.Current.LineHeight;
        RSTextElement memberName = new RSTextElement("���Ϳo�@�C�ʻ��@�\�v�z"); memberName.style.fontSize = memberName.style.fontSize.value.value * 1.8f;
        memberName.style.marginBottom = 30;
        RSHorizontal randomUser = new RSHorizontal();
        VisualElement displayRandomUser = new VisualElement();
        displayRandomUser.style.justifyContent = Justify.Center;
        randomUser.Add(displayRandomUser);
        VisualElement userImage = new VisualElement();
        RSTextElement userName = new RSTextElement();
        userName.style.marginTop = 15;
        displayRandomUser.Add(userImage);
        displayRandomUser.Add(userName);
        userImage.style.width = 250;
        userImage.style.height = 250;
        userImage.style.SetRS_Style(RSRadius.Pixel(30));
        userName.style.fontSize = userName.style.fontSize.value.value * 1.8f;
        userName.style.unityTextAlign = TextAnchor.MiddleCenter;

        Button getRandomUser = new RSButton("���");
        getRandomUser.style.unityTextAlign = TextAnchor.MiddleCenter;
        getRandomUser.style.fontSize = getRandomUser.style.fontSize.value.value * 1.8f;
        getRandomUser.style.SetRS_Style(RSRadius.Pixel(7));
        getRandomUser.style.marginBottom = 15;
        getRandomUser.style.maxHeight = 70;
        getRandomUser.style.minWidth = 200;
        getRandomUser.style.marginTop = StyleKeyword.Auto;
        var data = getRandData(-1);
        Action setRand = () =>
        {
            data = getRandData(data.newIndex);
            Texture2D texture;
            if (data.userData.Base64Icon != "")
            {
                texture = new Texture2D(1, 1);
                texture.LoadImage(Convert.FromBase64String(data.userData.Base64Icon));
            }
            else
            {
                texture = Resources.Load<Texture2D>("Image/default_icon");
            }
            userImage.style.backgroundImage = Background.FromTexture2D(texture);
            userName.text = data.userData.Name;
        };
        setRand();
        bool randing = false;
        getRandomUser.clicked += ()=>
        {
            int mili = 25;
            randing = true;
            displayRandomUser.style.opacity = 0.6f;
            displayRandomUser.pickingMode = PickingMode.Ignore;
            var item = getRandomUser.schedule.Execute(() =>
            {
                setRand();
            });
            item.Until(() =>
            {
                mili = (int)(mili * 1.2f);
                item.Every(mili);
                if (mili > 1250)
                {
                    item.Pause();
                    displayRandomUser.style.opacity = 1f;
                    displayRandomUser.pickingMode = PickingMode.Position;
                    randing = false;
                    return true;
                }
                return false;
            });
        };

        userImage.RegisterCallback<PointerDownEvent>(evt =>
        {
            if (randing) return;
            DetailedUserDataWindow.Open(data.userData, true, false);
        });

        root.Add(label);
        root.Add(memberName);
        root.Add(randomUser);
        root.Add(getRandomUser);
        rightContainer.Add(root);
    }

    private VisualElement simpleVisualsContainer;
    private List<SimpleUserDataVisual> simpleVisuals;
    private void openUserList()
    {
        clearPage();
        btnUserList.style.SetRS_Style(new RSBorder(RSTheme.Current.FrontgroundColor, 2.5f));
        userPageContainer.Clear();

        VisualElement topElement = new VisualElement();
        topElement.style.flexDirection = FlexDirection.Row;

        VisualElement searchContainer = new VisualElement();
        searchContainer.style.width = Length.Percent(30);
        searchContainer.style.flexDirection = FlexDirection.Row;
        searchContainer.style.backgroundColor = RSTheme.Current.FieldStyle.Background.tintColor;
        searchContainer.style.alignItems = Align.Center;
        searchContainer.style.SetRS_Style(RSRadius.Pixel(10));
        searchContainer.style.left = StyleKeyword.Auto;
        searchContainer.style.right = 0;
        searchContainer.style.marginLeft = StyleKeyword.Auto;
        searchContainer.style.marginRight = 10;
        searchContainer.style.marginTop = 5;
        searchContainer.style.marginBottom = 5;

        simpleVisuals = new List<SimpleUserDataVisual>();

        RSTextField searchField = new RSTextField("", (evt) =>
        {
            simpleVisualsContainer.Clear();

            //Debug.Log(simpleVisuals.Count);

            foreach (SimpleUserDataVisual visual in simpleVisuals.OrderBy((v) =>
            {
                int distance = int.MaxValue;

                foreach (string s in v.UserData.GetAllValue())
                {
                    if (!s.Contains(evt.newValue))
                    {
                        distance++;
                    }
                }

                return distance;
            }))
            {
                simpleVisualsContainer.Add(visual);
            }
        });
        searchField.value = "�j�M";
        searchField.style.flexGrow = 1;
        searchField.style.height = 40;
        searchField.style.marginLeft = 10;
        searchField.style.marginRight = 10;
        searchField[0].style.backgroundImage = null;
        searchField[0].style.SetRS_Style(new RSBorder(Color.clear, 0));
        searchField[0].style.fontSize = RSTheme.Current.MainText.size * 1.5f;

        searchField.RegisterCallback<FocusInEvent>((evt) =>
        {
            if (searchField.value == "�j�M")
            {
                searchField.SetValueWithoutNotify("");
            }
        });

        searchField.RegisterCallback<FocusOutEvent>((evt) =>
        {
            if (searchField.value == "")
            {
                searchField.SetValueWithoutNotify("�j�M");
            }
        });

        VisualElement image = new VisualElement();

        image.style.backgroundImage = Resources.Load<Texture2D>("Image/search_white");
        image.style.width = 30;
        image.style.height = 30;
        image.style.marginTop = 5;
        image.style.marginBottom = 5;
        image.style.marginLeft = 10;

        searchContainer.Add(image);
        searchContainer.Add(searchField);

        topElement.Add(searchContainer);

        userPageContainer.Add(topElement);

        simpleVisualsContainer = new VisualElement();

        foreach (UserData data in UserDataHandler.Datas)
        {
            SimpleUserDataVisual simpleUserDataVisual = new SimpleUserDataVisual(data);
            simpleUserDataVisual.RegisterCallback<PointerDownEvent>((evt) => DetailedUserDataWindow.Open(data, true));

            VisualElement tools = new VisualElement();
            tools.style.position = Position.Absolute;
            tools.style.top = 10;
            tools.style.right = 10;
            tools.style.left = StyleKeyword.Auto;
            tools.style.bottom = StyleKeyword.Auto;
            tools.style.marginTop = 0;
            tools.style.marginRight = 0;
            tools.style.marginLeft = StyleKeyword.Auto;
            tools.style.marginBottom = StyleKeyword.Auto;

            var localData = data;
            RSButton editBtn = new RSButton("�s��", RSTheme.Current.HintColorSet);
            editBtn.style.SetRS_Style(RSRadius.Pixel(10));
            editBtn.clicked += () => { openCreateNewUser(localData); };

            RSButton btnDelete = new RSButton("�R��", RSTheme.Current.HintColorSet, () =>
            {
                DoubleCheckWindow.Open(new Rect(20, 30, 60, 40), "�T�w�n�ä[�R�����ϥΪ̡H�T�{���ƱN�ä[�����C", (confirm) =>
                {
                    if (confirm)
                    {
                        DataHandler.DeleteData(DataHandler.UserDataDir, localData.Name + ".json");

                        UserDataHandler.LoadAll();

                        simpleVisuals.Remove(simpleUserDataVisual);
                        simpleVisualsContainer.Remove(simpleUserDataVisual);

                        HintWindow.Open(new Rect(35, 80, 30, 10), $"���\�R��'{localData.Name}'");
                    }
                });
            });
            btnDelete.style.SetRS_Style(RSRadius.Pixel(10));

            tools.Add(editBtn);
            tools.Add(btnDelete);

            simpleUserDataVisual.Add(tools);
            simpleVisuals.Add(simpleUserDataVisual);
            simpleVisualsContainer.Add(simpleUserDataVisual);
        }

        userPageContainer.Add(simpleVisualsContainer);

        rightContainer.Add(userPageContainer);
    }

    private void openCreateNewUser(UserData editData = null)
    {
        clearPage();
        btnCreateNewUser.style.SetRS_Style(new RSBorder(RSTheme.Current.FrontgroundColor, 2.5f));

        CreateNewUserWindow window = CreateNewUserWindow.Open(openMainPage, editData);

        rightContainer.Add(window);
    }
    //private void openDocument()
    //{
    //    clearPage();
    //    btnDocument.style.SetRS_Style(new RSBorder(RSTheme.Current.FrontgroundColor, 2.5f));

    //    rightContainer.Add(new DocBookVisual(DocumentPage) { DontPlayAnimation = true });
    //}
}

public class SimpleUserDataVisual : VisualElement
{
    public UserData UserData;

    private Texture2D texture;
    public SimpleUserDataVisual(UserData data)
    {
        UserData = data;

        style.backgroundColor = RSTheme.Current.BackgroundColor;
        RSMargin.op_temp.any = 7;
        style.SetRS_Style(RSMargin.op_temp);
        style.marginBottom = 0;
        style.paddingLeft = 20;
        style.flexDirection = FlexDirection.Row;
        style.alignItems = Align.Center;
        RegisterCallback<PointerEnterEvent>(evt => style.backgroundColor = RSTheme.Current.BackgroundColor2);
        RegisterCallback<PointerLeaveEvent>(evt => style.backgroundColor = Color.clear);
        VisualElement preview = new VisualElement();
        preview.style.marginRight = 20;
        preview.style.SetRS_Style(RSRadius.Pixel(10));

        texture = new Texture2D(1, 1);
        if (data.Base64Icon != "")
            texture.LoadImage(Convert.FromBase64String(data.Base64Icon));
        else
            texture = Resources.Load<Texture2D>("Image/default_icon");

        preview.style.backgroundImage = new StyleBackground(texture);

        preview.style.width = 100;
        preview.style.height = 100;

        VisualElement rightContainer = new VisualElement();
        rightContainer.style.flexGrow = 1;

        RSTextElement nameElement = new RSTextElement(data.Name);
        RSTextElement contactElement = new RSTextElement("�p���覡�G" + data.Contact);
        RSTextElement researchTopicElement = new RSTextElement("��s�D�D�G" + data.ResearchTopic);

        nameElement.style.fontSize = nameElement.style.fontSize.value.value * 1.5f;
        contactElement.style.fontSize = contactElement.style.fontSize.value.value * 1.5f;
        researchTopicElement.style.fontSize = researchTopicElement.style.fontSize.value.value * 1.5f;

        rightContainer.Add(nameElement);
        rightContainer.Add(contactElement);
        rightContainer.Add(researchTopicElement);

        Add(preview);
        Add(rightContainer);
    }
}
