namespace Wasabi.TouhouDominionSelector

/// カードリストファイルを読み込む関数を定義するモジュール
module CardListReader =

    open System
    open System.IO

    /// JSON形式のカードリストファイルを読み込む関数を定義するモジュール
    module Json =

        open Newtonsoft.Json
        open Newtonsoft.Json.FSharp

        /// JSON形式のカードリストファイルを読み込みます。
        /// dir : カードリストファイルが格納されているディレクトリのフルパス
        let read dirPath =
            if dirPath |> Directory.Exists |> not then dirPath |> sprintf "ディレクトリが存在しません : %s" |> invalidArg "dirPath"
            dirPath |> Directory.EnumerateFiles |> Seq.collect (fun filePath ->
                let text = File.ReadAllText(filePath, Text.Encoding.UTF8)
                JsonConvert.DeserializeObject<Card[]>(text, UnionConverter<CardEffect>()))
            |> Seq.toList
