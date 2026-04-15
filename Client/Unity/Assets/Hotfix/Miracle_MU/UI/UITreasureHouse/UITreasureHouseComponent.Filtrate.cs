using ETModel;
using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    //筛选
    public partial class UITreasureHouseComponent
    {
        public ReferenceCollector filtrateCollector;
        public Dropdown ClassDropdown, OutstandingDropdown, EnhanceDropdown, SuitDropdown;
        public Toggle CrystalToggle;
        public GameObject Souch;
        public Button SouchBtn;
        public Image SouchBtnHind;
        public FiltrateData filtrateData = new FiltrateData();

        List<string> optionDatas = new List<string>() { "(卓越)否", "一条卓越","两条卓越","三条卓越","四条卓越","五条卓越" };
        
        //初始化筛选
        public void Filtrate()
        {
            filtrateCollector = collector.GetImage("FiltrateType").gameObject.GetReferenceCollector();
            //职业
            ClassDropdown = filtrateCollector.GetImage("ClassDropdown").gameObject.GetComponent<Dropdown>();
            //卓越
            OutstandingDropdown = filtrateCollector.GetImage("OutstandingDropdown").gameObject.GetComponent<Dropdown>();
            OutstandingDropdown.ClearOptions();
            OutstandingDropdown.AddOptions(optionDatas);
           
            //强化
            EnhanceDropdown = filtrateCollector.GetImage("EnhanceDropdown").gameObject.GetComponent<Dropdown>();
            //套装
            SuitDropdown = filtrateCollector.GetImage("SuitDropdown").gameObject.GetComponent<Dropdown>();
            //魔晶
            CrystalToggle = filtrateCollector.GetToggle("CrystalToggle");
            //魔晶
            Souch = filtrateCollector.GetGameObject("Souch");

            SouchBtn = Souch.transform.Find("SouchBtn").GetComponent<Button>();
            SouchBtnHind = Souch.transform.Find("SouchBtnHind").GetComponent<Image>();
            InitFiltrate();
        }
        //重置
        public void SetInitFiltrate(FiltrateData filtrate)
        {
            filtrate.Name = string.Empty;//道具名称
            filtrate.RoleClass = "";//职业类型
            filtrate.Excellent = 0;//卓越条数
            filtrate.Enhance = 0;//强化等级
            filtrate.Readdition = 0;//套装，1是套装，2是洞装，3是一起
            filtrate.MaxType = 1;//道具所在大类
            filtrate.SortType = 0;//排序方式0降序1升序默认降序

            SearchInput.text = string.Empty; 
            LastInputContent = string.Empty;
            SouchHideShowBtn(true);
            ClassDropdown.SetValueWithoutNotify(0);
            OutstandingDropdown.SetValueWithoutNotify(0);
            EnhanceDropdown.SetValueWithoutNotify(0);
            SuitDropdown.SetValueWithoutNotify(0);
            CrystalToggle.transform.Find("Background/Checkmark/Down").gameObject.SetActive(true);
            CrystalToggle.transform.Find("Background/Checkmark/Up").gameObject.SetActive(false);
        }
        //筛选
        public void InitFiltrate()
        {
            ClassDropdown.onValueChanged.AddListener((value) =>
            {
                filtrateData.RoleClass = value.ToString()/*ClassDropdown.options[value].text*/;
                SouchHideShowBtn(false);
            });
            OutstandingDropdown.onValueChanged.AddListener((value) =>
            {
               // filtrateData.Excellent = value + 3 - 1 == 2 ? 0 : value + 3 - 1;
                filtrateData.Excellent = value;
                SouchHideShowBtn(false);
            });
            EnhanceDropdown.onValueChanged.AddListener((value) =>
            {
                filtrateData.Enhance = value + 3 - 1 == 2 ? 0 : value + 3 - 1;
                SouchHideShowBtn(false);
            });
            SuitDropdown.onValueChanged.AddListener((value) =>
            {
                filtrateData.Readdition = value;
                SouchHideShowBtn(false);
            });
            CrystalToggle.onValueChanged.AddSingleListener((value) =>
            {
                filtrateData.SortType = value ? 0 : 1;
                CrystalToggle.transform.Find("Background/Checkmark/Down").gameObject.SetActive(value);
                CrystalToggle.transform.Find("Background/Checkmark/Up").gameObject.SetActive(!value);
                SouchHideShowBtn(false);
            });
            SouchBtn.onClick.AddSingleListener(() =>
            {
                SearchForItems(filtrateData).Coroutine();
            });
        }

        public void SouchHideShowBtn(bool isShow)
        {
            SouchBtnHind.gameObject.SetActive(isShow);
        }

    }

}
