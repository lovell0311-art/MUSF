import * as XLSX from 'xlsx';

export async function ReadXlsxConfig(file,sheetNames){
    return new Promise(function (resolve, reject) {
        const reader = new FileReader();
        reader.onload = function (e) {
            const data = e.target.result;
            let datajson = XLSX.read(data, {
                type: 'binary',
            }
            );
            var resultConfigData = [];
                for(var sheetId = 0;sheetId < datajson.SheetNames.length;sheetId++)
                {
                    const sheetName = datajson.SheetNames[sheetId];
                    if(sheetNames.indexOf(sheetName) < 0) continue;
                    console.log(sheetName);
                    const result = XLSX.utils.sheet_to_json(datajson.Sheets[sheetName],{header:1});
                    const configData = ReadSheetConfigData(result);

                    resultConfigData.push(...configData);
                }

            resolve(resultConfigData);
        };
        reader.readAsBinaryString(file);
    });
}

function ReadSheetConfigData(jsonSheet)
{
    const headerRow = 3;     // key开始行
    const typeRow = 3;     // 类型开始行
    const noteCol = 1;    // 注释列
    const headerBegin = 2;
    const headerEnd = jsonSheet[headerRow].length;
    const dataRow = 5;  // 数据开始行
    let result  = [];
    const headerRowData = jsonSheet[headerRow];
    const typeRowData = jsonSheet[typeRow];
    for(var rowId = dataRow;rowId < jsonSheet.length;rowId++)
    {
        const rowData = jsonSheet[rowId];
        if(rowData[noteCol] && rowData[noteCol].indexOf('#') == 0) continue;
        var temp = {};
        for(var colId = headerBegin;colId<headerEnd;colId++)
        {
            if(!headerRowData[colId] || headerRowData[colId].trim() == '') continue;
            switch(typeRowData[colId])
            {
                case 'int':
                case 'long':
                    temp[headerRowData[colId]] = Number(rowData[colId]);
                    break;
                case 'string':
                    temp[headerRowData[colId]] = String(rowData[colId]);
                    break;
                default:
                    temp[headerRowData[colId]] = rowData[colId];
                    break;
            }
        }
        result.push(temp);
    }
    return result;
}