namespace Mock.Tools.Controls
{
    using mshtml;
    internal class IEMenuObject : WebObject
    {
        internal IEMenuObject(string windowName, string menuName)
        {
            _windowName = windowName;
            _elementName = menuName;
            EWindow.Search(windowName);
        }

        internal void Click(string itemName)
        {
            GetElement(_windowName, _elementName, itemName);
            element.click();
        }
    }
}
