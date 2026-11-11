using System;

namespace TriangleLab
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Лабораторна\n");
            double a = 3.0;
            double b = 4.0;
            double c = 5.0;

            Console.WriteLine($"Задані сторони: a = {a}, b = {b}, c = {c}\n");

            if (a <= 0 || b <= 0 || c <= 0)
            {
                Console.WriteLine("Помилка: Усі сторони трикутника повинні бути додатними числами (> 0).");
                return;
            }
            bool exists = (a + b > c) && (a + c > b) && (b + c > a);

            if (!exists)
            {
                Console.WriteLine("Трикутник із такими сторонами НЕ існує (не виконується нерівність трикутника).");
                return;
            }

            Console.WriteLine("✓ Трикутник існує!");
            double perimeter = a + b + c;
            Console.WriteLine($"Периметр (P): {perimeter}");
            double p = perimeter / 2.0; 
            double area = Math.Sqrt(p * (p - a) * (p - b) * (p - c));
            Console.WriteLine($"Площа (S): {area:F2}");
            string type = DetermineTriangleType(a, b, c);
            Console.WriteLine($"Вид трикутника: {type}");
        }
        static string DetermineTriangleType(double a, double b, double c)
        {
            const double eps = 1e-9;

            bool sideAB = Math.Abs(a - b) < eps;
            bool sideBC = Math.Abs(b - c) < eps;
            bool sideAC = Math.Abs(a - c) < eps;
            if (sideAB && sideBC)
            {
                return "Рівносторонній";
            }
            double[] sides = { a, b, c };
            Array.Sort(sides); 
            bool isRight = Math.Abs(Math.Pow(sides[0], 2) + Math.Pow(sides[1], 2) - Math.Pow(sides[2], 2)) < eps;
            if (sideAB || sideBC || sideAC)
            {
                return isRight ? "Рівнобедрений прямокутний" : "Рівнобедрений";
            }
            if (isRight)
            {
                return "Прямокутний";
            }

            return "Довільний";
        }
    }
}
