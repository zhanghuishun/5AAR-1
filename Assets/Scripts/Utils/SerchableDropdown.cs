using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;
using UnityEngine.UI;

public class SerchableDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public TMP_InputField inputField;

    public int minSerchCharactersToDisplayOptions = 7;
    public int maxResultsIfSearchTooShort = 100;

    private List<TMP_Dropdown.OptionData> defaultOptions;
    private List<TMP_Dropdown.OptionData> currentOptions;

    private bool validSelection = false;

    private bool loadingOptions = true;

    public bool firstEditDone = false;


    // Start is called before the first frame update
    void Start()
    {
        defaultOptions = dropdown.options;
        currentOptions = dropdown.options;
        Deselect();
    }

    // Update is called once per frame
    void Update()
    {
        if(inputField.isFocused && !dropdown.IsExpanded)
        {
            ShowAndRefocus();
        }
    }

    public void UpdateSearch(string value)
    {
        firstEditDone = true;
        validSelection = false;
        _UpdateOptions(defaultOptions.FindAll(x => x.text.ToLower().Contains(value.ToLower())));
    }

    private void _UpdateOptions(List<TMP_Dropdown.OptionData> optionDatas)
    {
        currentOptions = optionDatas;
        dropdown.options = optionDatas;
        Deselect();
        HideAndRefocus();
    }

    public void UpdateOptions(List<TMP_Dropdown.OptionData> optionDatas)
    {
        loadingOptions = false;
        _UpdateOptions(optionDatas);
    }

    public void UpdateDefaultOptions(List<TMP_Dropdown.OptionData> optionDatas)
    {
        defaultOptions = optionDatas;
        UpdateOptions(optionDatas);
    }

    public void UpdateField()
    {
        if (dropdown.captionText.text == "") return;
         
        inputField.text = dropdown.captionText.text;
        validSelection = true;
    }

    public void ShowAndRefocus()
    {
        _UpdateOptions(defaultOptions.FindAll(x => x.text.ToLower().Contains(inputField.text.ToLower())));
        bool tooManyOptions = inputField.text.Length < minSerchCharactersToDisplayOptions && currentOptions.Count > maxResultsIfSearchTooShort;
        if (tooManyOptions)
        {
            dropdown.options = new List<TMP_Dropdown.OptionData>();
            dropdown.options.Add(new TMP_Dropdown.OptionData("Loading..."));
            Deselect();
        }
        bool refocus = inputField.isFocused;
        int oldCarretPosition = inputField.caretPosition;
        /*Color oldCursorColor = GUI.skin.settings.cursorColor;
        if (refocus)
        {
            GUI.skin.settings.cursorColor = new Color(0, 0, 0, 0);
        }*/
        dropdown.Show();
        if(tooManyOptions || loadingOptions) dropdown.transform.Find("Dropdown List").GetComponentsInChildren<Toggle>()[0].interactable = false;
        if (refocus)
        {
            inputField.ActivateInputField();
            inputField.caretPosition = oldCarretPosition;
            //GUI.skin.settings.cursorColor = oldCursorColor;
        }
    }

    public void HideAndRefocus()
    {
        bool refocus = inputField.isFocused;
        int oldCarretPosition = inputField.caretPosition;
        /*Color oldCursorColor = GUI.skin.settings.cursorColor;
        if (refocus)
        {
            GUI.skin.settings.cursorColor = new Color(0, 0, 0, 0);
        }*/
        dropdown.Hide();
        if (refocus)
        {
            inputField.ActivateInputField();
            inputField.caretPosition = oldCarretPosition;
            //GUI.skin.settings.cursorColor = oldCursorColor;
        }
    }

    private void Deselect()
    {
        dropdown.options.Add(new TMP_Dropdown.OptionData() { text = "" });
        dropdown.value = dropdown.options.Count - 1;  
        dropdown.options.RemoveAt(dropdown.options.Count - 1);
    }

    public string getSelection()
    {
        return validSelection ? inputField.text : null;
    }
}
