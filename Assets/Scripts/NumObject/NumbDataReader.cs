using OfficeOpenXml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NumbDataReader
{
    public static void ReadNumbDataUseEPPlus(out float mass, out float drag, out float gravityScale,out float numSwitchMassScale)
    {
        using (var package = new ExcelPackage(new FileInfo(Path.Combine(Application.streamingAssetsPath, "NumbData.xlsx"))))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            mass = worksheet.Cells[1, 2].GetValue<float>();
            drag = worksheet.Cells[2, 2].GetValue<float>();
            gravityScale = worksheet.Cells[3, 2].GetValue<float>();
            numSwitchMassScale = worksheet.Cells[4, 2].GetValue<float>();
        }
    }
}
