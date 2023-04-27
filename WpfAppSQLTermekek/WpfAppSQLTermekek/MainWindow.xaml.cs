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
using MySql.Data.MySqlClient;
using System.IO;

namespace WpfAppSQLTermekek
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string kapcsolatVonal = "datasource=127.0.0.1;port=3306;username=root;password=;database=hardware;charset=utf8;";
        private List<Termek> termekek = new List<Termek>();
        private MySqlConnection? kapcsolat;
        private MySqlDataReader? olvaso;
        public MainWindow()
        {
            InitializeComponent();
            AdatbazisNyitas();
            TermekekBetolteseListaba();
            GyartokBetoltes(false);
            KategoriaBetoltes(false);
        }

        private void AdatbazisNyitas() {
            try
            {
                kapcsolat = new MySqlConnection(kapcsolatVonal);
                kapcsolat.Open();
            }
            catch (Exception hiba)
            {
                MessageBox.Show($"Nem lehet az adatbázishoz kapcsolódni: {hiba.Message}");
                this.Close();
            }
        }

        private void TermekekBetolteseListaba()
        {
            olvaso = new MySqlCommand("select * from termékek", kapcsolat).ExecuteReader();
            while (olvaso.Read())
            {
                termekek.Add(new Termek(olvaso.GetString("Kategória"), olvaso.GetString("Gyártó"), olvaso.GetString("Név"), Convert.ToInt32(olvaso.GetString("Ár")), Convert.ToInt32(olvaso.GetString("Garidő"))));
            }
            olvaso.Close();
            dgTermekek.ItemsSource = termekek;
        }

        private void KategoriaBetoltes(bool gyartoFigyelembeVetel)
        {
            cbKategoria.Items.Clear();
            string parancs = "select distinct kategória from termékek";
            if (gyartoFigyelembeVetel)
            {
                parancs = $"{parancs} where Gyártó='{cbGyarto.SelectedItem.ToString()}'";
            }
            parancs = $"{parancs} order by kategória";
            olvaso = new MySqlCommand(parancs, kapcsolat).ExecuteReader();
            cbKategoria.Items.Add("- Nincs kiválasztva -");
            while (olvaso.Read())
            {
                cbKategoria.Items.Add(olvaso.GetString("kategória"));
            }
            olvaso.Close();
            if (!gyartoFigyelembeVetel)
            {
                cbKategoria.SelectedIndex = 0;
            }
        }

        private void GyartokBetoltes(bool kategoriaFigyelembeVetel)
        {
            cbGyarto.Items.Clear();
            string parancs = "select distinct gyártó from termékek";
            if (kategoriaFigyelembeVetel)
            {
                parancs = $"{parancs} where Kategória='{cbKategoria.SelectedItem.ToString()}'";
            }
            parancs = $"{parancs} order by gyártó";
            olvaso = new MySqlCommand(parancs, kapcsolat).ExecuteReader();
            cbGyarto.Items.Add("- Nincs kiválasztva -");
            while (olvaso.Read())
            {
                cbGyarto.Items.Add(olvaso.GetString("gyártó"));
            }
            olvaso.Close();
            if (!kategoriaFigyelembeVetel)
            {
                cbGyarto.SelectedIndex = 0;
            }
        }

        private string SzukitettListaIras()
        {
            string parancs = "select * from termékek";
            if (cbGyarto.SelectedIndex > 0)
            {

            }
            return parancs;
        }

        private void AdatbazisZaras()
        {
            if (kapcsolat != null)
            {
                kapcsolat.Close();
                kapcsolat.Dispose();
            }
        }

        private void btnSzukit_Click(object sender, RoutedEventArgs e)
        {
            olvaso = new MySqlCommand(SzukitettListaIras(), kapcsolat).ExecuteReader();
            termekek.Clear();
            while (olvaso.Read())
            {
                termekek.Add(new Termek(olvaso.GetString("Kategória"), olvaso.GetString("Gyártó"), olvaso.GetString("Név"), Convert.ToInt32(olvaso.GetString("Ár")), Convert.ToInt32(olvaso.GetString("Garidő"))));
            }
            olvaso.Close();
            dgTermekek.Items.Refresh();
        }

        private void btnMentes_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = new StreamWriter("adatok.csv");
            foreach (Termek termek in termekek)
            {
                sw.WriteLine(Termek.ToCSVFromTermek(termek));
            }
            sw.Close();
            MessageBox.Show("Sikeres mentés");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            AdatbazisZaras();
        }

        private void cbKategoria_Selected(object sender, RoutedEventArgs e)
        {
            if (cbGyarto.SelectedIndex > 0)
            {
                KategoriaBetoltes(true);
            }
            else
            {
                KategoriaBetoltes(false);
            }
        }

        private void cbGyarto_Selected(object sender, RoutedEventArgs e)
        {
            if (cbKategoria.SelectedIndex > 0)
            {
                GyartokBetoltes(true);
            }
            else
            {
                GyartokBetoltes(false);
            }
        }
    }
}
