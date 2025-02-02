using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace LibrarySystem
{
    public class LibraryForm : Form
    {
        private Library library;
        private ALLusers loggedInUser;

        private TextBox txtEmail, txtPassword, txtSearch, txtBookTitle, txtDescription;
        private Button btnLogin, btnSearch, btnPublish;
        private Label lblWelcome;
        private ListBox lstBooks;

        public LibraryForm()
        {
            this.Text = "Library System";
            this.Size = new System.Drawing.Size(600, 400);

            library = new Library();
            SetupSampleData();
            InitializeComponents();
        }

        private void SetupSampleData()
        {
            Author author = new Author("Ahmed", "ahmed@books.com", "authorPass");
            Reader reader = new Reader("Nada", "nada@example.com", "password123");

            library.RegisterUser(author);
            library.RegisterUser(reader);

            Book book1 = new Book(".NET Guide", "Ahmed", "A guide on .NET development.");
            author.PublishBook(book1, library.Books);
        }

        private void InitializeComponents()
        {
            Label lblEmail = new Label() { Text = "Email:", Left = 20, Top = 20 };
            txtEmail = new TextBox() { Left = 100, Top = 20, Width = 200 };

            Label lblPassword = new Label() { Text = "Password:", Left = 20, Top = 50 };
            txtPassword = new TextBox() { Left = 100, Top = 50, Width = 200, PasswordChar = '*' };

            btnLogin = new Button() { Text = "Login", Left = 100, Top = 80, Width = 100 };
            btnLogin.Click += BtnLogin_Click;

            lblWelcome = new Label() { Text = "", Left = 20, Top = 120, Width = 300 };

            txtSearch = new TextBox() { Left = 20, Top = 150, Width = 200 };
            btnSearch = new Button() { Text = "Search", Left = 230, Top = 150 };
            btnSearch.Click += BtnSearch_Click;

            lstBooks = new ListBox() { Left = 20, Top = 180, Width = 300, Height = 100 };

            Label lblBookTitle = new Label() { Text = "Book Title:", Left = 20, Top = 290 };
            txtBookTitle = new TextBox() { Left = 100, Top = 290, Width = 200 };

            Label lblDescription = new Label() { Text = "Description:", Left = 20, Top = 320 };
            txtDescription = new TextBox() { Left = 100, Top = 320, Width = 200 };

            btnPublish = new Button() { Text = "Publish Book", Left = 100, Top = 350 };
            btnPublish.Click += BtnPublish_Click;

            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(lblWelcome);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(lstBooks);
            this.Controls.Add(lblBookTitle);
            this.Controls.Add(txtBookTitle);
            this.Controls.Add(lblDescription);
            this.Controls.Add(txtDescription);
            this.Controls.Add(btnPublish);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Text;

            loggedInUser = library.Login(email, password);

            if (loggedInUser != null)
            {
                lblWelcome.Text = $"Welcome, {loggedInUser.Name}!";
                RefreshBookList();
            }
            else
            {
                MessageBox.Show("Invalid email or password!", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshBookList()
        {
            lstBooks.Items.Clear();
            foreach (var book in library.Books)
            {
                lstBooks.Items.Add($"{book.Title} - {book.Author}");
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string searchTitle = txtSearch.Text;
            var results = library.SearchBooks(searchTitle);

            lstBooks.Items.Clear();
            foreach (var book in results)
            {
                lstBooks.Items.Add($"{book.Title} - {book.Author}");
            }

            if (results.Count == 0)
            {
                MessageBox.Show("No books found!", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnPublish_Click(object sender, EventArgs e)
        {
            if (loggedInUser is Author author)
            {
                string title = txtBookTitle.Text;
                string description = txtDescription.Text;

                if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(description))
                {
                    Book newBook = new Book(title, author.Name, description);
                    author.PublishBook(newBook, library.Books);

                    RefreshBookList();
                    MessageBox.Show("Book published successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Please enter a title and description.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Only authors can publish books!", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LibraryForm());
        }
    }

    public class ALLusers
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }

        public ALLusers(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }
    }

    public class Reader : ALLusers
    {
        public Reader(string name, string email, string password) : base(name, email, password) { }
    }

    public class Author : ALLusers
    {
        public List<Book> PublishedBooks { get; private set; } = new List<Book>();

        public Author(string name, string email, string password) : base(name, email, password) { }

        public void PublishBook(Book book, List<Book> libraryBooks)
        {
            libraryBooks.Add(book);
            PublishedBooks.Add(book);
        }
    }

    public class Book
    {
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Description { get; private set; }

        public Book(string title, string author, string description)
        {
            Title = title;
            Author = author;
            Description = description;
        }
    }

    public class Library
    {
        public List<Book> Books { get; private set; } = new List<Book>();
        private Dictionary<string, ALLusers> users = new Dictionary<string, ALLusers>();

        public void RegisterUser(ALLusers user)
        {
            if (!users.ContainsKey(user.Email))
            {
                users.Add(user.Email, user);
            }
        }

        public ALLusers Login(string email, string password)
        {
            if (users.ContainsKey(email) && users[email].Password == password)
            {
                return users[email];
            }
            return null;
        }

        public List<Book> SearchBooks(string title)
        {
            return Books.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
