using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Newtonsoft.Json;

namespace Zzdats.Carrer.Vacancies.Developer.Task
{
    public interface ISandwich
    {
        string Name
        {
            get;
            set;
        }
        void Create();
        string GetRecipe();
        void ImportRecipe();
        void Verify();
    }

    interface ISandwichMaker : ISandwich
    {
        void Create(ISandwich sandwich);
        void DisplayRecipe();
    }

    [Serializable]
    public class Sandwich : ISandwichMaker
    {
        //[Required(ErrorMessage = "Name is required")]
        private string name;

        #region "Properties"
        public string Name
        {
            get { return name; }
            set
            {
                if (value == "")
                {
                    throw new ArgumentException("Error: Name is required");
                }

                foreach (char item in value)
                {
                    if (char.IsDigit(item))
                    {
                        throw new ArgumentException("Error: Only letters allowed");
                    }
                }

                name = value;
            }
        }

        #endregion

        private List<string> basicComponent;
        public List<string> optionalComponents;

        #region "Methods"
        public void Create()
        {
            bool success = false;

            while (!success)
            {
                try
                {
                    Console.WriteLine("Sandwich name:");
                    Name = Console.ReadLine();
                    success = true;
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            basicComponent = new List<string>();
            string userInput;
            var i = 1;

            while (true)
            {
                Console.WriteLine("Add basic component (Y/N)? ");
                userInput = Console.ReadLine();

                if (userInput == "Y")
                {
                    i = 1;
                    Console.WriteLine("{0} basic component to add: ", i);
                    basicComponent.Add(Console.ReadLine().Trim());
                    break;
                }
                else Console.WriteLine("Error: Basic component is required");

                i++;
            }

            optionalComponents = new List<string>();
            var index = 3; // index is equal to optional component number

            while (i <= index)
            {
                Console.WriteLine("Add optional component (Y/N)? ");
                userInput = Console.ReadLine();

                if (userInput == "Y")
                {
                    Console.WriteLine("{0} optional component to add: ", i);
                    optionalComponents.Add(Console.ReadLine().Trim());
                }
                else Console.WriteLine("Optional component not added");

                i++;
            }

            optionalComponents.InsertRange(0, basicComponent);
        }

        public string GetRecipe()
        {
            Console.WriteLine();
            string str = string.Empty;
            Console.Write("\"Sandwich '" + Name + "' components: ");

            for (int i = 0; i < optionalComponents.Count; i++)
            {
                str = str + (i + 1) + "." + optionalComponents[i] + ", ";
            }
            str = str.Remove(str.Length - 2);
            Console.Write(str);
            Console.Write("\"");

            return str;
        }

        public void ImportRecipe()
        {
            try
            {
                try
                {
                    List<Sandwich> sandwichList = new List<Sandwich>();
                    string path = "test2.json";
                    Console.WriteLine();

                    string jsonRead = File.ReadAllText(path);

                    Console.WriteLine("Recipe(s) imported:");

                    using (StreamReader file = File.OpenText(path))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        List<Sandwich> objSandwich = (List<Sandwich>)serializer.Deserialize(file, typeof(List<Sandwich>));

                        JsonConvert.PopulateObject(jsonRead, sandwichList);

                        foreach (Sandwich item in objSandwich)
                        {
                            Console.Write("\"Sandwich '" + item.name + "' components: ");
                            string str = string.Empty;

                            for (int i = 0; i < item.optionalComponents.Count; i++)
                            {
                                str = str + (i + 1) + "." + item.optionalComponents[i] + ", ";
                            }
                            str = str.Remove(str.Length - 2);
                            Console.Write(str);
                            Console.Write("\"");
                            Console.WriteLine();
                        }
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Error: Could not create sandwich. Please check source");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error: Could not create sandwich. Please check source");
            }
        }

        public void Verify()
        {
            if (string.IsNullOrEmpty(Name) || !optionalComponents.Any())
            {
                throw new ArgumentNullException();
            }
        }

        public void Create(ISandwich sandwich)
        {
            Console.WriteLine("\n\tMENU:");
            Console.WriteLine("Press 'C' to create recipe:");
            Console.WriteLine("Press 'I' to import recipe:");
            Console.WriteLine("Press 'D' to display recipe:");
            Console.WriteLine("Press 'E' to exit menu:");

            string choice;

            do
            {
                Console.WriteLine("\nUser choice:");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "C":
                    case "c":
                        sandwich.Create();
                        Console.WriteLine("\nRecipe is created");
                        break;
                    case "I":
                    case "i":
                        sandwich.ImportRecipe();
                        break;
                    case "D":
                    case "d":
                        DisplayRecipe();
                        Console.WriteLine();
                        break;
                    case "E":
                    case "e":
                        Console.WriteLine("\nExiting...");
                        break;
                    default:
                        Console.WriteLine("Error: Please select C, I, D or E");
                        break;
                }
            }
            while (choice != "E");
        }

        public void DisplayRecipe()
        {
            try
            {
                Verify();
                GetRecipe();
            }
            catch (ArgumentException e)
            {
                Console.Write("\nThere is no recipe(s) to display. Create recipe first", e);
            }
        }

        #endregion

    }

    class Program
    {
        static void Main(string[] args)
        {
            ISandwichMaker sandwich = new Sandwich();
            sandwich.Create(sandwich);

            Console.Read();
        }
    }
}
