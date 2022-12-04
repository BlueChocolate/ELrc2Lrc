using System.IO;
using System.Linq;
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
    Console.WriteLine(Local.Intro);
    Console.WriteLine();
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
    Console.WriteLine("(" + (filePaths.IndexOf(filePath)+1) + ")" + filePath);
}
Console.WriteLine(Local.Total + filePaths.Count);
Console.WriteLine(Local.ModeList);
Console.Write(Local.ChoiceMode);

switch (Console.ReadLine())
{
    case "1":
        foreach (var filePath in filePaths)
        {
            Console.WriteLine("(" + (filePaths.IndexOf(filePath) + 1) + ")" + ConvertTimetAndId(filePath, new string[] { "ar", "al", "ti", "au", "length", "by", "offset", "re", "ve" }));
        }
        break;
    case "2":
        foreach (var filePath in filePaths)
        {
            Console.WriteLine("(" + (filePaths.IndexOf(filePath) + 1) + ")" + ConvertTimetAndId(filePath, new string[] { "XD" }));
        }
        break;
    case "3": break;
    case "4": break;
    case "5": break;
    default:
        foreach (var filePath in filePaths)
        {
            Console.WriteLine("(" + (filePaths.IndexOf(filePath) + 1) + ")" + ConvertTimetAndId(filePath, new string[] { "ar", "al", "ti", "au", "length", "by", "offset", "re", "ve" }));
        }
        break;
}

Console.WriteLine(Local.PressToEnd);
Console.ReadLine();

static string ConvertTimetAndId(string filePath, string[] standardIdTags =null)
{
    string lrc = "";
    using (var sr = new StreamReader(filePath))
    {
        lrc = sr.ReadToEnd();
    }
    //Console.WriteLine("[before]" + lrc);

    var wordTimeRegex = new Regex(@"<[0-9:.]+?>"); // 单词时间标签
    var idTagRegex = new Regex(@"\[([a-zA-Z]+):?.+?\]"); // ID 标签
    var emptyLineRegex = new Regex(@"\n\s*\r"); // 空行
    var consecutiveSpacesRegex = new Regex(@"[ ]{2,}"); // 连续空格
    var lineTimeRegex = new Regex(@"\[[0-9:\.]+\]?"); // 行时间标签

    var timeIdMatches = wordTimeRegex.Matches(lrc);
    var idTagMatches = idTagRegex.Matches(lrc);
    var emptyLineMatches = emptyLineRegex.Matches(lrc);
    var lineTimeMatches = lineTimeRegex.Matches(lrc);

    int lineTimeCount = 0;
    foreach (Match match in lineTimeMatches)
    {
        lrc = lrc.Insert(match.Index + 2*(lineTimeCount++), "\r\n");
    }

    int idTagCount = 0;
    foreach (Match match in idTagMatches)
    {
        lrc = lrc.Insert(match.Index + 2 * (idTagCount++), "\r\n");
    }

    int idTagRemoveCount = 0;
    standardIdTags ??= new string[]{ "ar", "al", "ti", "au", "length", "by", "offset", "re", "ve" }; // 其实就是参数默认值
    foreach (Match match in idTagMatches)
    {
        GroupCollection groupIdTag = match.Groups;
        //int removeConut = 0;
        if (!standardIdTags.Contains(groupIdTag[1].Value))
        {
            //lrc = lrc.Remove(match.Index, match.Length);
            lrc = lrc.Replace(match.Value, "");
            idTagRemoveCount++;
        }
    }

    lrc = wordTimeRegex.Replace(lrc, ""); // 去除尖括号时间标签
    lrc = consecutiveSpacesRegex.Replace(lrc, " "); // 去除连续空格
    lrc = emptyLineRegex.Replace(lrc, ""); // 去除空行



    //Console.WriteLine("[after]" + lrc);
    using (var wr = new StreamWriter(filePath))
    {
        wr.Write(lrc);
    }
    return string.Format("{0}[{1} {2}][{3} {4}] {5}", Local.Removed, Local.NumTimeTag, timeIdMatches.Count, Local.NumIdTag, idTagRemoveCount, filePath);
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
    public static string Intro { get => GetLocal(); }
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
    public static string Removed { get => GetLocal(); }
    public static string NumIdTag { get => GetLocal(); }
    public static string NumTimeTag { get => GetLocal(); }
    public static string NumEmptyLine { get => GetLocal(); }


    static string[,] langArr = new string[,] {
        {"Intro",
            "本工具可以将增强歌词格式（Enhanced LRC format）文件转换为简单歌词格式文件（Simple LRC format）"+ Environment.NewLine +
            "简单歌词格式示例：[mm:ss.xx]这是一行歌词吗"+ Environment.NewLine +
            "增强歌词格式示例：[mm:ss.xx]<mm:ss.xx>确<mm:ss.xx>实<mm:ss.xx>",
            "This tool can convert Enhanced LRC format files to Simple LRC format files" + Environment.NewLine +
            "Simple LRC format example:[00:12.00]Line 1 lyrics" + Environment.NewLine +
            "Enhanced LRC format example:[mm:ss.xx] <mm:ss.xx> line 1 word 1 <mm:ss.xx> line 1 word 2 <mm:ss.xx>" },
        {"InputPath","请输入路径：","Please input path:" },
        {"IsFile","这是一个文件","It's a file" },
        {"IsFolder","这是一个文件夹","It's a folder" },
        {"PathIllegal","路径非法！","The path is illegal" },
        {"Detecting","检测中...","Detecting..." },
        {"Finish","完成","Finished." },
        {"Error","错误！","Error!" },
        {"Total","共计：","Total:" },
        {"AskEndAdd","继续吗？输入 Q 结束添加","Continue? Please enter Q to end adding files." },
        {"PressToEnd","按任意键结束程序...","Press any key to end the program..." },
        {"ConfirmLrc","这似乎不是一个歌词文件，请选择是或否？输入 Y 或 N（默认是 N）：","This does not appear to be a lyrics file, Yes or no? press Y or N(Default is N):" },
        {"PressYes","你选择是","You choose yes" },
        {"PressNo","你选择否","You choose no" },
        {"ChoiceMode","请选择模式：","Please select a mode:" },
        {"ModeList",
            "1.转换为简单时间标签、清除非标准 ID 标签（默认）" + Environment.NewLine +
            "2.转换为简单时间标签、清除所有 ID 标签" + Environment.NewLine +
            "3.仅转换为标准时间标签" + Environment.NewLine +
            "4.仅清除非标准 ID 标签" + Environment.NewLine +
            "5.清除所有 ID 标签",
            "1.Convert to simple time tags, clear non-standard ID tags(Default)" + Environment.NewLine +
            "2.Convert to simple time tags, clear all ID tags" + Environment.NewLine +
            "3.Convert to standard time tags only" + Environment.NewLine +
            "4.Clear non-standard ID tags" + Environment.NewLine +
            "5.Clear all ID tags" },
        {"Removed","已移除","Removed" },
        {"NumIdTag","ID标签","ID tags" },
        {"NumTimeTag","时间标签","Time tags" },
        {"NumEmptyLine","空行","Empty Lines" },



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
