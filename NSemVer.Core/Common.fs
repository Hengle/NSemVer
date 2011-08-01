module Common

type Match<'a> = { Old:'a; New:'a }
type CollectionChangeResult<'a> = { Added:list<'a>; Removed:list<'a>; Matched:list<Match<'a>> }
type ComparisonResult<'a> = Matched of ('a * 'a) | Removed of 'a
type CollChangeState<'a> = { Matched:list<Match<'a>>; Removed:list<'a> }

let updateCollectionChangeResult (state:CollChangeState<'a>) item (comparer:'a -> ComparisonResult<'a>) =
    match comparer item with
    | Matched (oldItem, newItem) -> { state with Matched = state.Matched @ [ { Old=oldItem; New=newItem } ] }
    | Removed a -> { state with Removed = state.Removed @ [item]; }

let compareItemAgainstSeq (coll:seq<'a>) (comparer:'a -> 'a -> bool) (item:'a) =
    match coll |> Seq.tryFind (fun n -> comparer n item) with
    | Some matched -> ComparisonResult.Matched(item, matched)
    | None -> ComparisonResult.Removed(item)

let calcCollChanges oldItems newItems (comparer:'a -> 'a -> bool) = 
    // inefficient. Does for now
    let oldToNewChanges =
        oldItems
        |> Seq.fold (fun state item -> updateCollectionChangeResult state item (compareItemAgainstSeq newItems comparer)) { Matched=[]; Removed=[] }
    let newToOldChanges =
        newItems
           |> Seq.fold (fun state item -> updateCollectionChangeResult state item (compareItemAgainstSeq oldItems comparer)) { Matched=[]; Removed=[] }
    { Added=newToOldChanges.Removed; Removed=oldToNewChanges.Removed; Matched=oldToNewChanges.Matched; }