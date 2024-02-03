using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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

namespace DwaCalcter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point offset;
        public MainWindow()
        {
            InitializeComponent();

            // 订阅窗口大小变化事件
            this.SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 设置阈值，当窗口宽度小于200或高度小于150时显示滚动条
            double minWidthThreshold = 1200;
            double minHeightThreshold = 1000;

            if (e.NewSize.Width < minWidthThreshold || e.NewSize.Height < minHeightThreshold)
            {
                // 显示水平和竖直滚动条
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            }
            else
            {
                // 隐藏滚动条
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
        }
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (Exception)
            {
                // throw;
            }
        }

        private void MaxMinWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                e.Handled = true;
            }
        }

        private void MinMumBt_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void MaximumBt_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                // 窗口最大化后显示任务栏
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            }
        }


        private void CloseBt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Update_Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Tbx_Q_d_Knoz != null)
                {
                    // 一、设计流量
                    double Q_d_Knoz = double.Parse(Tbx_Q_d_Knoz.Text.Trim());
                    double Q_h_Knoz = Q_d_Knoz / 24;
                    Tbx_Q_h_Knoz.Text = Q_h_Knoz.ToString("F3");
                    if (Q_h_Knoz <= 13)
                    {
                        Tbx_Kz.Text = "2.7";
                    }
                    else if (Q_h_Knoz >= 2600)
                    {
                        Tbx_Kz.Text = "1.5";
                    }
                    else
                    {
                        Tbx_Kz.Text = (3.5778 * Math.Pow(Q_h_Knoz, -0.112)).ToString("F3");
                    }

                    double Q_d_max = Q_d_Knoz * double.Parse(Tbx_Kz.Text);
                    Tbx_Q_d_max.Text = Q_d_max.ToString("F3");
                    Tbx_Q_h_max.Text = (Q_d_max / 24).ToString("F3");

                    // 二、碳平衡
                    double f_s = double.Parse(Tbx_F_s.Text);
                    double f_A = double.Parse(Tbx_F_A.Text);
                    double f_B = double.Parse(Tbx_F_B.Text);
                    double f_COD = double.Parse(Tbx_F_COD.Text);
                    double C_COD_ZB = double.Parse(Tbx_C_COD_ZB.Text);
                    double X_COD_ZB = double.Parse(Tbx_X_COD_ZB.Text);
                    double S_COD_inert_ZB = double.Parse(Tbx_S_COD_inert_ZB.Text);
                    double X_COD_inert_ZB = double.Parse(Tbx_X_COD_inert_ZB.Text);
                    double C_COD_abb_ZB = double.Parse(Tbx_C_COD_abb_ZB.Text);
                    double X_TS_ZB = double.Parse(Tbx_C_SS_ZB.Text);
                    double S_NH4_AN = double.Parse(Tbx_S_NH4_AN.Text);
                    double S_orgN_AN = double.Parse(Tbx_S_orgN_AN.Text);
                    double S_TN_AN = double.Parse(Tbx_S_TN_AN.Text);
                    double S_TKN_AN = double.Parse(Tbx_S_TKN_AN.Text);

                    Tbx_X_TS_ZB.Text = X_TS_ZB.ToString();
                    Tbx_X_COD_ZB.Text = (X_TS_ZB * 1.6 * (1 - f_B)).ToString("F3");
                    Tbx_S_COD_ZB.Text = (C_COD_ZB - X_COD_ZB).ToString("F3");  //可溶解性COD
                    Tbx_S_COD_inert_ZB.Text = (f_s * C_COD_ZB).ToString("F3");  //溶解性惰性组分
                    Tbx_X_COD_inert_ZB.Text = (f_A * X_COD_ZB).ToString("F3");  //颗粒性惰性组分
                    Tbx_C_COD_abb_ZB.Text = (C_COD_ZB - S_COD_inert_ZB - X_COD_inert_ZB).ToString("F3");  //可降解COD
                    Tbx_C_COD_la_ZB.Text = (f_COD * C_COD_abb_ZB).ToString("F3");  //易降解COD

                    Tbx_X_anorg_TS_ZB.Text = (f_B * X_TS_ZB).ToString("F3");

                    //2.3出水氮平衡
                    Tbx_S_TKN_AN.Text = (S_NH4_AN + S_orgN_AN).ToString("F3");  //mg/L,出水凯氏氮
                    Tbx_S_anorgN_UW.Text = (S_TN_AN - S_TKN_AN).ToString("F3");  //mg/L,出水硝酸盐氮

                    //三、硝化菌泥龄
                    double B_D_COD_Z = double.Parse(Tbx_B_d_COD_Z.Text);
                    double miu_A_max = double.Parse(Tbx_miu_A_max.Text);
                    double T_C = double.Parse(Tbx_T_C.Text);

                    Tbx_B_d_COD_Z.Text = (Q_d_Knoz * C_COD_ZB / 1000).ToString("F3");  //kg/d,COD日负荷
                    if (B_D_COD_Z <= 2400) { Tbx_PF.Text = "2.1"; }
                    else if (B_D_COD_Z > 12000) { Tbx_PF.Text = "1.5"; }
                    else { Tbx_PF.Text = (2.1 - (B_D_COD_Z - 2400) * 0.6 / 9600).ToString("F3"); } //硝化反应系数
                    double PF = double.Parse(Tbx_PF.Text);
                    Tbx_t_TS_aerob_Bem.Text = (PF * 1.6 / miu_A_max * Math.Pow(1.103, (15 - T_C))).ToString("F3"); //d,硝化菌污泥龄

                    //四、反硝化体积比例VD/VBB

                    string COD_dos_name = Cbx_COD_dos_name.SelectedItem as string; //投加碳源类型
                    double Y_COD_dos = 0;
                    if (COD_dos_name == "甲醇") { Y_COD_dos = 0.45; }
                    if (COD_dos_name == "乙醇" || COD_dos_name == "醋酸") { Y_COD_dos = 0.42; }
                    Tbx_F_T.Text = (Math.Pow(1.072, (T_C - 15))).ToString("F3");  //内源呼吸的衰减系数


                    double C_COD_dos_f = 0;     //外加碳源化学需氧量
                    double x_f = 0;
                    double V_D_over_V_BB_f = 0.2;
                    double t_TS_aerob_Bem = double.Parse(Tbx_t_TS_aerob_Bem.Text);
                    double Y_COD_abb = double.Parse(Tbx_Y_COD_abb.Text);
                    double F_T = double.Parse(Tbx_F_T.Text);
                    double b = double.Parse(Tbx_B.Text);
                    double S_anorgN_UW = double.Parse(Tbx_S_anorgN_UW.Text);
                    double C_TN_ZB = double.Parse(Tbx.Text);

                    while (x_f < 1)
                    {
                        //4.1污泥产量的计算
                        double t_TS_Bem_f = t_TS_aerob_Bem / (1 - V_D_over_V_BB_f);  //设计污泥泥龄
                        Tbx_t_TS_Bem.Text = t_TS_Bem_f.ToString("F3");

                        double X_COD_BM_f = (C_COD_abb_ZB * Y_COD_abb + C_COD_dos_f * Y_COD_dos) /
                            (1 + b * t_TS_Bem_f * F_T);   //生物体中的COD
                        Tbx_X_COD_BM.Text = X_COD_BM_f.ToString("F3");

                        double X_COD_inert_BM_f = 0.2 * X_COD_BM_f * t_TS_Bem_f * b * F_T;  //剩余惰性固体
                        Tbx_X_COD_inert_BM.Text = X_COD_inert_BM_f.ToString("F3");

                        double US_d_C_f = Q_d_Knoz * (X_COD_inert_ZB / 1.33 + (X_COD_BM_f + 
                            X_COD_inert_ZB) / (0.93 * 1.42) + f_B * X_TS_ZB) / 1000;  //污泥产量
                        Tbx_US_d_C.Text = US_d_C_f.ToString("F3");

                        //4.2反硝化硝态氮浓度计算
                        double S_NO3_AN_f = 0.7 * S_anorgN_UW;  //出水硝态氮
                        Tbx_S_NO3_AN.Text = S_NO3_AN_f.ToString("F3");

                        double X_orngN_BM_f = 0.07 * X_COD_BM_f; //形成活性污泥的氮
                        Tbx_X_orngN_BM.Text = X_orngN_BM_f.ToString("F3");

                        double X_orgN_inert_f = 0.03 * (X_COD_inert_BM_f + X_COD_inert_ZB);  //与惰性颗粒结合的氮
                        Tbx_X_orgN_inert.Text = X_orgN_inert_f.ToString("F3");

                        double S_NO3_D_f = C_TN_ZB - S_NO3_AN_f - S_orgN_AN - S_NH4_AN - X_orngN_BM_f - X_orgN_inert_f;  //每日平均反硝化的硝态氮浓度
                        
                        
                        //4.3碳降解的需氧量
                        double OV_C_f = C_COD_abb_ZB + C_COD_dos_f - X_COD_BM_f - X_COD_inert_BM_f;  //碳降解的总需氧量
                        double OV_C_la_vorg_f = f_COD * C_COD_abb_ZB * (1 - Y_COD_abb) + C_COD_dos_f * (1 - Y_COD_dos); //反硝化区易降解及外加碳源需氧量
                        double OV_C_D_f = 0.75 * (OV_C_la_vorg_f + (OV_C_f - OV_C_la_vorg_f) * Math.Pow(V_D_over_V_BB_f, 0.68));  //反硝化区总需氧量
                        //4.4耗氧量和供氧量平衡
                        x_f = OV_C_D_f / 2.86 / S_NO3_D_f;
                        if (V_D_over_V_BB_f < 0.6 & x_f< 1) { V_D_over_V_BB_f += 0.01; }
                        if (V_D_over_V_BB_f >= 0.6 & x_f< 1) 
                        {
                            V_D_over_V_BB_f = 0.6;
                            C_COD_dos_f += 0.01;
                        }
                    }
                }
            }
            catch 
            {
                // continue;

            }
        }
    }
}
