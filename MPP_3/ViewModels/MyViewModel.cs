using AssemblyExplorer.Models;
using MPP_3.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AssemblyExplorer.ViewModel
{
    public class MyViewModel : INotifyPropertyChanged
    {
        private string sourceString = "C:\\Users\\Admin\\Desktop\\Assembly_Browser-master\\Tests\\Resources\\MPP_3_TEST.dll";
        public string SourceString
        {
            get { return sourceString; }
            set
            {
                if (sourceString != value)
                {
                    sourceString = value;
                    OnPropertyChanged(nameof(SourceString));
                }
            }
        }

        private AssemblyNode selectedAssm;
        public AssemblyNode SelectedAssm
        {
            get { return selectedAssm; }
            set
            {
                if (selectedAssm != value)
                {
                    selectedAssm = value;
                }
            }
        }

        private RelayCommand addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand(obj =>
                  {
                      string path = obj as string ?? throw new NullReferenceException("Path is null");
                      Assembly? a = null;
                      try
                      {
                          a = Assembly.LoadFrom(path);
                      }
                      catch (Exception) 
                      {
                          sourceString = "File not found";
                          OnPropertyChanged(nameof(SourceString));
                          return;
                      }
                      AssemblyNode assembly = new AssemblyNode(new AssemblyModel(a));
                      if (this.assemblies.Where(asm => asm.Name == assembly.Name).ToList().Count == 0)
                      {
                          this.assemblies.Add(assembly);
                      }
                      OnPropertyChanged(nameof(Assemblies));
                  }));
            }
        }

        private RelayCommand removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ??
                  (removeCommand = new RelayCommand(obj =>
                  {
                      if (selectedAssm != null)
                      {
                          this.Assemblies.Remove(selectedAssm);
                            OnPropertyChanged(nameof(Assemblies));
                      }
                      selectedAssm = null;
                  }));
            }
        }

        private RelayCommand refreshCommand;
        public RelayCommand RefreshCommand
        {
            get
            {
                return removeCommand ??
                  (removeCommand = new RelayCommand(obj =>
                  {
                      if (selectedAssm != null)
                      {
                          string path = selectedAssm.path;
                          this.Assemblies.Remove(selectedAssm);
                          Assembly a = Assembly.LoadFrom(path);
                          AssemblyNode assembly = new AssemblyNode(new AssemblyModel(a));
                          this.assemblies.Add(assembly);
                          OnPropertyChanged(nameof(Assemblies));
                      }
                      selectedAssm = null;
                  }));
            }
        }

        private ObservableCollection<INode> assemblies;

        public ObservableCollection<INode> Assemblies
        {
            get { return assemblies; }
            set
            {
                if (assemblies != value)
                {
                    assemblies = value;
                    OnPropertyChanged(nameof(Assemblies));
                }
            }
        }

        public MyViewModel()
        {
            this.assemblies = new ObservableCollection<INode>();
        }

        public void add(AssemblyModel a) { this.Assemblies.Add(new AssemblyNode(a)); }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public interface INode: INotifyPropertyChanged
    {
        public string Name { get; }
    }

    public class AssemblyNode : INode
    {
        public AssemblyNode(AssemblyModel assemly) {
            this.Name = assemly.name;
            this.path = assemly.assembly.Location;
            this.Items = new List<INode>();
            foreach (var n in assemly.namespaces) {
                Items.Add(new NamespaceNode(n));
            }
        }
        internal string path;
        public string Name { get; set; }
        public List<INode> Items { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class NamespaceNode : INode
    {
        public NamespaceNode(NamespaceModel namespaceModel) { 
            this.Name = namespaceModel.name;
            this.Items = new List<INode>();
            foreach (var item in namespaceModel.Classes)
            {
                Items.Add(new ClassNode(item));
            }
        }
        public string Name { get; set; }
        public List<INode> Items { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class ClassNode : INode
    {
        public ClassNode(ClassModel cls) {
            this.Name = cls.name;
            this.Items = new List<INode>();
            if (cls._class.IsEnum)
            {
                ImagePath = "Images\\Enum.png";
            }
            else if (cls._class.IsSubclassOf(typeof(Delegate)))
            { 
                ImagePath = "Images\\Delegate.png"; 
            }
            else if (cls._class.IsValueType)
            {
                ImagePath = "Images\\Struct.png";
            }
            else if (cls._class.IsInterface)
            {
                ImagePath = "Images\\Interface.png";
            }
            else {
                ImagePath = "Images\\Class.png";
            }

            foreach (var item in cls.innerClasses)
            {
                Items.Add(new ClassNode(item));       
            }
            foreach (var item in cls._properties)
            {
                Items.Add(new PropertyNode(item));
            }
            foreach (var item in cls._fields)
            {
                Items.Add(new FieldNode(item));
            }
            foreach (var item in cls._constructors)
            {
                Items.Add(new ConstructorNode(item));
            }
            foreach (var item in cls._methods)
            {
                Items.Add(new MethodNode(item));
            }
            foreach (var item in cls._events)
            {
                Items.Add(new EventNode(item));
            }
        }
        public List<INode> Items { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class EventNode : INode
    {
        public EventNode(EventModel eventModel)
        {
            this.Name = eventModel.ToString();            
        }
        public string Name { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class MethodNode : INode
    {
        public MethodNode(MethodModel method)
        { 
            this.Name = method.ToString();
            if (method.extension)
            {
                ImagePath = "Images\\ExtensionMethod.png";
            }
            else {
                ImagePath = "Images\\Method.png";
            }
        }
        public string Name { get; set; }
        public string ImagePath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class FieldNode : INode
    {
        public FieldNode(FieldModel field)
        {
            this.Name = field.ToString();
            if (field.field.DeclaringType!.IsEnum && !field.field.Name.Contains("value__"))
            {
                ImagePath = "Images\\EnumValue.png";
            }
            else
            {
                ImagePath = "Images\\Field.png";
            }
        }

        public string Name { get; set; }
        public string ImagePath { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class ConstructorNode : INode
    {
        public ConstructorNode(ConstructorModel constructor)
        {
            this.Name = constructor.ToString();
        }
        public string Name { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class PropertyNode : INode
    {
        public PropertyNode(PropertyModel property)
        {
            this.Name = property.ToString();
        }
        public string Name { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
