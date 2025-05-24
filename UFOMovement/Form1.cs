using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace UFOMovement;

public partial class Form1 : Form
{

    private float currentX;
    private float currentY;
    private int objectRadius = 5;

    private PointF startPoint = new PointF(50, 50);
    private PointF endPoint = new PointF(700, 400);

    private float k_slope;
    private float b_intercept;

    private float movementStep = 2.0f;
    private int movementMethod = 0;

    private System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();


    public Form1()
    {
        InitializeComponent();
        this.DoubleBuffered = true;

        this.Paint += new PaintEventHandler(Form1_Paint);
        animationTimer.Tick += AnimationTimer_Tick;
        animationTimer.Interval = 20;

        InitializeMovementParameters();
    }

    private void InitializeMovementParameters()
    {
        currentX = startPoint.X;
        currentY = startPoint.Y;

        if (movementMethod == 0)
        {
            Line(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
        }

        animationTimer.Start();
    }

    private void Form1_Paint(object? sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.FillEllipse(Brushes.Red, currentX - objectRadius, currentY - objectRadius, 2 * objectRadius, 2 * objectRadius);

        g.FillEllipse(Brushes.Green, startPoint.X - 5, startPoint.Y - 5, 10, 10);
        g.FillEllipse(Brushes.Blue, endPoint.X - 5, endPoint.Y - 5, 10, 10);
    }

    private void AnimationTimer_Tick(object? sender, EventArgs e)
    {
        if (UpdateObjectPosition())
        {
            animationTimer.Stop();
        }
        this.Invalidate();
    }

    private bool UpdateObjectPosition()
    {
        if (movementMethod == 0)
        {
            return MoveLine();
        }
        return false;
    }

    private bool MoveLine()
    {
        float directionX = Math.Sign(endPoint.X - currentX);

        if (directionX == 0 || Math.Abs(currentX - endPoint.X) < movementStep / 2)
        {
            currentY = endPoint.Y;
            return true;
        }

        currentX += directionX * movementStep;
        currentY = k_slope * currentX + b_intercept;

        bool isReach = false;
        if (directionX > 0 && currentX >= endPoint.X)
        {
            isReach = true;
        }
        else if (directionX < 0 && currentX <= endPoint.X)
        {
            isReach = true;
        }

        if (isReach)
        {
            currentX = endPoint.X;
            currentY = endPoint.Y;
            return true;
        }
        return false;
    }

    private void Line(float x1, float y1, float x2, float y2)
    {
        k_slope = (y2 - y1) / (x2 - x1);
        b_intercept = y1 - k_slope * x1;
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
