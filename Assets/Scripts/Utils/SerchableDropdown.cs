using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SerchableDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public TMP_InputField inputField;

    private List<TMP_Dropdown.OptionData> defaultOptions;

    private bool validSelection = false;


    // Start is called before the first frame update
    void Start()
    {
        defaultOptions = dropdown.options;
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
        validSelection = false;
        dropdown.options = defaultOptions.FindAll(x => x.text.ToLower().Contains(value.ToLower()));
        Deselect();
        HideAndRefocus();
    }

    public void UpdateField()
    {
        if (dropdown.captionText.text == "") return;
         
        //showDropdown = false;
        inputField.text = dropdown.captionText.text;
        validSelection = true;
    }

    public void ShowAndRefocus()
    {
        int oldCarretPosition = inputField.caretPosition;
        dropdown.Show();
        inputField.ActivateInputField();
        inputField.caretPosition = oldCarretPosition;
    }

    public void HideAndRefocus()
    {
        int oldCarretPosition = inputField.caretPosition;
        dropdown.Hide();
        inputField.ActivateInputField();
        inputField.caretPosition = oldCarretPosition;
    }

    private void Deselect()
    {
        // Add a blank dropdown option you will then remove at the end of the options list
        dropdown.options.Add(new TMP_Dropdown.OptionData() { text = "" });
        // Select it
        dropdown.value = dropdown.options.Count - 1;
        // Remove it    
        dropdown.options.RemoveAt(dropdown.options.Count - 1);
        // Done!
    }

    public string getSelection()
    {
        return validSelection ? inputField.text : "";
    }
}
