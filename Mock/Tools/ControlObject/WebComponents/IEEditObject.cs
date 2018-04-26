namespace Mock.Tools.Controls
{
    using mshtml;
    using Mock.Tools.Exception;
    internal class IEEditObject : WebObject
    {
        internal IEEditObject(string windowName, string editName)
        {
            _windowName = windowName;
            _elementName = editName;
            EWindow.Search(windowName);
            GetElement(windowName, editName);
        }

        internal void Input(string msg)
        {
            for (int i = 0; i < Config.RedoCount; i++)
            {
                try
                {
                    HTMLInputElement input = (HTMLInputElement)element;
                    input.select();
                    input.value = msg;
                    input.FireEvent("onkeypress");
                    input.FireEvent("onkeydown");
                    input.FireEvent("onkeyup");
                }
                catch
                {
                    Robot.Recess(1000);
                }
            }
        }

        internal string GetValue()
        {
            for (int i = 0; i < Config.RedoCount; i++)
            {
                try
                {
                    HTMLInputElement input = (HTMLInputElement)element;
                    input.select();
                    return input.value;
                }
                catch
                {
                    Robot.Recess(1000);
                    EWindow.Search(_windowName);
                    GetElement(_windowName, _elementName);
                }
            }

            throw new ControlUnableException(_windowName, _elementName);
        }
    }
}
