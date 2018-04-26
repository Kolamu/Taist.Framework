namespace Runner
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Collections.Generic;
    using System.Reflection;

    using Mock;
    using Mock.Data;
    internal class RunnerParameters
    {
        internal RunnerParameters(string[] args)
        {
            Type pType = this.GetType();
            foreach (string param in args)
            {
                MethodInfo method = pType.GetMethod(param, BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Instance);
                if (method == null) continue;
                method.Invoke(this, null);
            }
        }

        private void NoUI()
        {
            userInterface = false;
        }

        private bool userInterface = true;
        public bool HasUserInterface
        {
            get
            {
                return userInterface;
            }
        }

        private void Multiton()
        {
            isSingleton = false;
        }

        private bool isSingleton = true;
        public bool Singleton
        {
            get
            {
                return isSingleton;
            }
        }
    }
}
