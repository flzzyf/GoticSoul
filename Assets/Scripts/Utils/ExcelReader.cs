using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OfficeOpenXml;
using System.IO;

public static class ExcelReader {
	//数据类型
	const string type_String = "string";
	const string type_Int = "int";
	const string type_Float = "float";
	const string type_Bool = "bool";

	public static List<T> ReadExcel<T>(string path, string tableName) where T : new() {
		//检测文件存在
		if (!File.Exists(path)) {
			Debug.LogError("不存在文件：" + path);

			return null;
		}

		List<T> list = new List<T>();
		FileInfo file = new FileInfo(path);

		//检测文件是否正在被使用
		try {
			using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read)) {
				fs.Close();
			}
		}
		catch (IOException) {
			string errorText = string.Format("数据表{0}正在被使用，请将其关闭后再打开该软件。", file.Name);

			Debug.LogError(errorText);
		}

		ExcelPackage package = new ExcelPackage(file);
		ExcelWorksheets sheets = package.Workbook.Worksheets;

		//遍历表
		foreach (var sheet in sheets) {
			if (sheet.Name == tableName) {
				for (int y = 5; y <= sheet.Dimension.Rows; y++) {
					T item = new T();
					list.Add(item);

					for (int x = 1; x <= sheet.Dimension.Columns; x++) {
						//字段名
						string id = sheet.GetValue<string>(2, x);
						//类型
						string type = sheet.GetValue<string>(3, x);

						if (item.GetType().GetField(id) == null) {
							Debug.LogError("不存在字段:" + id);
							continue;
						}

						//如果数据为空，用默认值行的数据
						int dataColumn = sheet.GetValue(y, x) == null ? 4 : y;
						string value = sheet.GetValue<string>(dataColumn, x);

						//赋值
						if (type == type_String) {
							item.GetType().GetField(id).SetValue(item, value);
						}
						else if (type == type_Int) {
							item.GetType().GetField(id).SetValue(item, int.Parse(value));
						}
						else if (type == type_Float) {
							item.GetType().GetField(id).SetValue(item, float.Parse(value));
						}
						else if (type == type_Bool) {
							item.GetType().GetField(id).SetValue(item, bool.Parse(value));
						}
					}
				}
			}
		}

		return list;
	}
}
