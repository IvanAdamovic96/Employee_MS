using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsApp1
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());

            /*
             1. Skalabilnost: MongoDB je dizajniran da rukuje velikim količinama podataka i operacijama velike propusnosti.
                Kako vaša aplikacija raste i vi dodajete više zaposlenih, MongoDB može horizontalno da se povećava 
                dodavanjem više servera u klaster baze podataka.

             2. Fleksibilnost šeme: MongoDB-ova fleksibilna šema vam omogućava da ponavljate i razvijate svoje modele podataka 
                kako vaša aplikacija postaje složenija. Možete dodati nova polja u dokumente bez ometanja postojećih podataka, što 
                je korisno za aplikacije koje se razvijaju.
             
             3. Performanse: MongoDB pruža visoke performanse za operacije čitanja i pisanja zbog svoje prirode skladištenja podataka
                u BSON formatu. Njegove mogućnosti indeksiranja mogu dodatno poboljšati performanse upita.

             
             
             
             */
            //mongodb://localhost:27017
        }
    }
}
