﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

namespace clipboard_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Using Visual Studio Ultimate 2012
        // Using .NET Framework 4.5 (see project properties)

        // the sample.txt file included in project contains a (fictional) sample from a medical laboratory result (x-ray exam)
        // the sample.txt file contents are encoded using the ISO-8859-1 encoding
        // the sample text contains misencoded characters are from the 8th bit range, i.e. from the 128 code number to the 255 code number
        // the misencoded-characters.json file (included in project and encoded in UTF-8) lists those misencoded characters

        public MainWindow()
        {

            /* HOW TO REPRODUCE MY STRANGE DISCOVERY */

            /*  1. Copy the sample text to the clipboard :
                Open the sample.txt file (included in project) in visual studio and copy the misencoded text to your clipboard (habitual copy action : mouse-right-click->copy text or Control-C shorctut)
              
                */

            /*  2. Start debugging (F5) :
                Read the Debug ouput : you should see the text recoded correctly
                Also : in the Debug Locals tab, you can check the string values of the "oemTextFormatObject" and "oemTextFromClipboard" variables through the Text visualizer (not the html or xml visualizer)
                the values contain the same sample text but the misencoded characters are recoded correctly to a string 
             */
            
            IDataObject clipboardData = Clipboard.GetDataObject();
            string oemTextFormatName = DataFormats.OemText;
            object oemTextFormatObject = clipboardData.GetData(oemTextFormatName);
            string oemTextFromClipboard = oemTextFormatObject.ToString();

            Debug.WriteLine("");
            Debug.WriteLine("--- DISCOVERY REPRODUCTION ---");
            Debug.WriteLine(@"The next line should be the repaired text with correctly encoded characters. If not, remember to copy the sample text with misencoded characters from sample.txt before starting debugging");
            Debug.WriteLine(@"'" + oemTextFromClipboard + @"'");
            Debug.WriteLine(@"'Un kyste développé au niveau de la lèvre antérieure du rein a un diamètre transversal de' <== If this text was displayed on previous line, you correctly reproduced my discovery");
            Debug.WriteLine("END-------------------------------");
            Debug.WriteLine("");


            //The following tests do not yield the same result, i.e. the misencoded characters are still wrongly interpreted

            // the iso-8859-1 encoded text "été" for testing purpose, where the letter "é" is misencoded as the 218 code number instead of the iso 8859-1 code number 233
            byte[] wronglyEncodedTextBytes = new byte[] { 218, 116, 218 };

            //try decoding the byte array using the console output oem encoding : failing
            Encoding outputEncoding = Console.OutputEncoding;
            string correctTextOutputEncoding = outputEncoding.GetString(wronglyEncodedTextBytes);

            //try decoding the byte array using the locale oem code page :  failing
            Encoding encodingOEM = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
            string encodingOEMCorectedTextFromBytes = encodingOEM.GetString(wronglyEncodedTextBytes);

            //try decoding the byte array with all the .net encodings : failing
            EncodingInfo[] encodingsInfos = Encoding.GetEncodings();
            ArrayList resultingStringsFromBytes = new ArrayList(encodingsInfos.Length);
            foreach (EncodingInfo encodingInfo in encodingsInfos)
            {
                Encoding currentEncoding = encodingInfo.GetEncoding();
                string encodedTextFromBytes = currentEncoding.GetString(wronglyEncodedTextBytes);
                resultingStringsFromBytes.Add(encodedTextFromBytes);

            }

            InitializeComponent();
        }

        // Ignore this useless method, will be removed in next commit
        // Prints to debug output all available data formats from clipboard
        private static void isOEMTextDataFormatAvailable()
        {
            IDataObject clipboardData = Clipboard.GetDataObject();


            /*  1. Copy the sample text to the clipboard (habitual copy action : mouse-right-click->copy text or Control-C shorctut)
                Copy the sample text from sample.txt to your clipboard : SAMPLE.TXT MUST BE OPENED WITH AN EXTERNAL EDITOR , NOT VISUAL STUDIO, WITH THE ENCODING SET TO ISO-8859-1 (works on notepad++ and Emacs)
                If you open the file with an external editor where you specify the encoding as iso-8859-1,
                the data format OemText will be available from the clipboard.
                But if you open sample.txt with visual studio, only 4 data formats are available from the clipboard.
             */

            /* 2. Run this class in Debug mode (default settings with F5) */

            /* 3. Verify the Debug output : the OEMText data format must be listed with the other available formats */
            Debug.WriteLine("");
            Debug.WriteLine(@"All available data formats from clipboard : the OEMText format MUST BE LISTED HERE");

            //retrieve an array of strings for all the formats available on the clipboard.
            string[] availableFormats = clipboardData.GetFormats();

            for (int i = 0; i < availableFormats.Length; i++)
            {
                Debug.WriteLine(i + " : " + availableFormats[i]);
            }
        }
    }
}
