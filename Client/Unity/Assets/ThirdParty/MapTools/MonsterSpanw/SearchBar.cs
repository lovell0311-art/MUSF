using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 可编辑 可输入 下拉框 带文本联想
/// </summary>
public class SearchBar : MonoBehaviour
{
    private Dropdown dropdown => transform.Find("Dropdown").GetComponent<Dropdown>();
    private InputField InputField => transform.Find("InputField").GetComponent<InputField>();

    private void HideInputField() 
    {
        InputField.GetComponent<Image>().color = new Color(1,1,1,0);
        InputField.placeholder.gameObject.SetActive(false);
        InputField.textComponent.gameObject.SetActive(false);
    }

    private void ShowInputField() 
    {
        InputField.GetComponent<Image>().color = new Color(1,1,1,1);
        InputField.placeholder.gameObject.SetActive(true);
        InputField.textComponent.gameObject.SetActive(true);
    }

    public void SetPlachoder(string txt)
    {
        InputField.placeholder.GetComponent<Text>().text = txt;
    }

    public List<string> libraryList = new List<string>();
    public void SetLibraryList(List<string> list)
    {
        libraryList = list;
    }
    private List<string> ResultList = new List<string>();

    private void Start()
    {

        Init();
        dropdown.onValueChanged.AddListener((value)=> 
        {
            InputField.text = dropdown.transform.Find("Label").GetComponent<Text>().text;
            HideInputField();
        });
        InputField.onEndEdit.AddListener((value)=>
        {
            Filter();
            ShowResult();
        });
    }
    private void Update()
    {
        if (InputField.isFocused && InputField.placeholder.gameObject.activeSelf == false)
        {
            ShowInputField();
        }
    }

    private void Init() 
    {
        libraryList.ForEach(i=> ResultList.Add(i));
        SetPlachoder("请输入....");
    }
    public void Clear() 
    {
        libraryList.Clear();
        InputField.placeholder.GetComponent<Text>().text = string.Empty;
    }
    private void Filter() 
    {
        ResultList = TextLenovo(InputField.textComponent.text,libraryList);
    }

    public void ShowResult()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(ResultList);
        if (ResultList.Count != 0)
        {
            dropdown.Show();
        }
    }
    /// <summary>
    /// 文本联想
    /// </summary>
    /// <param 传入的字符="text_item"></param>
    /// <param 库数组="TextLibraryList"></param>
    /// <param 返回结果数组="temp_list"></param>
    /// <returns></returns>
    public List<string> TextLenovo(string text_item, List<string> TextLibraryList)
    {
        List<string> temp_list = new List<string>();
        temp_list.Add(string.Empty);
        foreach (string item in TextLibraryList)
        {
            if (item.Contains(text_item))
            {
                temp_list.Add(item);
            }
        }
        return temp_list;
    }
}
