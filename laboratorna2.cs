using System;
using System.Collections.Generic;
using System.Linq;

namespace LibrarySystemApp
{
    public class Book
    {
        public string Id { get; }
        public string Title { get; }
        public string Author { get; }
        public bool IsAvailable { get; set; } = true;

        public Book(string id, string title, string author)
        {
            Id = id;
            Title = title;
            Author = author;
        }

        public override string ToString() => 
            $"[Книга ID: {Id}] \"{Title}\" — {Author} | Статус: {(IsAvailable ? "Доступна" : "Видана")}";
    }
    public class User
    {
        public string Id { get; }
        public string Name { get; }
        private List<Book> _rentedBooks = new List<Book>();
        public IReadOnlyList<Book> RentedBooks => _rentedBooks.AsReadOnly();

        public User(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public void AddRentedBook(Book book)
        {
            _rentedBooks.Add(book);
        }

        public void RemoveRentedBook(Book book)
        {
            _rentedBooks.Remove(book);
        }

        public override string ToString() => 
            $"[Користувач ID: {Id}] {Name} | Активних оренд: {_rentedBooks.Count}";
    }
    public class Library
    {
        private List<Book> _books = new List<Book>();
        private List<User> _users = new List<User>();
        public void AddBook(Book book)
        {
            _books.Add(book);
        }
        public void AddUser(User user)
        {
            _users.Add(user);
        }
        public void BorrowBook(string userId, string bookId)
        {
            Console.WriteLine($"\n> Спроба видати книгу [ID: {bookId}] користувачу [ID: {userId}]...");
            
            var user = _users.FirstOrDefault(u => u.Id == userId);
            var book = _books.FirstOrDefault(b => b.Id == bookId);

            if (user == null)
            {
                Console.WriteLine($"[Помилка] Користувача з ID '{userId}' не знайдено в системі.");
                return;
            }

            if (book == null)
            {
                Console.WriteLine($"[Помилка] Книгу з ID '{bookId}' не знайдено в бібліотеці.");
                return;
            }
            if (!book.IsAvailable)
            {
                Console.WriteLine($"[Помилка] Книга \"{book.Title}\" вже орендована іншим користувачем і наразі недоступна.");
                return;
            }

            // Виконання видачі
            book.IsAvailable = false;
            user.AddRentedBook(book);
            Console.WriteLine($"[Успіх] Користувач {user.Name} успішно отримав в оренду книгу \"{book.Title}\".");
        }

        public void ReturnBook(string userId, string bookId)
        {
            Console.WriteLine($"\n> Спроба повернути книгу [ID: {bookId}] від користувача [ID: {userId}]...");

            var user = _users.FirstOrDefault(u => u.Id == userId);
            var book = _books.FirstOrDefault(b => b.Id == bookId);

            if (user == null)
            {
                Console.WriteLine($"[Помилка] Користувача з ID '{userId}' не знайдено.");
                return;
            }

            if (book == null)
            {
                Console.WriteLine($"[Помилка] Книгу з ID '{bookId}' не знайдено.");
                return;
            }
            if (!user.RentedBooks.Contains(book))
            {
                Console.WriteLine($"[Помилка] Користувач {user.Name} не брав в оренду книгу \"{book.Title}\", тому не може її повернути.");
                return;
            }
            book.IsAvailable = true;
            user.RemoveRentedBook(book);
            Console.WriteLine($"[Успіх] Користувач {user.Name} успішно повернув книгу \"{book.Title}\" до бібліотеки.");
        }
        public void ShowAllBooks()
        {
            Console.WriteLine("\n ЗАГАЛЬНИЙ ОБЛІК КНИГ У БІБЛІОТЕЦІ");
            foreach (var book in _books)
            {
                Console.WriteLine(book);
            }
        }
        public void ShowAvailableBooks()
        {
            Console.WriteLine("\n ДОСТУПНІ ДЛЯ ОРЕНДИ КНИГИ");
            var availableBooks = _books.Where(b => b.IsAvailable).ToList();
            
            if (availableBooks.Count == 0)
            {
                Console.WriteLine("Наразі немає доступних книг.");
            }
            else
            {
                foreach (var book in availableBooks)
                {
                    Console.WriteLine(book);
                }
            }
        }
        public void ShowAllUsers()
        {
            Console.WriteLine("\n ОБЛІК КОРИСТУВАЧІВ ТА ЇХНІХ ОРЕНД");
            foreach (var user in _users)
            {
                Console.WriteLine(user);
                if (user.RentedBooks.Count > 0)
                {
                    Console.WriteLine("  Орендовані книги:");
                    foreach (var book in user.RentedBooks)
                    {
                        Console.WriteLine($"    • \"{book.Title}\" — {book.Author} (ID: {book.Id})");
                    }
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== СИМУЛЯЦІЯ РОБОТИ СИСТЕМИ БІБЛІОТЕКИ ===\n");
            Library library = new Library();
            library.AddBook(new Book("B001", "Гаррі Поттер і філософський камінь", "Джоан Ролінґ"));
            library.AddBook(new Book("B002", "1984", "Джордж Орвелл"));
            library.AddBook(new Book("B003", "Маленький принц", "Антуан де Сент-Екзюпері"));
            library.AddUser(new User("U001", "Олександр Петренко"));
            library.AddUser(new User("U002", "Марія Коваленко"));
            library.ShowAllBooks();
            library.ShowAvailableBooks();
            library.BorrowBook("U001", "B002");
            library.ShowAvailableBooks();
            library.BorrowBook("U002", "B002");
            library.ShowAllUsers();
            library.ReturnBook("U001", "B002");
            library.ShowAvailableBooks();
            library.ShowAllUsers();
            library.ReturnBook("U002", "B001");
        }
    }
}