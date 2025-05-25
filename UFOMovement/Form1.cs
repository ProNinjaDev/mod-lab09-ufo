using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Windows.Forms;

namespace UFOMovement;

public partial class Form1 : Form
{

    private float currentX;
    private float currentY;
    private int objectRadius = 5;

    private PointF startPoint = new PointF(50, 50);
    private PointF endPoint = new PointF(700, 1950);
    private float targetZoneRadius = 5.0f;
    private bool useScaling = true;

    private float k_slope;
    private float b_intercept;

    private double targetAngle;
    private int nTermsForTrig = 5;

    private float movementStep = 1.0f;
    private int movementMethod = 1;

    private System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();

    private Button runExperimentButton;
    private Label statusLabel;


    public Form1()
    {
        InitializeComponent();
        this.DoubleBuffered = true;

        this.Paint += new PaintEventHandler(Form1_Paint);
        animationTimer.Tick += AnimationTimer_Tick;
        animationTimer.Interval = 20;

        runExperimentButton = new Button();
        runExperimentButton.Text = "Run experiment";
        runExperimentButton.AutoSize = true;
        runExperimentButton.Location = new Point(100, 10);
        runExperimentButton.Click += new EventHandler(RunExperimentButton_Click);
        this.Controls.Add(runExperimentButton);

        statusLabel = new Label();
        statusLabel.Text = "...";
        statusLabel.AutoSize = true;
        statusLabel.Location = new Point(100, runExperimentButton.Bottom + 5);
        this.Controls.Add(statusLabel);

        currentX = startPoint.X;
        currentY = startPoint.Y;

        if (movementMethod == 1) {
             targetAngle = Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X);
        }
    }

    private async void RunExperimentButton_Click(object? sender, EventArgs e)
    {
        runExperimentButton.Enabled = false;
        statusLabel.Text = "Experiment processing";

        PointF expStartPoint = this.startPoint;
        PointF expEndPoint = this.endPoint;
        float expMovementStep = this.movementStep;
        List<float> radiiTest = new List<float> { 0.1f, 0.2f, 0.5f, 1.0f, 2.0f, 4.0f, 8.0f, 12.0f, 16.0f };
        int minNTermsTest = 1;
        int maxNTermsTest = 20;

        List<string> csvLines = new List<string>();
        csvLines.Add("Radius;NTerms");

        await Task.Run(() =>
        {
            foreach (var currentTestRadius in radiiTest)
            {
                int foundNTerms = -1;

                for (int n = minNTermsTest; n <= maxNTermsTest; n++)
                {
                    double actualFinalDistance;
                    if (RunSilentSimulation(expStartPoint, expEndPoint,
                                            expMovementStep, n,
                                            currentTestRadius, out actualFinalDistance))
                    {
                        foundNTerms = n;
                        break;
                    }
                }

                csvLines.Add($"{currentTestRadius.ToString(CultureInfo.InvariantCulture)};{foundNTerms}");
            }
        });

        string baseDirectory = AppContext.BaseDirectory;
        string projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", ".."));
        string pathToCsv = Path.Combine(projectRoot, "result");
        string csvFilePath = Path.Combine(pathToCsv, "plot_data.csv");

        File.WriteAllLines(csvFilePath, csvLines);

        statusLabel.Text = "Experiment is over";
        runExperimentButton.Enabled = true;
        
    }

    private bool RunSilentSimulation(PointF simStartPoint, PointF simEndPoint,
                                    float simMovementStep, int simNTerms,
                                    float simTargetRadius, out double finalDistanceToTarget)
    {
        float currentSimX = simStartPoint.X;
        float currentSimY = simStartPoint.Y;

        double simTargetAngle = Math.Atan2(simEndPoint.Y - simStartPoint.Y, simEndPoint.X - simStartPoint.X);

        int maxIterations = 2500;

        for (int i = 0; i < maxIterations; i++)
        {
            double deltaX = simMovementStep * Cos(simTargetAngle, simNTerms);
            double deltaY = simMovementStep * Sin(simTargetAngle, simNTerms);

            currentSimX += (float)deltaX;
            currentSimY += (float)deltaY;

            finalDistanceToTarget = Math.Sqrt(Math.Pow(simEndPoint.X - currentSimX, 2) + Math.Pow(simEndPoint.Y - currentSimY, 2));

            if (finalDistanceToTarget < simMovementStep)
            {
                return (finalDistanceToTarget <= simTargetRadius);
            }

            double initialSimDistance = Math.Sqrt(Math.Pow(simEndPoint.X - simStartPoint.X, 2) + Math.Pow(simEndPoint.Y - simStartPoint.Y, 2));
            if (i > 10 && finalDistanceToTarget > initialSimDistance * 2 && initialSimDistance > 0) // TODO: зависимость от maxIterations
            {
                return false;
            }
        }

        finalDistanceToTarget = Math.Sqrt(Math.Pow(simEndPoint.X - currentSimX, 2) + Math.Pow(simEndPoint.Y - currentSimY, 2));
        return (finalDistanceToTarget <= simTargetRadius);
    }
    

    private void InitializeSingleVisualRun()
    {
        currentX = startPoint.X;
        currentY = startPoint.Y;

        if (movementMethod == 0)
        {
            Line(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
        }

        else if (movementMethod == 1)
        {
            targetAngle = Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X);
        }

        animationTimer.Start();
    }

    private void Form1_Paint(object? sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        if (useScaling)
        {
            g.ScaleTransform(0.5f, 0.5f);
        }

        g.DrawEllipse(Pens.LightGray, endPoint.X - targetZoneRadius, endPoint.Y - targetZoneRadius, 2 * targetZoneRadius, 2 * targetZoneRadius);

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
        else if (movementMethod == 1)
        {
            return MoveByAngle();
        }
        return true;
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

    private bool MoveByAngle()
    {
        double deltaX = movementStep * Cos(targetAngle, nTermsForTrig);
        double deltaY = movementStep * Sin(targetAngle, nTermsForTrig);

        currentX += (float)deltaX;
        currentY += (float)deltaY;

        double distanceToTarget = Math.Sqrt(Math.Pow(endPoint.X - currentX, 2) + Math.Pow(endPoint.Y - currentY, 2));

        if (distanceToTarget < movementStep / 2)
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

    private static double Atn(double x, int nTerms)
    {
        double result = 0;
        for (int i = 0; i < nTerms; i++)
        {
            double termSign = (i % 2 == 0) ? 1.0 : -1.0;
            int varTerm = 2 * i + 1;
            result += termSign * Math.Pow(x, varTerm) / varTerm;
        }
        return result;
    }
}
