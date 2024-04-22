using System.Reflection;

namespace AssemblyExplorer.Models
{
    public class AssemblyModel
    {
        public Assembly assembly { get; private set; }
        private Dictionary<string, NamespaceModel> _namespaces;
        internal List<MethodInfo> extensions;

        public string name { get; }
        public List<NamespaceModel> namespaces { get; set; }

        public AssemblyModel(Assembly assembly) {
            this.assembly = assembly;
            this.extensions = new List<MethodInfo>();
            this._namespaces = new Dictionary<string, NamespaceModel>();
            Type[] types = assembly.GetTypes().Where(t => !t.Name.Contains("<>c")).ToArray(); ;
            foreach (Type type in types)
            {
                string __namespace = type.Namespace == null ? "-" : type.Namespace;
                if (!_namespaces.ContainsKey(__namespace)) 
                {
                    var n = new NamespaceModel(__namespace, this);
                    n.AddClass(type);
                    _namespaces.Add(__namespace, n);
                }
                else
                    _namespaces[__namespace].AddClass(type);
            }
            this.namespaces = _namespaces.Values.ToList();
            this.name = assembly.GetName().Name ?? "null";
            ReplaceClasses();
            PlaceExtensionMethods();
        }

        private void PlaceExtensionMethods()
        {
            foreach (var item in extensions)
            {
                string name = item.DeclaringType!.Namespace ?? "-";
                NamespaceModel namespaceModel = _namespaces[name];  
                var t = item.GetParameters()[0].ParameterType.Name;
                ClassModel? classModel = namespaceModel.classes.Where(c => c.name == t).FirstOrDefault();
                if(classModel != null) classModel.AddExtensionMethod(item);
            }
        }

        private void ReplaceClasses() { 
            bool flag = true;
            while (flag)
            {
                flag = false;
                foreach (var n in namespaces)
                {
                    List<ClassModel> remove = new List<ClassModel>();
                    foreach (var c in n.classes)
                    {
                        if (n.ReplaceClasses(ref n.classes, c))
                        {
                            remove.Add(c);
                        }
                    }
                    foreach (var c in remove) n.classes.Remove(c);
                }
                foreach (var n in namespaces)
                {
                    foreach (var c in n.classes)
                    {
                        if (c.name.Contains("+")) {
                            flag=true;
                            break;
                        }
                    }
                    if (flag) break;
                }
            }
        }
    }
}
