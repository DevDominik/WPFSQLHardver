using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppSQLTermekek
{
    public class Termek
    {
        string kategoria;
        string gyarto;
        string nev;
        int ar;
        int garido;

        public Termek(string kategoria, string gyarto, string nev, int ar, int garido)
        {
            this.kategoria = kategoria;
            this.gyarto = gyarto;
            this.nev = nev;
            this.ar = ar;
            this.garido = garido;
        }

        public static string ToCSVFromTermek(Termek termek)
        {
            return $"{termek.kategoria};{termek.gyarto};{termek.nev};{termek.ar};{termek.garido}";
        }

        public string Kategoria { get => kategoria; set => kategoria = value; }
        public string Gyarto { get => gyarto; set => gyarto = value; }
        public string Nev { get => nev; set => nev = value; }
        public int Ar { get => ar; set => ar = value; }
        public int Garido { get => garido; set => garido = value; }

    }
}