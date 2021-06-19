using Gabriel.Cat.S.Utilitats;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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


namespace EFAuto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Context Context { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            //configuro para tener la BD lista
            Context = new Context(new DbContextOptionsBuilder<Context>().UseInMemoryDatabase("dbContext").Options);
            Context.AddObj(new Estudiante() { Persona=new Persona()});
        }

}

}
