using NaiveAPI.DocumentBuilder;
using NaiveAPI.RuntimeWindowUtils;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CustomRuntimeDrawer(typeof(UserData), Priority = 10)]
public class UserDataDrawer : RuntimeDrawer<UserData>
{
    private StringDrawer nameField;
    private StringDrawer phoneNumberField;
    private StringDrawer degreeField;
    private StringDrawer careerField;
    private StringDrawer genderField;
    private StringDrawer researchTopicField;
    private StringDrawer contactField;
    private StringDrawer skillsField;
    private StringDrawer habitsField;
    private StringDrawer graduatedSchoolField;

    public override void UpdateField()
    {
        nameField.SetValueWithoutNotify(value.Name);
    }

    public string Name = "";
    public string PhoneNumber = "";
    public string Degree = "";
    public string Career = "";
    public string Gender = "";
    public string ResearchTopic = "";
    public string Contact = "";
    public string Skills = "";
    public string Habits = "";
    public string GraduatedSchool = "";

    protected override void OnCreateGUI()
    {
        nameField = (MessageStringDrawer) RuntimeDrawerFactory
            .FromValueType(typeof(string))
            .Label("Name")
            .AddAttribute(new MessageStringAttribute()
                .AddCondition("Name {0} is already existed", (v) => UserDataHandler.FindByName(v) == null)
                .AddCondition("Name can not be empty!", (v) => v != null && v != ""))
            .Build();
        nameField.RegisterValueChangedCallback((evt) => value.Name = evt.newValue);
        phoneNumberField = new StringDrawer() { label = "PhoneName" };
        degreeField = new StringDrawer() { label = "Degree" };
        careerField = new StringDrawer() { label = "Career" };
        genderField = new StringDrawer() { label = "Gender" };
        researchTopicField = new StringDrawer() { label = "ResearchTopic" };
        contactField = new StringDrawer() { label = "Contact" };
        skillsField = new StringDrawer() { label = "Skills" };
        habitsField = new StringDrawer() { label = "Habits" };
        graduatedSchoolField = new StringDrawer() { label = "GraduatedSchool" };

        nameField.style.SetIS_Style(new ISBorder(Color.red, 5));

        Add(nameField);
        Add(phoneNumberField);
        Add(degreeField);
        Add(careerField);
        Add(genderField);
        Add(researchTopicField);
        Add(contactField);
        Add(skillsField);
        Add(habitsField); 
        Add(graduatedSchoolField);
    }
}

[CustomRuntimeDrawer(typeof(string), Priority = 10, RequiredAttribute = typeof(MessageStringAttribute))]
public class MessageStringDrawer : StringDrawer
{
    public MessageStringAttribute attribute;

    private DSTextElement warningField;

    public MessageStringDrawer()
    {
        this.Q<DSTextField>().style.flexGrow = 1;
        this.Q<DSTextField>().multiline = false;
        contentContainer.style.flexDirection = FlexDirection.Row;


        warningField = new DSTextElement();
        warningField.style.display = DisplayStyle.None;

        Add(warningField);

        RegisterCallback<FocusOutEvent>((evt) =>
        {
            warningField.style.display = DisplayStyle.None;
            foreach ((string message, var checkValid) in attribute.Conditions)
            {
                if (!checkValid(this.value))
                {
                    warningField.text = string.Format(message, this.value);
                    warningField.style.display = DisplayStyle.Flex;
                    break;
                }
            }
        });
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
