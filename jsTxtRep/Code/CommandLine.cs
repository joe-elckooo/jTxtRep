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

//  (2020.12.30)

using System;
using System.Collections.Generic;


namespace jnsCommandLine
{


  ///++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  /// <summary>
  /// 
  /// </summary>
  public readonly struct Command
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public Command(string key, string value)
    {
      Key = key;
      Value = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public readonly string Key;
    /// <summary>
    /// 
    /// </summary>
    public readonly string Value;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return string.Format("{0} [{1}]", Key ?? "$", Value);
    }

  }

  ///++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  /// <summary>
  /// 
  /// </summary>
  public class Commands : List<Command>
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add(string key, string value)
    {
      Add(new Command(key, value));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Insert(int index, string key, string value)
    {
      Insert(index, new Command(key, value));
    }

    /// <summary>
    /// 
    /// </summary>
    public void Add()
    {
      Add(new Command());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="src"></param>
    public void Append(Commands src)
    {
      this.AddRange(src);
    }

  }

  ///++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  /// <summary>
  /// 
  /// </summary>
  public class CommandLine
  {

    ///****************************************************************************
    /// <summary>
    /// Parsing the command line
    /// Format: -p -d:'Value' -s:Value, 
    ///   where: p, d, s -- command, Value -- value
    /// -p -d:'value' | -d:"value"
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public static Commands ParseCommandLine(string[] cmd)
    {
      Commands result = new Commands();

      int i = 0, j;

      string key;
      string val;

      for (; i < cmd.Length; i++)
      {
        key = null;
        val = null;
        j = 0;

        if (cmd[i][0] == '-')
        {
          if (__get_key()) __get_value();
        }
        else __get_value();

        if ((key != null) || (val != null)) result.Add(key, val);
      }

      return result;


      //----->>>
      bool __get_key()
      {
        string s = cmd[i];
        char c = '\x00';

        // find end of key string
        j = 1;
        for (; j < s.Length; j++)
        {
          c = s[j];
          if ((c == ':') || (c <= ' ')) break;
        }

        key = s.Substring(1, j - 1);
        if (key.Length == 0) key = null;

        if (c == ':') ++j;
        else if (j == s.Length)
        {
          int k = i + 1;
          if ((k < cmd.Length) && (cmd[k][0] == ':'))
          {
            i = k; j = 1; c = ':';
          }
        }
        else
        {
          int k = j;
          for (; k < s.Length; k++)
          {
            if (cmd[i][k] == ':')
            {
              j = k + 1;
              c = ':';
              break;
            }
          }
        }

        return (c == ':') && (key != null);
      }

      //----->>>
      bool __get_value()
      {
        if (j >= cmd[i].Length) { j = 0; ++i; }

        bool r = i < cmd.Length;
        if (r) val = cmd[i].Substring(j);

        return r;
      }

    }


  }
}
