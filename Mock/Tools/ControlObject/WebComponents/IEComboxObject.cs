namespace Mock.Tools.Controls
{
    using System;
    using System.Collections.Generic;
    using mshtml;
    internal class IEComboBoxObject : WebObject
    {
        internal IEComboBoxObject(string windowName, string comboxName)
        {
            _windowName = windowName;
            _elementName = comboxName;
            EWindow.Search(windowName);
            GetElement(windowName, comboxName);
        }

        internal void Select(string itemName)
        {
            HTMLSelectElement select = (HTMLSelectElement)element;
            int index = 0;
            foreach (HTMLOptionElement optBtn in (HTMLElementCollection)select.options)
            {
                if (!string.IsNullOrEmpty(optBtn.innerText) && string.Equals(itemName.Trim().ToLower(), optBtn.innerText.Trim().ToLower()))
                {
                    break;
                }
                if (string.IsNullOrEmpty(optBtn.innerText) && string.IsNullOrEmpty(itemName))
                {
                    break;
                }
                index++;
            }
            select.selectedIndex = index;
            select.FireEvent("onchange");
        }

        internal string GetValue()
        {
            HTMLSelectElement select = (HTMLSelectElement)element;
            return select.value;
        }

        internal string GetText()
        {
            HTMLSelectElement select = (HTMLSelectElement)element;
            int index = 0;
            string text = "";
            foreach (HTMLOptionElement optBtn in (HTMLElementCollection)select.options)
            {
                if (index == select.selectedIndex)
                {
                    text = optBtn.innerText;
                    break;
                }
                index++;
            }
            return text;
        }

        internal void Select(int index)
        {
            HTMLSelectElement select = (HTMLSelectElement)element;
            select.selectedIndex = index;
        }
    }
}
