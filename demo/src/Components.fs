module App

open Feliz.Bulma
open Feliz
open Fable.Core.JsInterop
open Feliz.AgGrid
open System
open Thoth.Fetch
open Thoth.Json
open Fable.Core


let citDarkBlue = "#102035"

type LinkData = { Text: string; Href: string }

let container (children: ReactElement list) =
    Html.div [
        prop.style [
            style.display.flex
            style.flexDirection.column
            style.padding 50
            style.maxWidth 1000
            style.margin (0,length.auto)
        ]
        prop.children children
    ]

let row (children: ReactElement list) =
    Html.div [
        prop.style [
            style.alignItems.center
            style.display.flex
        ]
        prop.children children
    ]

let navbar () =
    let logo : string = importDefault "./cit-logo.png"
    Html.div [
        prop.style [
            style.padding (0, 20)
            style.backgroundColor citDarkBlue
            style.height 70
            style.color "white"
            style.display.flex
            style.justifyContent.spaceBetween
            style.alignItems.center
        ]
        prop.children [
            Html.div [
                prop.style [
                    style.width 1000
                    style.margin (0, length.auto)
                    style.display.flex
                    style.justifyContent.spaceBetween
                    style.alignItems.center
                    style.padding (0, 40)
                ]
                prop.children [
                    row [
                        Html.img [
                            prop.style [
                                style.height 50
                            ]
                            prop.src logo
                        ]
                        Bulma.title [
                            prop.style [
                                style.color.white
                                style.fontSize (length.rem 1.5)
                            ]
                            prop.text "Compositional IT"
                        ]
                    ]
                ]
            ]
        ]
    ]

let subHeading (label: string) =
    Bulma.subtitle [
        prop.style [
            style.borderBottom(2, borderStyle.solid, citDarkBlue)
            style.marginTop 30
            style.paddingBottom 10
        ]
        prop.text label
    ]

let link p =
    Html.a [
        prop.style [
            style.color citDarkBlue
            style.fontWeight.bold
            style.borderBottom (2, borderStyle.solid, citDarkBlue)
        ]
        prop.text p.Text
        prop.href p.Href
    ]

let description (wrapperName: string) (wrappedComponent: string) links =
    Html.div [
        subHeading wrapperName
        Html.b $"Feliz style bindings for {wrappedComponent}"
        Bulma.content [
            Html.ul [
                for l in links do
                    Html.li [
                        link l
                    ]
            ]
        ]
    ]

let headingWithContent (title: string) (children: ReactElement) =
    Html.div [
        subHeading title
        children
    ]

let codeBlock (code: string) =
    Html.pre [
        prop.style [
            style.padding 20
            style.fontSize 15
            style.backgroundColor "#f5f5f5"
            style.borderRadius 5
        ]
        prop.text code
    ]

type Olympian =
    { Athlete: string
      Age: int option
      Country: string
      Year: int
      Date: string
      Sport: string
      Gold: int
      Silver: int
      Bronze: int
      Total: int }

[<ReactComponent>]
let Demo () =
    let (olympicData, setOlympicData) = React.useState(None)
    let getData (): JS.Promise<Olympian []> =
        promise {
            let url = sprintf "https://www.ag-grid.com/example-assets/olympic-winners.json"
            return! Fetch.get(url, caseStrategy = CamelCase)
        }

    React.useEffectOnce (fun () ->
        let d = getData()
        d.``then``(fun data ->
            data
            |> Some
            |> setOlympicData)
            |> ignore)

    let updateRowAthleteName newName row =
        olympicData
        |> Option.iter (
            Array.map (fun x -> if x = row then { row with Athlete = newName } else x)
            >> Some
            >> setOlympicData)

    container [
        Html.div [
            prop.style [ style.display.flex; style.flexWrap.wrap; style.flexDirection.column ]
            prop.children [
                Html.div [
                    description
                        "Feliz.AgGrid"
                        "ag-grid"
                        [
                            { Text = "GitHub repo"; Href = "https://github.com/CompositionalIT/feliz-ag-grid" }
                            { Text = "NuGet package"; Href = "https://www.nuget.org/packages/Feliz.AgGrid" }
                            { Text = "Corresponding npm package"; Href = "https://www.npmjs.com/package/ag-grid-react" }
                        ]

                    headingWithContent
                        "Demo"
                        (match olympicData with
                        | Some olympicData ->
                            Html.div [
                                prop.className ThemeClass.Balham
                                prop.children [
                                    AgGrid.grid [
                                        AgGrid.rowData olympicData
                                        AgGrid.pagination true
                                        AgGrid.defaultColDef [
                                            ColumnDef.resizable true
                                            ColumnDef.sortable true
                                        ]
                                        AgGrid.domLayout AutoHeight
                                        AgGrid.paginationPageSize 20
                                        AgGrid.onColumnGroupOpened (fun x -> x.AutoSizeGroupColumns())
                                        AgGrid.onGridReady (fun x -> x.AutoSizeAllColumns())
                                        AgGrid.singleClickEdit true
                                        AgGrid.columnDefs [
                                            ColumnDef.create<string> [
                                                ColumnDef.filter RowFilter.Text
                                                ColumnDef.headerName "Athlete (editable)"
                                                ColumnDef.valueGetter (fun x -> x.Athlete)
                                                ColumnDef.editable (fun _ _ -> true)
                                                ColumnDef.valueSetter (fun newValue _ row -> updateRowAthleteName newValue row)
                                            ]
                                            ColumnDef.create<int option> [
                                                ColumnDef.filter RowFilter.Number
                                                ColumnDef.columnType ColumnType.NumericColumn
                                                ColumnDef.headerName "Age"
                                                ColumnDef.valueGetter (fun x -> x.Age)
                                                ColumnDef.valueFormatter (fun age _ ->
                                                    match age with
                                                    | Some age -> $"{age} years"
                                                    | None -> "Unknown" )
                                            ]
                                            ColumnDef.create<string> [
                                                ColumnDef.filter RowFilter.Text
                                                ColumnDef.headerName "Country"
                                                ColumnDef.valueGetter (fun x -> x.Country)
                                            ]
                                            ColumnDef.create<DateTime> [
                                                ColumnDef.filter RowFilter.Date
                                                ColumnDef.headerName "Date"
                                                ColumnDef.valueGetter (fun x ->
                                                    x.Date.Split("/")
                                                    |> function
                                                        | [| d; m; y |] -> DateTime(int y, int m, int d)
                                                        | _ -> DateTime.MinValue)
                                                ColumnDef.valueFormatter (fun d _ -> d.ToShortDateString())
                                            ]
                                            ColumnDef.create<string> [
                                                ColumnDef.filter RowFilter.Text
                                                ColumnDef.headerName "Sport"
                                                ColumnDef.valueGetter (fun x -> x.Sport)
                                            ]
                                            ColumnGroup.create [
                                                ColumnGroup.headerName "Medal"
                                                ColumnGroup.marryChildren true
                                                ColumnGroup.openByDefault true
                                            ] [
                                                ColumnDef.create<int> [
                                                    ColumnDef.filter RowFilter.Number
                                                    ColumnDef.headerName "Total"
                                                    ColumnDef.columnType ColumnType.NumericColumn
                                                    ColumnDef.valueGetter (fun x -> x.Total)
                                                    ColumnDef.cellRendererFramework (fun x _ ->
                                                        Html.span [
                                                            Html.span [
                                                                prop.style [ style.fontSize 9 ]
                                                                prop.children [
                                                                    Html.text "🏅"
                                                                ]
                                                            ]
                                                            Html.textf "%i" x
                                                        ])
                                                    ColumnDef.columnGroupShow true
                                                ]
                                                ColumnDef.create<int> [
                                                    ColumnDef.filter RowFilter.Number
                                                    ColumnDef.headerName "Gold"
                                                    ColumnDef.columnType ColumnType.NumericColumn
                                                    ColumnDef.valueGetter (fun x -> x.Gold)
                                                    ColumnDef.columnGroupShow false
                                                ]
                                                ColumnDef.create<int> [
                                                    ColumnDef.filter RowFilter.Number
                                                    ColumnDef.headerName "Silver"
                                                    ColumnDef.columnType ColumnType.NumericColumn
                                                    ColumnDef.valueGetter (fun x -> x.Silver)
                                                    ColumnDef.columnGroupShow false
                                                ]
                                                ColumnDef.create<int> [
                                                    ColumnDef.filter RowFilter.Number
                                                    ColumnDef.headerName "Bronze"
                                                    ColumnDef.columnType ColumnType.NumericColumn
                                                    ColumnDef.valueGetter (fun x -> x.Bronze)
                                                    ColumnDef.columnGroupShow false
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        | None ->
                            Html.div [])
                    headingWithContent
                        "Installation"
                        (codeBlock """
cd ./project
femto install Feliz.AgGrid""" )

                    headingWithContent
                        "Sample Code"
                        (codeBlock """
open Feliz.AgGrid

type Olympian =
    { Athlete: string
      Age: int option
      Country: string
      Year: int
      Date: DateTime
      Sport: string
      Gold: int
      Silver: int
      Bronze: int
      Total: int }

Html.div [
    prop.className ThemeClass.Balham
    prop.children [
        AgGrid.grid [
            AgGrid.rowData olympicData
            AgGrid.pagination true
            AgGrid.defaultColDef [
                ColumnDef.resizable true
                ColumnDef.sortable true
                ColumnDef.editable (fun _ -> false)
            ]
            AgGrid.domLayout AutoHeight
            AgGrid.paginationPageSize 20
            AgGrid.onColumnGroupOpened (fun x -> x.AutoSizeGroupColumns())
            AgGrid.onGridReady (fun x -> x.AutoSizeAllColumns())
            AgGrid.columnDefs [
                ColumnDef.create<string> [
                    ColumnDef.filter RowFilter.Text
                    ColumnDef.headerName "Athlete"
                    ColumnDef.valueGetter (fun x -> x.Athlete)
                ]
                ColumnDef.create<int option> [
                    ColumnDef.filter RowFilter.Number
                    ColumnDef.columnType ColumnType.NumericColumn
                    ColumnDef.headerName "Age"
                    ColumnDef.valueGetter (fun x -> x.Age)
                    ColumnDef.valueFormatter (fun age _ ->
                        match age with
                        | Some age -> $"{age} years"
                        | None -> "Unknown" )
                ]
                ColumnDef.create<string> [
                    ColumnDef.filter RowFilter.Text
                    ColumnDef.headerName "Country"
                    ColumnDef.valueGetter (fun x -> x.Country)
                ]
                ColumnDef.create<DateTime> [
                    ColumnDef.filter RowFilter.Date
                    ColumnDef.headerName "Date"
                    ColumnDef.valueGetter (fun x ->
                        x.Date.Split("/")
                        |> function
                            | [| d; m; y |] -> DateTime(int y, int m, int d)
                            | _ -> DateTime.MinValue)
                    ColumnDef.valueFormatter (fun d _ -> d.ToShortDateString())
                ]
                ColumnDef.create<string> [
                    ColumnDef.filter RowFilter.Text
                    ColumnDef.headerName "Sport"
                    ColumnDef.valueGetter (fun x -> x.Sport)
                ]
                ColumnGroup.create [
                    ColumnGroup.headerName "Medal"
                    ColumnGroup.marryChildren true
                    ColumnGroup.openByDefault true
                ] [
                    ColumnDef.create<int> [
                        ColumnDef.filter RowFilter.Number
                        ColumnDef.headerName "Total"
                        ColumnDef.columnType ColumnType.NumericColumn
                        ColumnDef.valueGetter (fun x -> x.Total)
                        ColumnDef.columnGroupShow true
                    ]
                    ColumnDef.create<int> [
                        ColumnDef.filter RowFilter.Number
                        ColumnDef.headerName "Gold"
                        ColumnDef.columnType ColumnType.NumericColumn
                        ColumnDef.valueGetter (fun x -> x.Gold)
                        ColumnDef.columnGroupShow false
                    ]
                    ColumnDef.create<int> [
                        ColumnDef.filter RowFilter.Number
                        ColumnDef.headerName "Silver"
                        ColumnDef.columnType ColumnType.NumericColumn
                        ColumnDef.valueGetter (fun x -> x.Silver)
                        ColumnDef.columnGroupShow false
                    ]
                    ColumnDef.create<int> [
                        ColumnDef.filter RowFilter.Number
                        ColumnDef.headerName "Bronze"
                        ColumnDef.columnType ColumnType.NumericColumn
                        ColumnDef.valueGetter (fun x -> x.Bronze)
                        ColumnDef.columnGroupShow false
                    ]
                ]
            ]
        ]
    ]
]
""")
                ]
            ]
        ]
    ]

[<ReactComponent>]
let Documentation () =
    Html.div [
        navbar()
        Demo()
    ]

