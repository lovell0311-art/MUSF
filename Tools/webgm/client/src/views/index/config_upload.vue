<template>
    <div>
        <div class="container">
            <div class="handle-box">
                <el-upload
                    action="#"
                    :limit="1"
                    accept=".xlsx"
                    :show-file-list="false"
                    :before-upload="beforeUpload_ItemData"
                    :http-request="handleMany_ItemData"
                >
                    <el-button class="mr10" type="success">配置导入(W物品.xlsx)</el-button>
                </el-upload>
            </div>
            <div class="handle-box">
                <el-upload
                    action="#"
                    :limit="1"
                    accept=".xlsx"
                    :show-file-list="false"
                    :before-upload="beforeUpload_AttrEntryData"
                    :http-request="handleMany_AttrEntryData"
                >
                    <el-button class="mr10" type="success">配置导入(W物品属性.xlsx)</el-button>
                </el-upload>
            </div>
            <div class="handle-box">
                <el-upload
                    action="#"
                    :limit="1"
                    accept=".xlsx"
                    :show-file-list="false"
                    :before-upload="beforeUpload_ItemSetData"
                    :http-request="handleMany_ItemSetData"
                >
                    <el-button class="mr10" type="success">配置导入(W物品套装.xlsx)</el-button>
                </el-upload>
            </div>
            <div class="handle-box">
                <el-upload
                    action="#"
                    :limit="1"
                    accept=".xlsx"
                    :show-file-list="false"
                    :before-upload="beforeUpload_SocketData"
                    :http-request="handleMany_SocketData"
                >
                    <el-button class="mr10" type="success">配置导入(X镶嵌.xlsx)</el-button>
                </el-upload>
            </div>
            <div class="handle-box">
                <el-upload
                    action="#"
                    :limit="1"
                    accept=".xlsx"
                    :show-file-list="false"
                    :before-upload="beforeUpload_SkillData"
                    :http-request="handleMany_SkillData"
                >
                    <el-button class="mr10" type="success">配置导入(J技能.xlsx)</el-button>
                </el-upload>
            </div>
            <div class="handle-box">
                <el-upload
                    action="#"
                    :limit="1"
                    accept=".xlsx"
                    :show-file-list="false"
                    :before-upload="beforeUpload_TitleData"
                    :http-request="handleMany_TitleData"
                >
                    <el-button class="mr10" type="success">配置导入(C称号.xlsx)</el-button>
                </el-upload>
            </div>
        </div>
    </div>
</template>

<script>
import { UploadProps } from 'element-plus';
import { ref, reactive } from 'vue';
import * as XLSX from 'xlsx';
import {ReadXlsxConfig} from '../../game/xlsx_config';
import { ElMessage } from 'element-plus';

export default {
    setup(){
        let importList = [];

        return {
            importList,
        };

    },
    methods:
    {
        async beforeUpload_ItemData(rawFile){
            this.importList =await ReadXlsxConfig(rawFile,["装备","翅膀","项链","戒指","手环","耳环","宝石","坐骑","荧光宝石","技能书|石","守护","消耗品(血瓶|药水|实力提升卷轴)","其他物品","旗帜","宠物"]);
            return true;
        },
        async handleMany_ItemData(){
            // 把数据传给服务器后获取最新列表，这里只是示例，不做请求
            var itemMap = {};
            this.importList.forEach((item, index) => {
                var useRole = {};
                if(item['UseRole'])
                {
                    useRole = JSON.parse(item['UseRole']);
                }else{
                    useRole = {
                        "1" : 0,
                        "2" : 0,
                        "3" : 0,
                        "4" : 0,
                        "5" : 0,
                        "6" : 0,
                        "7" : 0,
                        "8" : 0,
                        "9" : 0,
                        "10" : 0,
                    };
                }
                useRole["0"] = 0;
                for(var i=1;i<=8;i++)
                {
                    useRole[String(i)] =  useRole[String(i)] != null ? useRole[String(i)] : -1;
                }
                itemMap[item['Id']] = {
                    Id: item['Id'],
                    Name: item['Name'],
                    Type: Math.floor(item['Id']/10000),
                    Slot: item['Slot'],
                    Skill: item['Skill'],
                    X: item['X'],
                    Y: item['Y'],
                    StackSize: item['StackSize'],
                    Level: item['Level'],
                    QualityAttr: item['QualityAttr'],
                    AppendAttrId: item['AppendAttrId'] != null ? JSON.parse(item['AppendAttrId']):[],
                    ExtraAttrId: item['ExtraAttrId'] != null ? JSON.parse(item['ExtraAttrId']):[],
                    SpecialAttrId: item['SpecialAttrId'] != null ? JSON.parse(item['SpecialAttrId']):[],
                    UseRole:useRole,
                };
            });

            let code = "module.exports = "+JSON.stringify(itemMap,(key,value)=>{return value;},2) + ";";
            this.download(code,"item_config.js");
        },
        async beforeUpload_AttrEntryData(rawFile){
            this.importList =await ReadXlsxConfig(rawFile,["基础属性","套装属性","卓越属性","再生属性","380属性","特殊属性","额外属性","追加属性"]);
            return true;
        },
        async handleMany_AttrEntryData(){
            var entryAttrMap = {};
            this.importList.forEach((item, index) => {
                entryAttrMap[item['Id']] = {
                    Id: item['Id'],
                    Name:item['Name'],
                    EntryType:item['EntryType'] != null ? item['EntryType'] : 0,
                    Value:[
                        item['Value0'] != null ? item['Value0'] : 0,
                        item['Value1'] != null ? item['Value1'] : 0,
                        item['Value2'] != null ? item['Value2'] : 0,
                        item['Value3'] != null ? item['Value3'] : 0,
                        item['Value4'] != null ? item['Value4'] : 0,
                        item['Value5'] != null ? item['Value5'] : 0,
                        item['Value6'] != null ? item['Value6'] : 0,
                        item['Value7'] != null ? item['Value7'] : 0,
                        item['Value8'] != null ? item['Value8'] : 0,
                        item['Value9'] != null ? item['Value9'] : 0,
                        item['Value10'] != null ? item['Value10'] : 0,
                        item['Value11'] != null ? item['Value11'] : 0,
                        item['Value12'] != null ? item['Value12'] : 0,
                        item['Value13'] != null ? item['Value13'] : 0,
                        item['Value14'] != null ? item['Value14'] : 0,
                        item['Value15'] != null ? item['Value15'] : 0,
                    ]
                };
            });

            let code = "module.exports = "+JSON.stringify(entryAttrMap,(key,value)=>{return value;},2) + ";";
            this.download(code,"attrentry_config.js");
        },
        async beforeUpload_ItemSetData(rawFile){
            this.importList =await ReadXlsxConfig(rawFile,["套装类型"]);
            return true;
        },
        async handleMany_ItemSetData(){
            var itemSetMap = {};
            this.importList.forEach((item, index) => {
                itemSetMap[item['Id']] = {
                    Id: item['Id'],
                    SetName:item['SetName'],
                    ItemsId:JSON.parse(item['ItemsId']),
                };
            });
            let code = "module.exports = "+JSON.stringify(itemSetMap,(key,value)=>{return value;},2) + ";";
            this.download(code,"itemset_config.js");
        },
        async beforeUpload_SocketData(rawFile){
            this.importList =await ReadXlsxConfig(rawFile,["荧光属性"]);
            return true;
        },
        async handleMany_SocketData(){
            var entryAttrMap = {};
            this.importList.forEach((item, index) => {
                entryAttrMap[item['Id']] = {
                    Id: item['Id'],
                    Info:item['Info'],
                    Fluore:item['fluore'],
                    Level:[
                        item['Level0'] != null ? item['Level0'] : 0,
                        item['Level1'] != null ? item['Level1'] : 0,
                        item['Level2'] != null ? item['Level2'] : 0,
                        item['Level3'] != null ? item['Level3'] : 0,
                        item['Level4'] != null ? item['Level4'] : 0,
                        item['Level5'] != null ? item['Level5'] : 0,
                        item['Level6'] != null ? item['Level6'] : 0,
                        item['Level7'] != null ? item['Level7'] : 0,
                        item['Level8'] != null ? item['Level8'] : 0,
                        item['Level9'] != null ? item['Level9'] : 0,
                    ]
                };
            });

            let code = "module.exports = "+JSON.stringify(entryAttrMap,(key,value)=>{return value;},2) + ";";
            this.download(code,"attrsocket_config.js");
        },
        async beforeUpload_TitleData(rawFile){
            this.importList =await ReadXlsxConfig(rawFile,["称号"]);
            return true;
        },
        async handleMany_TitleData(){
            var titleMap = {};
            this.importList.forEach((item, index) => {
                titleMap[item['Id']] = {
                    Id: item['Id'],
                    Name:item['Name']
                };
            });

            let code = "module.exports = "+JSON.stringify(titleMap,(key,value)=>{return value;},2) + ";";
            this.download(code,"title_config.js");
        },
        download (data,fileName) {
          if (!data) {
            return
          }
          let url = window.URL.createObjectURL(new Blob([data]));
          let link = document.createElement('a');
          link.style.display = 'none';
          link.href = url;
          link.setAttribute('download', fileName);
          document.body.appendChild(link);
          link.click();
          document.body.removeChild(link);
        },
    }
}
</script>

<style scoped>
.handle-box {
    display: flex;
    margin-bottom: 20px;
}

.table {
    width: 100%;
    font-size: 14px;
}
.mr10 {
    margin-right: 10px;
}
</style>
