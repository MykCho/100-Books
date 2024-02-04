namespace _100_Books
{
    internal class Program
    {
        static void Main()
        {
            var book1 = new Book("C:\\nw\\dotnet Academy\\100 books\\testbook.txt");
            Console.WriteLine($"[Book Title]: \"{book1.Title}\"");
        }
    }
}
