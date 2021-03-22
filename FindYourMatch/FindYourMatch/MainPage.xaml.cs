using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FindYourMatch
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            // A-Wert minus 1, damit später A = 1
            var aWert = (int)'A' - 1; 
            
            var name1 = SourceName.Text.ToUpper();
            var name2 = TargetName.Text.ToUpper();

            // summe startet mit 68, damit wir nicht von 0 starten
            var summe = 68; 

            // alle Buchstaben (1-26) von Person 1 addieren
            for (int i = 0; i < name1.Length; i++)
            {
                summe = summe + (name1[i] - aWert);
            }

            // alle Buchstaben (1-26) von Person 2 addieren
            for (int i = 0; i < name2.Length; i++)
            {
                summe = summe + (name2[i] - aWert);
            }

            // damit das Ergebnis zwischen 0 und 100 liegt
            var treffer = summe % 101; 
            Result.Text = treffer.ToString() + "%";
        }
    }
}
