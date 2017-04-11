using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFabric.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ReqestProcessorAttribute : System.Attribute
    {
        #region private vars
        private string _Name;
        private int _Identifier;
        Type _RequestType;
        Type _ResponseType;
        #endregion
        #region public properties
        public string Name
        {
            get { return (_Name); }
            set { _Name = value; }
        }
        public int Identifier
        {
            get { return (_Identifier); }
            set { _Identifier = value; }
        }

        public Type RequestType
        {
            get { return (_RequestType); }
            set { _RequestType = value; }
        }

        public Type ResponseType
        {
            get { return (_ResponseType); }
            set { _ResponseType = value; }
        }

        #endregion
        public ReqestProcessorAttribute(string Name, int Identifier, Type RequestType, Type ResponseType)
            : base()
        {
            _Name= Name;
            _Identifier= Identifier;
            _RequestType= RequestType;
            _ResponseType = ResponseType;
        }
    }
}
