using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFabric.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class FabricLocationAttribute : System.Attribute
    {
        #region private vars
        private string _Application;
        private string _Service;
        #endregion
        #region public properties
        public string Application
        {
            get { return (_Application); }
            set { _Application = value; }
        }
        public string Service
        {
            get { return (_Service); }
            set { _Service = value; }
        }

        public Uri ServiceUri
        {
            get
            {
                return (new Uri(_Application + _Service));
            }
        }

        #endregion
        public FabricLocationAttribute(string Application, string Service)
            : base()
        {
            _Application = Application;
            _Service = Service;
        }
    }
}
