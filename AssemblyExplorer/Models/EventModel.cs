using System.Reflection;

namespace AssemblyExplorer.Models
{
    public class EventModel
    {
        internal EventInfo _event;

        public EventModel(EventInfo eventModel)
        { 
            this._event = eventModel;
        }

        public override string ToString()
        {
            string res = "";
            res += SetModifier(_event.AddMethod!.Attributes);
            res += SetKeywords();
            res += _event.Name;
            res += SetProps(_event);
            res += " : ";
            res += _event.EventHandlerType!.Name;
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
            if (_event.AddMethod != null && _event.AddMethod.Attributes.HasFlag(MethodAttributes.Static))
            {
                return "static ";
            }
            else if (_event.RemoveMethod != null && _event.RemoveMethod.Attributes.HasFlag(MethodAttributes.Static))
                return "static ";
            return "";
        }

        private string SetProps(EventInfo ev)
        {
            string res = "{ add {} remove {} }";
            return res;
        }
    }
}
