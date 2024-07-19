module Feliz.AgGrid

open System

open Fable.Core
open Fable.Core.JsInterop
open Feliz

// Suppress unused value warnings - they are often necessary for Fable bindings.
#nowarn "1182"

let agGrid: obj = import "AgGridReact" "ag-grid-react"

[<Erase>]
[<Import("LicenseManager", "ag-grid-enterprise")>]
type LicenseManager =
    static member setLicenseKey(key: string) : unit = jsNative

/// See https://www.ag-grid.com/react-data-grid/row-object/.
[<Erase>]
type IRowNode<'row> = {
    id: string
    data: 'row
    updateData: 'row -> unit
    setData: 'row -> unit
    setSelected: bool -> unit
    rowIndex: int
    rowTop: int
    displayed: bool
    isHovered: bool
    isFullWidthCell: bool
    isSelected: bool
}

[<Erase>]
type ICellRange = {
    id: string
    startRow: obj
    endRow: obj
} with

    member this.startRowIndex: int = this.startRow?rowIndex
    member this.endRowIndex: int = this.endRow?rowIndex

/// See https://www.ag-grid.com/react-data-grid/grid-interface/#grid-api.
[<Erase>]
type IGridApi<'row> =
    abstract refreshCells: unit -> unit
    abstract redrawRows: unit -> unit
    abstract setGridOption: string -> obj -> unit
    abstract getSelectedNodes: unit -> IRowNode<'row>[]
    abstract getCellRanges: unit -> ICellRange[]

/// See https://www.ag-grid.com/react-data-grid/column-object/.
[<Erase>]
type IColumn = { getColId: unit -> string }

[<Erase>]
type IColumnDefProp<'row, 'value> = interface end

let columnDefProp<'row, 'value> = unbox<IColumnDefProp<'row, 'value>>

// Although the AG Grid docs suggest that this should have two type params, we only give it one so that column defs
// with different underlying value types can be used in the same list (for example in AgGrid.columnDefs).
[<Erase>]
type IColumnDef<'row> = interface end

let columnDef<'row> = unbox<IColumnDef<'row>>

[<AutoOpen>]
module CallbackParams =
    /// See https://www.ag-grid.com/react-data-grid/column-properties/#reference-editing-valueSetter.
    /// See https://www.ag-grid.com/react-data-grid/column-properties/#reference-editing-valueParser.
    [<Erase>]
    type IValueChangedParams<'row, 'value> = {
        oldValue: 'value
        newValue: 'value
        node: IRowNode<'row>
        data: 'row
        column: IColumn
        colDef: IColumnDef<'row>
        api: IGridApi<'row>
    } with

        member this.rowIndex = this.node.rowIndex

    /// See https://www.ag-grid.com/react-data-grid/cell-editors/#custom-components.
    [<Erase>]
    type IValueParams<'row, 'value> = {
        value: 'value
        data: 'row
        node: IRowNode<'row>
        colDef: IColumnDef<'row>
        column: IColumn
        api: IGridApi<'row>
        rowIndex: int
    }

    /// See https://www.ag-grid.com/react-data-grid/grid-events/#reference-selection-cellFocused.
    [<Erase>]
    type ICellFocusedEvent<'row> = {
        api: IGridApi<'row>
        rowIndex: int
        column: IColumn
        isFullWidthCell: bool
    }

    /// See https://www.ag-grid.com/react-data-grid//grid-options/#reference-rowModels-getRowId.
    [<Erase>]
    type IGetRowIdParams<'row> = {
        data: 'row
        level: int
        parentKeys: string[]
        api: IGridApi<'row>
        context: obj
    }

    [<Erase>]
    type ICellRendererParams<'row, 'value> = {
        value: 'value
        data: 'row
        node: IRowNode<'row>
        colDef: IColumnDef<'row>
        column: IColumn
        api: IGridApi<'row>
        rowIndex: int
    }

    [<Erase>]
    type IPasteEvent<'row> = {
        source: string
        api: IGridApi<'row>
        context: obj
        ``type``: string
    }

    [<Erase>]
    type IProcessDataFromClipboardParams<'row> = {
        data: string[][]
        api: IGridApi<'row>
        context: obj
    }

type RowSelection =
    | Single
    | Multiple



[<RequireQualifiedAccess>]
type RowFilter =
    | Number
    | Text
    | Date

    member this.FilterText = sprintf "ag%OColumnFilter" this

[<RequireQualifiedAccess>]
type CellDataType =
    | Text
    | Number
    | Date
    | DateString
    | Boolean
    | Object
    | Custom of string

    member this.CellDataTypeText =
        match this with
        | Text -> "text"
        | Number -> "number"
        | Date -> "date"
        | DateString -> "dateString"
        | Boolean -> "boolean"
        | Object -> "object"
        | Custom s -> s

[<RequireQualifiedAccess>]
type AgCellEditor =
    | SelectCellEditor
    | NumberCellEditor
    | DateCellEditor
    | DateStringCellEditor
    | CheckboxCellEditor
    | LargeTextCellEditor
    | TextCellEditor

    member this.CellEditorText = sprintf "ag%O" this

type DOMLayout =
    | Normal
    | AutoHeight
    | Print

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

type ColumnType =
    | RightAligned
    | NumericColumn

let openClosed =
    function
    | true -> "open"
    | false -> "closed"

[<ReactComponent>]
let CellRendererComponent<'row, 'value>
    (render: ICellRendererParams<'row, 'value> -> ReactElement, p: ICellRendererParams<'row, 'value>)
    =
    render p

[<Erase>]
type ColumnDef<'row> =
    // Constrain all props for a given column to be for the same value.
    static member inline create<'value>(props: IColumnDefProp<'row, 'value> seq) = createObj !!props |> columnDef<'row>

    static member inline autoComparator() =
        columnDefProp<'row, 'value> ("comparator" ==> compare)

    static member inline cellClass(setClass: 'value -> 'row -> #seq<string>) =
        columnDefProp<'row, 'value> ("cellClass" ==> fun p -> setClass p?value p?data |> Seq.toArray)

    static member inline cellClassRules(rules: (string * ('value -> 'row -> bool)) list) =
        columnDefProp<'row, 'value> (
            "cellClassRules"
            ==> (rules
                 |> List.map (fun (className, rule) -> className ==> fun p -> rule p?value p?data)
                 |> createObj)
        )

    static member cellDataType(v: bool) =
        columnDefProp<'row, 'value> ("cellDataType" ==> v)

    static member cellDataType(v: CellDataType) =
        columnDefProp<'row, 'value> ("cellDataType" ==> v.CellDataTypeText)

    [<Obsolete("cellRendererFramework isn't supported in the latest version of AgGrid. Use cellRenderer instead", true)>]
    static member cellRendererFramework _ =
        failwith "cellRendererFramework isn't supported in the latest version of AgGrid. Use cellRenderer instead"

    static member cellRenderer(render: ICellRendererParams<'row, 'value> -> ReactElement) =
        columnDefProp<'row, 'value> ("cellRenderer" ==> fun p -> CellRendererComponent(render, p))

    static member cellEditor(render: ICellRendererParams<'row, 'value> -> ReactElement) =
        columnDefProp<'row, 'value> ("cellEditor" ==> fun p -> CellRendererComponent(render, p))

    static member cellEditor(v: string) =
        columnDefProp<'row, 'value> ("cellEditor" ==> v)

    static member cellEditor(v: AgCellEditor) =
        columnDefProp<'row, 'value> ("cellEditor" ==> v.CellEditorText)

    static member cellEditorParams(v: string seq) =
        columnDefProp<'row, 'value> ("cellEditorParams" ==> {| values = v |> Seq.toArray |})

    static member cellEditorParams(v: obj) =
        columnDefProp<'row, 'value> ("cellEditorParams" ==> v)

    static member cellEditorPopup(v: bool) =
        columnDefProp<'row, 'value> ("cellEditorPopup" ==> v)

    static member inline cellStyle(setStyle: 'value -> 'row -> _) =
        columnDefProp<'row, 'value> ("cellStyle" ==> fun p -> setStyle p?value p?data)

    static member inline checkboxSelection(v: bool) =
        columnDefProp<'row, 'value> ("checkboxSelection" ==> v)

    static member inline colId(v: string) =
        columnDefProp<'row, 'value> ("colId" ==> v)

    static member inline columnGroupShow(v: bool) =
        columnDefProp<'row, 'value> ("columnGroupShow" ==> openClosed v)

    static member inline columnType ct =
        columnDefProp<'row, 'value> (
            "type"
            ==> match ct with
                | RightAligned -> "rightAligned"
                | NumericColumn -> "numericColumn"
        )

    static member inline comparator(callback: 'a -> 'a -> int) =
        columnDefProp<'row, 'value> ("comparator" ==> fun a b -> callback a b)

    static member inline editable(callback: 'value -> 'row -> bool) =
        columnDefProp<'row, 'value> ("editable" ==> fun p -> callback p?value p?data)

    static member inline editable(v: bool) =
        columnDefProp<'row, 'value> ("editable" ==> v)

    static member inline equals(callback: 'value -> 'value -> bool) =
        columnDefProp<'row, 'value> ("equals" ==> callback)

    static member inline enableRowGroup(v: bool) =
        columnDefProp<'row, 'value> ("enableRowGroup" ==> v)

    static member inline enableCellChangeFlash(v: bool) =
        columnDefProp<'row, 'value> ("enableCellChangeFlash" ==> v)

    static member inline field(v: string) =
        columnDefProp<'row, 'value> ("field" ==> v)

    /// Usage: `ColumnDef.field _.FieldName` or `ColumnDef.field (fun x -> x.FieldName)`
    static member inline field(f: 'row -> _) =
        let idxOfFirstDot = (string f).IndexOf('.')
        // `ColumnDef.field _.FirstName` and `ColumnDef.field (fun x -> x.FirstName)` both result in "FirstName".
        let field = (string f).Substring(idxOfFirstDot + 1)
        columnDefProp<'row, 'value> ("field" ==> field)

    static member inline filter(v: RowFilter) =
        columnDefProp<'row, 'value> ("filter" ==> v.FilterText)

    static member inline filter(v: bool) =
        columnDefProp<'row, 'value> ("filter" ==> v)

    static member inline floatingFilter(v: bool) =
        columnDefProp<'row, 'value> ("floatingFilter" ==> v)

    static member inline headerCheckboxSelection(v: bool) =
        columnDefProp<'row, 'value> ("headerCheckboxSelection" ==> v)

    static member inline headerClass(v: string) =
        columnDefProp<'row, 'value> ("headerClass" ==> v)

    static member inline headerComponentFramework(callback: 'colId -> 'props -> ReactElement) =
        columnDefProp<'row, 'value> ("headerComponentFramework" ==> fun p -> callback p?column?colId p)

    static member inline headerName(v: string) =
        columnDefProp<'row, 'value> ("headerName" ==> v)

    static member inline wrapHeaderText(v: bool) =
        columnDefProp<'row, 'value> ("wrapHeaderText" ==> v)

    static member inline autoHeaderHeight(v: bool) =
        columnDefProp<'row, 'value> ("autoHeight" ==> v)

    static member inline hide(v: bool) =
        columnDefProp<'row, 'value> ("hide" ==> v)

    static member inline maxWidth(v: int) =
        columnDefProp<'row, 'value> ("maxWidth" ==> v)

    static member inline minWidth(v: int) =
        columnDefProp<'row, 'value> ("minWidth" ==> v)

    static member inline onCellClicked(handler: 'value -> 'row -> unit) =
        columnDefProp<'row, 'value> ("onCellClicked" ==> (fun p -> handler p?value p?data))

    static member inline pinned(v: bool) =
        columnDefProp<'row, 'value> ("pinned" ==> v)



    static member inline resizable(v: bool) =
        columnDefProp<'row, 'value> ("resizable" ==> v)

    static member inline rowDrag(v: bool) =
        columnDefProp<'row, 'value> ("rowDrag" ==> v)

    static member inline rowGroup(v: bool) =
        columnDefProp<'row, 'value> ("rowGroup" ==> v)

    static member inline sortable(v: bool) =
        columnDefProp<'row, 'value> ("sortable" ==> v)

    static member inline suppressKeyboardEvent callback =
        columnDefProp<'row, 'value> ("suppressKeyboardEvent" ==> fun x -> callback x?event)

    static member inline suppressMovable() =
        columnDefProp<'row, 'value> ("suppressMovable" ==> true)

    static member inline valueFormatter(callback: IValueParams<'row, 'value> -> string) =
        columnDefProp<'row, 'value> ("valueFormatter" ==> callback)

    static member inline valueGetter(f: 'row -> 'value) =
        columnDefProp<'row, 'value> ("valueGetter" ==> (fun x -> f x?data))

    static member inline valueSetter(f: IValueChangedParams<'row, 'value> -> unit) =
        columnDefProp<'row, 'value> ("valueSetter" ==> f)

    static member inline valueSetter(f: IValueChangedParams<'row, 'value> -> bool) =
        columnDefProp<'row, 'value> ("valueSetter" ==> f)

    static member inline valueParser(f: IValueChangedParams<'row, 'value> -> obj) =
        columnDefProp<'row, 'value> ("valueParser" ==> f) // Is never called by AgGrid

    static member inline width(v: int) =
        columnDefProp<'row, 'value> ("width" ==> v)

[<Erase>]
type IColumnGroupDefProp<'row> = interface end

let columnGroupDefProp<'row> = unbox<IColumnGroupDefProp<'row>>

[<Erase>]
type ColumnGroup<'row> =
    static member inline headerName(v: string) =
        columnGroupDefProp<'row> ("headerName" ==> v)

    static member inline marryChildren(v: bool) =
        columnGroupDefProp<'row> ("marryChildren" ==> v)

    static member inline openByDefault(v: bool) =
        columnGroupDefProp<'row> ("openByDefault" ==> v)

    static member inline create (props: seq<IColumnGroupDefProp<'row>>) (children: seq<IColumnDef<'row>>) =
        let combinedProps = seq {
            yield! props
            columnGroupDefProp<'row> ("children" ==> Seq.toArray children)
        }

        createObj !!combinedProps |> columnDef<'row>

[<Erase>]
type IAgGridProp<'row> = interface end

let agGridProp<'row> (x: obj) = unbox<IAgGridProp<'row>> x

[<Erase>]
type AgGrid<'row> =
    static member inline animateRows(v: bool) = agGridProp<'row> ("animateRows" ==> v)

    static member inline alwaysShowVerticalScroll(v: bool) =
        agGridProp<'row> ("alwaysShowVerticalScroll" ==> v)

    static member inline columnDefs(columns: IColumnDef<'row> seq) =
        agGridProp<'row> ("columnDefs", Seq.toArray !!columns)

    static member inline copyHeadersToClipboard(v: bool) =
        agGridProp<'row> ("copyHeadersToClipboard" ==> v)

    static member inline domLayout(l: DOMLayout) =
        agGridProp<'row> ("domLayout", l.LayoutText)

    static member inline enableCellTextSelection(v: bool) =
        agGridProp<'row> ("enableCellTextSelection" ==> v)

    static member inline ensureDomOrder(v: bool) =
        agGridProp<'row> ("ensureDomOrder" ==> v)

    static member inline enterNavigatesVertically(v: bool) =
        agGridProp<'row> ("enterNavigatesVertically" ==> v)

    static member inline getRowNodeId(callback: 'row -> _) =
        agGridProp<'row> ("getRowNodeId", callback)

    static member inline getRowId(callback: IGetRowIdParams<'row> -> string) = agGridProp<'row> ("getRowId", callback)

    static member inline onCellEditRequest(callback: obj -> unit) =
        agGridProp<'row> ("onCellEditRequest", callback)

    static member inline onCellValueChanged callback =
        agGridProp<'row> ("onCellValueChanged", (fun x -> callback x?data))

    static member inline onPasteStart(callback: IPasteEvent<'row> -> unit) =
        agGridProp<'row> ("onPasteStart", callback)

    static member inline onPasteEnd(callback: IPasteEvent<'row> -> unit) =
        agGridProp<'row> ("onPasteEnd", callback)

    static member inline onRowClicked(handler: 'value -> 'row -> unit) =
        agGridProp<'row> ("onRowClicked" ==> (fun p -> handler p?value p?data))

    static member inline onSelectionChanged(callback: 'row array -> unit) =
        agGridProp<'row> ("onSelectionChanged", (fun x -> x?api?getSelectedRows () |> callback))

    static member inline readOnlyEdit(v: bool) = agGridProp<'row> ("readOnlyEdit" ==> v)

    static member inline singleClickEdit(v: bool) =
        agGridProp<'row> ("singleClickEdit" ==> v)

    static member inline rowDeselection(v: bool) = agGridProp<'row> ("rowDeselection", v)

    static member inline rowSelection(s: RowSelection) =
        agGridProp<'row> ("rowSelection", s.ToString().ToLower())

    static member inline isRowSelectable(callback: 'row -> bool) =
        agGridProp<'row> ("isRowSelectable" ==> fun x -> x?data |> callback)

    static member inline suppressRowClickSelection(v: bool) =
        agGridProp<'row> ("suppressRowClickSelection" ==> v)

    static member inline rowHeight(h: int) = agGridProp<'row> ("rowHeight", h)
    static member inline immutableData(v: bool) = agGridProp<'row> ("immutableData", v)

    /// Converts your data to a JS array to populate the grid. (This is less efficient than passing an array.)
    static member inline rowData(data: 'row seq) =
        agGridProp<'row> ("rowData", Seq.toArray data)

    static member inline rowData(data: 'row array) = agGridProp<'row> ("rowData", data)

    static member inline rowDragManaged(v: bool) =
        agGridProp<'row> ("rowDragManaged" ==> v)

    static member inline defaultColDef(defaults: IColumnDefProp<'row, 'value> seq) =
        agGridProp<'row> ("defaultColDef", defaults |> unbox<_ seq> |> createObj)

    static member onColumnGroupOpened(callback: _ -> unit) = // This can't be inline otherwise Fable produces invalid JS
        let onColumnGroupOpened =
            fun ev ->
                {|
                    AutoSizeGroupColumns =
                        fun () ->
                            // Runs the column autoSize in a 0ms timeout so that the cellRenderer cells render before
                            // the grid calculates how large each cell is
                            JS.setTimeout
                                (fun () ->
                                    let colIds =
                                        ev?columnGroups
                                        |> Seq.head
                                        |> fun cg -> cg?children
                                        |> Array.map (fun x -> x?colId)

                                    ev?api?autoSizeColumns colIds)
                                0
                            |> ignore
                |}
                |> callback

        agGridProp<'row> ("onColumnGroupOpened", onColumnGroupOpened)

    static member inline paginationPageSize(pageSize: int) =
        agGridProp<'row> ("paginationPageSize", pageSize)

    static member inline paginationAutoPageSize(v: bool) =
        agGridProp<'row> ("paginationAutoPageSize", v)

    static member inline pagination(v: bool) = agGridProp<'row> ("pagination", v)

    static member onGridReady(callback: _ -> unit) = // This can't be inline otherwise Fable produces invalid JS
        let onGridReady =
            fun ev ->
                {|
                    AutoSizeAllColumns =
                        fun () ->
                            // Runs the column autoSize in a 0ms timeout so that the cellRendererFramework cells render
                            // before the grid calculates how large each cell is
                            JS.setTimeout
                                (fun () ->
                                    let colIds = ev?api?getColumns () |> Array.map (fun x -> x?colId)
                                    ev?api?autoSizeColumns colIds)
                                0
                            |> ignore
                    Export = fun () -> ev?api?exportDataAsCsv (obj ())
                |}
                |> callback

        agGridProp<'row> ("onGridReady", onGridReady)

    static member inline processDataFromClipboard(callback: IProcessDataFromClipboardParams<'row> -> string[][]) =
        agGridProp<'row> ("processDataFromClipboard", callback)

    static member inline enableRangeHandle(v: bool) =
        agGridProp<'row> ("enableRangeHandle", v)

    static member inline enableRangeSelection(v: bool) =
        agGridProp<'row> ("enableRangeSelection", v)


    static member inline suppressAggFuncInHeader(v: bool) =
        agGridProp<'row> ("suppressAggFuncInHeader", v)

    static member inline headerHeight height =
        agGridProp<'row> ("headerHeight", height)

    static member inline groupHeaderHeight height =
        agGridProp<'row> ("groupHeaderHeight", height)

    [<Obsolete("Use the newer onCellFocused overload that passes an `ICellFocusedEvent`.", false)>]
    static member inline onCellFocused callback =
        agGridProp<'row> ("onCellFocused", (fun x -> callback (int x?rowIndex) (int x?column?colId)))

    static member inline onCellFocused callback =
        agGridProp<'row> ("onCellFocused", (fun (e: ICellFocusedEvent<'row>) -> callback e))

    static member inline onRangeSelectionChanged callback =
        agGridProp<'row> (
            "onRangeSelectionChanged",
            fun x ->
                let selectedRange = x?api?getCellRanges ()?at 0
                let startRow = selectedRange?startRow?rowIndex
                let startColumn = selectedRange?columns?at 0?colId
                let endRow = selectedRange?endRow?rowIndex
                let endColumn = selectedRange?columns?at (selectedRange?columns?length - 1)?colId

                callback startRow startColumn endRow endColumn
        )

    static member inline popupParent parent =
        agGridProp<'row> ("popupParent", parent)

    static member inline processDataFromClipboard(callback: string[][] -> string[][]) =
        agGridProp<'row> ("processDataFromClipboard", (fun x -> callback x?data))

    static member inline stopEditingWhenCellsLoseFocus(v: bool) =
        agGridProp<'row> ("stopEditingWhenCellsLoseFocus", v)

    static member inline stopEditingWhenGridLosesFocus(v: bool) =
        agGridProp<'row> ("stopEditingWhenGridLosesFocus", v)

    static member inline suppressClipboardApi(v: bool) =
        agGridProp<'row> ("suppressClipboardApi", v)

    static member inline suppressCopyRowsToClipboard(v: bool) =
        agGridProp<'row> ("suppressCopyRowsToClipboard", v)

    static member inline suppressCopySingleCellRanges(v: bool) =
        agGridProp<'row> ("suppressCopySingleCellRanges", v)

    static member inline suppressMultiRangeSelection(v: bool) =
        agGridProp<'row> ("suppressMultiRangeSelection", v)

    static member inline suppressRowHoverHighlight(v: bool) =
        agGridProp<'row> ("suppressRowHoverHighlight", v)

    static member inline suppressScrollOnNewData(v: bool) =
        agGridProp<'row> ("suppressScrollOnNewData", v)

    static member inline key(v: string) = agGridProp<'row> (prop.key v)
    static member inline key(v: int) = agGridProp<'row> (prop.key v)
    static member inline key(v: Guid) = agGridProp<'row> (prop.key v)

    static member inline dataTypeDefinitions(v: obj) =
        agGridProp<'row> ("dataTypeDefinitions", v)

    static member inline enableFillHandle(v: bool) =
        agGridProp<'row> ("enableFillHandle", v)

    static member inline undoRedoCellEditing(v: bool) =
        agGridProp<'row> ("undoRedoCellEditing", v)

    static member inline undoRedoCellEditingLimit(v: int) =
        agGridProp<'row> ("undoRedoCellEditingLimit", v)

    static member inline grid(props: IAgGridProp<'row> seq) =
        Interop.reactApi.createElement (agGrid, createObj !!props)

    module Enterprise =

        [<RequireQualifiedAccess>]
        type RowFilter =
            | Number
            | Text
            | Date
            | Set
            | MultiColumn

            member this.FilterText = sprintf "ag%OColumnFilter" this

        [<RequireQualifiedAccess>]
        type AgCellEditor =
            | SelectCellEditor
            | NumberCellEditor
            | DateCellEditor
            | DateStringCellEditor
            | CheckboxCellEditor
            | LargeTextCellEditor
            | TextCellEditor
            | RichSelectCellEditor

            member this.RichCellEditorText = sprintf "ag%O" this

        [<RequireQualifiedAccess>]
        type RowGroupingDisplayType =
            | SingleColumn
            | MultipleColumns
            | GroupRows
            | Custom

            member this.RowGroupingDisplayTypeText =
                match this with
                | SingleColumn -> "singleColumn"
                | MultipleColumns -> "multipleColumns"
                | GroupRows -> "groupRows"
                | Custom -> "custom"

        [<RequireQualifiedAccess>]
        type RowGroupPanelShow =
            | Always
            | OnlyWhenGrouping
            | Never

            member this.RowGroupPanelShowText =
                match this with
                | Always -> "always"
                | OnlyWhenGrouping -> "onlyWhenGrouping"
                | Never -> "never"

        [<RequireQualifiedAccess>]
        type AggregateFunction =
            | Sum
            | Min
            | Max
            | Count
            | Avg
            | First
            | Last

            member this.AggregateText = (sprintf "%O" this).ToLower()

        type MenuItemDef = {
            name: string
            action: unit -> unit
            shortcut: string
            icon: obj //HtmlElement
        }

        type MenuItem =
            | BuiltIn of string
            | Custom of MenuItemDef

        /// See https://www.ag-grid.com/react-data-grid/grid-interface/#grid-api.
        [<Erase>]
        type IGridApi<'row> =
            abstract copyToClipboard: unit -> unit
            abstract pasteFromClipboard: unit -> unit
            abstract refreshCells: unit -> unit
            abstract redrawRows: unit -> unit
            abstract setGridOption: string -> obj -> unit
            abstract getSelectedNodes: unit -> IRowNode<'row>[]
            abstract getCellRanges: unit -> ICellRange[]

        [<Erase>]
        type ColumnDef<'row> =
            static member inline filter(v: RowFilter) = columnDefProp<'row, 'value> ("filter" ==> v.FilterText)
            static member cellEditor(v: AgCellEditor) = columnDefProp<'row, 'value> ("cellEditor" ==> v.RichCellEditorText)
            static member inline pivot(v: bool) = columnDefProp<'row, 'value> ("pivot" ==> v)

        [<Erase>]
        type AgGrid<'row> =
            static member inline rowGroupPanelShow(v: RowGroupPanelShow) =
                agGridProp<'row> ("rowGroupPanelShow", v.RowGroupPanelShowText)
            static member inline aggFunc(v: AggregateFunction) = columnDefProp<'row, 'value> ("aggFunc" ==> v.AggregateText)

            static member inline groupDisplayType(v: RowGroupingDisplayType) =
                agGridProp<'row> ("groupDisplayType", v.RowGroupingDisplayTypeText)

            static member inline pivotMode(v: bool) = agGridProp<'row> ("pivotMode", v)
            static member inline treeData(v: bool) = agGridProp<'row> ("treeData", v)
            static member inline getContextMenuItems(callback: int -> int -> MenuItem list) =
                agGridProp<'row> (
                    "getContextMenuItems",
                    fun x ->
                        let menuItems = callback x?node?rowIndex x?column?colId

                        [|
                            for item in menuItems do
                                match item with
                                | BuiltIn builtInItemName -> box builtInItemName
                                | Custom customMenuItem -> box customMenuItem
                        |]
                )
