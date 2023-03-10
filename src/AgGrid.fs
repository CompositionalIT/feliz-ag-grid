// fsharplint:disable
module Feliz.AgGrid

open Fable.Core
open Fable.Core.JsInterop

open Feliz

let agGrid : obj = import "AgGridReact" "ag-grid-react"

importAll "ag-grid-community/dist/styles/ag-grid.css"
importAll "ag-grid-community/dist/styles/ag-theme-alpine.css"
importAll "ag-grid-community/dist/styles/ag-theme-alpine-dark.css"
importAll "ag-grid-community/dist/styles/ag-theme-balham.css"
importAll "ag-grid-community/dist/styles/ag-theme-balham-dark.css"
importAll "ag-grid-community/dist/styles/ag-theme-material.css"

type RowSelection = Single | Multiple
type RowFilter = Number | Text | Date member this.FilterText = sprintf "ag%OColumnFilter" this
type DOMLayout =
    Normal | AutoHeight | Print
    member this.LayoutText =
        match this with
        | Normal -> "normal"
        | AutoHeight -> "autoHeight"
        | Print -> "print"

module ThemeClass =
    let Alpine = "ag-theme-alpine"
    let AlpineDark = "ag-theme-alpine-dark"
    let Balham = "ag-theme-balham"
    let BalhamDark = "ag-theme-balham-dark"
    let Material = "ag-theme-material"

type MenuItemDef = {
    name : string
    action : unit -> unit
    shortcut : string
    icon : obj //HtmlElement
}
type MenuItem =
    | BuiltIn of string
    | Custom of MenuItemDef

[<Erase>]
type IColumnDefProp<'row, 'value> = interface end
let columnDefProp<'row, 'value> = unbox<IColumnDefProp<'row, 'value>>
let columnDefProps<'row, 'value> = unbox<IColumnDefProp<'row, 'value>>

[<Erase>]
type IColumnDef<'row> = interface end

type ColumnType = RightAligned | NumericColumn

let openClosed = function | true -> "open" | false -> "closed"

[<ReactComponent>]
let CellRendererComponent<'value,'row> (render:'value -> 'row -> ReactElement, p) =
    render p?value p?data

[<Erase>]
type ColumnDef<'row, 'value> =
    static member inline autoComparator = columnDefProp<'row, 'value> ("comparator" ==> compare)
    static member inline cellClass (setClass:'value -> 'row -> #seq<string>) = columnDefProp<'row, 'value> ("cellClass" ==> fun p -> setClass p?value p?data |> Seq.toArray)
    static member inline cellClassRules (rules: (string*('value -> 'row -> bool)) list) = columnDefProp<'row, 'value> ("cellClassRules" ==> (rules |> List.map (fun (className, rule) -> className ==> fun p -> rule p?value p?data) |> createObj))
    static member cellRendererFramework (render:'value -> 'row -> ReactElement) = columnDefProp<'row, 'value> ("cellRendererFramework" ==> fun p -> CellRendererComponent(render, p))
    static member inline cellStyle (setStyle:'value -> 'row -> _) = columnDefProp<'row, 'value> ("cellStyle" ==> fun p -> setStyle p?value p?data)
    static member inline checkboxSelection (v:bool) = columnDefProp<'row, 'value> ("checkboxSelection" ==> v)
    static member inline colId (v:string) = columnDefProp<'row, 'value> ("colId" ==> v)
    static member inline columnGroupShow (v:bool) = columnDefProp<'row, 'value> ("columnGroupShow" ==> openClosed v)
    static member inline columnType ct = columnDefProp<'row, 'value> ("type" ==> match ct with RightAligned -> "rightAligned" | NumericColumn -> "numericColumn")
    static member inline comparator (callback: 'a -> 'a -> int) = columnDefProp<'row, 'value> ("comparator" ==> fun a b -> callback a b)
    static member inline create<'v> (props:seq<IColumnDefProp<'row, 'v>>) = props |> unbox<_ seq> |> createObj |> unbox<IColumnDef<'row>>
    static member inline editable (callback:'value -> 'row -> bool) = columnDefProp<'row, 'value> ("editable" ==> fun p -> callback p?value p?data)
    static member inline field (v:'a -> string) = columnDefProp<'row, 'value> ("field" ==> v (unbox null))
    static member inline field (v:string) = columnDefProp<'row, 'value> ("field" ==> v)
    static member inline filter (v:RowFilter) = columnDefProp<'row, 'value> ("filter" ==> v.FilterText)
    static member inline headerCheckboxSelection (v:bool) = columnDefProp<'row, 'value> ("headerCheckboxSelection" ==> v)
    static member inline headerClass (v:string) = columnDefProp<'row, 'value> ("headerClass" ==> v)
    static member inline headerComponentFramework (callback:'colId -> 'props -> ReactElement) = columnDefProp<'row, 'value> ("headerComponentFramework" ==> fun p -> callback p?column?colId p)
    static member inline headerName (v:string) = columnDefProp<'row, 'value> ("headerName" ==> v)
    static member inline hide (v:bool) = columnDefProp<'row, 'value> ("hide" ==> v)
    static member inline maxWidth (v:int) = columnDefProp<'row, 'value> ("maxWidth" ==> v)
    static member inline minWidth (v:int) = columnDefProp<'row, 'value> ("minWidth" ==> v)
    static member inline onCellClicked (handler:'value -> 'row -> unit) = columnDefProp<'row, 'value> ("onCellClicked" ==> (fun p -> handler p?value p?data))
    static member inline resizable (v:bool) = columnDefProp<'row, 'value> ("resizable" ==> v)
    static member inline sortable (v:bool) = columnDefProp<'row, 'value> ("sortable" ==> v)
    static member inline suppressKeyboardEvent callback = columnDefProp<'row, 'value> ("suppressKeyboardEvent" ==> fun x -> callback x?event)
    static member inline suppressMovable = columnDefProp<'row, 'value> ("suppressMovable" ==> true)
    static member inline valueFormatter (v:'value -> 'row -> string) = columnDefProp<'row, 'value> ("valueFormatter" ==> (fun p -> v p?value p?data))
    static member inline valueGetter (f:'row -> 'value) = columnDefProp<'row, 'value> ("valueGetter" ==> (fun x -> f x?data))
    static member inline valueSetter (f:string -> 'value -> 'row -> unit) = columnDefProp<'row, 'value> ("valueSetter" ==> (fun x -> f x?newValue x?oldValue x?data; false)) // return false to prevent the grid from immediately refreshing
    static member inline width (v:int) = columnDefProp<'row, 'value> ("width" ==> v)

[<Erase>]
type IColumnGroupDefProp<'row> = interface end
let columnGroupDefProp<'row> = unbox<IColumnGroupDefProp<'row>>

[<Erase>]
type ColumnGroup<'row> =
    static member inline headerName (v:string) = columnGroupDefProp<'row> ("headerName" ==> v)
    static member inline marryChildren(v:bool) = columnGroupDefProp<'row> ("marryChildren" ==> v)
    static member inline openByDefault(v:bool) = columnGroupDefProp<'row> ("openByDefault" ==> v)

    static member inline create<'row> (props:seq<IColumnGroupDefProp<'row>>) (children:seq<IColumnDef<'row>>) =
        props |> Seq.append [(columnGroupDefProp<'row> ("children" ==> Seq.toArray children))] |> unbox<_ seq> |> createObj |> unbox<IColumnDef<'row>>

[<Erase>]
type IAgGridProp<'row> = interface end
let agGridProp<'row> (x:obj) = unbox<IAgGridProp<'row>> x

[<Erase>]
type AgGrid() =
    static member inline onSelectionChanged (callback:'row array -> unit) = agGridProp<'row>("onSelectionChanged", fun x -> x?api?getSelectedRows() |> callback)
    static member inline onCellValueChanged callback = agGridProp<'row>("onCellValueChanged", fun x -> callback x?data)
    static member inline onRowClicked (handler:'value -> 'row -> unit) = agGridProp<'row> ("onRowClicked" ==> (fun p -> handler p?value p?data))
    static member inline singleClickEdit (v:bool) = agGridProp<'row>("singleClickEdit" ==> v)
    static member inline rowDeselection (v:bool) = agGridProp<'row>("rowDeselection", v)
    static member inline rowSelection (s:RowSelection) = agGridProp<'row>("rowSelection", s.ToString().ToLower())
    static member inline isRowSelectable (callback:'row -> bool) = agGridProp<'row>("isRowSelectable" ==> fun x -> x?data |> callback)
    static member inline suppressRowClickSelection (v:bool) = agGridProp<'row>("suppressRowClickSelection" ==> v)
    static member inline enableCellTextSelection (v:bool) = agGridProp<'row> ("enableCellTextSelection" ==> v)
    static member inline ensureDomOrder (v:bool) = agGridProp<'row> ("ensureDomOrder" ==> v)
    static member inline rowHeight (h:int) = agGridProp<'row>("rowHeight", h)
    static member inline domLayout (l:DOMLayout) = agGridProp<'row>("domLayout", l.LayoutText)
    static member inline immutableData (v:bool) = agGridProp<'row>("immutableData", v)
    static member inline rowData (data:'row array) = agGridProp<'row>("rowData", data)
    static member inline getRowNodeId (callback: 'row -> _) = agGridProp<'row>("getRowNodeId", callback)
    static member inline columnDefs (columns:IColumnDef<'row> seq) = agGridProp<'row>("columnDefs", columns |> unbox |> Seq.toArray)
    static member inline defaultColDef (defaults:IColumnDefProp<'row, 'value> seq) = agGridProp<'row>("defaultColDef", defaults |> unbox<_ seq> |> createObj)
    static member onColumnGroupOpened (callback:_ -> unit) = // This can't be inline otherwise Fable produces invalid JS
        let onColumnGroupOpened = fun ev ->
            {| AutoSizeGroupColumns = fun () ->
                // Runs the column autoSize in a 0ms timeout so that the cellRendererFramework cells render
                // before the grid calculates how large each cell is
                JS.setTimeout (fun () ->
                    let colIds = ev?columnGroup?children |> Array.map (fun x -> x?colId)
                    ev?columnApi?autoSizeColumns(colIds)) 0 |> ignore |}
            |> callback
        agGridProp<'row>("onColumnGroupOpened", onColumnGroupOpened)

    static member inline paginationPageSize (pageSize:int) = agGridProp<'row>("paginationPageSize", pageSize)
    static member inline paginationAutoPageSize (v:bool) = agGridProp<'row>("paginationAutoPageSize", v)
    static member inline pagination (v:bool) = agGridProp<'row>("pagination", v)
    static member onGridReady (callback:_ -> unit) = // This can't be inline otherwise Fable produces invalid JS
        let onGridReady = fun ev ->
            {| AutoSizeAllColumns =
                fun () ->
                    // Runs the column autoSize in a 0ms timeout so that the cellRendererFramework cells render
                    // before the grid calculates how large each cell is
                    JS.setTimeout (fun () ->
                        let colIds = ev?columnApi?getAllColumns() |> Array.map (fun x -> x?colId)
                        ev?columnApi?autoSizeColumns(colIds)) 0 |> ignore
               Export = fun () -> ev?api?exportDataAsCsv(obj()) |}
            |> callback
        agGridProp<'row>("onGridReady", onGridReady)
    static member inline enableRangeHandle = prop.custom("enableRangeHandle", true)
    static member inline enableRangeSelection = prop.custom("enableRangeSelection", true)
    static member inline getContextMenuItems (callback : int -> int -> MenuItem list) = agGridProp<'row>("getContextMenuItems", fun x ->
            let menuItems = callback x?node?rowIndex x?column?colId
            [|
                for item in menuItems do
                    match item with
                    | BuiltIn builtInItemName -> box builtInItemName
                    | Custom customMenuItem -> box customMenuItem
            |])
    static member inline headerHeight height = agGridProp<'row>("headerHeight", height)
    static member inline onCellFocused callback = agGridProp<'row>("onCellFocused", fun x -> callback x?rowIndex x?column?colId)
    static member inline onRangeSelectionChanged callback = agGridProp<'row>("onRangeSelectionChanged", fun x ->
            let selectedRange = x?api?getCellRanges()?at(0)
            let startRow = selectedRange?startRow?rowIndex
            let startColumn = selectedRange?columns?at(0)?colId
            let endRow = selectedRange?endRow?rowIndex
            let endColumn = selectedRange?columns?at(selectedRange?columns?length-1)?colId

            callback startRow startColumn endRow endColumn)
    static member inline popupParent parent = agGridProp<'row>("popupParent", parent)
    static member inline processDataFromClipboard (callback : string[][] -> string[][]) = agGridProp<'row>("processDataFromClipboard", fun x -> callback x?data)
    static member inline stopEditingWhenCellsLoseFocus = prop.custom("stopEditingWhenCellsLoseFocus", true)
    static member inline stopEditingWhenGridLosesFocus = prop.custom("stopEditingWhenGridLosesFocus", true)
    static member inline suppressClipboardApi = prop.custom("suppressClipboardApi", true)
    static member inline suppressCopyRowsToClipboard = prop.custom("suppressCopyRowsToClipboard", true)
    static member inline suppressCopySingleCellRanges = prop.custom("suppressCopySingleCellRanges", true)
    static member inline suppressMultiRangeSelection = prop.custom("suppressMultiRangeSelection", true)
    static member inline suppressRowHoverHighlight = prop.custom("suppressRowHoverHighlight", true)
    static member inline suppressScrollOnNewData = prop.custom("suppressScrollOnNewData", true)

    static member inline key (v:string) = agGridProp<'row> (prop.key v)
    static member inline key (v:int) = agGridProp<'row> (prop.key v)
    static member inline key (v:System.Guid) = agGridProp<'row> (prop.key v)

    static member inline grid<'row> (props:IAgGridProp<'row> seq) = Interop.reactApi.createElement (agGrid, createObj !!props)
