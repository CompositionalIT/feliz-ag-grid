module Main

open Feliz
open Browser.Dom
open Fable.Core.JsInterop

importSideEffects "./styles/global.scss"

ReactDOM.render(
    App.Documentation(),
    document.getElementById "feliz-app"
)