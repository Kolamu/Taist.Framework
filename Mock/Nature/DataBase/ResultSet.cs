namespace Mock.Nature.DataBase
{
    using System;
    using System.Xml;
    using System.Reflection;
    using Mock.Nature.Exception;
    using Mock.Data.Exception;
    /// <summary>
    /// 结果集接口
    /// </summary>
    public interface ResultSet
    {
        /// <summary>
        /// Get string value
        /// </summary>
        /// <param name="Name">value name</param>
        /// <returns></returns>
        string GetString(string Name);

        /// <summary>
        /// Get string value
        /// </summary>
        /// <param name="index">value index</param>
        /// <returns></returns>
        string GetString(int index);

        /// <summary>
        /// Get the count of the resultset
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Get the next value of current in the resultset
        /// </summary>
        bool Next
        {
            get;
        }

        /// <summary>
        /// Go to first element of the resultset
        /// </summary>
        void First();

        /// <summary>
        /// Go to last element of the resultset
        /// </summary>
        void Last();

        /// <summary>
        /// String description of the resultset
        /// </summary>
        /// <returns></returns>
        string ToString();

        /// <summary>
        /// Change the value which in resultset to type T
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <returns></returns>
        T GetObject<T>();

        /// <summary>
        /// Change the value which in resultset to type objType
        /// </summary>
        /// <param name="objType">Object type</param>
        /// <returns></returns>
        object GetObject(Type objType);
    }
    
    /// <summary>
    /// 结果集实现类
    /// </summary>
    internal class ResultSetClass : ResultSet
    {
        private XmlElement result;
        private int _currentIndex;
        private int _count;
        private string resultString;
        
        internal ResultSetClass(string result)
        {
            this.resultString = result;
            if (result != string.Empty)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                this.result = doc.DocumentElement;
                this._count = this.result.ChildNodes.Count;
            }
            else
            {
                this.result = null;
                this._count = 0;
            }
            _currentIndex = -1;
        }

        public string GetString(string Name)
        {
            if (_currentIndex == -1)
            {
                throw new IllegalReadOperationException("Please call Next function before get value");
            }
            if (_currentIndex == _count)
            {
                return string.Empty;
            }
            string value = string.Empty;
            XmlNode node = result.ChildNodes[_currentIndex];
            node = node.SelectSingleNode(Name);
            if (node != null)
            {
                value = node.InnerText;
            }
            return value;
        }

        public string GetString(int index)
        {
            if (_currentIndex == -1)
            {
                throw new IllegalReadOperationException("Please call Next function before get value");
            }
            if (_currentIndex == _count)
            {
                return string.Empty;
            }
            string value = string.Empty;
            XmlNode node = result.ChildNodes[_currentIndex];
            node = node.ChildNodes[index];
            if (node != null)
            {
                value = node.InnerText;
            }
            return value;
        }

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public bool Next
        {
            get
            {
                _currentIndex += 1;
                if (_currentIndex < _count)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void First()
        {
            if (_count > 0)
            {
                _currentIndex = 0;
            }
            else
            {
                throw new IllegalReadOperationException("There is no data in ResultSet.");
            }
        }

        public void Last()
        {
            if (_count > 0)
            {
                _currentIndex = 0;
            }
            else
            {
                throw new IllegalReadOperationException("There is no data in ResultSet.");
            }
        }

        public override string ToString()
        {
            return resultString;
        }

        public T GetObject<T>()
        {
            if (_currentIndex == -1)
            {
                throw new IllegalReadOperationException("Please call Next function before get value");
            }
            if (_currentIndex == _count)
            {
                throw new IllegalReadOperationException("ResultSet index out of bounds");
            }

            Type objType = typeof(T);

            object obj = Activator.CreateInstance(objType);
            XmlNode node = result.ChildNodes[_currentIndex];

            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.OuterXml.Contains("/>")) continue;
                PropertyInfo pi = null;
                try
                {
                    pi = objType.GetProperty(xn.Name, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic);
                }
                catch (AmbiguousMatchException)
                {
                    pi = objType.GetProperty(xn.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                }
                if (pi == null)
                {
                    throw new CanNotFindPropertyException(objType.Name, xn.Name);
                }

                pi.SetValue(obj, Convert.ChangeType(xn.InnerText, pi.PropertyType), null);
            }

            return (T)obj;
        }

        public object GetObject(Type objType)
        {
            if (_currentIndex == -1)
            {
                throw new IllegalReadOperationException("Please call Next function before get value");
            }
            if (_currentIndex == _count)
            {
                throw new IllegalReadOperationException("ResultSet index out of bounds");
            }

            object obj = Activator.CreateInstance(objType);
            XmlNode node = result.ChildNodes[_currentIndex];

            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.OuterXml.Contains("/>")) continue;
                PropertyInfo pi = null;
                try
                {
                    pi = objType.GetProperty(xn.Name, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic);
                }
                catch (AmbiguousMatchException)
                {
                    pi = objType.GetProperty(xn.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                } 
                if (pi == null)
                {
                    throw new CanNotFindPropertyException(objType.Name, xn.Name);
                }

                try
                {
                    pi.SetValue(obj, Convert.ChangeType(xn.InnerText, pi.PropertyType), null);
                }
                catch (Exception ex)
                {
                    LogManager.Debug(string.Format("{0} {1} {2} {3}", pi.Name, objType.Name, xn.InnerText, ex.Message));
                }
            }

            return obj;
        }
    }
}
