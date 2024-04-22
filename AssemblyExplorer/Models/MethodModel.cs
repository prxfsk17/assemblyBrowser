using System.Reflection;

namespace AssemblyExplorer.Models
{
    public class MethodModel
    {
        internal MethodInfo method;
        public bool extension { get; private set; }

        public MethodModel(MethodInfo method, bool extension = false)
        {
            this.extension = extension;
            this.method = method;
        }

        public override string ToString()
        {
            string res = "";
            if (this.extension) {
                res += $"extended from {this.method.DeclaringType!.FullName} ";
            }
            res += SetModifier(this.method.Attributes);
            res += SetKeywords(this.method.Attributes);
            res += SetGenericParams(this.method.GetGenericArguments());
            res += this.method.Name;
            res += SetParams(this.method.GetParameters());
            res += " : ";
            res += SetReturnType(this.method.ReturnType);
            return res;
        }

        private string SetGenericParams(Type[] types)
        {
            string res = "";
            for (int i = 0; i < types.Length; i++) { 
                if(i==0) res += "<";
                res += types[i].Name;
                if (i == types.Length - 1) 
                    res += ">";
                else
                    res += ", ";
            }
            return res;
        }

        private string SetReturnType(Type returnType)
        {
            string res = "";
            if (returnType.IsGenericType)
                res += CreateGenericTypeString(returnType);
            else
                res = returnType.Name;
            return res;
        }

        private string CreateGenericTypeString(Type type)
        {
            string res = "";
            int len = type.Name.ElementAt(type.Name.IndexOf('`') + 1) - '0';
            var a = type.GetGenericTypeDefinition();
            if (a != null)
            {
                res += a.Name;
                Type[] t = type.GenericTypeArguments;
                while (res.Contains("`"))
                {
                    string s = "";
                    for (int i = 0; i < len; i++)
                    {
                        if (i == 0)
                            s = $"<{t[i].Name}";
                        else
                            s += $", {t[i].Name}";
                    }
                    s += ">";
                    foreach (Type tmp in t)
                    {
                        if (tmp.IsGenericType)
                        {
                            s = s.Replace(tmp.Name, CreateGenericTypeString(tmp));
                        }
                    }
                    res = res.Replace($"`{len}", s);

                }
            }
            return res;
        }

        private string SetParams(ParameterInfo[] parameters) {
            string res = "";
            int len = parameters.Length;
            if (len == 0) res += "()";
            for (int i = 0; i < len; i++)
            {
                if (i == 0) res += "(";
                var param = parameters[i];
                string tmp = "";
                if (param.ParameterType.IsGenericType)
                    tmp += CreateGenericTypeString(param.ParameterType);
                else
                    tmp = param.ParameterType.Name;
                if (param.Attributes.HasFlag(ParameterAttributes.In))
                {
                    tmp = tmp.Replace("&", "");
                    res += "in ";
                    res += tmp;
                }
                else if (param.Attributes.HasFlag(ParameterAttributes.Out))
                {

                    tmp = tmp.Replace("&", "");
                    res += "out ";
                    res += tmp;
                }
                else if (tmp.Contains("&"))
                {
                    tmp = tmp.Replace("&", "");
                    res += "ref ";
                    res += tmp;
                }
                else if(param.ParameterType.IsArray && param.IsDefined(typeof(ParamArrayAttribute), false))
                {
                    res += "params ";
                    res += tmp;
                }
                else
                {
                    res += tmp;
                }
                res += " ";
                res += param.Name;
                if (param.Attributes.HasFlag(ParameterAttributes.HasDefault))
                {
                    res += " = ";
                    var def = param.DefaultValue;
                    if (def == null)
                        res += "null";
                    else
                        res += def.ToString();
                }
                if (i == len - 1)
                    res += ")";
                else
                    res += ", ";
            }
            return res;
        }

        private string SetModifier(MethodAttributes attributes) 
        {
            string res = "";
            if (attributes.HasFlag(MethodAttributes.FamORAssem))
            {
                res += "protected internal ";
            }
            else if (attributes.HasFlag(MethodAttributes.Public))
            {
                res += "public ";
            }
            else if (attributes.HasFlag(MethodAttributes.Family))
            {
                res += "protected ";
            }
            else if (attributes.HasFlag(MethodAttributes.Assembly))
            {
                res += "internal ";
            }
            else if (attributes.HasFlag(MethodAttributes.Private))
            {
                res += "private ";
            }
            else if (attributes.HasFlag(MethodAttributes.FamANDAssem))
            {
                res += "private protected ";
            }
            return res;
        }

        private string SetKeywords(MethodAttributes attributes)
        {
            string res = "";
            if (attributes.HasFlag(MethodAttributes.Static))
            {
                res += "static ";
            }
            else if (attributes.HasFlag(MethodAttributes.Virtual | MethodAttributes.NewSlot))
            {
                res += "virtual ";
            }
            else if (attributes.HasFlag(MethodAttributes.Final | MethodAttributes.Virtual))
            {
                res += "sealed override ";
            }
            else if (attributes.HasFlag(MethodAttributes.Virtual))
            {
                res += "override ";
            }
            else if (attributes.HasFlag(MethodAttributes.Final))
            {
                res += "sealed ";
            }
            return res;
        }
    }
}
