namespace Mock.Tools.Controls
{
    using System;
    using System.Data;
    using Mock.Tools.Exception;
    using System.Collections.Generic;
    /// <summary>
    /// 表示表格控件对象
    /// </summary>
    public class WTable
    {
        /// <summary>
        /// 向表格中指定单元格输入数据
        /// </summary>
        /// <param name="WindowName">表格所在窗口名称</param>
        /// <param name="TableName">表格名称</param>
        /// <param name="ColumnName">单元格列名称</param>
        /// <param name="Value">输入的数据</param>
        /// <param name="RowIndex">表格所在行索引（不输入或输入0为最后一行）</param>
        public static void Input(string WindowName,string TableName, string ColumnName, string Value, int RowIndex = 0)
        {
            if (Value == null)
            {
                Value = "";
                LogManager.Debug(string.Format("{0}窗口{1}表格中第{3}行第{2}列中输入内容 [null]", WindowName, TableName, ColumnName, RowIndex));
            }
            else
            {
                LogManager.Debug(string.Format("{0}窗口{1}表格中第{4}行第{2}列中输入内容[{3}]", WindowName, TableName, ColumnName, Value, RowIndex));
            }
            if (string.Equals(Value, "NOTINPUT", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            try
            {
                TableObject tableObj = new TableObject(WindowName, TableName);
                tableObj.Input(ColumnName, Value, RowIndex);
                tableObj = null;
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                LogManager.Debug("等待重新输入...");
                Robot.Recess(5000);
                TableObject tableObj = new TableObject(WindowName, TableName);
                tableObj.Input(ColumnName, Value, RowIndex);
                tableObj = null;
            }
        }

        /// <summary>
        /// 选择表格的指定行
        /// </summary>
        /// <param name="windowName">表格所在窗口名称</param>
        /// <param name="TableName">表格名称</param>
        /// <param name="rowIndex">选择的行索引（不输入或输入0为最后一行）</param>
        public static void Select(string windowName, string TableName, int rowIndex)
        {
            try
            {
                LogManager.Debug(string.Format("选择{0}窗口{1}表格中第{2}行", windowName, TableName, rowIndex));
                TableObject tableObj = new TableObject(windowName, TableName);
                tableObj.Select(rowIndex);
                tableObj = null;
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                LogManager.Debug("等待重新选择...");
                Robot.Recess(5000);
                TableObject tableObj = new TableObject(windowName, TableName);
                tableObj.Select(rowIndex);
                tableObj = null;
            }
        }

        /// <summary>
        /// 选择表格的指定行
        /// </summary>
        /// <param name="windowName">表格所在窗口名称</param>
        /// <param name="TableName">表格名称</param>
        /// <param name="indexList">选择的行索引列表</param>
        public static void Select(string windowName, string TableName, List<int> indexList)
        {
            try
            {
                if (indexList == null)
                {
                    LogManager.Error("indexList为null，不执行任何操作");
                    return;
                }
                LogManager.Debug(string.Format("选择{0}窗口{1}表格中第{2}行", windowName, TableName, string.Join(",", indexList)));
                TableObject tableObj = new TableObject(windowName, TableName);
                tableObj.Select(indexList);
                tableObj = null;
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                LogManager.Debug("等待重新选择...");
                Robot.Recess(5000);
                TableObject tableObj = new TableObject(windowName, TableName);
                tableObj.Select(indexList);
                tableObj = null;
            }
        }

        /// <summary>
        /// 查询表格中是否包含指定名称的列
        /// </summary>
        /// <param name="windowName">表格所在窗口名称</param>
        /// <param name="TableName">表格名称</param>
        /// <param name="columnName">列名称</param>
        /// <returns></returns>
        public static bool HasColumn(string windowName, string TableName, string columnName)
        {
            TableObject tableObj = new TableObject(windowName, TableName);
            bool ret = tableObj.HasColumn(columnName);
            tableObj = null;
            return ret;
        }

        /// <summary>
        /// 根据指定的条件查找数据在表格中的位置
        /// </summary>
        /// <param name="windowName">表格所在窗体名称</param>
        /// <param name="TableName">表格名称</param>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public static int GetRowIndex(string windowName, string TableName, Dictionary<string, string> condition)
        {
            TableObject tableObj = new TableObject(windowName, TableName);
            return tableObj.GetRowIndex(condition);
        }

        /// <summary>
        /// 查看表格是否存在
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static bool Exist(string windowName, string TableName)
        {
            try
            {
                new TableObject(windowName, TableName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        /// <param name="tableName">表格名称</param>
        /// <param name="rowIndex">从1开始的行号</param>
        /// <param name="columnIndex">列名</param>
        public static string GetContent(string windowName, string tableName, int rowIndex, int columnIndex)
        {
            TableObject tableObj = new TableObject(windowName, tableName);
            return tableObj.GetContent(rowIndex, columnIndex);
        }

        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        /// <param name="tableName">表格名称</param>
        /// <param name="rowIndex">从1开始的行号</param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        public static string GetContent(string windowName, string tableName, int rowIndex, string columnName)
        {
            TableObject tableObj = new TableObject(windowName, tableName);
            return tableObj.GetContent(rowIndex, columnName);
        }

        /// <summary>
        /// 获取表格数据总行数
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        /// <param name="tableName">表格名称</param>
        /// <returns></returns>
        public static int RowCount(string windowName, string tableName)
        {
            TableObject tableObje = new TableObject(windowName, tableName);
            return tableObje.GetRowCount();
        }

        /// <summary>
        /// 获取表格内容
        /// </summary>
        /// <param name="windowName">表格所在窗口名称</param>
        /// <param name="tableName">表格别名</param>
        /// <returns>表格内容</returns>
        public static DataTable GetContent(string windowName, string tableName)
        {
            TableObject tableObj = new TableObject(windowName, tableName);
            return tableObj.GetData();
        }

        /// <summary>
        /// 点击制定单元格
        /// </summary>
        /// <param name="WindowName">表格所在窗口名称</param>
        /// <param name="TableName">表格名称</param>
        /// <param name="ColumnName">单元格列名称</param>
        /// <param name="RowIndex">表格所在行索引（不输入或输入0为最后一行）</param>
        /// <param name="x">相对于表格左上角的X偏移，如果ColumnName为null，则相对于整行的左上角</param>
        /// <param name="y">相对于表格左上角的Y偏移，如果ColumnName为null，则相对于整行的左上角</param>
        public static void Click(string WindowName, string TableName, string ColumnName, int RowIndex = 0, int x = 5, int y = 5)
        {
            try
            {
                TableObject tableObj = new TableObject(WindowName, TableName);
                tableObj.Click(ColumnName, RowIndex, x, y);
                tableObj = null;
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                LogManager.Debug("等待重新输入...");
                Robot.Recess(5000);
                TableObject tableObj = new TableObject(WindowName, TableName);
                tableObj.Click(ColumnName, RowIndex, x, y);
                tableObj = null;
            }
        }

        /// <summary>
        /// 点击制定单元格
        /// </summary>
        /// <param name="WindowName">表格所在窗口名称</param>
        /// <param name="TableName">表格名称</param>
        /// <param name="ColumnName">单元格列名称</param>
        /// <param name="RowIndexList">表格所在行索引（不输入或输入0为最后一行）</param>
        /// <param name="x">相对于表格左上角的X偏移，如果ColumnName为null，则相对于整行的左上角</param>
        /// <param name="y">相对于表格左上角的Y偏移，如果ColumnName为null，则相对于整行的左上角</param>
        public static void Click(string WindowName, string TableName, string ColumnName, List<int> RowIndexList, int x = 5, int y = 5)
        {
            try
            {
                TableObject tableObj = new TableObject(WindowName, TableName);
                tableObj.Click(ColumnName, RowIndexList, x, y);
                tableObj = null;
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                LogManager.Debug("等待重新输入...");
                Robot.Recess(5000);
                TableObject tableObj = new TableObject(WindowName, TableName);
                tableObj.Click(ColumnName, RowIndexList, x, y);
                tableObj = null;
            }
        }

        /// <summary>
        /// 表示表格中带编辑框的单元格控件
        /// </summary>
        public class EditUnit
        {
            /// <summary>
            /// 向表格中的编辑框控件输入信息
            /// </summary>
            /// <param name="WindowName">表格所在窗口名称</param>
            /// <param name="TableName">表格名称</param>
            /// <param name="ColumnName">单元格所在列名称</param>
            /// <param name="Value">要输入的值</param>
            /// <param name="RowIndex">单元格所在行索引（不输入或输入0为最后一行）</param>
            public static void Input(string WindowName, string TableName, string ColumnName, string Value, int RowIndex = 0)
            {
                if (Value == null)
                {
                    Value = "";
                    LogManager.Debug(string.Format("{0}窗口{1}表格中第{3}行第{2}列中输入内容[null]", WindowName, TableName, ColumnName, RowIndex));
                }
                else
                {
                    LogManager.Debug(string.Format("{0}窗口{1}表格中第{4}行第{2}列中输入内容[{3}]", WindowName, TableName, ColumnName, Value, RowIndex));
                }
                if (string.Equals(Value, "NOTINPUT", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                try
                {
                    TableObject tableObj = new TableObject(WindowName, TableName);
                    EditObject editObj = tableObj.GetItemObject<EditObject>(ColumnName, RowIndex);
                    editObj.Input(Value);
                    editObj = null;
                    tableObj = null;
                }
                catch (Exception ex)
                {
                    LogManager.Error(ex);
                    LogManager.Debug("等待重做...");
                    Robot.Recess(5000);
                    TableObject tableObj = new TableObject(WindowName, TableName);
                    EditObject editObj = tableObj.GetItemObject<EditObject>(ColumnName, RowIndex);
                    editObj.Input(Value);
                    editObj = null;
                    tableObj = null;
                }
            }
        }

        /// <summary>
        /// 表示表格中的下拉框控件
        /// </summary>
        public class ComboBoxUnit
        {
            /// <summary>
            /// 选择表格中的下拉框控件的子项
            /// </summary>
            /// <param name="WindowName">表格所在窗口名称</param>
            /// <param name="TableName">表格名称</param>
            /// <param name="ColumnName">单元格所在列名称</param>
            /// <param name="comboxItemValue">要选择的下拉框子项</param>
            /// <param name="RowIndex">单元格所在行索引（不输入或输入0为最后一行）</param>
            public static void Select(string WindowName, string TableName, string ColumnName, string comboxItemValue, int RowIndex = 0)
            {
                try
                {
                    TableObject tableObj = new TableObject(WindowName, TableName);
                    ComboBoxObject comboxObj = tableObj.GetItemObject<ComboBoxObject>(ColumnName, RowIndex);
                    comboxObj.Select(comboxItemValue);
                    tableObj = null;
                    comboxObj = null;
                }
                catch (Exception ex)
                {
                    LogManager.Error(ex);
                    LogManager.Debug("等待重做...");
                    Robot.Recess(5000);
                    TableObject tableObj = new TableObject(WindowName, TableName);
                    ComboBoxObject comboxObj = tableObj.GetItemObject<ComboBoxObject>(ColumnName, RowIndex);
                    comboxObj.Select(comboxItemValue);
                    tableObj = null;
                    comboxObj = null;
                }
            }
        }

        public class CheckBoxUnit
        {
            public static void Check(string WindowName, string TableName, string ColumnName, int RowIndex = 0)
            {
                try
                {
                    TableObject tableObj = new TableObject(WindowName, TableName);
                    CheckBoxObject chkboxObj = tableObj.GetItemObject<CheckBoxObject>(ColumnName, RowIndex);
                    chkboxObj.Check();
                    tableObj = null;
                    chkboxObj = null;
                }
                catch (Exception ex)
                {
                    LogManager.Error(ex);
                    LogManager.Debug("等待重做...");
                    Robot.Recess(5000);
                    TableObject tableObj = new TableObject(WindowName, TableName);
                    CheckBoxObject chkboxObj = tableObj.GetItemObject<CheckBoxObject>(ColumnName, RowIndex);
                    chkboxObj.Check();
                    tableObj = null;
                    chkboxObj = null;
                }
            }

            public static void Check(string WindowName, string TableName, string ColumnName, List<int> RowIndexList)
            {
                TableObject tableObj = new TableObject(WindowName, TableName);
                foreach (int RowIndex in RowIndexList)
                {
                    CheckBoxObject chkboxObj = tableObj.GetItemObject<CheckBoxObject>(ColumnName, RowIndex);
                    chkboxObj.Check();
                    chkboxObj = null;
                }

                tableObj = null;
            }

            public static void UnCheck(string WindowName, string TableName, string ColumnName, int RowIndex = 0)
            {
                try
                {
                    TableObject tableObj = new TableObject(WindowName, TableName);
                    CheckBoxObject chkboxObj = tableObj.GetItemObject<CheckBoxObject>(ColumnName, RowIndex);
                    chkboxObj.UnCheck();
                    tableObj = null;
                    chkboxObj = null;
                }
                catch (Exception ex)
                {
                    LogManager.Debug("发生异常：" + ex.Message);
                    LogManager.Debug("等待重做...");
                    Robot.Recess(5000);
                    TableObject tableObj = new TableObject(WindowName, TableName);
                    CheckBoxObject chkboxObj = tableObj.GetItemObject<CheckBoxObject>(ColumnName, RowIndex);
                    chkboxObj.UnCheck();
                    tableObj = null;
                    chkboxObj = null;
                }
            }

            public static void UnCheck(string WindowName, string TableName, string ColumnName, List<int> RowIndexList)
            {
                TableObject tableObj = new TableObject(WindowName, TableName);
                foreach (int RowIndex in RowIndexList)
                {
                    CheckBoxObject chkboxObj = tableObj.GetItemObject<CheckBoxObject>(ColumnName, RowIndex);
                    chkboxObj.UnCheck();
                    chkboxObj = null;
                }
                tableObj = null;
            }
        }
    }
}
