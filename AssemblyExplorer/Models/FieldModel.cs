using System.Reflection;

namespace AssemblyExplorer.Models
{
    public class FieldModel
    {
        public FieldInfo field { get; private set; }

        public FieldModel(FieldInfo field)
        {
            this.field = field;
        }

        public override string ToString()
        {
            string res = "";
            res += SetModifier(this.field.Attributes);
            res += SetKeywords(this.field.Attributes);
            res += this.field.Name;
            res += " : ";
            if (this.field.FieldType.IsGenericType)
                res += CreateGenericTypeString(this.field.FieldType);
            else
                res += this.field.FieldType.Name;
            return res;
        }

        private string SetKeywords(FieldAttributes attributes)
        {
            string res = "";
            if (attributes.HasFlag(FieldAttributes.Static))
            {
                res += "static ";
            }
            return res;
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
