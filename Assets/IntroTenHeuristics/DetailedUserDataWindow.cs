using NaiveAPI.DocumentBuilder;
using NaiveAPI.RuntimeWindowUtils;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DetailedUserDataWindow : DSRuntimeWindow
{
    private Texture2D texture;
    private VisualElement preview;
    private LabelDrawer nameElement;
    private LabelDrawer phoneNumberElement;
    private LabelDrawer majorElement;
    private LabelDrawer careerElement;
    private LabelDrawer genderElement;    
    private LabelDrawer researchTopicElement;
    private LabelDrawer contactElement;
    private LabelDrawer skillsElement;
    private LabelDrawer hobbiesElement;
    private LabelDrawer graduatedSchoolElement;
    private UserData data;

    public DetailedUserDataWindow()
    {
        Color background = style.backgroundColor.value; background.a = 0.5f;
        style.backgroundColor = background;
        contentContainer.style.flexGrow = 1;
        contentContainer.style.flexDirection = FlexDirection.Row;
        contentContainer.focusable = true;

        contentContainer.RegisterCallback<KeyDownEvent>((evt) =>
        {
            switch (evt.keyCode)
            {
                case KeyCode.A:
                case KeyCode.LeftArrow:
                    Open(UserDataHandler.FindPrevData(data));
                    break;
                case KeyCode.D:
                case KeyCode.RightArrow:
                    Open(UserDataHandler.FindNextData(data));
                    break;

                default: break;
            }

            evt.StopPropagation();
        });

        VisualElement leaveContainer = new VisualElement();
        leaveContainer.style.width = Length.Percent(50);
        leaveContainer.RegisterCallback<PointerDownEvent>((evt) => Close());

        VisualElement fullContainer = new VisualElement();
        fullContainer.style.flexGrow = 1;
        fullContainer.style.backgroundColor = DocStyle.Current.BackgroundColor;

        VisualElement container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Center;
        container.style.marginBottom = 10;

        preview = new VisualElement();
        preview.style.SetIS_Style(ISRadius.Pixel(10));

        preview.style.width = 100;
        preview.style.height = 100;

        int origin = DocStyle.Current.MainTextSize;
        DocStyle.Current.MainTextSize = (int)(origin * 1.5f);

        nameElement             = new LabelDrawer("姓　　名：");
        phoneNumberElement      = new LabelDrawer("電話號碼：");
        careerElement           = new LabelDrawer("工　　作：");
        majorElement            = new LabelDrawer("系　　級：");
        genderElement           = new LabelDrawer("性　　別：");
        researchTopicElement    = new LabelDrawer("研究主題：");
        contactElement          = new LabelDrawer("聯絡方式：");
        skillsElement           = new LabelDrawer("專　　長：");
        hobbiesElement          = new LabelDrawer("愛　　好：");
        graduatedSchoolElement  = new LabelDrawer("畢業學校：");

        DocStyle.Current.MainTextSize = origin;

        VisualElement prevPage = new VisualElement();
        prevPage.style.SetIS_Style(DocStyle.Current.ArrowIcon);
        prevPage.style.rotate = new Rotate(180);
        prevPage.style.width = prevPage.style.width.value.value * 2;
        prevPage.style.height = prevPage.style.height.value.value * 2;
        prevPage.style.position = Position.Absolute;
        prevPage.style.top = StyleKeyword.Auto;
        prevPage.style.bottom = 10;
        prevPage.style.left = 10;
        prevPage.style.right = StyleKeyword.Auto;
        prevPage.RegisterCallback<PointerDownEvent>((evt) => Open(UserDataHandler.FindPrevData(data)));

        VisualElement nextPage = new VisualElement();
        nextPage.style.SetIS_Style(DocStyle.Current.ArrowIcon);
        nextPage.style.width = nextPage.style.width.value.value * 2;
        nextPage.style.height = nextPage.style.height.value.value * 2;
        nextPage.style.position = Position.Absolute;
        nextPage.style.top = StyleKeyword.Auto;
        nextPage.style.bottom = 10;
        nextPage.style.left = StyleKeyword.Auto;
        nextPage.style.right = 10;
        nextPage.RegisterCallback<PointerDownEvent>((evt) => Open(UserDataHandler.FindNextData(data)));

        VisualElement rightContainer = new VisualElement();

        rightContainer.Add(nameElement);
        rightContainer.Add(majorElement);
        rightContainer.Add(careerElement);

        container.Add(preview);
        container.Add(rightContainer);

        fullContainer.Add(container);
        fullContainer.Add(DocVisual.Create(DocDividline.CreateComponent()));
        fullContainer.Add(genderElement);
        fullContainer.Add(phoneNumberElement);
        fullContainer.Add(researchTopicElement);
        fullContainer.Add(contactElement);
        fullContainer.Add(skillsElement);
        fullContainer.Add(hobbiesElement);
        fullContainer.Add(graduatedSchoolElement);
        fullContainer.Add(prevPage);
        fullContainer.Add(nextPage);

        Add(leaveContainer);
        Add(fullContainer);

        EnableTab = false;
    }

    public static void Open(UserData data)
    {
        DetailedUserDataWindow window = GetWindow<DetailedUserDataWindow>();

        window.data = data;

        window.schedule.Execute(window.contentContainer.Focus).ExecuteLater(1);

        window.EnableTab = false;

        window.texture = new Texture2D(1, 1);
        if (data.Base64Icon != "")
            window.texture.LoadImage(Convert.FromBase64String(data.Base64Icon));
        else
            window.texture = Resources.Load<Texture2D>("Image/default_icon");

        window.preview.style.backgroundImage = new StyleBackground(window.texture);

        window.nameElement.text = data.Name;
        window.phoneNumberElement.text = data.PhoneNumber;
        window.careerElement.text = data.Career;
        window.genderElement.text = data.Gender;
        window.researchTopicElement.text = data.ResearchTopic;
        window.contactElement.text = data.Contact;
        window.skillsElement.text = data.Skills;
        window.hobbiesElement.text = data.Hobbies;
        window.graduatedSchoolElement.text =　data.GraduatedSchool;
    }
}

public class LabelDrawer : RuntimeDrawer<string>
{
    public string text { get => textElement.text; set => textElement.text = value; }
    private DSTextElement textElement;

    public LabelDrawer(string label) : base()
    {
        this.label = label;
    }

    public LabelDrawer(string label, string text) : this(label)
    {
        textElement.text = text;
    }

    public override void UpdateField()
    {
        textElement.text = value;
    }

    protected override void OnCreateGUI()
    {
        style.marginTop = 10;

        textElement = new DSTextElement();

        Add(textElement);
    }
}
