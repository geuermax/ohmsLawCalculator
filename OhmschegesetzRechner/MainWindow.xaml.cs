﻿using System;
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
            string textCurrent = this.txtCurrent.Text;
            string textVoltage = this.txtVoltage.Text;
            string textResistence = this.txtResistence.Text;

            // Zürücksetzen der Ergebnisse
            this.ClearOutputs();

            // Überprüfen ob ein Eingabefeld noch frei ist
            bool isOneInputLeft = textCurrent.Equals(String.Empty) || textVoltage.Equals(String.Empty) || textResistence.Equals(String.Empty);            

            // show / hide Errormessage
            if (!isOneInputLeft)
            {
                this.ErrorLabel.Content = "Ein Feld muss leer bleiben.";
                this.ErrorLabel.Visibility = Visibility.Visible;
            } else
            {
                // Fehlermeldung ausblenden
                this.ErrorLabel.Visibility = Visibility.Hidden;

                // Ermittlen welcher Wert berechnet werden muss.
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
                        // Die Eingabe in eine Kommazahl konvertieren
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
                        // Fehler angeben, wenn die Konvertierung fehlgeschlagen ist.
                        this.ErrorLabel.Content = "Ein eingegebener Wert ist ungültig.";
                        this.ErrorLabel.Visibility = Visibility.Visible;
                        Console.WriteLine(ex.Message);
                    }
                }

                // Fehler anzeigen, wenn nicht mindestens zwei Felder ausgefüllt sind.
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
        /// <param name="voltage">Spannung - Wenn fehlend ist diese 0.</param>
        /// <param name="resistence">Widerstand - Wenn fehlend ist dieser 0.</param>
        /// <param name="current">Strom - Wenn fehlend ist dieser 0.</param>
        public void CalculateMissingValue(double voltage = 0, double resistence = 0, double current = 0 )
        {
            // Zahl mit -1 multiplizieren wenn dieser > 0 ist
            voltage = voltage < 0 ? voltage * -1 : voltage;
            resistence = resistence < 0 ? resistence * -1 : resistence;
            current = current < 0 ? current * -1 : current;
            


            if (voltage == 0)
            {
                // berechne Spannung
                double value = resistence * current;
                value = Math.Round(value, 3);
                this.outVoltage.Content = value.ToString() + " V";
                this.outCurrent.Content = current.ToString() + " A";
                this.outResistence.Content = resistence.ToString() + " \u2126";

            } else if (resistence == 0)
            {
                // berechne Widerstand
                double value = voltage / current;
                value = Math.Round(value, 3);
                this.outResistence.Content = value.ToString() + " \u2126";
                this.outVoltage.Content = voltage.ToString() + " V";
                this.outCurrent.Content = current.ToString() + " A";
            } else
            {
                // berechne Strom
                double value = voltage / resistence;
                value = Math.Round(value, 3);
                this.outCurrent.Content = value.ToString() + " A";
                this.outVoltage.Content = voltage.ToString() + " V";
                this.outResistence.Content = resistence.ToString() + " \u2126";
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
                "Der elektrische Strom ist die Anzahl an elektischen Ladungsträgern, die innerhalb eines Stromkreises fließen \n\nFormelzeichen : I\nEinheit : Ampere / A",
                "Hilfe zum Strom",
                MessageBoxButton.OK
                );
        }
    }
}