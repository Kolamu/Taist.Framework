using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mock.Provider
{
    using System.Windows.Automation;
    using System.Windows.Automation.Provider;
    internal class SetupModeWindowProvider : IRawElementProviderSimple
    {
        IntPtr clientPtr = IntPtr.Zero;
        int idChild = 0;

        internal SetupModeWindowProvider(IntPtr ptr,int idChild)
        {
            clientPtr = ptr;
            this.idChild = idChild;
        }

        public object GetPatternProvider(int patternId)
        {
            if (patternId == TogglePattern.Pattern.Id)
            {
                return null;
            }
            else
            {
                return null;
            }
        }

        public object GetPropertyValue(int propertyId)
        {
            if (AutomationElementIdentifiers.NameProperty == AutomationProperty.LookupById(propertyId))
            {
                return "SetupMode";
            }
            else if (AutomationElementIdentifiers.ControlTypeProperty == AutomationProperty.LookupById(propertyId))
            {
                return ControlType.CheckBox;
            }
            else
            {
                return null;
            }
        }

        public IRawElementProviderSimple HostRawElementProvider
        {
            get { return AutomationInteropProvider.HostProviderFromHandle(clientPtr); }
        }

        public ProviderOptions ProviderOptions
        {
            get { return ProviderOptions.ClientSideProvider; }
        }
    }

    internal class CheckBoxClickPattern : IToggleProvider
    {
        public void Toggle()
        {
            throw new NotImplementedException();
        }

        public ToggleState ToggleState
        {
            get { throw new NotImplementedException(); }
        }
    }
}
