namespace Mock.Tools.Controls
{
    using mshtml;
    internal class IECheckBoxObject : WebObject
    {
        internal IECheckBoxObject(string windowName, string checkBoxName)
        {
            _windowName = windowName;
            _elementName = checkBoxName;
            //GetInternetExploreWindow(windowName);
            EWindow.Search(windowName);
            GetElement(windowName, checkBoxName);
        }
        
        internal void Check()
        {
            HTMLInputElement checkBox = (HTMLInputElement)element;
            if (!checkBox.@checked)
            {
                checkBox.click();
            }
        }

        internal void UnCheck()
        {
            HTMLInputElement checkBox = (HTMLInputElement)element;
            if (checkBox.@checked)
            {
                checkBox.click();
            }
        }
    }
}
