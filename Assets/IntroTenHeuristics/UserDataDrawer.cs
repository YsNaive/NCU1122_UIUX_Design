using NaiveAPI.UITK;


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
    private MessageStringDrawer specialExperienceField;
    private MessageStringDrawer favoriteClassesField;


    private List<VisualElement> pages = new List<VisualElement>();
    private List<List<MessageStringDrawer>> pageDrawers = new List<List<MessageStringDrawer>>();
    private VisualElement page;

    public override void RepaintDrawer()
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
        specialExperienceField.SetValueWithoutNotify(value.SpecialExperience);
        favoriteClassesField.SetValueWithoutNotify(value.FavoriteClasses);
    }

    protected override void CreateGUI()
    {
        style.marginTop = 15;
        style.paddingRight = 45;
        RSScrollView scrollView = new RSScrollView();

        base64ImageField = new Base64ImageDrawer();
        base64ImageField.OnValueChanged += () => { value.Base64Icon = base64ImageField.value; InvokeMemberValueChange(base64ImageField); };
        nameField = (MessageStringDrawer)RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("姓　　名：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("姓名為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        nameField.OnValueChanged += () => { value.Name = nameField.value; InvokeMemberValueChange(nameField); };
        majorField = (MessageStringDrawer)RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("系　　級：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("系級為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        majorField.OnValueChanged += () => { value.Major = majorField.value; InvokeMemberValueChange(majorField); };
        careerField = (MessageStringDrawer) RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("工　　作：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("工作為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        careerField.OnValueChanged += () => { value.Career = careerField.value; InvokeMemberValueChange(careerField); };
        genderField = (MessageStringDrawer) RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("性　　別：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("性別為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        genderField.OnValueChanged += () => { value.Gender = genderField.value; InvokeMemberValueChange(genderField); };
        phoneNumberField = (MessageStringDrawer)RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("電話號碼：")
            .AddAttribute(new MessageStringAttribute()
                /*.AddCondition("PhoneNumber can not be empty!", (v) => v != null && v != "")*/)
            .Build();
        phoneNumberField.OnValueChanged += () => { value.PhoneNumber = phoneNumberField.value; InvokeMemberValueChange(phoneNumberField); };
        researchTopicField = (MessageStringDrawer) RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("研究主題：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("研究主題為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        researchTopicField.OnValueChanged += () => { value.ResearchTopic = researchTopicField.value; InvokeMemberValueChange(researchTopicField); };
        contactField = (MessageStringDrawer) RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("聯絡方式：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("聯絡方式為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        contactField.OnValueChanged += () => { value.Contact = contactField.value; InvokeMemberValueChange(contactField); };
        skillsField = (MessageStringDrawer) RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("專　　長：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("專長為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        skillsField.OnValueChanged += () => { value.Skills = skillsField.value; InvokeMemberValueChange(skillsField); };
        hobbiesField = (MessageStringDrawer) RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("愛　　好：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("愛好為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        hobbiesField.OnValueChanged += () => { value.Hobbies = hobbiesField.value; InvokeMemberValueChange(hobbiesField); };
        graduatedSchoolField = (MessageStringDrawer) RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("畢業學校：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("畢業學校為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        graduatedSchoolField.OnValueChanged += () => { value.GraduatedSchool = graduatedSchoolField.value; InvokeMemberValueChange(graduatedSchoolField); };
        specialExperienceField = (MessageStringDrawer)RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("特殊經驗：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("特殊經驗為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        specialExperienceField.OnValueChanged += () => { value.SpecialExperience = specialExperienceField.value; InvokeMemberValueChange(specialExperienceField); };
        favoriteClassesField = (MessageStringDrawer)RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("喜愛專科：")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("喜愛專科為必填，不得為空！", (v) => v != null && v != ""))
            .Build();
        favoriteClassesField.OnValueChanged += () => { value.FavoriteClasses = favoriteClassesField.value; InvokeMemberValueChange(favoriteClassesField); };

        specialExperienceField.EnableMultiline(true);

        pages.Add(generateBasicInfoView());
        pages.Add(generateSecondView());
        pages.Add(generateThirdView());

        //VisualElement container = new VisualElement();
        //container.style.flexDirection = FlexDirection.Row;
        //container.style.marginBottom = 10;

        //VisualElement fieldContainer = new VisualElement();
        //fieldContainer.style.paddingTop = RSTheme.Current.LineHeight;
        //fieldContainer.style.paddingRight = RSTheme.Current.LineHeight*1.5f;
        //fieldContainer.style.flexGrow = 1;
        //fieldContainer.Add(nameField);
        //fieldContainer.Add(majorField);
        //fieldContainer.Add(careerField);

        //container.Add(fieldContainer);
        //container.Add(base64ImageField);

        //scrollView.Add(container);
        //scrollView.Add(genderField);
        //scrollView.Add(phoneNumberField);
        //scrollView.Add(researchTopicField);
        //scrollView.Add(contactField);
        //scrollView.Add(skillsField);
        //scrollView.Add(hobbiesField);
        //scrollView.Add(graduatedSchoolField);
        //scrollView.Add(pages[0]);

        page = pages[0];

        //Add(scrollView);
        Add(page);
    }
    private VisualElement generateBasicInfoView()
    {
        VisualElement container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.marginBottom = 10;

        VisualElement fieldContainer = new VisualElement();
        fieldContainer.style.paddingRight = RSTheme.Current.LineHeight * 1.5f;
        fieldContainer.style.flexGrow = 1;
        fieldContainer.Add(nameField);
        fieldContainer.Add(majorField);
        fieldContainer.Add(genderField);

        container.Add(fieldContainer);
        container.Add(base64ImageField);

        pageDrawers.Add(new List<MessageStringDrawer>
        {
            nameField, majorField, genderField
        });

        return container;
    }

    private VisualElement generateSecondView()
    {
        VisualElement container = new VisualElement();
        container.style.marginBottom = 10;

        container.Add(careerField);
        container.Add(phoneNumberField);
        container.Add(contactField);
        container.Add(graduatedSchoolField);

        pageDrawers.Add(new List<MessageStringDrawer>
        {
            careerField, phoneNumberField, contactField, graduatedSchoolField
        });

        return container;
    }

    private VisualElement generateThirdView()
    {
        VisualElement container = new VisualElement();
        container.style.marginBottom = 10;

        container.Add(researchTopicField);
        container.Add(skillsField);
        container.Add(hobbiesField);
        container.Add(favoriteClassesField);
        container.Add(specialExperienceField);

        pageDrawers.Add(new List<MessageStringDrawer>
        {
            researchTopicField, skillsField, hobbiesField, favoriteClassesField, specialExperienceField
        });

        return container;
    }

    public int PageIndex => pages.IndexOf(page);

    public bool IsPageValid()
    {
        return IsPageValid(PageIndex);
    }

    public bool IsPageValid(int index)
    {
        if (index < 0 || index >= pageDrawers.Count)
        {
            return false;
        }

        var drawers = pageDrawers[index];

        foreach (MessageStringDrawer drawer in drawers)
        {
            if (!drawer.IsValid())
            {
                return false;
            }
        }

        return true;
    }

    public void ShowPageInvalidMessage()
    {
        ShowPageInvalidMessage(PageIndex);
    }

    public void ShowPageInvalidMessage(int index)
    {
        if (index < 0 || index >= pageDrawers.Count)
        {
            return;
        }

        var drawers = pageDrawers[index];

        foreach (MessageStringDrawer drawer in drawers)
        {
            if (!drawer.IsValid())
            {
                drawer.ShowInvalidMessage();
            }
        }
    }

    public bool IsFirstPage()
    {
        return pages.IndexOf(page) == 0;
    }

    public bool IsLastPage()
    {
        return pages[^1] == page;
    }

    public void PrevPage()
    {
        int index = pages.IndexOf(page);

        if (index <= 0)
        {
            return;
        }

        Remove(page);
        page = pages[index - 1];

        Add(page);
    }

    public void NextPage()
    {
        int index = pages.IndexOf(page);

        if (index == pages.Count - 1)
        {
            return;
        }

        if (index != -1)
        {
            Remove(page);
        }

        page = pages[index + 1];

        Add(page);
    }

    public void SetPageIndex(int index)
    {
        if (index < 0 ||  index >= pages.Count)
        {
            throw new ArgumentOutOfRangeException("index");
        }

        Remove(page);

        page = pages[index];

        Add(page);
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
        if (!specialExperienceField.IsValid()) return false;
        if (!favoriteClassesField.IsValid()) return false;

        return true;
    }

    public void ShowAllInvalidMessage()
    {
        nameField.ShowInvalidMessage();
        majorField.ShowInvalidMessage();
        careerField.ShowInvalidMessage();
        genderField.ShowInvalidMessage();
        phoneNumberField.ShowInvalidMessage();
        researchTopicField.ShowInvalidMessage();
        contactField.ShowInvalidMessage();
        skillsField.ShowInvalidMessage();
        hobbiesField.ShowInvalidMessage();
        graduatedSchoolField.ShowInvalidMessage();
        specialExperienceField.ShowInvalidMessage();
        favoriteClassesField.ShowInvalidMessage();
    }

    public void Reset()
    {
        value = new UserData();

        HideAllInvalidMessage();
    }

    public void Reset(UserData data)
    {
        value = data;

        HideAllInvalidMessage();
    }

    public void HideAllInvalidMessage()
    {
        nameField.HideInvalidMessage();
        majorField.HideInvalidMessage();
        careerField.HideInvalidMessage();
        genderField.HideInvalidMessage();
        phoneNumberField.HideInvalidMessage();
        researchTopicField.HideInvalidMessage();
        contactField.HideInvalidMessage();
        skillsField.HideInvalidMessage();
        hobbiesField.HideInvalidMessage();
        graduatedSchoolField.HideInvalidMessage();
        specialExperienceField.HideInvalidMessage();
    }
}

public class Base64ImageDrawer : RuntimeDrawer<string>
{
    private VisualElement preview;
    private Texture2D texture;

    public override void RepaintDrawer()
    {
        // titleElement.style.display = StyleKeyword.None;

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

    protected override void CreateGUI()
    {
        LayoutExpand();
        iconElement.style.display = DisplayStyle.None;
        labelElement.style.unityTextAlign = TextAnchor.MiddleCenter;
        contentContainer.style.alignItems = Align.Center;
        contentContainer.style.marginLeft = 0;

        preview = new VisualElement();

        texture = new Texture2D(1, 1);
        //contentContainer.style.flexDirection = FlexDirection.Row;

        RSButton btChoice = new RSButton("選擇圖片...", () =>
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

    private RSTextElement warningVisual;
    private RSTextElement warningField;

    public void EnableMultiline(bool multiline)
    {
        if (multiline)
        {
            LayoutExpand();
            this.Q<RSTextField>().multiline = true;
            this.Q<RSTextField>().RegisterValueChangedCallback(multilineValueChange);
        }
        else
        {
            LayoutInline();
            this.Q<RSTextField>().multiline = false;
            this.Q<RSTextField>().UnregisterValueChangedCallback(multilineValueChange);
        }
    }

    private void multilineValueChange(ChangeEvent<string> evt)
    {
        LayoutExpand();
        evt.StopPropagation();
    }

    public MessageStringDrawer()
    {
        this.Q<RSTextField>().style.flexGrow = 1;
        this.Q<RSTextField>().multiline = false;


        warningVisual = new RSTextElement();
        warningVisual.style.backgroundColor = RSTheme.Current.WarningColorSet.BackgroundColor;
        warningField = warningVisual.Q<RSTextElement>();
        warningVisual.style.visibility = Visibility.Hidden;

        Add(warningVisual);

        RegisterCallback<FocusOutEvent>((evt) =>
        {
            ShowInvalidMessage();
        });
    }

    public void HideInvalidMessage()
    {
        warningVisual.style.visibility = Visibility.Hidden;
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

    public void UpdateWarningMessage()
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
