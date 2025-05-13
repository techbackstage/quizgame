using QuizGame.API;

namespace application
{
    class Programm
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new MainWindow();
            app.InitializeComponent();
            app.Show();
            //ApiController.Run();
            
            Console.WriteLine("TEst");
        }
    }
}