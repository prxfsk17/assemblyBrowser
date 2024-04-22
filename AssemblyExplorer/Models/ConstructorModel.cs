using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyExplorer.Models
{
    public class ConstructorModel
    {
        internal ConstructorInfo constructor;

        public ConstructorModel(ConstructorInfo constructor)
        {
            this.constructor = constructor;
        }

        public override string ToString()
        {
            string res = "";
            res += SetModifier(constructor.Attributes);
            res += SetKeywords(constructor.Attributes);
            res += this.constructor.DeclaringType!.Name;
            res += SetParams(this.constructor.GetParameters());
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
                res += "sealed ovveride ";
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

        private string SetParams(ParameterInfo[] parameters)
        {
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
                else if (param.ParameterType.IsArray && param.IsDefined(typeof(ParamArrayAttribute), false))
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
    }
}
