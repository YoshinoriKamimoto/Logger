using System;
using System.IO;
using System.Linq;

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
        get { return logFolderPath; }
    }



    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Logger()
    {
        try
        {
            // 設定ファイルからログ出力先フォルダパスを取得
            logFolderPath = Utility.GetLogFolderPath();
            Debug.WriteLine($"ログ出力先フォルダの取得完了\n{logFolderPath}");
        }
        catch (Exception ex)
        {
            Error("コンストラクタ", ex);
        }
    }


    /// <summary>
    /// ログファイル書き込みメソッド
    /// </summary>
    /// <param name="subject">ログメッセージ見出し：INFO,ERRORなど</param>
    /// <param name="msg">ログメッセージ本文</param>
    public void Write(string subject, string msg)
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
                sw.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},[{subject}]:{msg}");
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
    /// 過去のログファイルを削除するメソッド
    /// </summary>
    public void Delete()
    {
        // 削除対象のファイルをすべて削除する
        try
        {
            // 削除対象ファイルの情報を取得
            DateTime date = DateTime.Now;
            DirectoryInfo di = new DirectoryInfo(logFolderPath);
            for (int i = 12; i < 24; i++)
            {
                string deleteDate = date.AddMonths(-i).ToString("yyyyMM");

                // 12~24ヶ月前のログファイルをすべて削除
                foreach (FileInfo fi in di.EnumerateFiles($"{deleteDate}*.log"))
                {
                    string fileName = fi.Name;
                    File.Delete(logFolderPath + fileName);
                }
            }

        }
        catch (Exception ex)
        {
            Error("Delete", ex);
        }
    }


    /// <summary>
    /// ログ作成・書き込み・削除などでエラーが発生した場合に呼び出すメソッド
    /// </summary>
    /// <param name="errorPoint">エラーが発生した場所(メソッド名など)</param>
    /// <param name="ex">エラー詳細</parama>
    private void Error(string errorPoint, Exception ex)
    {
        Debug.WriteLine($"Loggerクラスの\"{errorPoint}\"箇所でエラーが発生しました。\n{ex}");
        MessageBox.Show($"ログ機能の\"{errorPoint}\"箇所でエラーが発生しました。\nソフトは利用できますが、ログ出力に影響がある可能性があるため、\nシステム管理者に確認してください。\n\n{ex}",
                        "エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
    }

}
