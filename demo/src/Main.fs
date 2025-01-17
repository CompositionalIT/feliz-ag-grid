module Main

open Feliz
open Browser.Dom
open Fable.Core.JsInterop
open Elmish
open Elmish.React

importSideEffects "./styles/global.scss"


Program.mkProgram App.init App.update App.Documentation
//-:cnd:noEmit
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app"
|> Program.run
