using Microsoft.Extensions.Configuration;
using Net6FileIo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Net6FileIo
{
  internal class App
  {
    readonly IConfiguration _config;
    readonly RandomService _randSvc;

    public App(IConfiguration config, RandomService randSvc)
    {
      _config = config;
      _randSvc = randSvc;
    }

    /// <summary>
    /// 取代原本 Program.Main() 函式的效用。
    /// </summary>
    public void Run(string[] arguments)
    {
      try
      {
        Console.WriteLine($">>>>>> 程式 {Assembly.GetEntryAssembly().GetName().Name} 已啟動。");

        AppArguemnts args;
        if (!ParseArguments(arguments, out args))
          return;

        var fi = new FileInfo(args.inputFilename);
        if (!fi.Exists)
        {
          Console.WriteLine($"輸入檔案不存在！[{fi.FullName}] ");
          return;
        }

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // 註冊Big5等編碼
        Encoding Big5Enc = Encoding.GetEncoding(950);

        using var file = new StreamReader(fi.FullName, Big5Enc);

        string line = file.ReadLine();
        int lineCounter = 0;
        while (line != null)
        {
          Console.WriteLine($"讀取一行：" + line);
          lineCounter++;

          // 解析
          var itemArray = line.Split(',');
          var json = JsonSerializer.Serialize(itemArray, new JsonSerializerOptions
          {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
          });

          Console.WriteLine($"解析({itemArray.Length}) → " + json);

          // next
          line = file.ReadLine();
        }

        Console.WriteLine($"統計：共有 {lineCounter} 行。");

      }
      catch (Exception ex)
      {
        Console.WriteLine($"出現例外：{ex.Message}\r\n" + ex.ToString());
      }

      Console.WriteLine("Press any key to continue.");
      Console.ReadKey();
    }

    public bool ParseArguments(string[] args, out AppArguemnts argsInfo)
    {
      argsInfo = null;

      if (args.Length == 0)
      {
        Console.WriteLine("No input file specified! 未指定輸入檔案！");
        return false;
      }

      argsInfo = new AppArguemnts {
        inputFilename = args[0]
      };

      return true;
    }
  }

  class AppArguemnts
  {
    public string inputFilename { get; set; }
  }
}