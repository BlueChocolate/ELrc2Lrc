using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

var filePaths=new List<string>();

if (args.Length == 0) //直接打开
{
    Console.WriteLine("1.简体中文\n2.English");
    switch (Console.ReadLine())
    {
        case "1": Local.CurrentLanguage = Local.Language.Chinese; break;
        case "2": Local.CurrentLanguage = Local.Language.English; break;
        default: Local.CurrentLanguage = Local.Language.English; break;
    }
    Console.Clear();

    while (true)
    {
        // 输入路径
        Console.Write(Local.InputPath);
        string path = Console.ReadLine() ?? "";

        // 去除开头结尾的引号，如果路径中包含空格则会自动加上双引号
        if (path.StartsWith('\"') && path.EndsWith('\"'))
        {
            path = path.Substring(1, path.Length - 2);
        }

        if (File.Exists(path)) // 是一个文件
        {
            Console.WriteLine(Local.IsFile);
            if (path.ToLower().EndsWith(".lrc")) // 如果后缀不是 lrc
            {
                filePaths.Add(path);
            }
            else
            {
                Console.Write(Local.ConfirmLrc);
                if ((Console.ReadLine() ?? "n").ToLower() == "n")
                {
                    //Console.Write("\r");
                    //Console.WriteLine(Local.PressNo);
                }
                else
                {
                    //Console.Write("\r");
                    //Console.WriteLine(Local.PressYes);
                    filePaths.Add(path);
                }
            }
        }
        else if (Directory.Exists(path)) // 是一个文件夹
        {
            Console.WriteLine(Local.IsFolder);
            filePaths.AddRange(GetFiles(path));
        }
        else // 路径非法
        {
            if (path.ToLower() == "q")
            {
                break;
            }
            Console.WriteLine(Local.PathIllegal);
        }
        Console.WriteLine();
        Console.WriteLine(Local.AskEndAdd);
    }
}
else if (args.Length == 1) //把文件或目录拖到上面打开
{
    string path = args[0];
    Console.WriteLine(path);
    if (File.Exists(path))
    {
        Console.WriteLine(Local.IsFile);
        filePaths.Add(path);
    }
    else if (Directory.Exists(path))
    {
        Console.WriteLine(Local.IsFolder);
        filePaths.AddRange(GetFiles(path));
    }
    else
    {
        Console.WriteLine(Local.PathIllegal);
    }
}
Console.Clear();

// 开始处理文件
foreach (var filePath in filePaths)
{
    Console.WriteLine(filePath);
}
Console.WriteLine(Local.Total + filePaths.Count);
Console.WriteLine(Local.ChoiceMode);
Console.WriteLine(Local.ModeList);

switch (Console.ReadLine())
{
    case "1":
        foreach (var filePath in filePaths)
        {
            Console.WriteLine(filePath);
            string lrc = "";
            using (var sr = new StreamReader(filePath))
            {
                lrc = sr.ReadToEnd();
            }
            //Console.WriteLine("before\n" + lrc);
            lrc = Regex.Replace(lrc, @"<[0-9:.]+?>", ""); // 去除尖括号时间标签

            var  = Regex.Matches(lrc, @"\[[a-zA-Z]+:?.+?]"); // 去除空行

            lrc = Regex.Replace(lrc, @"\n\s*\r", ""); // 去除空行


            //Console.WriteLine("after\n" + lrc);
            using (var wr = new StreamWriter(filePath))
            {
                wr.Write(lrc);
            }
        }
        break;
    case "2": 

        break;
    default: 

        break;
}

Console.WriteLine(Local.PressToEnd);
Console.ReadLine();

static string ConvertTimeTag(string lrc)
{
    
}

static string CleanIdTag(string lrc)
{

}

static List<string> GetFiles(string directory, string pattern = "*.lrc")
{
    List<string> files = new List<string>();
    try
    {
        foreach (var item in Directory.GetFiles(directory, pattern))
        {
            files.Add(item);
        }
        foreach (var item in Directory.GetDirectories(directory))
        {
            files.AddRange(GetFiles(item, pattern));
        }
        return files;
    }
    catch (Exception ex)
    {
        Console.WriteLine(Local.Error + ex.Message);
        return files;
    }
}

static class Local
{
    public enum Language { Chinese, English }
    public static Language CurrentLanguage { get; set; }
    public static string InputPath { get => GetLocal(); }
    public static string IsFile { get => GetLocal(); }
    public static string IsFolder { get => GetLocal(); }
    public static string PathIllegal { get => GetLocal(); }
    public static string Finish { get => GetLocal(); }
    public static string PressToEnd { get => GetLocal(); }
    public static string Error { get => GetLocal(); }
    public static string AskEndAdd { get => GetLocal(); }
    public static string Total { get => GetLocal(); }
    public static string ConfirmLrc { get => GetLocal(); }
    public static string PressYes { get => GetLocal(); }
    public static string PressNo { get => GetLocal(); }
    public static string ChoiceMode { get => GetLocal(); }
    public static string ModeList { get => GetLocal(); }

    static string[,] langArr = new string[,] {
        {"InputPath","请输入路径：","Please input path:" },
        {"IsFile","这是一个文件","It's a file." },
        {"IsFolder","这是一个文件夹","It's a folder." },
        {"PathIllegal","路径非法！","The path is illegal." },
        {"Detecting","检测中...","Detecting..." },
        {"Finish","完成","Finished." },
        {"Error","错误！","Error!" },
        {"Total","共计：","Total:" },
        {"AskEndAdd","继续吗？输入 Q 结束添加","Continue? Please enter Q to end adding files." },
        {"PressToEnd","按任意键结束程序...","Press any key to end the program..." },
        {"ConfirmLrc","这似乎不是一个歌词文件，是或否？输入 Y 或 N（默认是 N）：","This does not appear to be a lyrics file, Yes or no, press Y or N (Default is N):" },
        {"PressYes","你选择是","You choose yes." },
        {"PressNo","你选择否","You choose no." },
        {"SelectMode","请选择模式（默认是模式 1）：","Please select a mode (Default is Mode 1):" },
        {"ModeList'",
            "1.转换为标准时间标签、清除非标准 ID 标签" +
            "2.仅转换为标准时间标签",
            "1.Convert to standard time tags, clear non-standard ID tags." +
            "2.Convert to standard time tags only." },

    };
    private static string GetLocal([CallerMemberName] string str = "Error")
    {
        for (int i = 0; i < langArr.GetLength(0); i++) //获得行数
        {
            if (langArr[i, 0] == str)
            {
                return langArr[i, (int)CurrentLanguage + 1];
            }
        }
        return str;
    }
}