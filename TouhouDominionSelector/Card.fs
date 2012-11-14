namespace Wasabi.TouhouDominionSelector

/// カードの効果の種類
type CardEffect =
    /// 普通のアクション
    | Action
    /// アクション・アタック
    | Attack
    /// アクション・リアクション
    | Reaction
    /// アクション・持続
    | Staying
    /// リソース
    | Resource
with
    /// 文字列からCardEffectに変換します。
    static member OfString = function
        | "Action" -> Action | "Attack" -> Attack | "Reaction" -> Reaction | "Staying" -> Staying | "Resource" -> Resource
        | x -> x |> sprintf "不正な値が指定されました : %s" |> invalidArg "effectString"

/// カードのプロパティ
type Card = {
    /// カード名
    Name : string
    /// カードが含まれるシリーズ名
    Series : string
    /// 購入時のコスト
    Cost : int
    /// カードの効果の種類
    Effect : CardEffect
    /// アクション追加回数（確定分のみ）
    ActionIncrement : int
    /// カードを引く枚数（確定分のみ）
    DrawingCard : int
    /// 購入フェイズでのコスト増分（確定分のみ）
    CostIncrement : int
    /// カードを廃棄する効果のあるカードかどうか
    CanScrap : bool
    /// マイナスカードを押し付ける効果のあるカードかどうか
    CanForceMinus : bool
    /// カードに描かれているキャラクターのリスト
    Characters : string[]
    /// 本家ドミニオンでの対応するカードの名前。本家ドミニオンに対応するカードがない場合は空文字列。
    DominionName : string
    /// カードの説明
    Description : string
}
