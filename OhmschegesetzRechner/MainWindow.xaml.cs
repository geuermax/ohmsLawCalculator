using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OhmschegesetzRechner
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            // Oberfläche initaialisieren
            InitializeComponent();

            // Den inhalt der Inputfelder initial setzen
            this.txtCurrent.Text = "";
            this.txtVoltage.Text = "";
            this.txtResistence.Text = "";

        }

        /// <summary>
        /// Eingabefelder überprüfen und ggf. Berechnung starten. Wird bei jeder Änderung in einem Eingabefeld ausgeführt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckInputs(object sender, TextChangedEventArgs e)
        {
            // Eingaben entnehmen
            string textVoltage = this.txtVoltage.Text;
            string textCurrent = this.txtCurrent.Text;
            string textResistence = this.txtResistence.Text;
            string textPower = this.txtPower.Text;

            // Zürücksetzen der Ergebnisse
            this.ClearOutputs();

            bool[] isInputSet = new bool[4];

            isInputSet[0] = !textVoltage.Equals(String.Empty);
            isInputSet[1] = !textCurrent.Equals(String.Empty);
            isInputSet[2] = !textResistence.Equals(String.Empty);
            isInputSet[3] = !textPower.Equals(String.Empty);

            int inputsSet = 0;
            foreach(bool isSet in isInputSet)
            {
                if (isSet)
                {
                    inputsSet += 1;
                }
            }

            // Fehlermeldung ausblenden
            this.ErrorLabel.Visibility = Visibility.Hidden;

            if (inputsSet <= 0)
            {
                // kein Wert ist eingegeben
            } else if (inputsSet == 1)
            {
                // Fehler es müssen 2 gesetzt sein
                this.ErrorLabel.Content = "Es müssen 2 Felder ausgefüllt sein.";
                this.ErrorLabel.Visibility = Visibility.Visible;
            } else if (inputsSet == 2)
            {
                // Berechnen
                // Convert string to double
                double voltage = !textVoltage.Equals(String.Empty) ? Convert.ToDouble(textVoltage) : 0;
                double current = !textCurrent.Equals(String.Empty) ? Convert.ToDouble(textCurrent) : 0;
                double resistence = !textResistence.Equals(String.Empty) ? Convert.ToDouble(textResistence) : 0;
                double power = !textPower.Equals(String.Empty) ? Convert.ToDouble(textPower) : 0;

                if (voltage == 0)
                {
                    // Try to calc voltage
                    if (current != 0 && resistence != 0)
                    {
                        voltage = CalcVoltage(current, resistence);
                    } else if (current != 0 && power != 0) 
                    {
                        voltage = CalcVoltageByPower(power, current);
                    }
                }

                if (current == 0)
                {
                    if (voltage != 0 && resistence != 0)
                    {
                        current = CalcCurrent(voltage, resistence);
                    } else if (power != 0 &&  resistence != 0) 
                    {
                        current = CalcCurrentByPowerAndResistence(power, resistence);
                    } else if (power != 0 && voltage != 0)
                    {
                        current = CalcCurrentByPowerAndVoltage(power, voltage);
                    }
                }

                if (resistence == 0)
                {
                    if (voltage != 0 && current != 0)
                    {
                        resistence = CalcResistence(voltage, current);
                    } else if (power != 0 && current != 0)
                    {
                        resistence = CalcResistenceByPower(power, current);
                    }
                }

                if (power == 0)
                {
                    power = CalcPower(voltage, current);
                }

                SetOutput(voltage, resistence, current, power);
            } else 
            {
                // Fehler es dürfen nur 2 gesetzt sein
                this.ErrorLabel.Content = "Es dürfen nur 2 Felder ausgefüllt sein.";
                this.ErrorLabel.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Ausgabefelder zurücksetzen.
        /// </summary>
        private void ClearOutputs()
        {
            this.outCurrent.Content = "";
            this.outResistence.Content = "";
            this.outVoltage.Content = "";
            this.outPower.Content = "";
        }

        private double CalcPower(double current, double voltage)
        {
            return current * voltage;
        }

        private double CalcCurrent(double voltage, double resistence) 
        {
            return voltage / resistence;
        }

        private double CalcCurrentByPowerAndVoltage(double power, double voltage)
        {
            return power / voltage;
        }

        private double CalcCurrentByPowerAndResistence(double power, double resistence)
        {
            return Math.Sqrt(power / resistence);
        }

        private double CalcVoltage(double current, double resistence)
        {
            return current * resistence;
        }

        private double CalcVoltageByPower(double power, double current)
        {
            return power / current;
        }        

        private double CalcResistence(double voltage, double current)
        {
            return voltage / current;
        }

        private double CalcResistenceByPower(double power, double current)
        {
            return power / Math.Pow(current, 2);
        }


        private void SetOutput(double voltage, double resistence, double current, double power)
        {
            this.outCurrent.Content = Math.Round(current, 3).ToString() + " A";
            this.outVoltage.Content = Math.Round(voltage, 3).ToString() + " V";
            this.outResistence.Content = Math.Round(resistence, 3).ToString() + " \u2126";
            this.outPower.Content = Math.Round(power, 3).ToString() + " W";
        }
        
        /// <summary>
        /// Validierung für die Eingabefelder, sodass nur Zahlen eingegeben werden können.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex reg = new Regex("[^0-9]+.");
            e.Handled = reg.IsMatch(e.Text);
        }

        /// <summary>
        /// Hilfe zur Spannung anzeigen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHelpVoltage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Die elektrische Spannung ist die Kraft, durch die der elektrische Strom bewegt wird. \n\nFormelzeichen : U\nEinheit : Volt / V",
                "Hilfe zur Spannung",
                MessageBoxButton.OK
                );
        }

        /// <summary>
        /// Hilfe zum Wiederstand anzeigen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHelpResistence_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Die elektrische Widerstand gibt die erforderliche Spannung an, die benötigt wird um eine bestimmte elektische Stromstärke durch einen elektrischen Leiter fließen zu lassen. \n\nFormelzeichen : R\nEinheit : Ohm / \u2126",
                "Hilfe zum Widerstand",
                MessageBoxButton.OK
                );
        }

        /// <summary>
        /// Hilfe zum Strom anzeigen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHelpCurrent_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Der elektrische Strom ist die Anzahl an elektischen Ladungsträgern, die innerhalb eines Stromkreises fließen. \n\nFormelzeichen : I\nEinheit : Ampere / A",
                "Hilfe zum Strom",
                MessageBoxButton.OK
                );
        }

        /// <summary>
        /// Hilfe zur elektischen Leistung anzeigen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHelpPower_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Die elektrische Leistung gibt die in eriner Zeitspanne umgesetzte Energie an. \n\nFormelzeichen : P\nEinheit : Watt / W",
                "Hilfe zur Leistung",
                MessageBoxButton.OK
                );
        }
    }
}
