/// <summary>
/// ログ機能を実装するクラス
/// </summary>
internal class Logger
{
    // ログ出力先フォルダ
    private string logFolderPath;

    // プロパティ
    public string LogFolderPath
    {
        get { return this.logFolderPath; }
    }


    /// <summary>
    /// 設定ファイルからログフォルダパスを取得するメソッド
    /// </summary>
    public void GetLogFolderPath()
    {
        try
        {
            // XMLファイルを読み込む
            XElement xml = XElement.Load(Program.configPath);

            // 要素の値を取得する
            XElement log = xml.Element("Log");
            logFolderPath = log.Element("LogPath").Value;
        }
        catch (Exception ex)
        {
            Error("GetLogFolderPath", ex);
        }
    }



    /// <summary>
    /// ログ書き込みメソッド
    /// </summary>
    /// <param name="logLevel">ログレベル</param>
    /// <param name="loginId">ログインID</param>
    /// <param name="message">メッセージ</param>
    public void Write(string logLevel, string loginId, string message)
    {
        // 書き込み先ログファイル情報を取得
        DateTime date = DateTime.Now;
        string dateStr = date.ToString("yyyyMMdd");
        string logFilePath = $"{logFolderPath}{dateStr}.log";

        // ログフォルダ・ログファイルがなければ作成
        Create(logFilePath);

        // ログファイルに書き込む
        try
        {
            Encoding enc = Encoding.GetEncoding("UTF-8");
            using (StreamWriter sw = new StreamWriter(logFilePath, true, enc))
            {
                sw.WriteLine($"{logLevel}  {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")} [{loginId}] {message}");
            }
        }
        catch (Exception ex)
        {
            Error("Write", ex);
        }
    }



    /// <summary>
    /// ログフォルダ・ログファイル作成メソッド
    /// </summary>
    /// <param name="logFilePath">書き込み先ログファイルパス<parama>
    private void Create(string logFilePath)
    {
        // ログフォルダがなければ作成する
        if (Directory.Exists(logFolderPath) == false)
        {
            try
            {
                Directory.CreateDirectory(logFolderPath);
                Debug.WriteLine($"ログフォルダ作成完了\n{logFolderPath}");
            }
            catch (Exception ex)
            {
                Error("Create", ex);
            }
        }

        // ログファイルがなければ作成
        try
        {
            if (File.Exists(logFilePath) == false)
            {
                using (File.Create(logFilePath))
                {
                    Debug.WriteLine($"ログファイル作成完了\n{logFilePath}");
                }
            }
        }
        catch (Exception ex)
        {
            Error("Create", ex);
        }

    }



    /// <summary>
    /// 古いログファイルを削除するメソッド
    /// </summary>
    public void Delete()
    {
        // 削除対象のファイルをすべて削除する
        try
        {
            // ログフォルダ配下の全ファイルを取得
            string[] files = Directory.GetFiles(logFolderPath);

            for (int i = 0; i < files.Length; i++)
            {
                // ファイル名を取得
                string fileFullName = files[i].Substring(files[i].LastIndexOf(@"\") + 1);

                // 拡張子なしのファイルであればスキップ
                if (fileFullName.IndexOf(".") == -1)
                {
                    continue;
                }

                // 拡張子を除いたファイル名を取得
                string fileName = fileFullName.Substring(0, fileFullName.LastIndexOf("."));

                // 拡張子を取得
                string fileExtension = files[i].Substring(files[i].LastIndexOf(@".") + 1);


                // ファイルがログファイルでなければスキップ
                // 拡張子をチェック
                if (fileExtension != "log")
                {
                    continue;
                }

                // ファイル名の文字タイプをチェック
                int n;
                if (int.TryParse(fileName, out n) == false)
                {
                    continue;
                }

                // ファイル名の文字数をチェック
                if (fileName.Length != 8)
                {
                    continue;
                }

                // ファイル名の日時が不正値でないかチェック
                int year = int.Parse(fileName.Substring(0, 4));
                int month = int.Parse(fileName.Substring(4, 2));
                int day = int.Parse(fileName.Substring(6, 2));
                if (year < 0)
                {
                    continue;
                }

                if (month < 1 || month > 12)
                {
                    continue;
                }

                if (day < 1 || day > 31)
                {
                    continue;
                }


                // 現在の日付と比較して、365日以上前であればファイル削除
                DateTime fileNameTime = new DateTime(year, month, day);
                DateTime nowTime = DateTime.Now;
                TimeSpan diff = nowTime - fileNameTime;
                TimeSpan span = new TimeSpan(365, 0, 0, 0, 0);
                if (diff > span)
                {
                    File.Delete(files[i]);
                }
            }
        }
        catch (Exception ex)
        {
            Error("Delete", ex);
        }
    }



    /// <summary>
    /// ログクラスメソッドでエラーが発生した場合に呼び出すメソッド
    /// </summary>
    /// <param name="errorPoint">エラーが発生した場所(メソッド名など)</param>
    /// <param name="ex">エラー詳細</parama>
    private void Error(string errorPoint, Exception ex)
    {
        Debug.WriteLine($"Loggerクラス{errorPoint}メソッドでエラー発生\n{ex}");
        MessageBox.Show($"ログ機能の{errorPoint}メソッドでエラーが発生しました。\nソフトは利用できますが、ログ出力に影響がある可能性があります。\nシステム管理者に確認してください。\n\n{ex}",
                        "エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
    }

}