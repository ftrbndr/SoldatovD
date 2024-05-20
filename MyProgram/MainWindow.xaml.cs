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
using System.Windows.Threading;

namespace MyProgram
{
    class MyObj
    {
        double vstart, tcycle, tstart, tfall, tcollision, tstep, a1, a2, pi = Math.PI, g = 9.81, obstx, obsty;
        static double[] xm = new double[10], ym = new double[10], vm = new double[10];
        public static int j = 0;
        public static double x_scale = 0, y_scale = 0;

        public delegate void Message();
        public event Message collision;
        public event Message ncollision;


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
            {
                tstart = tcollision;// Если тело попало в препятствие tstart приравниваю времени попадания
                collision();
            }
            else ncollision();
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

        public double get_max_x()
        {
            double res = obstx;
            foreach (double x in xm)
                if (x > res) res = x;
            return res;
        }

        public double get_max_y()
        {
            double res = obsty;
            foreach (double y in ym)
                if (y > res) res = y;
            return res;
        }


        public static double[] get_vm()
        {
            return vm;
        }
        public static double[] get_xm()
        {
            return xm;
        }
        public static double[] get_ym()
        {
            return ym;
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
            Obstx.Text = "500";
            Obsty.Text = "600";
        }
         private void Start_click(object sender, RoutedEventArgs e)
        {
            MyObj obj = new MyObj();
            obj.collision += coll;
            obj.ncollision += ncoll;
            try
            {
                input_test(Angle.Text, Velocity.Text, Time.Text, Obstx.Text, Obsty.Text);
                obj.get_var(Convert.ToDouble(Angle.Text), Convert.ToDouble(Velocity.Text), Convert.ToDouble(Time.Text), Convert.ToDouble(Obstx.Text), Convert.ToDouble(Obsty.Text));
                obj.calc();
                Finalx.Text = MyObj.get_xm()[9].ToString();
                Finaly.Text = MyObj.get_ym()[9].ToString();
                Finalv.Text = MyObj.get_vm()[9].ToString();
                MyObj.x_scale = obj.get_max_x() / 900;
                MyObj.y_scale = obj.get_max_y() / 300;
                Wall.X1 = Convert.ToDouble(Obstx.Text) / MyObj.x_scale;
                Wall.X2 = Wall.X1;
                Wall.Y2 = 300 - Convert.ToDouble(Obsty.Text) / MyObj.y_scale;
                Wall.Visibility = Visibility.Visible;
                Obj.Visibility = Visibility.Visible;
                DispatcherTimer tmr = new DispatcherTimer();
                tmr.Interval = TimeSpan.FromMilliseconds(500);
                tmr.Tick += TimerOnTick;
                tmr.Start();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Введите все данные", "Ошибка");
            }
            
        }
       public void coll()
        {
            Event_B.Visibility = Visibility.Visible;
        }

        public void ncoll()
        {
            Event_B.Visibility = Visibility.Collapsed;
        }

        public void TimerOnTick(object sender, EventArgs e)
        {
            
            double x = MyObj.get_xm()[MyObj.j] / MyObj.x_scale;
            double y = MyObj.get_ym()[MyObj.j] / MyObj.y_scale;
            Currx.Text = MyObj.get_xm()[MyObj.j].ToString();
            Curry.Text = MyObj.get_ym()[MyObj.j].ToString();
            Currv.Text = MyObj.get_vm()[MyObj.j].ToString();
            if (MyObj.j < 9) ++MyObj.j;
            else MyObj.j = 0;
            Obj.SetValue(Canvas.LeftProperty, x - 15);
            Obj.SetValue(Canvas.TopProperty, 300 - y);

        }
        public void input_test(string _a1, string _vstart, string _tstart, string _obstx, string _obsty)
        {
            if (_a1 == "" || _vstart == "" || _tstart == "" || _obstx == "" || _obsty == "")
                throw new NullReferenceException();
        }
    }
}
