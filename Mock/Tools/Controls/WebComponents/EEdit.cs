namespace Mock.Tools.Controls
{
    public class EEdit
    {
        public static void Input(string windowName, string editName, string value)
        {
            if (string.Equals(value, "NOTINPUT", System.StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            LogManager.Debug(string.Format("Input {2} to {1} edit in {0} window.", windowName, editName, value));
            IEEditObject editObj = new IEEditObject(windowName, editName);
            editObj.Input(value);
        }

        public static string GetValue(string windowName, string editName)
        {
            LogManager.Debug(string.Format("GetValue from {1} edit in {0} window.", windowName, editName));
            IEEditObject editObj = new IEEditObject(windowName, editName);
            return editObj.GetValue();
        }
    }
}
