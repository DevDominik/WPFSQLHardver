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
        private bool engedve = false;
        private string? parancs;
        public MainWindow()
        {
            InitializeComponent();
            AdatbazisNyitas();
            TermekekBetolteseListaba();
            GyartokBetoltes();
            KategoriaBetoltes();
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
            olvaso = new MySqlCommand(ParancsIro(), kapcsolat).ExecuteReader();
            while (olvaso.Read())
            {
                termekek.Add(new Termek(olvaso.GetString("Kategória"), olvaso.GetString("Gyártó"), olvaso.GetString("Név"), Convert.ToInt32(olvaso.GetString("Ár")), Convert.ToInt32(olvaso.GetString("Garidő"))));
            }
            olvaso.Close();
            dgTermekek.ItemsSource = termekek;
        }

        private void KategoriaBetoltes()
        {
            engedve = false;
            if (cbKategoria.SelectedIndex == 0)
            {
                cbKategoria.Items.Clear();
            }
            parancs = "select distinct kategória from termékek";
            if (cbGyarto.SelectedIndex > 0)
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
            if (cbKategoria.SelectedIndex < 0)
            {
                cbKategoria.SelectedIndex = 0;
            }
            if (cbGyarto.SelectedIndex < 0)
            {
                cbGyarto.SelectedIndex = 0;
            }
            engedve = true;
            TartalomFrissites();
        }

        private void GyartokBetoltes()
        {
            engedve = false;
            if (cbGyarto.SelectedIndex == 0)
            {
                cbGyarto.Items.Clear();
            }
            parancs = "select distinct gyártó from termékek";
            if (cbKategoria.SelectedIndex > 0)
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
            if (cbGyarto.SelectedIndex < 0)
            {
                cbGyarto.SelectedIndex = 0;
            }
            if (cbKategoria.SelectedIndex < 0)
            {
                cbKategoria.SelectedIndex = 0;
            }
            engedve = true;
            TartalomFrissites();
        }

        private void TartalomFrissites()
        {
            engedve = false;
            olvaso = new MySqlCommand(ParancsIro(), kapcsolat).ExecuteReader();
            termekek.Clear();
            while (olvaso.Read())
            {
                termekek.Add(new Termek(olvaso.GetString("Kategória"), olvaso.GetString("Gyártó"), olvaso.GetString("Név"), Convert.ToInt32(olvaso.GetString("Ár")), Convert.ToInt32(olvaso.GetString("Garidő"))));
            }
            olvaso.Close();
            dgTermekek.Items.Refresh();
            engedve = true;
        }

        private string ParancsIro()
        {
            bool voltKriterium = false;
            parancs = "select * from termékek";
            if (cbKategoria.SelectedIndex > 0)
            {
                parancs = $"{parancs} where Kategória='{cbKategoria.SelectedItem.ToString()}'";
                voltKriterium = true;
            }
            if (voltKriterium)
            {
                if (cbGyarto.SelectedIndex > 0)
                {
                    parancs = $"{parancs} and Gyártó='{cbGyarto.SelectedItem.ToString()}'";
                }
            }
            else
            {
                if (cbGyarto.SelectedIndex > 0)
                {
                    parancs = $"{parancs} where Gyártó='{cbGyarto.SelectedItem.ToString()}'";
                    voltKriterium = true;
                }
            }
            if (voltKriterium)
            {
                if (txtTermek.Text.Trim().Length > 0)
                {
                    parancs = $"{parancs} and Név like '%{txtTermek.Text.Trim()}%'";
                }
            }
            else
            {
                if (txtTermek.Text.Trim().Length > 0)
                {
                    parancs = $"{parancs} where Név like '%{txtTermek.Text.Trim()}%'";
                }
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
            TartalomFrissites();
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

        private void cbKategoria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (engedve)
            {
                if (cbGyarto.SelectedIndex > 0)
                {
                    GyartokBetoltes();
                }
                else
                {
                    GyartokBetoltes();
                }
            }
        }

        private void cbGyarto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (engedve)
            {
                if (cbKategoria.SelectedIndex > 0)
                {
                    KategoriaBetoltes();
                }
                else
                {
                    KategoriaBetoltes();
                }
            }
            
        }
    }
}
