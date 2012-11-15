namespace Wasabi.TouhouDominionSelector

/// カードの種類
type CardKind =
    /// アクションカード
    | Action
    /// リソースカード
    | Resource
    /// 勝利点カード
    | Victory

/// カードの特殊効果
type CardEffect =
    /// アタック
    | Attack
    /// リアクション
    | Reaction
    /// 持続
    | Staying

/// カードのプロパティ
type Card = {
    /// カード名
    Name : string
    /// カードが含まれるシリーズ名
    Series : string
    /// 購入時のコスト
    Cost : int
    /// カードの種類
    Kinds : CardKind[]
    /// カードの特殊効果
    Effects : CardEffect[]
    /// アクション追加回数（確定分のみ）
    ActionIncrement : int
    /// カードを引く枚数（確定分のみ）
    DrawingCardCount : int
    /// 購入フェイズでのカード購入追加枚数（確定分のみ）
    BuyingIncrement : int
    /// 購入フェイズでのコスト増分（確定分のみ）
    CostIncrement : int
    /// カードを廃棄する効果のあるカードかどうか
    CanScrap : bool
    /// マイナスカードを押し付ける効果のあるカードかどうか
    CanForceCurse : bool
    /// 褒賞カードを獲得する効果のあるカードかどうか
    CanWinPrize : bool
    /// カードに描かれているキャラクターのリスト
    Characters : string[]
    /// 本家ドミニオンでの対応するカードの名前。本家ドミニオンに対応するカードがない場合は空文字列。
    DominionName : string
    /// カードの説明
    Description : string
}
