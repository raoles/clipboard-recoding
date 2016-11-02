
## Intro

This project demonstrates a strange behaviour I observed from the Windows clipboard.  
Copying Iso-8859-1 encoded text with corrupted characters to the clipboard then retrieving it using the OemText data format, the text comes back as a correctly decoded string without the previous corrupted characters.  

**Example :**  
corrupted text copied to clipboard : *"Un kyste dÚveloppÚ au niveau de la lÞvre antÚrieure du rein a un diamÞtre transversal de"*  
clean text coming back : *"Un kyste développé au niveau de la lèvre antérieure du rein a un diamètre transversal de"*  

I want to understand how the clipboard's OemText dataformat cleans the text to use it in my C# WPF solution.    

## My first solution

So far I know this:  
The text is encoded as iso-8859-1.  
The corrupted characters are from the iso-8859-1 8th bit range, from 128 to 255 code number.  
I listed them in this [JSON file](https://github.com/raoles/clipboard-recoding/blob/master/clipboard-wpf/misencoded-characters.json).  
 
I decided to make a C# WPF application that takes the corrupted text as input, corrects it via the [misencoded characters table](https://github.com/raoles/clipboard-recoding/blob/master/clipboard-wpf/misencoded-characters.json) then outputs the corrected text.  


## My strange discovery
While working on the WPF app, I was exploring the Windows clipboard functions.  
The OemText data format from the clipboard correctly recode the corrupted text, the misencoded characters are displayed correctly.  
So this means that there's a way to recode directly the corrupted text to a string, which is easier and more maintenable than transforming the text via a misencoded characters table.  
[See the code manipulating the clipboard and OEMText data format below](https://github.com/raoles/clipboard-recoding/blob/master/clipboard-wpf/MainWindow.xaml.cs#L50)  

```c#
IDataObject clipboardData = Clipboard.GetDataObject();
string oemTextFormatName = DataFormats.OemText;
object oemTextFormatObject = clipboardData.GetData(oemTextFormatName);

// the corrupted text copied to clipboard is "Un kyste dÚveloppÚ au niveau de la lÞvre antÚrieure du rein a un diamÞtre transversal de" from the sample.txt
//oemTextFromClipboard's debug local value is "Un kyste développé au niveau de la lèvre antérieure du rein a un diamètre transversal de", the text correctly decoded
string oemTextFromClipboard = oemTextFormatObject.ToString();

```



## How to reproduce my discovery

Downlaod the project then read this [commented code](https://github.com/raoles/clipboard-recoding/blob/master/clipboard-wpf/MainWindow.xaml.cs#L37) to reproduce my discovery in 2 steps  

## My second solution (after my discovery)

First, I want to understand why the OEMDataFormat understands correctly the corrupted text and what code page it uses to achieve that.  
Then I want to use its mechanism or code page to correct the corrupted text directly, instead of using an intermediate misencoded characters table.  

## What i tried already to find the origin of the OEMText data format correct decoding

* Some [c# tests](https://github.com/raoles/clipboard-recoding/blob/master/clipboard-wpf/MainWindow.xaml.cs#L64)
* Comparing my misencoded characters table to all [Windows OEM code pages](http://www.aivosto.com/vbtips/charsets-codepages-dos.html#codepage863) : it's a failure, wrong bytes in the sample text do not correspond to the misencoded characters in the OEM code pages.  
Example : the wrong 218 byte that represents "é" in my corrupted text (but should be the code number 233 for iso-8859-1) is never the code number for the character "é" in the OEM code pages.  

* Exploring the .Net source code :  
1. [Clipboard.GetData()](https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/Clipboard.cs,cd4ae5de51327684) 
2. [Clipboard.GetDataObject()](https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/Clipboard.cs,cd4ae5de51327684)
3. [Clipboard.GetDataObject(int retryTimes, int retryDelay)](https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/Clipboard.cs,f4f3734655d33a95,references)
4. [UnsafeNativeMethods.OleGetClipboard(ref IComDataObject data)](https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/UnsafeNativeMethods.cs,e953edbf1bc55d0c)




