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

namespace Projet_Csharp
{
    public partial class MainWindow : Window
    {
        Srt s = new Srt(); // Utilisation de la classe de stockage des informations sous-titres

        // --Les booléen-- //
        bool time_st = false;
        bool continu = false; // Active ou non le programme de lecture de sous-titre

        // --Les string-- //
        string text_after_adapt; // Texte après adaptation avec retour à la ligne au bout de 6 mots

        // --Les int-- //
        int comp_lecteur_sous_titre = 0; // compteur fonction --> LecteurSousTitre()
        int comp_interval_time_afficher = 1; // Compteur fonction --> IntervalTimeAfficher()
        int comp_interval_time_non_afficher = 2; // Compteur fonction --> IntervalTimeNonAfficher()

        TimeSpan interval;

        public MainWindow() //Fonction d'initialisation des Composant WPF
        {
            InitializeComponent();
        }


////////////////
///Utilisation des sous-titre de la classe Srt et transformation de ces derniers
////////////////     
        public async Task LecteurSousTitre(int nbr_st)
        {
            string text_before_adapt = s.text[nbr_st - comp_lecteur_sous_titre];
            comp_lecteur_sous_titre++;
            string[] split_text = text_before_adapt.Split(new string[] { " " }, System.StringSplitOptions.None); // tableau découpe d'un string
            List<string> text_adapt = new List<string>();


            for (int i = 0; i < split_text.Length; i++)
            {
                text_adapt.Add(split_text[i]);

                if (text_adapt.Count > 6)
                {
                    text_adapt.Insert(6, "\n");
                }
            }
            text_after_adapt = "";

            for (int j = 0; j < text_adapt.Count; j++)
            {
                text_after_adapt += text_adapt[j];
                text_after_adapt += " ";
            }
            labelText.Content = text_after_adapt;
        }


        /////////////////////
        ///Calcul du temps d'apparition et disparition des sous-titres
        /////////////////////
        public async Task IntervalTimeNonAfficher(int number) // Lorsque le sous-titre n'est pas afficher
        {
            TimeSpan start_timer = TimeSpan.Parse(s.debut[number - comp_interval_time_non_afficher + 1]);
            TimeSpan end_timer = TimeSpan.Parse(s.fin[number - comp_interval_time_non_afficher]);
            comp_interval_time_non_afficher++;
            interval = start_timer - end_timer;
        }

        public async Task IntervalTimeAfficher(int number) // Lorsque le sous-titre est afficher
        {
            TimeSpan start_timer = TimeSpan.Parse(s.debut[number - comp_interval_time_afficher]);
            TimeSpan end_timer = TimeSpan.Parse(s.fin[number - comp_interval_time_afficher]);
            comp_interval_time_afficher++;
            interval = end_timer - start_timer;
        }


        /////////////////////////
        ///Lancement de l'affichage des sous-titres
        /////////////////////////
        public async Task TimeSt(bool time_st, bool continu)
        {
            bool activated = false; //Lorsque le sous-titre est affiché (activé)
            TimeSpan ts;


            //Premier timespan d'attente du premier sous-titre
            ts = TimeSpan.Parse(s.debut[0]);
            await Task.Delay(ts);


            // Boucle d'affichage des sous-titre
            while (continu == true)
            {
                for (int i = 0; i < s.debut.Count; i++)
                {
                    if (activated == true) //Si le sous-titre est afficher
                    {
                        Task interval_time_afficher = IntervalTimeAfficher(i);
                        await Task.Delay(interval);
                        labelText.Content = "";
                        activated = false;
                    }

                    else //Si le sous-titre n'est pas afficher
                    {
                        if (i == 0)
                        {
                            Task lecteur_sous_titre_zero = LecteurSousTitre(i);
                            activated = true;
                        }
                        else
                        {
                            Task interval_time_non_afficher = IntervalTimeNonAfficher(i);
                            await Task.Delay(interval);
                            Task lecteur_sous_titre = LecteurSousTitre(i);
                            activated = true;
                        }
                    }
                }
                continu = false;
            }
        }


        /////////////////
        ///Démarage deu code de sous-titrage
        /////////////////
        public async Task Start()
        {
            //Remplissage de la classe Srt servant a stocker toutes les informations du fichier srt
            s.SrtStock();

            // Lecture des sous-titre et adaptation de la taille (retour à la ligne si besoin
            Task run = TimeSt(time_st, continu);
        }


        ////////////////////
        ///Les boutons interface graphique
        ////////////////////
        private void ClickPlay(object sender, RoutedEventArgs e) // Bouton de lancement vidéo et sous-titre
        {
            myOptions.Play();
            continu = true;
            Task start = Start();
        }

        private void ClickPause(object sender, RoutedEventArgs e) // Bouton Pause vidéos
        {
            myOptions.Pause();
        }

        private void ClickStop(object sender, RoutedEventArgs e) // Bouton Stop vidéo
        {
            myOptions.Stop();
            continu = false;
        }
    }
}