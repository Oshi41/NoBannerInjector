namespace CheatLib;

public class Startup
{
    // Debug enter
    public static void Run()
    {
        Main("");
    }

    // DLL enter
    public static int Main(string args)
    {
        var app = new App();
        app.InitializeComponent();
        return app.Run();
    }
}