using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Projet_Csharp
{

    //////////////////////
    ///Classe de stockage des informations du fichier srt
    //////////////////////
    class Srt
    {
        //Création des listes nbr / time début / time fin / texte sous-titre
        public List<string> nbrs = new List<string>();
        public List<string> debut = new List<string>();
        public List<string> fin = new List<string>();
        public List<string> text = new List<string>();


        //Fonction d'implémentation des informations dans des listes diverses
        public void SrtStock()
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            using (StreamReader sr = new StreamReader(mydocpath + @"\Star.Trek.Discovery.S03E05.Die.Trying.1080p.NF.WEB-DL.DDP5.1.x264-LAZY.srt"))
            {
                string ligne = "";
                int comp = 1;
                bool time_text = true;

                while ((ligne = sr.ReadLine()) != null)
                {

                    if (comp == 1)
                    {
                        nbrs.Add(ligne);
                    }

                    else if (comp == 2 && time_text == true)
                    {
                        if (ligne != "")
                        {
                            string[] split_ligne = ligne.Split(new string[] { " --> " }, System.StringSplitOptions.None);
                            debut.Add(split_ligne[0]);
                            fin.Add(split_ligne[1]);

                        }
                        else
                        {
                            time_text = false;
                        }

                    }
                    else if (comp == 3)
                    {
                        text.Add(ligne);
                    }
                    else if (comp == 4)
                    {
                        if (ligne != "")
                        {
                            if (text.Count > 0)
                            {
                                string last_text = text.LastOrDefault(); // Ligne 1/2 des sous-titre stockée dans une variable afin d'ajouter la 2/2
                                int index_text = text.LastIndexOf(last_text); // Récupération de l'index du sous-titre 1/2
                                text.Insert(index_text, last_text + " " + ligne);
                                string last_texte = text.LastOrDefault(); // Stockage text 1/2 incomplet
                                int index_texte = text.LastIndexOf(last_text); // Récupération index texte 1/2
                                text.Remove(last_texte);
                            }
                        }
                        else
                        {
                            comp = 0;
                        }
                    }
                    else
                    {
                        comp = 0;
                    }
                    comp++;
                }
            }

            /*Console.Write("Les numéros des sous-titre" + "\n");
            foreach (string nbr in nbrs)
            {
                Console.Write(nbr);
            }

            Console.Write("Les text des sous-titres" + "\n");
            foreach (string texte in text)
            {
                Console.Write(texte + "\n");
            }

            Console.Write("Le premier temps :" + "\n");
            foreach (string time1 in debut)
            {
                Console.Write(time1 + "\n");
            }

            Console.Write("Le deuxième temps :" + "\n");
            foreach (string time2 in fin)
            {
                Console.Write(time2 + "\n");
            }*/
        }
    }
}
