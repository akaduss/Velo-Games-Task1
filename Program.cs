using System.Globalization;

namespace Velo_Games_Task1
{
    class Program
    {
        private const string DateFormat = "dd/MM/yyyy";

        static void Main()
        {
            BookLibrary library = new();

            Console.WriteLine($"\nWelcome to {library.LibrarySettings.LibraryName} Library");

            while (true)
            {
                PrintMenu();

                Console.Write("Enter your choice: ");
                if (int.TryParse(ConsoleMethods.ReadUserInput(), out int choice))
                {
                    Console.WriteLine();
                    switch (choice)
                    {
                        case 1:
                            AddNewBook(library);
                            break;

                        case 2:
                            library.DisplayBooks();
                            break;

                        case 3:
                            SearchForBook(library);
                            break;

                        case 4:
                            BorrowBook(library);
                            break;

                        case 5:
                            ReturnBook(library);
                            break;

                        case 6:
                            library.ListOverdueBooks();
                            break;

                        case 7:
                            RemoveOrDecreaseBook(library);
                            break;

                        case 8:
                            ConsoleMethods.ClearConsole();
                            break;

                        case 0:
                            Environment.Exit(0);
                            break;

                        default:
                            ConsoleMethods.PrintColoredMessage("Invalid choice. Please enter a valid option.", ConsoleColor.Red);
                            break;
                    }
                }
                else
                {
                    ConsoleMethods.PrintColoredMessage("Invalid choice. Please enter a valid option.", ConsoleColor.Red);
                }
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine("\nLibrary Management System Menu:");
            Console.WriteLine("1. Add a New Book");
            Console.WriteLine("2. Display All Books");
            Console.WriteLine("3. Search for a Book");
            Console.WriteLine("4. Borrow a Book");
            Console.WriteLine("5. Return a Book");
            Console.WriteLine("6. Display Overdue Books");
            Console.WriteLine("7. Remove Book or Decrease Copy Count");
            Console.WriteLine("8. Clear the Console");
            Console.WriteLine("0. Exit");
        }

        private static void AddNewBook(BookLibrary library)
        {
            Console.Write("Enter the title of the book: ");
            string title = ConsoleMethods.ReadUserInput();

            if (string.IsNullOrWhiteSpace(title))
            {
                ConsoleMethods.PrintColoredMessage("Title cannot be null or empty. Please enter a valid title.", ConsoleColor.Red);
                return;
            }

            Console.Write("Enter the author of the book: ");
            string author = ConsoleMethods.ReadUserInput();
            if (string.IsNullOrWhiteSpace(author))
            {
                ConsoleMethods.PrintColoredMessage("Author cannot be null or empty. Please enter a valid name.", ConsoleColor.Red);
                return;
            }

            Console.Write("Enter the ISBN of the book: ");
            string isbn = ConsoleMethods.ReadUserInput();
            if (!BookLibrary.IsISBNValid(isbn)) return;

            Console.Write("Enter the number of copies available: ");
            if (int.TryParse(ConsoleMethods.ReadUserInput(), out int copiesAvailable) && copiesAvailable >= 1)
            {
                library.AddBook(new Book(title, author, isbn, copiesAvailable));
                ConsoleMethods.PrintColoredMessage($"Book '{title}' added to the library.", ConsoleColor.Green);
            }
            else
            {
                ConsoleMethods.PrintColoredMessage("Invalid input for copies available. Please enter a valid number.", ConsoleColor.Red);
            }
        }

        private static void SearchForBook(BookLibrary library)
        {
            Console.Write("Enter the title or author to search: ");
            string searchTerm = ConsoleMethods.ReadUserInput().Trim();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.WriteLine("Please enter a book title or author name for searching.");
                return;
            }

            Book foundBook = library.SearchBook(searchTerm);
            if (foundBook == null)
            {
                Console.WriteLine("Book not found in the library.");
                return;
            }

            Console.WriteLine($"Book found: {foundBook.Title} by {foundBook.Author}");
        }

        private static void BorrowBook(BookLibrary library)
        {
            Console.Write("Enter the title of the book to be borrowed: ");
            string title = ConsoleMethods.ReadUserInput();

            Book foundBook = library.SearchBook(title);

            if (foundBook is null)
            {
                ConsoleMethods.PrintColoredMessage("Book not found!", ConsoleColor.Red);
                return;
            }
            else if (TryReadBorrowDate(out DateTime borrowDate))
            {
                library.BorrowBook(foundBook, borrowDate);

                // Check if the list is not empty before accessing its last element
                if (library.BorrowedBooks.ContainsKey(foundBook.ISBN) &&
                    library.BorrowedBooks[foundBook.ISBN].Any())
                {
                    var lastBorrowedItem = library.BorrowedBooks[foundBook.ISBN].Last();
                    ConsoleMethods.PrintColoredMessage($"Your borrow ID (Don't forget): {lastBorrowedItem.BorrowID}", ConsoleColor.Green);
                }

                Console.WriteLine($"Book '{foundBook.Title}' borrowed successfully on {borrowDate}");
            }
        }


        private static void ReturnBook(BookLibrary library)
        {
            Console.Write("Enter the title of the book to be returned: ");
            string title = ConsoleMethods.ReadUserInput();

            Console.Write("Enter the borrow ID of the book to be returned: ");
            string borrowIdInput = ConsoleMethods.ReadUserInput();

            Book foundBook = library.SearchBook(title);

            if (int.TryParse(borrowIdInput, out int borrowId))
            {
                if(library.TryReturnBook(foundBook, borrowId))
                {
                    Console.WriteLine($"Book '{foundBook.Title}' returned successfully.");
                }
                else
                {
                    ConsoleMethods.PrintColoredMessage("Invalid input for borrow ID. Please enter a valid number.", ConsoleColor.Red);
                }
            }
        }

        private static bool TryReadBorrowDate(out DateTime parsedBorrowDate)
        {
            Console.WriteLine("Borrow Date (Leave blank to set to \"Now\"),");
            Console.Write($"(Format should be {DateFormat} ex. 20/02/2002): ");
            string dateInput = ConsoleMethods.ReadUserInput();

            if (string.IsNullOrWhiteSpace(dateInput))
            {
                parsedBorrowDate = DateTime.Now;
                return true;
            }

            if (DateTime.TryParseExact(dateInput, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedBorrowDate))
            {
                return true;
            }
            else
            {
                Console.WriteLine($"Invalid date format. Please enter a date in the {DateFormat} format. Using current date and time.");
                return false;
            }
        }

        private static void RemoveOrDecreaseBook(BookLibrary library)
        {
            Console.Write("Enter the title of the book: ");
            string title = ConsoleMethods.ReadUserInput();
            if (string.IsNullOrWhiteSpace(title))
            {
                ConsoleMethods.PrintColoredMessage("Invalid input. Please enter a valid input.", ConsoleColor.Red);
                return;
            }

            Book bookToRemove = library.SearchBook(title);
            if (bookToRemove != null)
            {
                Console.WriteLine($"Book found: {bookToRemove.Title} by {bookToRemove.Author}");
                Console.WriteLine("Do you want to:\n1. Remove the entire book\n2. Decrease the number of copies\n0. Go back");

                Console.Write("Enter your choice (1 or 2): ");
                if (int.TryParse(ConsoleMethods.ReadUserInput(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            library.TryRemoveOrDecreaseCopiesOfBookByISBN(bookToRemove, userChoice: 1, copiesToRemove: 0);
                            break;

                        case 2:
                            Console.Write($"Enter the number of copies to decrease for '{bookToRemove.Title}': ");
                            if (int.TryParse(ConsoleMethods.ReadUserInput(), out int copiesToRemoveInner) && copiesToRemoveInner > 0)
                            {
                                library.TryRemoveOrDecreaseCopiesOfBookByISBN(bookToRemove, userChoice: 2, copiesToRemoveInner);
                                Console.WriteLine($"{copiesToRemoveInner} copies of '{bookToRemove.Title}' with ISBN '{bookToRemove.ISBN}' removed from the library.");
                            }
                            else
                            {
                                ConsoleMethods.PrintColoredMessage("Invalid input for the number of copies. No changes made.", ConsoleColor.Red);
                            }
                            break;

                        default:
                            Console.WriteLine("Invalid choice. No changes made.");
                            break;
                    }
                }
                else
                {
                    ConsoleMethods.PrintColoredMessage("Invalid input. Please enter a valid input.", ConsoleColor.Red);
                }
            }
            else
            {
                ConsoleMethods.PrintColoredMessage($"Book '{title}' not found in the library. Cannot remove.", ConsoleColor.Red);
            }
        }
    }
}
