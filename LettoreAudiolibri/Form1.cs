/* Programma scritto da Davide Caminati il 3/8/2014
 * davide.caminati@gmail.com
 * http://caminatidavide.it/
 * 
 * licenza copyleft 
 * http://it.wikipedia.org/wiki/Copyleft#Come_si_applica_il_copyleft
 */

// TODO 
// cleanup del codice

/* Example of c:\configfile.txt  the configuration file
@http://feeds.ilsole24ore.com/c/32276/f/438662/index.rss
@http://feeds.ilsole24ore.com/c/32276/f/566660/index.rss
|COM9
 */

// COMANDI
// ESC per uscire
// INVIO per leggere articolo
// slide per cambiare articolo
// button1 per cambio pagine
// button2 per pause
// button3 per play

// NOTE
// address.Text = "http://webvoice.tingwo.co/ilsole5642813vox?url=";


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Speech.Synthesis;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.IO.Ports;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        // VARIABLES
        bool onPause = false; // variable for trace WindowsMediaPlayer state
        int buttonState1 = 1;
        int buttonState2 = 1;
        int buttonState3 = 1;
        List<ListViewItem> filesPage = new List<ListViewItem>();
        List<string> listUrl = new List<string>();

        string currentPositionFilePath = "";
        int actualIndex = 0;
        int slideStart = 0;
        int lastSlideValue = 0;

        bool TitoloDaLeggere = false;
        int numElementInPage = 7; // Depends on Arduino Slide Settings

        // OBJECTS
        Configuration Conf = new Configuration();
        WMPLib.WindowsMediaPlayer Player;
        SerialPort Sp;
        SpeechSynthesizer Synth = new SpeechSynthesizer();

        public Form1()
        {
            InitializeComponent();

            Synth.SetOutputToDefaultAudioDevice();  // Configure the audio output for speech synth. 
            
            // configure the application
            
            currentPositionFilePath = @System.IO.Path.GetDirectoryName(@System.Reflection.Assembly.GetExecutingAssembly().Location);
            string fileconfig = currentPositionFilePath + @"\config.txt";

            if (Conf.LoadConfiguration(fileconfig))
            {
                PopolalistFileComplete(Conf.BookPath);               // create list of MP3 FILES

                Sp = new SerialPort(Conf.ComPort);
                ParlaBloccante("quando sei su un fail premi spazio se vuoi rimuovere il punto salvato e riascoltarlo dall'inizio");
                LeggiTitolo();
            }
            else
            {
                MessageBox.Show(this, "errore nel caricamento del file di configurazione");
            }

        }

        /// <summary>
        /// lettura titolo libro (viene lanciato automaticamente quando si seleziona un libro)
        /// </summary>
        private void LeggiTitolo()
        {
            //stop eventualy other file
            StopFile();
            try
            {
                Parla(actualIndex.ToString() + ". " + filesPage[actualIndex].Text); //dico il numero (posizione del file) e poi titolo libro
            }
            catch
            {
            }
        }

        /// <summary>
        /// Carica l'elenco dei file dal percorso caricato durante la lettura del file di configurazione
        /// </summary>
        private void PopolalistFileComplete(string BookPath)
        {
            string[] elencolibri = Directory.GetDirectories(BookPath);
            //append to the elencolibri each foldername finded in the path of the audiolibri
            foreach (string libro in elencolibri)
            {
                foreach (string file in GetListOfFile(libro))
                {

                    string[] r = { Path.GetFileNameWithoutExtension(file), file };
                    filesPage.Add(new ListViewItem(r));
                }
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            if (filesPage.Count > 0)
            {
                OpenSerialPort();   // Open the serial port
            }
            else
            {
                ParlaBloccante("Errore durante caricamento lista articoli. programma bloccato, si consiglia di chiudere il programma");
            }
        }


        /// <summary>
        /// return all the file in specified directory path
        /// </summary>
        /// <param name="path">directory to scan</param>
        /// <returns></returns>
        private string[] GetListOfFile(string path)
        {
            return Directory.GetFiles(path);
        }



        private void CancelOldSpeak()
        {
            Synth.SpeakAsyncCancelAll();
        }

        /// <summary>
        /// attiva il Synt vocale e riproduce il testo passato come parametro (puo' essere interrotto da un'altra riproduzione)
        /// </summary>
        /// <param name="args">testo da pronunciare</param>
        private void Parla(string args)
        {
            CancelOldSpeak();
            Synth.SpeakAsync(args);
        }

        /// <summary>
        /// attiva il Synt vocale e riproduce il testo passato come parametro (NON puo' essere interrotto da un'altra riproduzione)
        /// </summary>
        /// <param name="args">testo da pronunciare</param>
        private void ParlaBloccante(string args)
        {
            CancelOldSpeak();
            Synth.Speak(args);
        }


        private void NextPage()
        {
            if (actualIndex < (filesPage.Count - numElementInPage))
            {
                actualIndex += numElementInPage;
                slideStart += numElementInPage;
            }
            else
            {
                actualIndex = 0;
                slideStart = 0;
            }
        }


        private void OpenSerialPort()
        {
            try
            {
                Sp.BaudRate = 9600;
                Sp.Parity = Parity.None;
                Sp.StopBits = StopBits.One;
                Sp.DataBits = 8;
                Sp.Handshake = Handshake.None;
                Sp.DataReceived += SerialPortDataReceived;
                Sp.Open();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }


        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                string buff = sp.ReadLine();
                if (!String.IsNullOrEmpty(buff))
                {
                    char[] delimiterChars = { ' ' };
                    string[] words = buff.Split(delimiterChars);
                    if (words.Count() == 4)
                    {
                        int a = Convert.ToInt32(words[0].ToString());
                        actualIndex = slideStart + a;
                        if (lastSlideValue != a)
                        {
                            TitoloDaLeggere = true;
                        }
                        lastSlideValue = a;
                        buttonState1 = Convert.ToInt32(words[1].ToString());
                        buttonState2 = Convert.ToInt32(words[2].ToString());
                        buttonState3 = Convert.ToInt32(words[3].ToString());
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Select next file
        /// </summary>
        private void SelezionaProssimoFile()
        {
            if (actualIndex < filesPage.Count - 1)
            {
                actualIndex += 1;
            }
            else
            {
                ParlaBloccante("ultimo elemento");
            }
        }

        /// <summary>
        /// select previous file
        /// </summary>
        private void SelezionaPrecedenteFile()
        {
            if (actualIndex > 0)
            {
                actualIndex -= 1;
            }
            else
            {
                ParlaBloccante("primo elemento");
            }
        }

        /// <summary>
        /// stop actual file
        /// </summary>
        private void StopFile()
        {
            try
            {
                Player.controls.stop();
                Player.close();
                onPause = false;
            }
            catch
            { }
        }

        /*
        private void Player_PlayStateChange(int NewState)
        {
            if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
            {
                // to do 
                // for eache file save the position and ask if continue from this position on load
                string posizione = Player.controls.currentPositionString;
            }
        }
        */

        private void Player_MediaError(object pMediaObject)
        {
            ParlaBloccante("errore nel caricamento del file");
        }

        /// <summary>
        /// play or pause file
        /// </summary>
        private void PauseResumeFile()
        {
            try
            {
                if (onPause)
                {
                    Player.controls.play();
                    onPause = false;
                }
                else
                {
                    Player.controls.pause();
                    SaveCurrentPosition();
                    ParlaBloccante("pausa. posizione salvata");
                    onPause = true;
                }
            }
            catch
            {
                ParlaBloccante("prima di mettere in pausa è necessario avviare il file");
            }
        }

        /// <summary>
        /// Start play
        /// </summary>
        private void PlayFile()
        {
            CancelOldSpeak();
            Player = new WMPLib.WindowsMediaPlayer();
            //Player.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);
            Player.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
            ListViewItem lwi = filesPage[actualIndex];
            string url = lwi.SubItems[1].Text;
            string filename = lwi.SubItems[0].Text;
            Player.URL = url;
            double pos = FindPosition(filename);
            Player.controls.currentPosition = pos;
        }


        private double FindPosition(string filename)
        {
            if (File.Exists(Path.Combine(currentPositionFilePath, filename)))
            {
                return GetOldPlayTime(filename);
            }
            else
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Save actual position to file
        /// </summary>
        private void SaveCurrentPosition()
        {
            string filename = Player.controls.currentItem.name;
            //get the actual player position
            double position = Player.controls.currentPosition;
            //save the position to file
            System.IO.File.WriteAllText(Path.Combine(currentPositionFilePath, filename), position.ToString());
        }

        /// <summary>
        /// delete file of position
        /// </summary>
        private void DeleteCurrentPosition()
        {
            ListViewItem lwi = filesPage[actualIndex];
            string filename = lwi.SubItems[0].Text;
            if (File.Exists(Path.Combine(currentPositionFilePath, filename)))
            {
                System.IO.File.Delete(Path.Combine(currentPositionFilePath, filename));
                ParlaBloccante("ora il fail ricomincerà dall'inizio");
            }
            else
            {
                ParlaBloccante("non esiste punto di salvataggio per questo fail");
            }
        }


        /// <summary>
        /// find old position from file
        /// </summary>
        /// <param name="filename">name of the file without extension</param>
        /// <returns></returns>
        private double GetOldPlayTime(string filename)
        {
            string currentPosition = "";
            currentPosition = System.IO.File.ReadAllText(Path.Combine(currentPositionFilePath, filename));
            if (Convert.ToDouble(currentPosition) > 10.0)
            {
                return Convert.ToDouble(currentPosition) - 10.0; // 10 seconds before old time
            }
            else
            {
                return Convert.ToDouble(currentPosition);
            }
        }


        // FAST (but not so elegant) solution for cross threading in serial data input
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (TitoloDaLeggere)
                {
                    LeggiTitolo();
                    TitoloDaLeggere = false;
                }

                if (buttonState3 == 0)
                {
                    UseFile();
                    buttonState3 = 1;       // variable reset
                }

                if (buttonState2 == 0)
                {
                    PauseResumeFile();      // pause or resume
                    buttonState2 = 1;       // variable reset
                }

                if (buttonState1 == 0)
                {
                    VaiAllaProssimaPagina();// change page
                    buttonState1 = 1;       //variable reset
                }

            }
            catch
            {
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    UseFile();
                    break;

                case Keys.Escape:
                    CloseProgram();
                    break;

                case Keys.Down:
                    StopFile();
                    SelezionaProssimoFile();
                    LeggiTitolo();
                    break;

                case Keys.Up:
                    StopFile();
                    SelezionaPrecedenteFile();
                    LeggiTitolo();
                    break;

                case Keys.Right:
                    VaiAllaProssimaPagina();
                    break;

                case Keys.Space:
                    //PauseResumeFile();
                    DeleteCurrentPosition();
                    break;
            }
        }

        private void VaiAllaProssimaPagina()
        {
            StopFile();
            NextPage();
            LeggiTitolo();
        }

        private void CloseProgram()
        {
            StopFile();
            Console.Beep();
            Console.Beep();
            Console.Beep();
            this.Close();
        }


        /// <summary>
        /// stop previous play and start again
        /// </summary>
        private void UseFile()
        {
            if (filesPage.Count > 0)
            {
                StopFile();
                PlayFile();
            }
        }

    }
}
