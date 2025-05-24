using System.Drawing;
using System.Windows.Forms;

namespace UFOMovement;

public partial class Form1 : Form
{

    private int objectX = 50;
    private int objectY = 50;
    private int objectRadius = 20;
    public Form1()
    {
        InitializeComponent();
        this.DoubleBuffered = true;
        this.Paint += new PaintEventHandler(Form1_Paint);
    }

    private void Form1_Paint(object? sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.FillEllipse(Brushes.Red, objectX - objectRadius, objectY - objectRadius, 2 * objectRadius, 2 * objectRadius);
    }

    public static long Factorial(int n)
    {
        if (n == 0)
        {
            return 1;
        }

        long result = 1;
        for (int i = 1; i <= n; i++)
        {
            result *= i;
        }

        return result;
    }

    private static double Sin(double angle, int nTerms)
    {
        double result = 0;

        for (int i = 0; i < nTerms; i++)
        {
            double termSign = (i % 2 == 0) ? 1.0 : -1.0;
            int varTerm = 2 * i + 1;
            result += termSign * Math.Pow(angle, varTerm) / Factorial(varTerm);
        }

        return result;
    }

    private static double Cos(double angle, int nTerms)
    {
        double result = 0;

        for (int i = 0; i < nTerms; i++)
        {
            double termSign = (i % 2 == 0) ? 1.0 : -1.0;
            int varTerm = 2 * i;
            result += termSign * Math.Pow(angle, varTerm) / Factorial(varTerm);
        }

        return result;
    }
}
