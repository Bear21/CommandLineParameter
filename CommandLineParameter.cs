using System;
using System.Collections.Generic;
using System.IO;

namespace EightBitBear
{
   public struct CommandLineParameter
   {
      public string shortName;
      public string longName;
      public string help;
      public bool takesData;

      public CommandLineParameter(string ShortName = null, string LongName = null, string Help = null, bool TakesData = false)
      {
         shortName = ShortName;
         longName = LongName;
         help = Help;
         takesData = TakesData;
      }
   }
   public class CommandLineProcessor
   {
      Dictionary<CommandLineParameter, string> results = new Dictionary<CommandLineParameter, string>();
      Dictionary<string, CommandLineParameter> paramShort = new Dictionary<string, CommandLineParameter>();
      Dictionary<string, CommandLineParameter> paramLong = new Dictionary<string, CommandLineParameter>();
      string error;
      public string Error { get => error; }

      public void Add(CommandLineParameter[] parameters)
      {
         foreach (CommandLineParameter param in parameters)
         {
            paramShort.Add(param.shortName, param);
            paramLong.Add(param.longName, param);
         }
      }

      public bool Process(string[] args)
      {
         for (int i = 0; i < args.Length; i++)
         {
            string arg = args[i].ToLower();
            CommandLineParameter foundParam;
            if (arg.StartsWith("--") && arg.Length > 2)
            {
               paramLong.TryGetValue(arg.Substring(2), out foundParam);
            }
            else if (arg.StartsWith("-"))
            {
               paramShort.TryGetValue(arg.Substring(1), out foundParam);
            }
            else
            {
               if (paramShort.TryGetValue(arg, out foundParam))
               {
                  error = $"Error, unknown cmdline Did you mean '-{arg}'?";
               }
               else if (paramLong.TryGetValue(arg, out foundParam))
               {
                  error = $"Error, unknown cmdline Did you mean '--{arg}'?";
               }
               else
               {
                  error = $"Unknown arguement {arg}";
               }
               return false;
            }

            if (foundParam.takesData)
            {
               try
               {
                  results.Remove(foundParam);
                  results.Add(foundParam, args[++i]);
               }
               catch (System.IndexOutOfRangeException)
               {
                  error = $"Error --{foundParam.longName} requires value";
                  return false;
               }
               catch (System.ArgumentException)
               {
                  error = $"Error --{foundParam.longName} Already listed";
                  return false;
               }
            }
            else
            {
               try
               {
                  results.Remove(foundParam);
                  results.Add(foundParam, null);
               }
               catch (System.ArgumentException)
               {
                  error = $"Error --{foundParam.longName} Already listed";
                  return false;
               }
            }
         }
         return true;
      }

      public bool HasParameter(string LongName)
      {
         return results.ContainsKey(paramLong[LongName]);
      }

      public string GetValue(string LongName)
      {
         return results[paramLong[LongName]];
      }

      public void PrintHelp(TextWriter outputStream)
      {
         foreach (var param in paramLong.Values)
         {
            outputStream.WriteLine(String.Format("-{0, -4} --{1, -20} {2, -50}", param.shortName, param.longName, param.help));
         }
      }
   }
}
