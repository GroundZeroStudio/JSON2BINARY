using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace JSON2BINARY
{
    public class AssemblyManager
    {
        public static AssemblyManager Instance
        {
            get
            {
                if(_Instance == null)
                {
                    _Instance = new AssemblyManager();
                }
                return _Instance;
            }
        }

        private static AssemblyManager _Instance;

        public Dictionary<string, Type> TypeDict;

        public void Initialize()
        {
            this.TypeDict = new Dictionary<string, Type>();

            var rInfo = System.AppDomain.CurrentDomain.GetAssemblies().Single(rAssembly => rAssembly.GetName().Name.Equals("JSON2BINARY"));
            var rTypes = rInfo.GetTypes();

            for (int i = 0; i < rTypes.Length; i++)
            {
                this.TypeDict.Add(rTypes[i].Name, rTypes[i]);
            }
        }

        public Type GetContainType(string rTypeName)
        {
            this.TypeDict.TryGetValue(rTypeName, out var rType);
            return rType;
        }
    }
}
