using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AssemblyExplorer.Models
{
    public class PropertyModel
    {
        internal PropertyInfo property;
        private string[] modifiers = [];

        public PropertyModel(PropertyInfo property)
        {
            this.property = property;
        }

        public override string ToString()
        {
            this.modifiers = new string[2];
            string res = "";
            res += "<modifier> ";
            res += SetKeywords();
            if (this.property.PropertyType.IsGenericType)
                res += CreateGenericTypeString(this.property.PropertyType);
            else
                res += this.property.PropertyType.Name;
            res += " ";
            res += property.Name;
            res += SetProps(property);
            FormatString(ref res);
            return res;
        }

        private void FormatString(ref string res)
        {
            string modifier;
            if (this.modifiers.Contains("public "))
            {
                modifier = "public";
            }
            else if (this.modifiers.Contains("protected internal "))
            {
                modifier = "protected internal";
            }
            else if (this.modifiers.Contains("protected "))
            {
                modifier = "protected";
            }
            else if (this.modifiers.Contains("internal "))
            {
                modifier = "internal";
            }
            else if (this.modifiers.Contains("private protected "))
            {
                modifier = "private protected";
            }
            else
            {
                modifier = "private";
            }
            if (modifiers[0] == (modifier + " "))
                modifiers[0] = "";
            else
                modifiers[1] = "";
            res = res.Replace("<modifier>", modifier);
            res = res.Split('{')[0];
            modifiers[0] = modifiers[0].Trim();
            modifiers[1] = modifiers[1].Trim();
            if (!(modifiers[0] == "")) modifiers[0] += " ";
            if (!(modifiers[1] == "")) modifiers[1] += " ";
            res += $"{{ {modifiers[0]}get; {modifiers[1]}set; }}";
        }

        private string SetProps(PropertyInfo property)
        {
            string res = "";
            res += " {";
            res += SetGetter(property.GetMethod);
            res += SetSetter(property.SetMethod);
            res += "}";
            return res;
        }

        private string SetSetter(MethodInfo? setter)
        {
            if (setter == null) return "";
            string res = "";
            this.modifiers[1] += SetModifier(setter.Attributes);
            res += this.modifiers[1];
            res += "set; ";
            return res;
        }

        private string SetGetter(MethodInfo? getter)
        {
            if (getter == null) return "";
            string res = "";
            this.modifiers[0] += SetModifier(getter.Attributes);
            res += this.modifiers[0];
            res += "get; ";
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
                res += "private";
            }
            else if (attributes.HasFlag(MethodAttributes.FamANDAssem))
            {
                res += "private protected ";
            }
            return res;
        }

        private string SetKeywords()
        {
            if (property.SetMethod!=null && property.SetMethod.Attributes.HasFlag(MethodAttributes.Static))
            {
                return "static ";
            }
            else if (property.GetMethod != null && property.GetMethod.Attributes.HasFlag(MethodAttributes.Static))
                return "static ";
            return "";
        }

        private string SetModifier(FieldAttributes attributes)
        {
            string res = "";
            if (attributes.HasFlag(FieldAttributes.FamORAssem))
            {
                res += "protected internal ";
            }
            else if (attributes.HasFlag(FieldAttributes.Public))
            {
                res += "public ";
            }
            else if (attributes.HasFlag(FieldAttributes.Family))
            {
                res += "protected ";
            }
            else if (attributes.HasFlag(FieldAttributes.Assembly))
            {
                res += "internal ";
            }
            else if (attributes.HasFlag(FieldAttributes.Private))
            {
                res += "private ";
            }
            else if (attributes.HasFlag(FieldAttributes.FamANDAssem))
            {
                res += "private protected ";
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
