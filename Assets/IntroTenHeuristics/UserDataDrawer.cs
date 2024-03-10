using NaiveAPI.DocumentBuilder;
using NaiveAPI.RuntimeWindowUtils;
using NaiveAPI_UI;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

[CustomRuntimeDrawer(typeof(UserData), Priority = 10)]
public class UserDataDrawer : RuntimeDrawer<UserData>
{
    private Base64ImageDrawer base64ImageField;
    private MessageStringDrawer nameField;
    private MessageStringDrawer phoneNumberField;
    private MessageStringDrawer majorField;
    private MessageStringDrawer careerField;
    private MessageStringDrawer genderField;
    private MessageStringDrawer researchTopicField;
    private MessageStringDrawer contactField;
    private MessageStringDrawer skillsField;
    private MessageStringDrawer hobbiesField;
    private MessageStringDrawer graduatedSchoolField;

    public override void UpdateField()
    {
        base64ImageField.SetValueWithoutNotify(value.Base64Icon);
        nameField.SetValueWithoutNotify(value.Name);
        phoneNumberField.SetValueWithoutNotify(value.PhoneNumber);
        majorField.SetValueWithoutNotify(value.Major);
        careerField.SetValueWithoutNotify(value.Career);
        genderField.SetValueWithoutNotify(value.Gender);
        researchTopicField.SetValueWithoutNotify(value.ResearchTopic);
        contactField.SetValueWithoutNotify(value.Contact);
        skillsField.SetValueWithoutNotify(value.Skills);
        hobbiesField.SetValueWithoutNotify(value.Hobbies);
        graduatedSchoolField.SetValueWithoutNotify(value.GraduatedSchool);
    }

    protected override void OnCreateGUI()
    {
        style.marginTop = 15;
        style.paddingRight = 45;
        DSScrollView scrollView = new DSScrollView();

        base64ImageField = new Base64ImageDrawer() { label = "大頭貼" };
        base64ImageField.RegisterValueChangedCallback((evt) => { value.Base64Icon = evt.newValue; evt.StopPropagation(); });
        nameField = (MessageStringDrawer)RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("姓名：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("姓名為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        nameField.RegisterValueChangedCallback((evt) => { value.Name = evt.newValue; evt.StopPropagation(); });
        majorField = (MessageStringDrawer)RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("系級：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("系級為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        majorField.RegisterValueChangedCallback((evt) => { value.Major = evt.newValue; evt.StopPropagation(); });
        careerField = (MessageStringDrawer) RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("工作：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("工作為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        careerField.RegisterValueChangedCallback((evt) => { value.Career = evt.newValue; evt.StopPropagation(); });
        genderField = (MessageStringDrawer) RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("性別：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("性別為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        genderField.RegisterValueChangedCallback((evt) => { value.Gender = evt.newValue; evt.StopPropagation(); });
        phoneNumberField = (MessageStringDrawer)RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("電話號碼：")
            .AddAttribute(new MessageStringAttribute()
                /*.AddCondition("PhoneNumber can not be empty!", (v) => v != null && v != "")*/)
            .Build();
        phoneNumberField.RegisterValueChangedCallback((evt) => { value.PhoneNumber = evt.newValue; evt.StopPropagation(); });
        researchTopicField = (MessageStringDrawer) RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("研究主題：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("研究主題為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        researchTopicField.RegisterValueChangedCallback((evt) => { value.ResearchTopic = evt.newValue; evt.StopPropagation(); });
        contactField = (MessageStringDrawer) RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("聯絡方式：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("聯絡方式為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        contactField.RegisterValueChangedCallback((evt) => { value.Contact = evt.newValue; evt.StopPropagation(); });
        skillsField = (MessageStringDrawer) RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("專長：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("專長為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        skillsField.RegisterValueChangedCallback((evt) => { value.Skills = evt.newValue; evt.StopPropagation(); });
        hobbiesField = (MessageStringDrawer) RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("愛好：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("愛好為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        hobbiesField.RegisterValueChangedCallback((evt) => { value.Hobbies = evt.newValue; evt.StopPropagation(); });
        graduatedSchoolField = (MessageStringDrawer) RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("畢業學校：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("畢業學校為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        graduatedSchoolField.RegisterValueChangedCallback((evt) => { value.GraduatedSchool = evt.newValue; evt.StopPropagation(); });

        VisualElement container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.marginBottom = 10;

        VisualElement fieldContainer = new VisualElement();
        fieldContainer.style.paddingTop = DocStyle.Current.LineHeight;
        fieldContainer.style.paddingRight = DocStyle.Current.LineHeight*1.5f;
        fieldContainer.style.flexGrow = 1;
        fieldContainer.Add(nameField);
        fieldContainer.Add(majorField);
        fieldContainer.Add(careerField);

        container.Add(fieldContainer);
        container.Add(base64ImageField);

        scrollView.Add(container);
        scrollView.Add(genderField);
        scrollView.Add(phoneNumberField);
        scrollView.Add(researchTopicField);
        scrollView.Add(contactField);
        scrollView.Add(skillsField);
        scrollView.Add(hobbiesField);
        scrollView.Add(graduatedSchoolField);

        Add(scrollView);
    }

    public bool IsAllValid()
    {
        if (!nameField.IsValid()) return false;
        if (!majorField.IsValid()) return false;
        if (!careerField.IsValid()) return false;
        if (!genderField.IsValid()) return false;
        if (!phoneNumberField.IsValid()) return false;
        if (!researchTopicField.IsValid()) return false;
        if (!contactField.IsValid()) return false;
        if (!skillsField.IsValid()) return false;
        if (!hobbiesField.IsValid()) return false;
        if (!graduatedSchoolField.IsValid()) return false;

        return true;
    }

    public void ShowAllInvalidMessage()
    {
        if (!nameField.IsValid()) nameField.ShowInvalidMessage();
        if (!majorField.IsValid()) majorField.ShowInvalidMessage();
        if (!careerField.IsValid()) careerField.ShowInvalidMessage();
        if (!genderField.IsValid()) genderField.ShowInvalidMessage();
        if (!phoneNumberField.IsValid()) phoneNumberField.ShowInvalidMessage();
        if (!researchTopicField.IsValid()) researchTopicField.ShowInvalidMessage();
        if (!contactField.IsValid()) contactField.ShowInvalidMessage();
        if (!skillsField.IsValid()) skillsField.ShowInvalidMessage();
        if (!hobbiesField.IsValid()) hobbiesField.ShowInvalidMessage();
        if (!graduatedSchoolField.IsValid()) graduatedSchoolField.ShowInvalidMessage();
    }
}

public class Base64ImageDrawer : RuntimeDrawer<string>
{
    private VisualElement preview;
    private Texture2D texture;

    public override void UpdateField()
    {
        if (value != "")
        {
            try
            {
                texture = new Texture2D(1, 1);
                texture.LoadImage(Convert.FromBase64String(value));
            }
            catch
            {
                texture = Resources.Load<Texture2D>("Image/default_icon");
            }
        }
        else
        {
            texture = Resources.Load<Texture2D>("Image/default_icon");
        }

        preview.style.backgroundImage = new StyleBackground(texture);

        preview.style.width = 100;
        preview.style.height = 100;
    }

    protected override void OnCreateGUI()
    {
        LayoutExpand();
        iconElement.style.display = DisplayStyle.None;
        labelElement.style.unityTextAlign = TextAnchor.MiddleCenter;
        contentContainer.style.alignItems = Align.Center;
        contentContainer.style.marginLeft = 0;

        preview = new VisualElement();

        texture = new Texture2D(1, 1);
        //contentContainer.style.flexDirection = FlexDirection.Row;

        DSButton btChoice = new DSButton("選擇圖片...", () =>
        {
            string[] path = StandaloneFileBrowser.OpenFilePanel("UserImage", "", new ExtensionFilter[]
            {
                new ExtensionFilter("image", "jpg", "png", "jpeg")
            }, false);

            if (path.Length > 0)
            {
                byte[] bytes = File.ReadAllBytes(path[0]);

                SetValue(Convert.ToBase64String(bytes));
            }
            else
            {
                SetValue("");
            }
        });
        btChoice.style.ClearMarginPadding();
        btChoice.style.marginTop = 5;

        btChoice.style.width = 100;

        Add(preview);
        Add(btChoice);
    }
}

[CustomRuntimeDrawer(typeof(string), Priority = 10, RequiredAttribute = typeof(MessageStringAttribute))]
public class MessageStringDrawer : StringDrawer
{
    public MessageStringAttribute attribute;

    private DocDescription warningVisual;
    private DSTextElement warningField;

    public MessageStringDrawer()
    {
        this.Q<DSTextField>().style.flexGrow = 1;
        this.Q<DSTextField>().multiline = false;
        // contentContainer.style.flexDirection = FlexDirection.Row;


        warningVisual = (DocDescription) DocVisual.Create(DocDescription.CreateComponent("", DocDescription.DescriptionType.Warning));
        warningField = warningVisual.Q<DSTextElement>();
        warningVisual.style.visibility = Visibility.Hidden;

        Add(warningVisual);

        RegisterCallback<FocusOutEvent>((evt) =>
        {
            ShowInvalidMessage();
        });
    }

    public void ShowInvalidMessage()
    {
        warningVisual.style.visibility = Visibility.Hidden;
        foreach ((string message, var checkValid) in attribute.Conditions)
        {
            if (!checkValid(this.value))
            {
                ShowInvalidMessage(message);
                break;
            }
        }
    }

    public void ShowInvalidMessage(string message)
    {
        warningField.text = string.Format(message, this.value);
        warningVisual.style.visibility = Visibility.Visible;
    }

    public bool IsValid()
    {
        foreach ((_, var checkValid) in attribute.Conditions)
        {
            if (!checkValid(this.value))
                return false;
        }
        return true;
    }

    public override void ReciveAttribute(Attribute attribute)
    {
        this.attribute = attribute as MessageStringAttribute;
    }
}

public class MessageStringAttribute : Attribute
{
    public List<(string message, Func<string, bool> checkValid)> Conditions = new List<(string message, Func<string, bool> checkValid)>();

    public MessageStringAttribute AddCondition(string message, Func<string, bool> checkValid)
    {
        Conditions.Add((message, checkValid));

        return this;
    }
}
