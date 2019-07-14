# cyclefs
A Cycle-like Fable library for build reactive webapp

# Simple
```f#
module App
 open Fable.Import
 open Fable.Cycle
 open FSharp.Control
 open Fable.Core.JsInterop
 open Fable.Helpers.React.Props
 open Operators
 
 
 module R = Fable.Helpers.React
 
 // declare a view
 type Model = {
     Content: string
     Title: string
     OnChange: string -> unit
  }
 
 let SimpleView<'T> = ToView(fun model ->
       R.div [] [
         R.div [] [
             R.str model.Title
         ]
         R.div [] [
             R.textarea [ OnChange(fun ev -> !!ev.target?value |> model.OnChange); Value model.Content ] []
         ]
     ]
 )
 
 // end declare view
 
 
 
 // build view props
 let behaviorSubject<'T> (p: 'T) =
     let dispatch, obs = AsyncRx.subject<'T> ()
     let mutable value: Option<'T> = None
     
     dispatch, obs |> AsyncRx.map (fun x ->
             value <- Some(x)
             x
         ), (fun () -> value)
     
 let dispatchContent, contentObs, getValue1 = behaviorSubject("")
 
 let onChange x =
     async {
         do! dispatchContent.OnNextAsync x
     } |> Async.Start
     ()
 
 //end build view props
 
 type Temp(props) =
     inherit React.Component<unit, unit>(props)
     
     // use view
     override this.render () =
         SimpleView {
             Content =  contentObs |> ofType;   // observable props
             Title = "I am title" // other value
             OnChange =  onChange; // other value
         } []
 let render() =
     ReactDom.render(
         R.ofType<Temp,_,_>  () [],
         Browser.document.getElementById("appcomment")
     )
 render()
```


# use
add **github bigteech/cyclefs main.fs**  to paket.dependencies  
add **File:main.fs** to paket.references
