using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.AspNetCore.Model
{
    public class Sort
    {

        public string Column { get; set; }

        private SortType _type = SortType.ASC;
        public SortType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public string GetTypeStr()
        {
            if (_type == SortType.ASC)
            {
                return "ASC";
            }
            else
            {
                return "DESC";
            }
        }
    }

    public enum SortType
    {
        ASC, DESC
    }
}
