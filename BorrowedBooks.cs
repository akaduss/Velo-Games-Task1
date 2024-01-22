namespace Velo_Games_Task1
{
    [Serializable]
    public class BorrowedBook
    {
        public DateTime BorrowedDate { get; set; }
        public int BorrowID { get; set; }

        private static int nextID = 0;

        public BorrowedBook(DateTime borrowedDate)
        {
            BorrowedDate = borrowedDate;
            BorrowID = nextID++;
        }

        public BorrowedBook()
        {
            BorrowedDate = DateTime.Now;
            BorrowID = nextID++;
        }
    }
}
