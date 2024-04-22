namespace AssemblyExplorer.Models
{
    public class NamespaceModel
    {
        public string name { get; }
        public AssemblyModel assembly;
        public List<ClassModel> classes;
        public List<ClassModel> Classes { get { return classes; } }
        public NamespaceModel(string name)
        { 
            this.name = name;
            this.classes = new List<ClassModel>();
        }

        public NamespaceModel(string name, AssemblyModel asm)
        {
            this.assembly = asm;
            this.name = name;
            this.classes = new List<ClassModel>();
        }

        public NamespaceModel(string name, List<ClassModel> classes)
        {
            this.name = name;
            this.classes = classes;
        }

        public void AddClass(Type _class)
        {
            this.classes.Add(new ClassModel(_class, this));
        }

        public bool ReplaceClasses(ref List<ClassModel> cl, ClassModel cm) {
            if (cm._class.DeclaringType == null) return false;
            foreach (var c in cl) {
                if (c.innerClasses.Count > 0) {
                    ReplaceClasses(ref c.innerClasses, cm);
                }
                if (c.name == cm._class.DeclaringType.Name) { 
                    if(!c.InnerClasses.Contains(cm)) c.innerClasses.Add(cm);
                    return true;
                }
            }
            return false;
        }
    }
}
