# jTxtRep
Find and replace a text string in a text file.

## Usage:
```
 jTxtRep.exe "file name" [+ "file name" [+ ...]] [-cp:code] -s:"search string" -r:"replacement string" [-pn:prefix] [-sn:suffix] [-cs]
  -cp:code page number or code page name
   Example:
    65001 or utf-8 (default)
    12000 or utf-32
      855 or IBM855
      866 or cp866
     1251 or windows-1251
    20866 or koi8-r
      ...
  -cs : case-sensitive (otherwise case-insensitive)
  -pn:prefix : prefix for the new file name (default '_')
  -sn:suffix : suffix for the new file name
   Example: "name.ext -pn:f_ sn:_u" : "f_name_u.ext"

 jTxtRep.exe (without options: -s, -r)
  In the same directory the configuration file "jTxtRep.conf".
  Each line of the file "jTxtRep.conf" contains the command:
   -cp:code (it is recommended to use in the first line)
   file_name1.ext
   file_name2.ext
   ...
   -s:search string
   -r:replacement string
   -pn:prefix
   -sn:suffix

Example:
 jTxtRep file1.txt file2.txt *.txt -s:text1 -r:text2
```
