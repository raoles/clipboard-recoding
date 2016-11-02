hexdumplink
json file x 2
link to OEMDataFormat code x 2
add screenshot of OEM data format code

Disclaimer
English is not my first language, pardon the weird phrasings.


LONG VERSION

Problem context


I'm making a c# WPF desktop application for my doctor because of an issue he has with results from a medical laboratory.
My doctor and the medical laboratory both have the same medical emailing software installed on Windows to communicate with each other.
The laboratory sends the medical result (requested by my doctor) to my doctor through the emailing software.
The emailing software encode the laboratory result text with the ISO-8859-1 encoding.
The lab result text contains misencoded characters when my doctor opens the email with the emailing software on his computer.
So the misencoding is done on the laboratory's side before it sends the result through the medical emailing software.
Sadly the laboratory refuses to be contacted anymore for this matter.
I decided to make my doctor a desktop app to fix the text's misencoded characters.

My intent was and still is to learn about encodings and realizing a real-world small project.
I'm not interested in contacting the sender laboratory, I want to find a programming solution.

What i did

I extracted a sample text from the medical emailing software (cleared of all personal information of course).
I encoded the sample text as iso-8859-1, the same encoding used by the emailing software.

I googled and found this article ["Dealing with inconsistent or corrupt character encodings"](http://www.martinaulbach.net/linux/command-line-magic/41-dealing-with-inconsistent-or-corrupt-character-encodings)
I did a [hexadecimal dump](link to github repo hexdump file) of the sample.
I noted the misencoded characters, each with the wrong byte that represents it in the corrupted text and the correct iso-8859-1 code number (see the [misencoded-characters.json](link to json file)).
The misencoded characters come from the 8th bit range, i.e. from the 128 code number to the 255 code number.

I read those SO's posts [1](http://stackoverflow.com/questions/132318/how-do-i-correct-the-character-encoding-of-a-file) [2](http://stackoverflow.com/questions/64860/best-way-to-convert-text-files-between-character-sets)
I played a lot with [Recode FSF](https://directory.fsf.org/wiki/Recode), a tool similar to iconv.
I tried recoding the text to all encodings offered by Recode and used the "-k BEFORE:AFTER" flag on the misencoded characters.
I dived deep in Recode's documentation but the experiment was not successful.

At this point, I'm informed enough (I grasp it enough to be sure of what I'm doing) about 7 bit encoding scheme, 8 bit encoding scheme, US_ASCII, ISO 646 and national variants, ISO-8859-X standards, Unicode, UCS-2, UTF-8 and its retro compatibility, .Net string encoding (16 bits).
I decided to make a C# WPF application that takes the corrupted text as input, corrects it via the [misencoded characters table]() then outputs the corrected text.


My strange discovery
While working on the WPF app, I was exploring the Windows clipboard functions (the doctor will paste the garbled text in a text field in my app).
The OemText data format from the clipboard correctly encode the corrupted text, the misencoded characters are displayed correctly.
So this means that there's a way to recode directly the corrupted text to a string, which is easier and more maintenable than transforming the text via a misencoded characters table.
[See the code]()

Here is the screenshot of the code and the values in debug mode


How to reproduce my discovery
Downlaod the project then read this [commented code]() to reproduce my discovery in 2 steps

What I want to do
First, I want to understand why the OEMDataFormat understands correctly the corrupted text and what code page it uses to achieve that.
Then I want to use its mechanism or code page to correct the corrupted text directly, instead of using an intermediate misencoded characters table.

What i tried already to find the origin of the OEMText data format correct decoding
* Comparing my misencoded characters table to all Windows OEM code pages : failed, wrong bytes in the sample text do not correspond to the misencoded characters in the OEM code pages.
Example : the wrong 218 byte that represents "�" in my corrupted text (but should be the code number 233 for iso-8859-1) is never the code number for the character "�" in the OEM code pages.

* Exploring the .Net source code :  
1. [Clipboard.GetData()](https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/Clipboard.cs,cd4ae5de51327684) 
2. [Clipboard.GetDataObject()](https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/Clipboard.cs,cd4ae5de51327684)
3. [Clipboard.GetDataObject(int retryTimes, int retryDelay)](https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/Clipboard.cs,f4f3734655d33a95,references)
4. [UnsafeNativeMethods.OleGetClipboard(ref IComDataObject data)](https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/UnsafeNativeMethods.cs,e953edbf1bc55d0c)

What I'm doing next
Posting this to help forums 
Exploring the Windows Ole And COM subjects (why? because of the 4th bullet from my source code exploring : "OleGetClipboard"), but I don't know if that's the right direction.




