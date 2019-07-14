module Fable.Cycle
open System
open Fable.Core
open Fable.Core.JsInterop
open FSharp.Control
open Fable.Import
open Operators


let react: Object =
    importDefault "react"

module private Helper =
    [<Emit("(function(f1, f2, f3, f4, f5) {
    class Temp extends f5.Component {
        constructor(props) {
            super(props);
            this.state= {};
            f4([props, (function(p) {
                this.state[p[0]] = p[1];
            }).bind(this)]);
            f1((function(p) {
                this.state[p[0]] = p[1];
                this.setState(this.state);
            }).bind(this))
        }

        unMounted() {
            f2();
        }

        render() {
            return f3(this.state);
        }
    }
    return function(p) {
        return function(k) {
            return f5.createElement(Temp, p, k)
        }
    }
})($0, $1, $2, $3, $4)")>]
    let getComponent (f1, f2, f3, f4, f5) = jsNative
        
    
    type KV = {
        name : string;
        value : Object
     }

    [<Emit("(function a(x){return !!x.SubscribeAsync;})($0)")>]
    let isObservable x = jsNative
    
    
    [<Emit("(function a(x){var ret = []; Object.getOwnPropertyNames(x).forEach(function(y){y !== 'children' && ret.push({name: y, value: x[y]});});return ret;})($0)")>]
    let getKVs x : KV [] = jsNative

    
[<Emit("(function a(x){return x;})($0)")>]
let ofType (x:IAsyncObservable<'T>): 'T = jsNative


let ToView<'T> (x:('T -> React.ReactElement)) =
    let mutable funToSetState = (fun x -> ())
    let mutable unsubs = []
    let construct (props, setState)=
        let fields = Helper.getKVs(props)
        for i in fields do
             if (Helper.isObservable i.value) then
                let s = AsyncRx.single 10
                let obv n =
                    async {
                        match n with
                        | OnNext x -> funToSetState (i.name, x)
                        | OnError ex -> printfn "OnError: %s" (ex.ToString())
                        | OnCompleted -> printfn "OnCompleted"
                    }
                let v : IAsyncObservable<_> = downcast i.value
                async {
                    let! disposableAsync = v.SubscribeAsync obv
                    unsubs <- disposableAsync :: unsubs
                    ()
                } |> Async.Start
                ()
             else
                 setState(i.name, i.value)
                 ()
    let ret:'T -> seq<React.ReactElement> -> React.ReactElement =
            Helper.getComponent(
                (fun x -> funToSetState <- x),
                (fun x -> for i in unsubs do
                            i.DisposeAsync() |> ignore
                ),
                x,
                construct,
                react
            )
    ret

