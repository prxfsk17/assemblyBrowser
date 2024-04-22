// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

public class TestClass
{
    public class InnerClass
    {
        int a;
        int b;
        static int s;
        public enum En
        {
            CLASS,
            FIELD
        }

    }

    public delegate void AccountHandler(string message);
    protected event AccountHandler Notify { add { } remove { } }

    protected internal int prop { internal get; set; }

    public Dictionary<int, List<Dictionary<int, int>>> field;

    protected int aasd(ref int asdas, in int n, out int a, int[] b = null, params int[] p) { return a = 5; }

}
public static class MyExtensions
{
    public static int WordCount(this TestClass str)
    {
        return 5;
    }
}