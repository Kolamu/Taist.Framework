namespace Mock.Tools.Controls
{
    public class EComboBox
    {
        public static void Select(string windowName, string comboxName, string itemName)
        {
            if (string.Equals(itemName, "NOTINPUT", System.StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            LogManager.Debug(string.Format("Select {0} item in {1} combox in {2} window", itemName, comboxName, windowName));
            IEComboBoxObject comboxObj = new IEComboBoxObject(windowName, comboxName);
            comboxObj.Select(itemName);
        }

        public static void Select(string windowName, string comboxName, int index)
        {
            LogManager.Debug(string.Format("Select No.{0} item in {1} combox in {2} window", index, comboxName, windowName));
            try
            {
                IEComboBoxObject comboxObj = new IEComboBoxObject(windowName, comboxName);
                comboxObj.Select(index);
            }
            catch (System.Exception ex)
            {
                LogManager.Error(ex);
                throw ex;
            }

        }

        public static string GetValue(string windowName, string comboxName)
        {
            try
            {
                IEComboBoxObject comboxObj = new IEComboBoxObject(windowName, comboxName);
                return comboxObj.GetValue();
            }
            catch (System.Exception ex)
            {
                LogManager.Error(ex);
                throw ex;
            }
        }

        public static string GetText(string windowName, string comboxName)
        {
            try
            {
                IEComboBoxObject comboxObj = new IEComboBoxObject(windowName, comboxName);
                return comboxObj.GetText();
            }
            catch (System.Exception ex)
            {
                LogManager.Error(ex);
                throw ex;
            }
        }
    }
}
