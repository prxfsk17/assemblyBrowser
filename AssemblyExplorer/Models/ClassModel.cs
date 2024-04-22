using System.Reflection;
using System.Runtime.CompilerServices;

namespace AssemblyExplorer.Models
{
    public class ClassModel
    {
        public Type _class { get; private set; }
        public NamespaceModel _namespace;
        public List<MethodModel> _methods { get; private set; }
        public List<ConstructorModel> _constructors { get; private set; }
        public List<FieldModel> _fields { get; private set; }
        public List<PropertyModel> _properties { get; private set; }
        public List<EventModel> _events { get; private set; }
        public List<ClassModel> innerClasses;

        public string name { get; }
        public List<MemberModel> members { get; }
        public List<ClassModel> InnerClasses { get{return innerClasses;} }

        public ClassModel(Type _class, NamespaceModel _namespace)
        {
            this._class = _class;
            this._namespace = _namespace;
            this.innerClasses = new List<ClassModel>();
            this.name = GetName();
            this._methods = new List<MethodModel>();
            this._constructors = new List<ConstructorModel>();
            this._fields = new List<FieldModel>();
            this._properties = new List<PropertyModel>();
            this._events = new List<EventModel>();
            this.members = new List<MemberModel>();
            FillConstructors();
            FillMethods();
            FillFields();
            FillProperties();
            FillMembers();
            FillEvents();
        }

        private void FillEvents()
        {
            var __events = this._class.GetEvents(BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static);
            foreach (EventInfo ev in __events)
            {
                var c = new EventModel(ev);
                _events.Add(c);
            }
        }

        private string GetName()
        {
            string res = _class.Name;
            Type[] types = [];
            if (_class.IsGenericType)
                types = _class.GetGenericArguments();
            res = res.Replace($"`{types.Length}","");
            for (int i = 0; i < types.Length; i++)
            {
                if (i == 0) res += "<";
                res += types[i].Name;
                if (i == types.Length - 1)
                    res += ">";
                else
                    res += ", ";
            }
            return res;
        }

        private void FillConstructors()
        {
            var __constructors = this._class.GetConstructors().ToList();
            foreach (ConstructorInfo constructor in __constructors)
            {
                var c = new ConstructorModel(constructor);
                _constructors.Add(c);
            }
        }

        private void FillMembers()
        {
            foreach(var s in this._methods) members.Add(new MemberModel(s.ToString()));
            foreach(var s in this._fields) members.Add(new MemberModel(s.ToString()));
            foreach(var s in this._properties) members.Add(new MemberModel(s.ToString()));
            foreach(var s in this._constructors) members.Add(new MemberModel(s.ToString()));
        }

        private void FillMethods()
        {
            var __methods = this._class.GetMethods(BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static);
            foreach (MethodInfo method in __methods)
            {
                //methods.Add(new MethodModel(method));
                if (method.IsDefined(typeof(ExtensionAttribute), false)) {
                    this._namespace.assembly.extensions.Add(method);
                }
                if (!method.Attributes.HasFlag(MethodAttributes.SpecialName))
                {
                    var m = new MethodModel(method);
                    _methods.Add(m);
                }
            }
        }

        private void FillFields()
        {
            var __fields = this._class.GetFields(BindingFlags.Instance | 
                BindingFlags.Static | BindingFlags.Public | 
                BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (_class.Name.Contains("En1"))
            { _ = 5; }
            foreach (FieldInfo field in __fields)
            {
                if (!field.Name.Contains(">k__BackingField"))
                {
                    var f = new FieldModel(field);
                    _fields.Add(f);
                }
            }
        }

        private void FillProperties()
        {
            var __properties = this._class.GetProperties(BindingFlags.Instance |
                BindingFlags.Static | BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo property in __properties)
            {
                var p = new PropertyModel(property);
                _properties.Add(p);
            }
        }

        internal void AddExtensionMethod(MethodInfo item)
        {
            var m = new MethodModel(item, true);
            this._methods.Add(m);
            this.members.Add(new MemberModel(m.ToString()));
        }
    }
}
