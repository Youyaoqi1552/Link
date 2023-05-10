using System;
using System.IO;
using System.Reflection;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;

public static class ExcelHelper
{
    public static readonly char[] TrimChars = new char[] {' ', '[', ']'};

    public static IWorkbook LoadWorkBook(string excelPath)
    {
        using var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        if (Path.GetExtension(excelPath) == ".xls") return new HSSFWorkbook(stream);
        else return new XSSFWorkbook(stream);
    }
    
    public static object CellToFieldObject(ICell cell, FieldInfo fieldInfo)
    {
        switch(cell.CellType)
        {
            case CellType.String:
                if (fieldInfo.FieldType.IsEnum) return Enum.Parse(fieldInfo.FieldType, cell.StringCellValue);
                else return cell.StringCellValue;
            case CellType.Boolean:
                return cell.BooleanCellValue;
            case CellType.Numeric:
                return Convert.ChangeType(cell.NumericCellValue, fieldInfo.FieldType);
            default:
                if(fieldInfo.FieldType.IsValueType)
                {
                    return Activator.CreateInstance(fieldInfo.FieldType);
                }
                return null;
        }
    }
}
