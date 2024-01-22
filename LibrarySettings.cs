namespace Velo_Games_Task1
{
    public class LibrarySettings
    {
        private int maximumBorrowDays;

        public string LibraryName { get; set; }
        public string LibraryLocation { get; set; }
        public int MaximumBorrowDays
        {
            get => maximumBorrowDays;
            set
            {
                if (value > 0)
                {
                    maximumBorrowDays = value;
                }
                else
                {
                    throw new ArgumentException("MaximumBorrowDays must be greater than zero.");
                }
            }
        }

        public LibrarySettings(string libraryName, string libraryLocation, int maximumBorrowDays)
        {
            LibraryName = libraryName;
            LibraryLocation = libraryLocation;
            MaximumBorrowDays = maximumBorrowDays;
        }
    }
}