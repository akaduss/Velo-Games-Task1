namespace Velo_Games_Task1
{
    public class Book
    {
        //Başlık, Yazar, ISBN, Kopya Sayısı ve Ödünç Alınan Kopyalar
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int CopiesBorrowed { get; set; }
        public int CopiesAvailable { get; set; }

        public Book(string title, string author, string isbn, int copiesAvailable)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
            CopiesAvailable = copiesAvailable;
        }

        public override string ToString()
        {
            return $"{Title} by {Author} - Copies Available: {CopiesAvailable}";
        }
    }
}
