using Codice.Client.BaseCommands.BranchExplorer;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.RuntimeWindowUtils
{
    public class RuntimeCommandField : VisualElement
    {
        public ChoiceContainer Container;
        private DSHorizontal commandsContainer;
        private RuntimeMethodField commandField;
        private List<RuntimeParamField> paramFields;
        private RuntimeCommandInfo commandInfo;

        private List<CommandLog> logs;
        private int logIndex = -1;

        public RuntimeCommandField()
        {
            Container = new ChoiceContainer();
            commandsContainer = new DSHorizontal();
            commandField = new RuntimeMethodField(this, "", RuntimeCommandHandler.ActiveMethods
                                                .Select((method) => method.Attribute.Name ?? method.Info.Name).ToList());
            paramFields = new List<RuntimeParamField>();
            logs = new List<CommandLog>();

            commandField.style.minWidth = DocStyle.Current.LabelWidth;
            commandField.FieldFinish = (name) =>
            {
                generateParameterVisual(name);

                if (!FocusNext(commandField))
                {
                    commandField.FocusToLast();
                }
            };

            commandsContainer.Add(commandField);

            Add(Container);
            Add(commandsContainer);
        }

        public bool FocusNext(RuntimeChoiceFocusField focusing)
        {
            if (focusing == commandField)
            {
                generateParameterVisual(name);

                if (paramFields.Count > 0)
                {
                    paramFields[0].FocusToFirst();

                    return true;
                }
            }
            else
            {
                for (int i = 0; i < paramFields.Count - 1; i++)
                {
                    if (focusing == paramFields[i])
                    {
                        paramFields[i + 1].FocusToFirst();

                        return true;
                    }
                }
            }

            return false;
        }

        public bool FocusPrev(RuntimeChoiceFocusField focusing)
        {
            if (paramFields.Count > 0)
            {
                if (focusing == paramFields[0])
                {
                    commandField.FocusToLast();

                    return true;
                }
                else
                {
                    for (int i = 1; i < paramFields.Count; i++)
                    {
                        if (focusing == paramFields[i])
                        {
                            paramFields[i - 1].FocusToLast();

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void SetToLog()
        {
            if (logIndex >= 0 && logIndex < logs.Count)
            {
                CommandLog log = logs[logIndex];
                commandField.SetValueWithoutNotify(log.Command);

                generateParameterVisual(log.Command);

                for (int i = 0;i < log.Args.Length; i++)
                {
                    paramFields[i].SetValueWithoutNotify(log.Args[i]);
                }
            }
            else if (logIndex == -1)
            {
                clearField();
            }
        }

        public void NextLog()
        {
            if (logIndex < logs.Count - 1)
            {
                logIndex++;
            }
        }

        public void PrevLog()
        {
            if (logIndex > -1)
            {
                logIndex--;
            }
        }

        public void Invoke()
        {
            string command = commandField.text;
            string[] paramValues = new string[paramFields.Count];

            for (int i = 0; i < paramValues.Length; i++)
            {
                paramValues[i] = paramFields[i].text;
            }

            Container.SetEnable(false);

            logIndex = -1;

            logs.Insert(0, new CommandLog(command, paramValues));

            clearField();

            setCommandInfo(command);

            if (commandInfo == null)
            {
                Debug.Log($"No match command: '{command}'");
                return;
            }

            object[] args = null;

            ParameterInfo[] parameters = commandInfo.Info.GetParameters();
            if (parameters.Length > 0)
            {
                args = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    Type paramType = parameters[i].ParameterType;

                    if (!commandInfo.ToObject(parameters[i].Name, paramType, paramValues[i], out args[i]))
                    {
                        Debug.Log($"Invalid Param: '{command}' in {i}");
                        return;
                    }
                }
            }

            commandInfo.Info.Invoke(null, args);
        }

        private void clearField()
        {
            commandField.SetValueWithoutNotify("");
            
            for (int i = 0;i < paramFields.Count; i++)
            {
                paramFields[i].SetValueWithoutNotify("");
                commandsContainer.Remove(paramFields[i]);
            }

            paramFields.Clear();
        }

        private void generateParameterVisual(string name)
        {
            setCommandInfo(name);

            generateParameterVisual();
        }

        private void generateParameterVisual()
        {
            if (commandInfo == null) return;

            commandsContainer.Clear();
            paramFields.Clear();

            commandsContainer.Add(commandField);

            ParameterInfo[] parameters = commandInfo.Info.GetParameters();

            for (int i = 0;i < parameters.Length; i++)
            {
                ParameterInfo param = parameters[i];
                RuntimeParamField textField = new RuntimeParamField(this, 
                    "Type: " + TypeReader.GetName(param.ParameterType) + " I'm a hint, not just a type display. (angry)(angry)"
                    , commandInfo.GetCommandInfoChoice(i));
                textField.Type = param.ParameterType;
                //textField.style.minWidth = DocStyle.Current.LabelWidth;

                textField.FieldFinish = (choice) =>
                {
                    if (!FocusNext(textField))
                    {
                        textField.FocusToLast();
                    }
                };

                commandsContainer.Add(textField);
                paramFields.Add(textField);
            }
        }

        private void setCommandInfo(string name)
        {
            commandInfo = RuntimeCommandHandler.GetCommandInfo(name);
        }

        public class ChoiceContainer : VisualElement
        {
            private DSScrollView choiceContainer;
            private List<DSTextElement> choiceTextElement;
            private int navigation = 0;
            public int Navigation => navigation;
            public ChoiceContainer()
            {
                choiceContainer = new DSScrollView();
                choiceTextElement = new List<DSTextElement>();
                style.display = DisplayStyle.None;
                style.backgroundColor = DocStyle.Current.BackgroundColor;
            }

            public void SetChoiceContainerFocus(RuntimeChoiceFocusField focusing)
            {
                Clear();

                if (focusing.GetType() == typeof(RuntimeParamField))
                {
                    Type paramType = (focusing as RuntimeParamField).Type;

                    if (paramType != null)
                    {
                        Add(new DSTypeNameElement(paramType));
                    }
                }

                if (focusing.Hint != "")
                    Add(new DSTextElement(focusing.Hint));

                choiceContainer.Clear();
                choiceTextElement.Clear();
                foreach (string choice in focusing.Choice)
                {
                    DSTextElement textElement = new DSTextElement(choice);
                    textElement.RegisterCallback<PointerDownEvent>((evt) =>
                    {
                        focusing.SetFinishValue(choice);
                    });
                    textElement.RegisterCallback<PointerEnterEvent>((evt) =>
                    {
                        textElement.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
                    });
                    textElement.RegisterCallback<PointerLeaveEvent>((evt) =>
                    {
                        textElement.style.backgroundColor = Color.clear;
                    });

                    choiceTextElement.Add(textElement);
                    choiceContainer.Add(textElement);
                }

                Add(choiceContainer);

                SetSelecting(choiceTextElement.Count - 1);
            }

            public void SetOrder(RuntimeChoiceFocusField focusing)
            {
                int index = 0;
                foreach (string choice in focusing.Choice.OrderBy((choice) => -DocumentBuilderParser.LevenshteinDistance(choice, focusing.text)))
                {
                    choiceTextElement[index++].text = choice;
                }

                SetSelecting(choiceTextElement.Count - 1);
            }

            public void SetSelecting(int select)
            {
                if (select < 0) select = 0;
                if (select >= choiceTextElement.Count) select = choiceTextElement.Count - 1;

                if (choiceTextElement.Count == 0) return;

                navigation = select;

                for (int i = 0; i < choiceTextElement.Count; i++)
                {
                    choiceTextElement[i].style.backgroundColor = Color.clear;
                }

                choiceTextElement[navigation].style.backgroundColor = DocStyle.Current.SubBackgroundColor;

                schedule.Execute(() => choiceContainer.ScrollTo(choiceTextElement[navigation])).ExecuteLater(1);
            }

            public string GetSelectingValue()
            {
                return choiceTextElement[navigation].text;
            }

            public void SetEnable(bool enable = true)
            {
                style.display = enable ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        public class CommandLog
        {
            public string Command;
            public string[] Args;

            public CommandLog(string command, string[] args)
            {
                Command = command;
                Args = args;
            }
        }
    }

    public abstract class RuntimeChoiceFocusField : DSTextField
    {
        public string Hint;
        public IEnumerable<string> Choice;

        public Action<string> FieldFinish;

        public virtual void SetFinishValue(string value)
        {
            this.SetValueWithoutNotify(value);

            this.FieldFinish?.Invoke(value);
        }

        public virtual void FocusTo(int index)
        {
            schedule.Execute(() =>
            {
                this[0].Focus();
                SelectRange(index, index);
            }).ExecuteLater(1);
        }

        public virtual void FocusToFirst()
        {
            FocusTo(0);
        }

        public virtual void FocusToLast()
        {
            FocusTo(text.Length);
        }
    }

    public class RuntimeMethodField : RuntimeChoiceFocusField
    {
        private RuntimeCommandField commandField;
        private RuntimeCommandField.ChoiceContainer choiceView;
        private bool usingLog = false;
        public RuntimeMethodField(RuntimeCommandField commandField) : this(commandField, "", Enumerable.Empty<string>()) { }
        public RuntimeMethodField(RuntimeCommandField commandField, string paramHint) : this(commandField, paramHint, Enumerable.Empty<string>()) { }

        public RuntimeMethodField(RuntimeCommandField commandField, string paramHint, IEnumerable<string> paramChoice)
        {
            this.commandField = commandField;
            this.choiceView = commandField.Container;
            this.Hint = paramHint;
            this.Choice = paramChoice;

            this.RegisterCallback<FocusInEvent>(onFocusIn);

            this.RegisterValueChangedCallback(evt =>
            {
                evt.StopPropagation();

                usingLog = false;

                choiceView.SetEnable(true);

                choiceView.SetOrder(this);
            });

            this.RegisterCallback<FocusOutEvent>(onFocusOut);
        }

        private void onFocusIn(FocusInEvent evt)
        {
            choiceView.SetChoiceContainerFocus(this);

            RegisterCallback<KeyDownEvent>(onKeyDown);
        }

        private void onFocusOut(FocusOutEvent evt)
        {
            UnregisterCallback<KeyDownEvent>(onKeyDown);

            choiceView.SetEnable(false);
        }

        private void onKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.UpArrow)
            {
                if (text == "" || usingLog)
                {
                    usingLog = true;
                    commandField.NextLog();
                    commandField.SetToLog();
                }
                else
                {
                    choiceView.SetSelecting(choiceView.Navigation - 1);
                }
            }
            else if (evt.keyCode == KeyCode.DownArrow)
            {
                if (text == "" || usingLog)
                {
                    usingLog = true;
                    commandField.PrevLog();
                    commandField.SetToLog();
                }
                else
                {
                    choiceView.SetSelecting(choiceView.Navigation + 1);
                }
            }
            else if (evt.keyCode == KeyCode.RightArrow)
            {
                if (cursorIndex == text.Length)
                {
                    this.FieldFinish?.Invoke(text);
                }
            }
            else if (evt.keyCode == KeyCode.Tab)
            {
                SetFinishValue(choiceView.GetSelectingValue());
            }
            else if (evt.keyCode == KeyCode.Space)
            {
                this.FieldFinish?.Invoke(text);
            }
            else if (evt.keyCode == KeyCode.Return)
            {
                commandField.Invoke();
            }
        }
    }

    public class RuntimeParamField : RuntimeChoiceFocusField
    {
        private RuntimeCommandField commandField;
        private RuntimeCommandField.ChoiceContainer choiceView;
        public Type Type;

        public RuntimeParamField(RuntimeCommandField commandField) : this(commandField, "", Enumerable.Empty<string>()) { }
        public RuntimeParamField(RuntimeCommandField commandField, string paramHint) : this(commandField, paramHint, Enumerable.Empty<string>()) { }

        public RuntimeParamField(RuntimeCommandField commandField, string paramHint, IEnumerable<string> paramChoice)
        {
            this.commandField = commandField;
            this.choiceView = commandField.Container;
            this.Hint = paramHint;
            this.Choice = paramChoice;

            this.RegisterCallback<FocusInEvent>(onFocusIn);

            this.RegisterValueChangedCallback(evt =>
            {
                evt.StopPropagation();

                choiceView.SetOrder(this);
            });

            this.RegisterCallback<FocusOutEvent>(onFocusOut);
        }

        private void onFocusIn(FocusInEvent evt)
        {
            choiceView.SetEnable(true);

            choiceView.SetChoiceContainerFocus(this);

            RegisterCallback<KeyDownEvent>(onKeyDown);
        }

        private void onFocusOut(FocusOutEvent evt)
        {
            UnregisterCallback<KeyDownEvent>(onKeyDown);

            choiceView.SetEnable(false);
        }

        private void onKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.UpArrow)
            {
                choiceView.SetSelecting(choiceView.Navigation - 1);
            }
            else if (evt.keyCode == KeyCode.DownArrow)
            {
                choiceView.SetSelecting(choiceView.Navigation + 1);
            }
            else if (evt.keyCode == KeyCode.LeftArrow)
            {
                if (cursorIndex == 0)
                {
                    commandField.FocusPrev(this);
                }
            }
            else if (evt.keyCode == KeyCode.RightArrow)
            {
                if (cursorIndex == text.Length)
                {
                    commandField.FocusNext(this);
                }
            }
            else if (evt.keyCode == KeyCode.Backspace)
            {
                if (text == "")
                {
                    commandField.FocusPrev(this);
                }
            }
            else if (evt.keyCode == KeyCode.Tab)
            {
                if (Choice.Count() > 0)
                {
                    SetFinishValue(choiceView.GetSelectingValue());
                }
            }
            else if (evt.keyCode == KeyCode.Space)
            {
                this.FieldFinish?.Invoke(text);
            }
            else if (evt.keyCode == KeyCode.Return)
            {
                commandField.Invoke();
            }
        }
    }
}
