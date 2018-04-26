using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mock.Tools.Web
{
    public class HttpParam
    {
        private static List<HUnit> _unitList = null;
        //private static Dictionary<string, int> _unitDic = null;

        public static void Set(string key, string value)
        {
            if (_unitList == null)
            {
                _unitList = new List<HUnit>();
                //_unitDic = new Dictionary<string, int>();
            }

            HUnit unit = new HUnit();
            unit.Key = key;
            unit.Value = value;

            _unitList.Add(unit);
            


            //if (_unitDic.ContainsKey(key))
            //{
            //    _unitList.RemoveAt(_unitDic[key]);
            //    _unitList.Insert(0, unit);
            //}
            //else
            //{
            //    _unitList.Add(unit);
            //    _unitDic.Add(key, _unitList.IndexOf(unit));
            //}
        }

        public static void Set(string value)
        {
            if (_unitList == null)
            {
                _unitList = new List<HUnit>();
                //_unitDic = new Dictionary<string, int>();
            }

            _unitList.Clear();

            HUnit unit = new HUnit();
            unit.Key = "";
            unit.Value = value;
            _unitList.Add(unit);
            //if (_unitDic.ContainsKey(""))
            //{
            //    _unitList.RemoveAt(_unitDic[""]);
            //    _unitList.Insert(0, unit);
            //}
            //else
            //{
            //    _unitList.Add(unit);
            //    _unitDic.Add("", _unitList.IndexOf(unit));
            //}
        }

        public static List<string> Get(string key)
        {
            //if (_unitDic.ContainsKey(key))
            //{
            //    return _unitList[_unitDic[key]].Value;
            //}
            //else
            //{
            //    return null;
            //}
            List<string> valueList = new List<string>();
            foreach (HUnit unit in _unitList)
            {
                if (string.Equals(unit.Key, key))
                {
                    valueList.Add(unit.Value);
                }
            }
            return valueList;
        }

        public static void Clear()
        {
            if (_unitList != null)
            {
                _unitList.Clear();
                //_unitDic.Clear();
            }
        }

        public static List<HUnit> Value
        {
            get
            {
                return _unitList;
            }
        }
    }

    public class HUnit
    {
        public string Key
        {
            get;
            set;
        }
        private string _value;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
    }
}
