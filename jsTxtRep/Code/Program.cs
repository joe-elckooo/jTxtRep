/*
Copyright 2020 Joe-Elckooo

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using jnsCommandLine;


namespace jnsTxtRep
{
  class Program
  {

    ///****************************************************************************
    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
      List<string> Files = new List<string>();

      string src_text = null;
      string dst_text = null;
      string prefix_name = null;
      string suffix_name = null;
      StringComparison comparation = StringComparison.InvariantCultureIgnoreCase;
      var encoding = Encoding.UTF8;

      if (!__commands(args))
      {
        args = null;

        string app_name = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
        string conf = Path.Combine(Directory.GetCurrentDirectory(), app_name + ".conf");

        if (File.Exists(conf))
        {
          List<string> lines = new List<string>();

          bool r = false;
          for (int i = 0; i < 2; i++)
          {
            lines.Clear();

            try
            {
              using (StreamReader reader = new StreamReader(conf, encoding))
                while (!reader.EndOfStream)
                {
                  string l = reader.ReadLine().Trim();
                  if (string.IsNullOrEmpty(l)) continue;

                  if (l.ToUpper().IndexOf("-CP:") == 0)
                  {
                    string v = l.Substring(4);
                    if (r = __set_encoding(v)) break;
                  }
                  else lines.Add(l);
                }
              if (!r) i = 2;
            }
            catch { }
          }

          args = lines.ToArray();
        }

        if ((args == null) || !__commands(args))
        {
          __print_help();
          return;
        }
      }

      if (string.IsNullOrEmpty(prefix_name) && string.IsNullOrEmpty(suffix_name)) prefix_name = "_";

      StringBuilder buff = new StringBuilder();


      //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      #region Search and replace procedure

      foreach (var src in Files)
      {
        try
        {
          string dst = prefix_name + Path.GetFileNameWithoutExtension(src) + suffix_name;
          string ext = Path.GetExtension(src);
          string path = Path.GetDirectoryName(src);

          dst = Path.Combine(path, dst + ext);

          using (var reader = new StreamReader(src, encoding, true))
          using (var writer = new StreamWriter(dst, false, reader.CurrentEncoding))
          {
            while (!reader.EndOfStream)
            {
              string line = reader.ReadLine();

              line = __replace(line, src_text, dst_text);
              writer.WriteLine(line);
            }
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex);
        }
      }

      #endregion


      //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      #region Helper functions

      //----->>>
      void __print_help() => Console.WriteLine(Resource.HelpText);

      //----->>>
      bool __set_encoding(string v)
      {
        try
        {
          if (int.TryParse(v, out var cp))
          {
            if (encoding.CodePage != cp)
            {
              encoding = Encoding.GetEncoding(cp);
              return true;
            }
          }
          else
          {
            if (!encoding.HeaderName.Equals(v, StringComparison.InvariantCultureIgnoreCase))
            {
              encoding = Encoding.GetEncoding(v);
              return true;
            }
          }
        }
        catch
        {
          Console.WriteLine("? Unknown code page (-cp): {0}", v);
        }

        return false;
      }

      //----->>>
      bool __commands(string[] command_line)
      {
        var commands = CommandLine.ParseCommandLine(command_line);

        foreach (var c in commands)
        {
          string key = c.Key?.ToUpper();
          string value = c.Value?.Trim();

          if (key == "S") src_text = value;
          else if (key == "R") dst_text = value;
          else if (key == "PN") prefix_name = value;
          else if (key == "SN") suffix_name = value;
          else if (key == "CS") comparation = StringComparison.InvariantCulture;
          else if (key == "CP")
          {
            __set_encoding(value);
          }
          else if ((key == null) && (!string.IsNullOrEmpty(value)))
          {
            int i = value.IndexOfAny(new char[] { '*', '?' });
            if (i >= 0) GetFiles(value, Files);
            else if (File.Exists(value)) Files.Add(value);
          }
          else if (key == "H")
          {
            __print_help();
            Environment.Exit(0);
            return false;
          }
        }

        return !(string.IsNullOrEmpty(src_text) || string.IsNullOrEmpty(dst_text) || (Files.Count == 0));
      }

      //----->>>
      string __replace(string line, string src, string dst)
      {
        int i = line.IndexOf(src, comparation);
        if (i < 0) return line;

        int j = 0;
        buff.Clear();

        do
        {
          buff.Append(line.Substring(j, i - j));
          buff.Append(dst);
          j = i + src.Length;
          if (j > line.Length) return buff.ToString();

          i = line.IndexOf(src, j, comparation);
        } while (i > 0);

        buff.Append(line.Substring(j));

        return buff.ToString();
      }

      #endregion

    }

    ///****************************************************************************
    /// <summary>
    /// 
    /// </summary>
    /// <param name="file_mask"></param>
    /// <param name="list"></param>
    static void GetFiles(string file_mask, List<string> list)
    {
      string path = Path.GetDirectoryName(file_mask);
      if (string.IsNullOrEmpty(path)) path = Directory.GetCurrentDirectory();

      foreach (var f in Directory.EnumerateFiles(path, file_mask, SearchOption.TopDirectoryOnly))
      {
        list.Add(f);
      }
    }


  }
}
