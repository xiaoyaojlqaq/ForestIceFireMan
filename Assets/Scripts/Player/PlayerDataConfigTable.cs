using OfficeOpenXml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Runtime.Versioning;
using UnityEngine.UI;

public class PlayerDataConfigTable
{
    public static void ReadDataUseEPPlus(out float mass, out float drag, out float gravityScale, out float moveSpeed, out float jumpSpeed)
    {
        //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        using (var package = new ExcelPackage(new FileInfo(Path.Combine(Application.streamingAssetsPath, "PlayerData.xlsx"))))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            mass = worksheet.Cells[1, 2].GetValue<float>();
            drag = worksheet.Cells[2, 2].GetValue<float>();
            gravityScale = worksheet.Cells[3, 2].GetValue<float>();
            moveSpeed = worksheet.Cells[4, 2].GetValue<float>();
            jumpSpeed = worksheet.Cells[5, 2].GetValue<float>();
        }
    }
}
