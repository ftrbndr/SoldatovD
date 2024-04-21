using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyProgram
{
    class MyObj
    {
        double vstart, tcycle, tstart, tfall, tcollision, tstep, a1, a2, pi = Math.PI, g = 9.81, obstx, obsty;
        double[] xm = new double[10], ym = new double[10], vm = new double[10];

        public void get_var(double _a1, double _vstart, double _tstart, double _obstx, double _obsty) // Передача начальных данных
        {
            a1 = _a1;
            vstart = _vstart;
            tstart = _tstart;
            obstx = _obstx;
            obsty = _obsty;
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
        }

        public double get_fin_x()
        {
            return xm[9];
        }
        public double get_fin_y()
        {
            return ym[9];
        }
        public double get_fin_v()
        {
            return vm[9];
        }
    }

   


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Set_var.Click += Set_var_click;
            Start.Click += Start_click;
        }
            
        private void Set_var_click(object sender, RoutedEventArgs e)
        {
            Angle.Text = "40";
            Velocity.Text = "100";
            Time.Text = "50";
            Obstx.Text = "2000";
            Obsty.Text = "1000";
        }
         private void Start_click(object sender, RoutedEventArgs e)
        {
            MyObj obj = new MyObj();
            obj.get_var(Convert.ToDouble(Angle.Text), Convert.ToDouble(Velocity.Text), Convert.ToDouble(Time.Text), Convert.ToDouble(Obstx.Text), Convert.ToDouble(Obsty.Text));
            obj.calc();
            Finalx.Text = obj.get_fin_x().ToString();
            Finaly.Text = obj.get_fin_y().ToString();
            Finalv.Text = obj.get_fin_v().ToString();
        }
       
    }
}
