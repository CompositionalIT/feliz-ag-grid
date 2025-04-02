module App

open Feliz.Bulma
open Feliz
open Fable.Core.JsInterop
open Feliz.AgGrid
open System
open Elmish
open Thoth.Fetch
open Thoth.Json

let citDarkBlue = "#102035"

type LinkData = { Text: string; Href: string }

let container (children: ReactElement list) =
    Html.div [
        prop.style [
            style.display.flex
            style.flexDirection.column
            style.padding 50
            style.maxWidth 1000
            style.margin (0, length.auto)
        ]
        prop.children children
    ]

let row (children: ReactElement list) =
    Html.div [
        prop.style [ style.alignItems.center; style.display.flex ]
        prop.children children
    ]

let navbar () =
    let logo: string = importDefault "./cit-logo.png"

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
                        Html.img [ prop.style [ style.height 50 ]; prop.src logo ]
                        Bulma.title [
                            prop.style [ style.color.white; style.fontSize (length.rem 1.5) ]
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
            style.borderBottom (2, borderStyle.solid, citDarkBlue)
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
                    Html.li [ link l ]
            ]
        ]
    ]

let headingWithContent (title: string) (children: ReactElement) = Html.div [ subHeading title; children ]

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


importAll "ag-grid-community/styles/ag-grid.css"
importAll "ag-grid-community/styles/ag-theme-balham.css"

type Olympian = {
    Athlete: string
    Age: int option
    Country: string
    Year: int
    Date: string
    Sport: string
    Gold: int
    Silver: int
    Bronze: int
    Total: int
}

type Message =
    | LoadData
    | UpdateName of olympian: Olympian * name: string
    | DataLoaded of Olympian array

let init _ = None, Cmd.ofMsg LoadData

let update message data =
    match message with
    | UpdateName(olympian, name) ->
        data
        |> Option.map (Array.map (fun row -> if row = olympian then { row with Athlete = name } else row)),
        Cmd.none
    | LoadData ->
        let url = "https://www.ag-grid.com/example-assets/olympic-winners.json"
        data, Cmd.OfPromise.perform (fun url -> Fetch.get (url, caseStrategy = CamelCase)) url DataLoaded

    | DataLoaded data -> Some data, Cmd.none

[<ReactComponent>]
let Demo olympicData (updateRowAthleteName: _ -> _ -> unit) =

    container [
        Html.div [
            prop.style [ style.display.flex; style.flexWrap.wrap; style.flexDirection.column ]
            prop.children [
                Html.div [
                    description "Feliz.AgGrid" "ag-grid" [
                        {
                            Text = "GitHub repo"
                            Href = "https://github.com/CompositionalIT/feliz-ag-grid"
                        }
                        {
                            Text = "NuGet package"
                            Href = "https://www.nuget.org/packages/Feliz.AgGrid"
                        }
                        {
                            Text = "Corresponding npm package"
                            Href = "https://www.npmjs.com/package/ag-grid-react"
                        }
                    ]

                    let columnDefinitions =
                        React.useMemo (fun _ ->
                            AgGrid.columnDefs [
                                ColumnDef.create [
                                    ColumnDef.filter RowFilter.Text
                                    ColumnDef.headerName "Athlete (editable)"
                                    ColumnDef.headerTooltip "The name of the athlete"
                                    ColumnDef.valueGetter (fun x -> x.Athlete)
                                    ColumnDef.editable (fun _ _ -> true)
                                    ColumnDef.valueSetter (fun valueChangedParams ->
                                        updateRowAthleteName valueChangedParams.newValue valueChangedParams.data)
                                ]
                                ColumnDef.create [
                                    ColumnDef.filter RowFilter.Number
                                    ColumnDef.columnType ColumnType.NumericColumn
                                    ColumnDef.headerName "Age"
                                    ColumnDef.valueGetter (fun x -> x.Age)
                                    ColumnDef.valueFormatter (fun valueParams ->
                                        match Option.flatten valueParams.value with
                                        | Some age -> $"%i{age} years"
                                        | None -> "Unknown")
                                ]
                                ColumnDef.create [
                                    ColumnDef.filter RowFilter.Text
                                    ColumnDef.headerName "Country"
                                    ColumnDef.valueGetter (fun x -> x.Country)
                                ]
                                ColumnDef.create [
                                    ColumnDef.filter RowFilter.Date
                                    ColumnDef.headerName "Date"
                                    ColumnDef.valueGetter (fun x ->
                                        x.Date.Split("/")
                                        |> function
                                            | [| d; m; y |] -> DateTime(int y, int m, int d)
                                            | _ -> DateTime.MinValue)
                                    ColumnDef.valueFormatter (fun valueParams ->
                                        valueParams.value
                                        |> Option.map _.ToShortDateString()
                                        |> Option.defaultValue "")
                                ]
                                ColumnDef.create [
                                    ColumnDef.filter RowFilter.Text
                                    ColumnDef.headerName "Sport"
                                    ColumnDef.valueGetter (fun x -> x.Sport)
                                ]
                                ColumnGroup.create [
                                    ColumnGroup.headerName "Medal"
                                    ColumnGroup.marryChildren true
                                    ColumnGroup.openByDefault true
                                ] [
                                    ColumnDef.create [
                                        ColumnDef.filter RowFilter.Number
                                        ColumnDef.headerName "Total"
                                        ColumnDef.columnType ColumnType.NumericColumn
                                        ColumnDef.valueGetter (fun x -> x.Total)
                                        ColumnDef.cellRenderer (fun rendererParams ->
                                            match rendererParams.value with
                                            | Some value ->
                                                Html.span [
                                                    Html.span [
                                                        prop.style [ style.fontSize 9 ]
                                                        prop.children [ Html.text "🏅" ]
                                                    ]
                                                    Html.text $"%i{value}"
                                                ]
                                            | None -> React.fragment [])
                                        ColumnDef.columnGroupShow true
                                    ]
                                    ColumnDef.create [
                                        ColumnDef.filter RowFilter.Number
                                        ColumnDef.headerName "Gold"
                                        ColumnDef.columnType ColumnType.NumericColumn
                                        ColumnDef.valueGetter (fun x -> x.Gold)
                                        ColumnDef.columnGroupShow false
                                    ]
                                    ColumnDef.create [
                                        ColumnDef.filter RowFilter.Number
                                        ColumnDef.headerName "Silver"
                                        ColumnDef.columnType ColumnType.NumericColumn
                                        ColumnDef.valueGetter (fun x -> x.Silver)
                                        ColumnDef.columnGroupShow false
                                    ]
                                    ColumnDef.create [
                                        ColumnDef.filter RowFilter.Number
                                        ColumnDef.headerName "Bronze"
                                        ColumnDef.columnType ColumnType.NumericColumn
                                        ColumnDef.valueGetter (fun x -> x.Bronze)
                                        ColumnDef.columnGroupShow false
                                    ]
                                ]
                            ]

                        )

                    headingWithContent
                        "Demo"
                        (match olympicData with
                         | Some olympicData ->
                             Html.div [
                                 prop.className ThemeClass.Balham
                                 prop.children [
                                     AgGrid.grid [
                                         columnDefinitions
                                         AgGrid.rowData olympicData
                                         AgGrid.pagination true
                                         AgGrid.domLayout AutoHeight
                                         AgGrid.paginationPageSize 20
                                         AgGrid.onColumnGroupOpened (fun x -> x.AutoSizeGroupColumns())
                                         AgGrid.onGridReady (fun x -> x.AutoSizeAllColumns())
                                         AgGrid.singleClickEdit true
                                         AgGrid.enableCellTextSelection true
                                         AgGrid.ensureDomOrder true
                                     ]
                                 ]
                             ]
                         | None -> Html.div [])

                    headingWithContent
                        "Installation"
                        (codeBlock
                            """cd ./project
femto install Feliz.AgGrid""")

                    headingWithContent
                        "Sample Code"
                        (codeBlock
                            """open Feliz.AgGrid

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

importAll "ag-grid-community/styles/ag-grid.css"
importAll "ag-grid-community/styles/ag-theme-balham.css"

let colDefs =
    React.useMemo (fun _ ->
        AgGrid.columnDefs [
            ColumnDef.create [
                ColumnDef.filter RowFilter.Text
                ColumnDef.headerName "Athlete (editable)"
                ColumnDef.valueGetter (fun x -> x.Athlete)
                ColumnDef.editable (fun _ _ -> true)
                ColumnDef.valueSetter (fun valueChangedParams ->
                    updateRowAthleteName valueChangedParams.newValue valueChangedParams.data)
            ]
            ColumnDef.create [
                ColumnDef.filter RowFilter.Number
                ColumnDef.columnType ColumnType.NumericColumn
                ColumnDef.headerName "Age"
                ColumnDef.valueGetter (fun x -> x.Age)
                ColumnDef.valueFormatter (fun valueParams ->
                    match Option.flatten valueParams.value with
                    | Some age -> $"%i{age} years"
                    | None -> "Unknown")
            ]
            ColumnDef.create [
                ColumnDef.filter RowFilter.Text
                ColumnDef.headerName "Country"
                ColumnDef.valueGetter (fun x -> x.Country)
            ]
            ColumnDef.create [
                ColumnDef.filter RowFilter.Date
                ColumnDef.headerName "Date"
                ColumnDef.valueGetter (fun x ->
                    x.Date.Split("/")
                    |> function
                        | [| d; m; y |] -> DateTime(int y, int m, int d)
                        | _ -> DateTime.MinValue)
                ColumnDef.valueFormatter (fun valueParams ->
                    valueParams.value
                    |> Option.map _.ToShortDateString()
                    |> Option.defaultValue "")
            ]
            ColumnDef.create [
                ColumnDef.filter RowFilter.Text
                ColumnDef.headerName "Sport"
                ColumnDef.valueGetter (fun x -> x.Sport)
            ]
            ColumnGroup.create [
                ColumnGroup.headerName "Medal"
                ColumnGroup.marryChildren true
                ColumnGroup.openByDefault true
            ] [
                ColumnDef.create [
                    ColumnDef.filter RowFilter.Number
                    ColumnDef.headerName "Total"
                    ColumnDef.columnType ColumnType.NumericColumn
                    ColumnDef.valueGetter (fun x -> x.Total)
                    ColumnDef.cellRenderer (fun rendererParams ->
                        match rendererParams.value with
                        | Some value ->
                            Html.span [
                                Html.span [
                                    prop.style [ style.fontSize 9 ]
                                    prop.children [ Html.text "🏅" ]
                                ]
                                Html.text $"%i{value}"
                            ]
                        | None -> React.fragment [])
                    ColumnDef.columnGroupShow true
                ]
                ColumnDef.create [
                    ColumnDef.filter RowFilter.Number
                    ColumnDef.headerName "Gold"
                    ColumnDef.columnType ColumnType.NumericColumn
                    ColumnDef.valueGetter (fun x -> x.Gold)
                    ColumnDef.columnGroupShow false
                ]
                ColumnDef.create [
                    ColumnDef.filter RowFilter.Number
                    ColumnDef.headerName "Silver"
                    ColumnDef.columnType ColumnType.NumericColumn
                    ColumnDef.valueGetter (fun x -> x.Silver)
                    ColumnDef.columnGroupShow false
                ]
                ColumnDef.create [
                    ColumnDef.filter RowFilter.Number
                    ColumnDef.headerName "Bronze"
                    ColumnDef.columnType ColumnType.NumericColumn
                    ColumnDef.valueGetter (fun x -> x.Bronze)
                    ColumnDef.columnGroupShow false
                ]
            ]
        ]
    )

Html.div [
     prop.className ThemeClass.Balham
     prop.children [
         AgGrid.grid [
             colDefs
             AgGrid.rowData olympicData
             AgGrid.pagination true
             AgGrid.domLayout AutoHeight
             AgGrid.paginationPageSize 20
             AgGrid.onColumnGroupOpened (fun x -> x.AutoSizeGroupColumns())
             AgGrid.onGridReady (fun x -> x.AutoSizeAllColumns())
             AgGrid.singleClickEdit true
             AgGrid.enableCellTextSelection true
             AgGrid.ensureDomOrder true
         ]
     ]
 ]
""")
                ]
            ]
        ]
    ]



[<ReactComponent>]
let Documentation data dispatch =
    Html.div [
        navbar ()
        Demo data (fun newValue row -> UpdateName(row, newValue) |> dispatch)
    ]
