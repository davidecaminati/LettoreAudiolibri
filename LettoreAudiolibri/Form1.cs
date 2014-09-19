/* Programma scritto da Davide Caminati il 19/9/2014
 * davide.caminati@gmail.com
 * http://caminatidavide.it/
 * 
 * licenza copyleft 
 * http://it.wikipedia.org/wiki/Copyleft#Come_si_applica_il_copyleft
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Speech.Synthesis;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        string percorso;
        WMPLib.WindowsMediaPlayer Player;
        bool onPause = false;
        Configuration conf;
        string currentPositionFilePath = "";
        SpeechSynthesizer Synth = new SpeechSynthesizer();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Parla("Audiolibri");
            string folder = @System.IO.Path.GetDirectoryName(@System.Reflection.Assembly.GetExecutingAssembly().Location);
            string path_fileconfig = folder + @"\config.txt";
            currentPositionFilePath = folder;
            //check if config file exist
            if (!System.IO.File.Exists(path_fileconfig))
            {
            //create default file config
                //first line for directory of the audiolibri directorys
                string[] lines = { @"C:", "|COM9" };
                // WriteAllLines creates a file, writes a collection of strings to the file,
                // and then closes the file.
                System.IO.File.WriteAllLines(path_fileconfig, lines);
            }
            // read config file
            ReadConfigFile(path_fileconfig);
            percorso = conf.BookPath;
            //StreamReader sr =  System.IO.File.OpenText(path_fileconfig);
            //percorso = sr.ReadLine();


            string[] elencolibri = Directory.GetDirectories(conf.BookPath);
            //append to the elencolibri each foldername finded in the path of the audiolibri
            foreach (string libro in elencolibri)
            {
                listViewFolder.Items.Add(System.IO.Path.GetFileNameWithoutExtension(libro));
            }

            if (listViewFolder.Items.Count > 0)
            {
                listViewFolder.Items[0].Selected = true;
                listViewFolder.Select();
            }
            
        }

        private bool CheckIfAllreadyPlayed(string filename)
        {
            if (!System.IO.File.Exists(Path.Combine(currentPositionFilePath, filename)))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void SaveCurrentPosition()
        {
            string filename = Player.controls.currentItem.name;
            //get the actual player position
            double position = Player.controls.currentPosition;
            //save the position to file
            System.IO.File.WriteAllText(Path.Combine(currentPositionFilePath, filename), position.ToString());
        }


        private void SetActualPlayTime(string filename)
        {
            string currentPosition = "";
            currentPosition = System.IO.File.ReadAllText(Path.Combine(currentPositionFilePath, filename));
            Player.controls.currentPosition =  Convert.ToDouble(currentPosition);
        }

        private void ReadConfigFile(string configFilePath)
        {
            try
            {
                string line;
                string portName = "";
                string path = "";
                // Read the configuration file line by line.
                System.IO.StreamReader file = new System.IO.StreamReader(@configFilePath);
                while ((line = file.ReadLine()) != null)
                {
                    if (line.StartsWith("@"))
                    {
                        path = line.Split('@')[1];
                    }
                    if (line.StartsWith("|"))
                    {
                        portName = line.Split('|')[1];
                    }
                }
                file.Close();

                conf = new Configuration(portName, path);
            }
            catch (Exception ex)
            {
                ParlaBloccante("il file di configurazione non puo' essere aperto causa: " + ex.Message);
            }
        }



        private void Muto()
        {
            Synth.SpeakAsyncCancelAll();
        }

        /// <summary>
        /// attiva il Synt vocale e riproduce il testo passato come parametro (puo' essere interrotto da un'altra riproduzione)
        /// </summary>
        /// <param name="args">testo da pronunciare</param>
        private void Parla(string args)
        {
            Muto();
            Synth.SpeakAsyncCancelAll();
            Synth.SpeakAsync(args);
        }

        /// <summary>
        /// attiva il Synt vocale e riproduce il testo passato come parametro (NON puo' essere interrotto da un'altra riproduzione)
        /// </summary>
        /// <param name="args">testo da pronunciare</param>
        private void ParlaBloccante(string args)
        {
            Muto();
            Synth.SpeakAsyncCancelAll();
            Synth.Speak(args);
        }

        private void listViewFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewFolder.SelectedItems.Count == 1)
            {
                int selectedIndex = listViewFolder.SelectedIndices[0];
                try
                {
                    Parla(listViewFolder.SelectedItems[0].Text.ToString());
                }
                catch { }
            }
        }

        private string[] GetListOfFile(string path)
        {
            return Directory.GetFiles(path);
        }

        private void listViewFolder_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if ((listViewFolder.Items.Count > 0) && (listViewFolder.SelectedItems.Count > 0))
                    {
                        //popolate listViewMp3
                        string[] listafile = GetListOfFile(Path.Combine(percorso, listViewFolder.SelectedItems[0].SubItems[0].Text));
                        //delete old element in mp3 list
                        listViewMp3.Clear();
                        //add new element
                        foreach (string mp3 in listafile)
                        {
                            listViewMp3.Items.Add(System.IO.Path.GetFileName(mp3),mp3);
                        }
                        //Focus on listViewMp3
                        if (listViewMp3.Items.Count > 0)
                        {
                            listViewMp3.Enabled = true;
                            listViewMp3.Items[0].Selected = true;
                            listViewMp3.Select();
                            listViewFolder.Enabled = false;
                        }
                        else 
                        {
                            Parla("non è possibile caricare i file audio");
                            listViewFolder.Enabled = true;
                        }
                    }
                    break;

                case Keys.Escape:
                    StopFile();
                    Parla("Chiusura programma");
                    Console.Beep();
                    this.Close();
                    break;

                case Keys.Down:
                    if (listViewFolder.Items.Count > 0)
                    {
                        if (listViewFolder.Items.Count - 1 == listViewFolder.Items.IndexOf(listViewFolder.SelectedItems[0]))
                        {
                            Parla("Ultimo elemento");
                        }
                    }
                    break;


                case Keys.Up:
                    StopFile();

                    if (listViewFolder.Items.Count > 0)
                    {
                        if (0 == listViewFolder.Items.IndexOf(listViewFolder.SelectedItems[0]))
                        {
                            Parla("primo elemento");
                        }
                    }
                    break;

            }
        }

        private void listViewMp3_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:

                    StopFile();
                    listViewFolder.Enabled = true;
                    listViewMp3.Clear();
                    listViewMp3.Enabled = false;

                    if (listViewFolder.Items.Count > 0)
                    {
                        //listViewFolder.Items[0].Selected = true;
                        listViewFolder.Focus ();
                    }
                    break;

                case Keys.Space:
                    PauseResumeFile();
                    break;

                case Keys.Down:
                    if (listViewMp3.Items.Count > 0)
                    {
                        if (listViewMp3.Items.Count - 1 == listViewMp3.Items.IndexOf(listViewMp3.SelectedItems[0]))
                        {
                            Parla("Ultimo elemento");
                        }
                    }
                    break;

                case Keys.Escape:
                    StopFile();
                    Parla("Chiusura programma");
                    Console.Beep();
                    this.Close();
                    break;


                case Keys.Enter:
                    if ((listViewMp3.Items.Count > 0) && (listViewMp3.SelectedItems.Count > 0))
                    {
                        //find path for mp3 file
                        string percorsofile = Path.Combine(percorso, listViewFolder.SelectedItems[0].SubItems[0].Text, listViewMp3.SelectedItems[0].SubItems[0].Text);
                        //stop eventualy other file
                        StopFile();
                        // open mp3file
                        PlayFile(percorsofile);
                    }
                    else
                    {
                        Parla("non è possibile caricare i file audio");
                        listViewFolder.Enabled = true;
                    }
                    break;

                case Keys.Up:
                    StopFile();

                    if (listViewMp3.Items.Count > 0)
                    {
                        if (0 == listViewMp3.Items.IndexOf(listViewMp3.SelectedItems[0]))
                        {
                            Parla("primo elemento");
                        }
                    }
                    break;

                case Keys.A:
                    SaveCurrentPosition();
                    break;
            }
        }

        private void listViewMp3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewMp3.SelectedItems.Count == 1)
            {
                int selectedIndex = listViewMp3.SelectedIndices[0];
                try
                {
                    Parla(listViewMp3.SelectedItems[0].Text.ToString());
                }
                catch { }
            }
        }

        private void PlayFile(String url, double pos = 0.0)
        {
            Muto();
            Player = new WMPLib.WindowsMediaPlayer();
            Player.PlayStateChange +=
                new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);
            Player.MediaError +=
                new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
            Player.URL = url;
            if (pos != 0.0)
            { 
                Player.controls.currentPosition = pos;
            }

        }

        private void StopFile()
        {
            try
            {
                SaveCurrentPosition();
                Player.controls.stop();
                
                onPause = false;
            }
            catch
            { }
        }

        private void PauseResumeFile()
        {
            try
            {
                if (onPause)
                { 
                    /* Pause the Player. */
                    Player.controls.play();
                    onPause = false;
                }
                else
                {
                    Player.controls.pause();
                    onPause = true;
                }
            }
            catch
            { }
        }


        private void Player_PlayStateChange(int NewState)
        {
            if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
            {
               // to do 
                //for eache file save the position and ask if continue from this position on load
                string posizione = Player.controls.currentPositionString;
            }
        }

        private void Player_MediaError(object pMediaObject)
        {
            Parla("errore nel caricamento del file");
        }


    }
}
