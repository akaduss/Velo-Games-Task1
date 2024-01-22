using System.Text.Json;

namespace Velo_Games_Task1
{
    public class BookLibrary
    {
        private const string SettingsFileName = "librarysettings.json";
        private const string BooksFileName = "librarybooks.json";
        private const string BorrowedBooksFileName = "borrowedbooks.json";


        private List<Book> books;
        private Dictionary<string, List<BorrowedBook>> borrowedBooks;
        private LibrarySettings librarySettings;

        public LibrarySettings LibrarySettings => librarySettings;
        public Dictionary<string, List<BorrowedBook>> BorrowedBooks => borrowedBooks;

        public BookLibrary()
        {
            books = new List<Book>();
            borrowedBooks = new Dictionary<string, List<BorrowedBook>>();
            librarySettings = new LibrarySettings("Atatürk Kültür Merkezi", "İstanbul, Türkiye", 30); //Default Settings
            LoadLibraryData();
        }

        private void LoadLibraryData()
        {
            LoadJsonData(SettingsFileName, ref librarySettings);
            LoadJsonData(BooksFileName, ref books);
            LoadJsonData(BorrowedBooksFileName, ref borrowedBooks);
        }

        private static void LoadJsonData<T>(string fileName, ref T data)
        {
            try
            {
                string jsonData = File.ReadAllText(fileName);
                data = JsonSerializer.Deserialize<T>(jsonData);
            }
            catch (FileNotFoundException)
            {
                // File does not exist, ignore.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {fileName}: {ex.Message}");
            }
        }

        private void SaveLibraryData()
        {
            SaveToJsonFile(SettingsFileName, librarySettings);
            SaveToJsonFile(BooksFileName, books);
            SaveToJsonFile(BorrowedBooksFileName, borrowedBooks);
        }

        private static void SaveToJsonFile<T>(string fileName, T data)
        {
            try
            {
                string jsonData = JsonSerializer.Serialize(data);
                File.WriteAllText(fileName, jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving {fileName}: {ex.Message}");
            }
        }

        public void DisplayLibrarySettings()
        {
            Console.WriteLine("Library Settings:");
            Console.WriteLine($"Name: {librarySettings.LibraryName}");
            Console.WriteLine($"Location: {librarySettings.LibraryLocation}");
            Console.WriteLine($"Maximum Borrow Days: {librarySettings.MaximumBorrowDays}");
        }

        public void AddBook(Book book)
        {
            Book? existingBook = books.Find(b => b.ISBN == book.ISBN);

            if (existingBook != null)
            {
                existingBook.CopiesAvailable += book.CopiesAvailable;
                Console.WriteLine("This ISBN already exists.");
                Console.WriteLine($"Copies of book '{book.Title}' by '{book.Author}' increased in the library. Available copies: {existingBook.CopiesAvailable}");
            }
            else
            {
                books.Add(book);
            }

            SaveLibraryData();
        }

        public bool TryRemoveOrDecreaseCopiesOfBookByISBN(Book bookToRemove, int userChoice, int copiesToRemove)
        {
            bool returnVal;
            switch (userChoice)
            {
                default:
                    return false;
                case 1:
                    RemoveBook(bookToRemove);
                    returnVal = true;
                    break;

                case 2:
                    returnVal = TryDecreaseCopies(bookToRemove, copiesToRemove);
                    break;

            }
            SaveLibraryData();
            return returnVal;
        }

        private void RemoveBook(Book bookToRemove)
        {
            books.Remove(bookToRemove);
            Console.WriteLine($"Book '{bookToRemove.Title}' by '{bookToRemove.Author}' removed from the library.");
        }

        private static bool TryDecreaseCopies(Book book, int copiesToRemove)
        {
            if (copiesToRemove < book.CopiesAvailable && copiesToRemove > 0)
            {
                book.CopiesAvailable -= copiesToRemove;
                return true;
            }
            return false;
        }

        public void DisplayBooks()
        {
            if (books.Count == 0)
            {
                Console.WriteLine("No books in the library.");
            }
            else
            {
                Console.WriteLine("Books in the library:");
                foreach (var book in books)
                {
                    Console.WriteLine(book);
                }
            }
        }

        public Book SearchBook(string bookToSearch)
        {
            return books.Find(book => book.Title.Contains(bookToSearch, StringComparison.OrdinalIgnoreCase) ||
            book.Author.Contains(bookToSearch, StringComparison.OrdinalIgnoreCase));
        }

        public void BorrowBook(Book book, DateTime borrowDate)
        {
            if (book.CopiesAvailable > 0)
            {
                book.CopiesAvailable--;
                book.CopiesBorrowed++;

                BorrowedBook borrowedBook = new(borrowDate);

                string bookKey = book.ISBN;

                if (borrowedBooks.ContainsKey(bookKey))
                {
                    borrowedBooks[bookKey].Add(borrowedBook);
                }
                else
                {
                    borrowedBooks.Add(bookKey, new List<BorrowedBook> { borrowedBook });
                }

                SaveLibraryData();
            }
        }

        public bool TryReturnBook(Book book, int borrowID)
        {
            if (borrowedBooks.TryGetValue(book.ISBN, out List<BorrowedBook> borrowInstances))
            {
                BorrowedBook borrowedBookToRemove = borrowInstances.FirstOrDefault(b => b.BorrowID == borrowID);

                if (borrowedBookToRemove != null)
                {
                    book.CopiesAvailable++;
                    book.CopiesBorrowed--;
                    borrowInstances.Remove(borrowedBookToRemove);
                    SaveLibraryData();
                    return true;
                }
                else
                {
                    Console.WriteLine($"Borrowed book with ID {borrowID} not found for book '{book.Title}'.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine($"No borrowed instances found for book '{book.Title}'.");
                return false;
            }
        }

        public void ListOverdueBooks()
        {
            Console.WriteLine($"Books borrowed for more than {librarySettings.MaximumBorrowDays} days:");

            foreach (var bookISBN in borrowedBooks.Keys)
            {
                if (!TryGetBookByISBN(bookISBN, out Book book))
                {
                    Console.WriteLine($"Book with ISBN {bookISBN} not found in the library.");
                    continue;
                }

                List<BorrowedBook> borrowInstances = borrowedBooks[bookISBN];
                PrintOverdueBooks(book, borrowInstances);
            }
        }

        private void PrintOverdueBooks(Book book, List<BorrowedBook> borrowInstances)
        {
            foreach (var borrowInstance in borrowInstances)
            {
                TimeSpan borrowDuration = DateTime.Now - borrowInstance.BorrowedDate;

                if (borrowDuration.TotalDays > librarySettings.MaximumBorrowDays)
                {
                    Console.WriteLine($"{book.Title} by {book.Author} - Borrowed on {borrowInstance.BorrowedDate.ToShortDateString()} - {Math.Round(borrowDuration.TotalDays)} days ago");
                }
            }
        }

        private bool TryGetBookByISBN(string isbn, out Book book)
        {
            book = books.FirstOrDefault(b => b.ISBN == isbn);

            return book is not null;
        }

        public static bool IsISBNValid(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                ConsoleMethods.PrintColoredMessage("ISBN cannot be null or empty.Please enter a valid ISBN.", ConsoleColor.Red);
                return false;
            }
            if (isbn.Length != 13 && isbn.Length != 10)
            {
                ConsoleMethods.PrintColoredMessage("Invalid length for ISBN. Please enter a valid ISBN (10 or 13 digits).", ConsoleColor.Red);
                return false;
            }
            if (isbn.All(char.IsDigit) == false)
            {
                ConsoleMethods.PrintColoredMessage("ISBN can only contain numeric digits. Please enter a valid ISBN.", ConsoleColor.Red);
                return false;
            }
            return true;
        }

    }
}