using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;

class MyProgram
{
    class MyObj
    {
        double vstart, tcycle, tstart, tfall, tcollision, tstep, a1, a2, pi = Math.PI, g = 9.81, obstx, obsty;
        double[] xm = new double[10], ym = new double[10], vm = new double[10];

        public void coll()
        {
            Console.WriteLine("Тело достигло земли");
        }


        public void get_var(double _a1, double _vstart, double _tstart, double _obstx, double _obsty) // Передача начальных данных
        {
            a1 = _a1;
            vstart = _vstart;
            tstart = _tstart;
            obstx = _obstx;
            obsty = _obsty;
        }
        public void ger_var_from() //Ввод начальных данных
        {
            double _a1, _vstart, _tstart, _obstx, _obsty;
            Console.WriteLine("Введите угол броска(в градусах), скорость(в м/с) и время(в секундах), прошедшее с момента броска, координату препятствия x, высоту препятствия y");
            _a1 = Convert.ToDouble(Console.ReadLine());
            _vstart = Convert.ToDouble(Console.ReadLine());
            _tstart = Convert.ToDouble(Console.ReadLine());
            _obstx = Convert.ToDouble(Console.ReadLine());
            _obsty = Convert.ToDouble(Console.ReadLine());
            get_var(_a1, _vstart, _tstart, _obstx, _obsty);
        }
        public void calc() //вычисление x и y от полученных начальных данных
        {
            a2 = a1 * (pi / 180); // Перевод градусов в радианы
            tfall = (2 * vstart * Math.Sin(a2)) / g;// Время в полете
            if (tfall < tstart) tstart = tfall;//Если с момента броска прошло больше времени, задает t1 значение времени приземления
            tcollision = obstx / (vstart * Math.Cos(a2));//Время столкновения тела с препятсвием
            if (tstart >= tcollision && (vstart * tcollision * Math.Sin(a2) - g * tcollision * tcollision / 2) <= obsty)//Проверка попало ли тело в препятствие
                tstart = tcollision;// Если тело попало в препятствие tstart приравниваю времени попадания
            tstep = tstart / 9; // Массив значений будет состоять из 10 элементов, рассчитываю шаг времени такой , чтобы за 10 итераций получить время падения
            for (int i = 0; i < 10; i++, tcycle += tstep)// Заполняю массив
            {
                vm[i] = Math.Sqrt(((vstart * Math.Cos(a2)) * (vstart * Math.Cos(a2))) + ((vstart * Math.Sin(a2) - g * tcycle) * (vstart * Math.Sin(a2) - g * tcycle)));// Вычисление скорости
                ym[i] = vstart * tcycle * Math.Sin(a2) - g * tcycle * tcycle / 2;// Вычисление ординаты
                xm[i] = vstart * tcycle * Math.Cos(a2);// Вычисление абсциссы
            }
            if (ym[9] < 1e-5) // Если тело приземлилось приравниваю его последнюю координату к 0
            {
                ym[9] = 0;
                
            }
            System.Windows.Forms.MessageBox.Show($"Конечные координаты: x: {xm[9]}  y: {ym[9]}", "Результат", MessageBoxButtons.OK); ; ; ;

        }


        public void print_coords()
        {
            Console.WriteLine("Координаты тела:");
            for (int i = 0; i < 10; i++)
                Console.WriteLine($"x: {xm[i]}  y: {ym[i]}  v: {vm[i]}");// Вывод координат тела во время полёта
            Console.WriteLine($"Шаг времени: {tstep} секунд");
        }
    }



    static void Main(string[] args)
    {
        DialogResult result = System.Windows.Forms.MessageBox.Show("Запустить программу?", "Бросок тела", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            MyObj obj = new MyObj();
            result = System.Windows.Forms.MessageBox.Show("Ввести начальные данные вручную?", "Ввод данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                obj.ger_var_from();
            obj.get_var(40, 100, 50, 7000, 1000);
            obj.calc();
            obj.print_coords();
        }
    }
}



