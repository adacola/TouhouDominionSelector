namespace Wasabi.TouhouDominionSelector

/// カードセレクタやフィルタを定義するモジュール
module Selector =

    open System
    open System.Collections.Generic

    /// <summary>
    /// カードをランダムに選択します。
    /// </summary>
    /// <remark>
    /// まずcardFilterの条件を満たすカードを選別します。この時点で10枚未満となった場合はNoneを返します。
    /// 次に、選別されたカードから10枚をランダムに選択し、このカードリストがlistFiltersの条件をすべて満たすかどうかを判別し、すべて満たす場合はそのカードリストを返します。
    /// すべての条件を満たさない場合は、ランダムに10枚選び直します。これを100回繰り返しても条件を満たせない場合、優先順位の最も低い条件を取り除いて、上記処理を繰り返します。
    /// 最終的にlistFiltersの条件がすべて取り除かれた場合、最初に選別されたカードから10枚をランダム選択したものを返します。
    /// </remark>
    /// <param name="cardFilter">各カードが満たさなければならない条件。この条件に合致するカードが10枚未満の場合はNoneを返します。</param>
    /// <param name="listFilters">カード全体として満たすべき条件のリスト。条件としての優先度の高い順のリストを指定します。</param>
    /// <param name="cards">選択肢となるカードのリスト。</param>
    /// <return>選択されたカードリストのSome。cardFilterの条件を満たすカードが10枚未満の場合はNone。</return>
    let select =
        let random = Random()
        let cardCount = 10
        let trialCount = 100

        /// カードをcardCount枚ランダムに選択します。
        let selectRandom cards =
            let result = HashSet(HashIdentity.Structural)
            let rec selectRandom = function
                | 0 -> result |> Seq.map (Array.get cards) |> Seq.toList
                | count ->
                    let index = random.Next cards.Length
                    let nextCount = if result.Add index then count - 1 else count
                    selectRandom nextCount
            selectRandom cardCount

        /// カードをランダムに選択し、カードリストの条件を満たすものを返します。
        /// filters : 優先順位の低い順のリスト。
        let select filters cards =
            let initialCount = trialCount
            let rec select filters cards = function
                | 0 -> printfn "むりぽ"; select (List.tail filters) cards initialCount
                | count ->
                    let selectedCards = cards |> selectRandom
                    if filters |> List.forall ((|>) selectedCards) then selectedCards else count - 1 |> select filters cards
            select filters cards initialCount

        fun cardFilter listFilters (cards : Card list) ->
            let cards = cards |> List.toArray |> Array.filter cardFilter
            if cards.Length < cardCount then None else
            let listFilters = listFilters |> List.rev
            select listFilters cards |> Some
    
    /// 恒真フィルタ。条件を指定したくない場合のフィルタにします。
    let trueFilter _ = true

    /// 論理積フィルタ。2つのフィルタの両方を満たす場合のみtrueを返すフィルタを生成します。
    let andFilter filter1 filter2 cards = filter1 cards && filter2 cards

    /// 論理和フィルタ。2つのフィルタの少なくとも片方を満たす場合にtrueを返すフィルタを生成します。
    let orFilter filter1 filter2 cards = filter1 cards || filter2 cards

    /// 指定したシリーズに含まれるカードのみを対象とするカードフィルタです。
    let seriesFilter seriesList { Series = series } = seriesList |> List.exists ((=) series)

    /// 任意のカードプロパティごとの最少枚数と最多枚数を指定するカードリストフィルタです。
    let countFilter propertySelector counts (cards : Card list) =
        let countDict = cards |> Seq.collect propertySelector |> Seq.countBy id |> dict
        counts |> List.forall (fun (property, minCount, maxCount) ->
            let count = match countDict.TryGetValue property with true, c -> c | false, _ -> 0
            minCount <= count && count <= maxCount)

    /// カードの種類ごとに最少枚数と最多枚数を指定するカードリストフィルタです。
    let kindFilter = countFilter (fun { Kinds = ks } -> ks)

    /// カードの特殊効果ごとに最少枚数と最多枚数を指定するカードリストフィルタです。
    let effectFilter = countFilter (fun { Effects = es } -> es)

    /// カードのコストごとに最少枚数と最多枚数を指定するカードリストフィルタです。
    let costFilter = countFilter (fun { Cost = c } -> [c])

    /// アクション追加回数ごとに最少枚数と最多枚数を指定するカードリストフィルタです。
    let actionIncrementFilter = countFilter (fun { ActionIncrement = ai } -> [ai])

    /// カードを引く枚数ごとに最少枚数と最多枚数を指定するカードリストフィルタです。
    let drawingCardFilter = countFilter (fun { DrawingCardCount = dc } -> [dc])

    /// 購入フェイズでのカード購入追加枚数ごとに最少枚数と最多枚数を指定するカードリストフィルタです。
    let buyingIncrementFilter = countFilter (fun { BuyingIncrement = bi } -> [bi])

    /// 購入フェイズでのコスト増分ごとに最少枚数と最多枚数を指定するカードリストフィルタです。
    let costIncrementFilter = countFilter (fun { CostIncrement = ci } -> [ci])

    /// カードを廃棄する効果のあるカードの最少枚数と最多枚数を指定するカードリストフィルタです。
    let canScrapFilter (minCount, maxCount) = countFilter (fun { CanScrap = cs } -> [cs]) [true, minCount, maxCount]

    /// マイナスカードを押し付ける効果のあるカードの最少枚数と最多枚数を指定するカードリストフィルタです。
    let canForceMinusFilter (minCount, maxCount) = countFilter (fun { CanForceCurse = cfm } -> [cfm]) [true, minCount, maxCount]

    /// 褒賞カードを獲得する効果のあるカードの最少枚数と最多枚数を指定するカードリストフィルタです。
    let canWinPrizeFilter (minCount, maxCount) = countFilter (fun { CanWinPrize = cwp } -> [cwp]) [true, minCount, maxCount]

    /// キャラクターを指定するカードリストフィルタです。
    let characterFilter characters cards =
        let includeCharacters = cards |> Seq.collect (fun { Characters = cs } -> cs) |> set
        characters |> set |> Set.isSuperset includeCharacters
