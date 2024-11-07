open PersistedConcurrentSortedList

open PCSL
open DefaultHelper
open System.Threading
open System.Collections.Generic
open System.Threading.Tasks


module PCSLTest =
    if false then
        let pcsl = PersistedConcurrentSortedList<string, fstring>(
            20, @"c:\pfcfsl", "test"
            , PCSLFunHelper<string, fstring>.oFun
            , PCSLFunHelper<string, fstring>.eFun)

        pcsl._idx._base.Keys |> Seq.toArray |> Array.iter (fun (SLK k) ->
                pcsl.RemoveAsync k |> ignore
            )


        let s = [|1..200000|]|>Array.map (fun i -> S $"{i}")

        let success = pcsl.Add ("OGC", (A [| S "GG"|]), 3000)
        let success2 = pcsl.Add ("ORZ2", (A s), 3000)


        printfn "%A" pcsl["ORZ2"]

    //open CSLTest
    open CSL
    let csl = CSLTest.csl ()
    let (Some lockId) = csl.RequireLock(None, None) |> Async.RunSynchronously

    printfn "lockId: %A" lockId

    let addString = csl.LockableOp(CAdd (SLK "myStr", SLV (S "Orz")))

    printfn "addString: %A, Id: %A" addString.IsCompleted addString.Id

    let addTuple = csl.LockableOp(CAdd (SLK "mykey", SLV <| T ("GG", A [|S "Orz"|])), lockId)
    
    

    printfn "addTuple: %A, Id: %A" addTuple.IsCompleted addTuple.Id
    

    //Thread.Sleep 1000
    //async {
    //    let! a = addTuple |> Async.AwaitTask
    //    printfn "=> %A" a
    //} |> Async.RunSynchronously

    printfn "addTuple: %A, Id: %A" addTuple.IsCompleted addTuple.Id
    printfn "addString: %A, Id: %A" addString.IsCompleted addString.Id
    printfn "%A" csl._base 


    //async {
    //    do! Async.Sleep 2000 
    //    printfn "%A" csl._base 
    //} |> Async.RunSynchronously



    csl.UnLock lockId
    //System.Console.ReadLine() |> ignore

    //async {
    //    do! Async.Sleep 2000 
    //    printfn "%A" csl._base 
    //} |> Async.RunSynchronously


    let kopTest l =
        csl.LockableOp (
            KeysOp(
                (Some "ttc")
                , 0
                , l
                , fun km _ _ -> 
                    let ll = km.ToIList()
                        //new List<_> (km.ToArray())            
                        //:> IList<_> 
                    CKeyList ll))
    
    let keyOpTask = kopTest 2

    //Thread.Sleep 2000
    printfn "%A" keyOpTask.Result
    

    let addTuple2 = csl.LockableOp(CAdd (SLK "mykey2", SLV <| T ("GG", A [|S "Orz"|])))
    //Thread.Sleep 2000
    printfn "%A" csl._base.Keys 
    printfn "%A" keyOpTask.Result
    

    //let (FoldOpR (FoldResult keyOpTask2)) = (kopTest 3).Result
    let keyOpTask2_ = (kopTest 3).Result.foldOpRWArr
    let keyOpTask2 = (kopTest 3).Result.foldOpResultValKList 0

    //Thread.Sleep 2000
    //printfn "%A" keyOpTask2[0].OpResult.Value.Result.KeyList
    printfn "%A" keyOpTask2_[0].OpResult.Value.Result.KeyList
    printfn "%A" keyOpTask2.Value


    let kvopTest2 l =
        csl.LockableOp (
            KeyValuesOp(
                (Some "ttc")
                , 0
                , l
                , fun km vm _ _ -> 
                    let kl = km.ToArray()
                    let vl = vm.ToArray()
                        //new List<_> (km.ToArray())            
                        //:> IList<_> 
                    CKVList ((kl, vl) ||> Array.zip)))


    let kvOpTask23R = (kvopTest2 3).Result
    
    let kvOpTask23 = kvOpTask23R.foldOpResultValKVList 0

    printfn "%A" kvOpTask23.Value

    let extracted = kvOpTask23R.foldOpResultValOpR |> Array.map (fun opr -> Option.map PCSLFunHelper<string, fstring>.kvExtract opr)

    printfn "%A" extracted

    Array.concat