using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace OhmschegesetzRechner
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();


            this.txtCurrent.Text = "";
            this.txtVoltage.Text = "";
            this.txtResistence.Text = "";

        }


      
        private void CheckInputs(object sender, TextChangedEventArgs e)
        {

            string textCurrent = this.txtCurrent.Text;
            string textVoltage = this.txtVoltage.Text;
            string textResistence = this.txtResistence.Text;

            this.ClearOutputs();

            bool isOneInputLeft = textCurrent.Equals(String.Empty) || textVoltage.Equals(String.Empty) || textResistence.Equals(String.Empty);            

            // show / hide Errormessage
            if (!isOneInputLeft)
            {
                this.ErrorLabel.Content = "Ein Feld muss leer bleiben.";
                this.ErrorLabel.Visibility = Visibility.Visible;
            } else
            {
                this.ErrorLabel.Visibility = Visibility.Hidden;
                // prüfen 2 von 3
                bool voltageIsMissing = textVoltage.Equals(String.Empty) && !textResistence.Equals(String.Empty) && !textCurrent.Equals(String.Empty);
                bool currentIsMussing = !textVoltage.Equals(String.Empty) && !textResistence.Equals(String.Empty) && textCurrent.Equals(String.Empty);
                bool resistenceIsMissing = !textVoltage.Equals(String.Empty) && textResistence.Equals(String.Empty) && !textCurrent.Equals(String.Empty);

                if (voltageIsMissing || currentIsMussing || resistenceIsMissing)
                {
                    double voltage = -1;
                    double resistence = -1;
                    double current = -1;

                    try
                    {
                        // convert to double
                        if (!voltageIsMissing)
                        {
                            voltage = Convert.ToDouble(textVoltage);
                        }
                        if (!currentIsMussing)
                        {
                            current = Convert.ToDouble(textCurrent);
                        }
                        if (!resistenceIsMissing)
                        {
                            resistence = Convert.ToDouble(textResistence);
                        }
                        // call Function
                        CalculateMissingValue(voltage, resistence, current);
                    }
                    // Abfangen ener ungültigen Zahl.
                    catch (FormatException ex)
                    {
                        this.ErrorLabel.Content = "Ein eingegebener Wert ist ungültig.";
                        this.ErrorLabel.Visibility = Visibility.Visible;
                        Console.WriteLine(ex.Message);
                    }
                }

                if (!voltageIsMissing && !currentIsMussing && !resistenceIsMissing)
                {
                    this.ErrorLabel.Content = "Es müssen mindestens zwei Werte gegeben sein.";
                    this.ErrorLabel.Visibility = Visibility.Visible;
                }
            }
            
        }


        /// <summary>
        /// Berechnen des Fehlenden Wertes und diesen in der Oberfläche anzeigen.
        /// </summary>
        /// <param name="voltage">Spannung - Wenn fehlend ist diese -1.</param>
        /// <param name="resistence"></param>
        /// <param name="current"></param>
        public void CalculateMissingValue(double voltage = -1, double resistence = -1, double current = -1 )
        {
            if (voltage == -1)
            {
                // berechne Spannung
                double value = resistence * current;
                this.outVoltage.Content = value.ToString() + " V";
                this.outCurrent.Content = current.ToString() + " A";
                this.outResistence.Content = resistence.ToString() + " \u2126";

            } else if (resistence == -1)
            {
                // berechne Widerstand
                double value = voltage / current;
                this.outResistence.Content = value.ToString() + " \u2126";
                this.outVoltage.Content = voltage.ToString() + " V";
                this.outCurrent.Content = current.ToString() + " A";
            } else
            {
                // berechne Strom
                double value = voltage / resistence;
                this.outCurrent.Content = value.ToString() + " A";
                this.outVoltage.Content = voltage.ToString() + " V";
                this.outResistence.Content = resistence.ToString() + " \u2126";
            }
        }

        private void ClearOutputs()
        {
            this.outCurrent.Content = "";
            this.outResistence.Content = "";
            this.outVoltage.Content = "";
        }
        
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex reg = new Regex("[^0-9]+.");
            e.Handled = reg.IsMatch(e.Text);
        }

        private void btnHelpVoltage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Die elektrische Spannung ist die Kraft, durch die der elektrische Strom bewegt wird. \n\nFormelzeichen : U\nEinheit : Volt / V",
                "Hilfe zur Spannung",
                MessageBoxButton.OK
                );
        }

        private void btnHelpResistence_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Die elektrische Widerstand gibt die erforderliche Spannung an, die benötigt wird um eine bestimmte elektische Stromstärke durch einen elektrischen Leiter fließen zu lassen. \n\nFormelzeichen : R\nEinheit : Ohm / \u2126",
                "Hilfe zum Widerstand",
                MessageBoxButton.OK
                );
        }

        private void btnHelpCurrent_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Der elektrische Strom ist die Anzahl an elektischen Ladungsträgern, die innerhalb eines Stromkreises fließen \n\nFormelzeichen : I\nEinheit : Ampere / A",
                "Hilfe zum Strom",
                MessageBoxButton.OK
                );
        }
    }
}
