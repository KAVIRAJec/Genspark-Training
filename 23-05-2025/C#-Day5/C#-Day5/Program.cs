using C__Day5.Concrete_Class;
using C__Day5.Factory;
using C__Day5.Inrterface;
using C__Day5.Singleton;

public class Program
{
    public static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Choose file type: 1. TXT  2. JSON");
            var fileType = Console.ReadLine();

            string filePath = fileType == "2"
                ? "C:\\Users\\kaviraja\\Downloads\\Genspark-Training\\23-05-2025\\C#-Day5\\C#-Day5\\Files\\test.json"
                : "C:\\Users\\kaviraja\\Downloads\\Genspark-Training\\23-05-2025\\C#-Day5\\C#-Day5\\Files\\test.txt";

            FileManager.Instance.OpenFile(filePath, FileMode.OpenOrCreate);

            // Abstract Factory
            IFileFactory factory = fileType == "2"
            ? new JsonFileFactory() as IFileFactory
            : new TxtFileFactory() as IFileFactory;

            IFileReader reader = factory.CreateReader();
            IFileWriter writer = factory.CreateWriter();

            Console.WriteLine("Choose operation: 1. Read  2. Write, 3. EXIT");
            var operation = Console.ReadLine();

            if (fileType == "1")
            {
                if (operation == "1")
                {
                    string content = reader.Read<string>();
                    Console.WriteLine("File Content:");
                    Console.WriteLine(content);
                }
                else if (operation == "2")
                {
                    Console.WriteLine("Enter text to write:");
                    string input = Console.ReadLine();
                    writer.Write(input + Environment.NewLine);
                    Console.WriteLine("Written to TXT file.");
                }
            }
            else if (fileType == "2")
            {
                if (operation == "1")
                {
                    var data = reader.Read<List<string>>();
                    Console.WriteLine("File Content:");
                    if (data != null)
                        foreach (var item in data)
                            Console.WriteLine(item);
                    else
                        Console.WriteLine("(empty or invalid JSON)");
                }
                else if (operation == "2")
                {
                    Console.WriteLine("Enter comma-separated values to write as JSON array:");
                    string input = Console.ReadLine();
                    var list = new List<string>(input.Split(','));
                    writer.Write(list);
                    Console.WriteLine("Written to JSON file.");
                }
            } else
            {
                break;
            }
            FileManager.Instance.CloseFile();
        }
    }
}