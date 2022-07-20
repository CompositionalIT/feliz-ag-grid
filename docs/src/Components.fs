namespace App

open Feliz.Bulma
open Feliz
open Fable.Core.JsInterop
open Feliz.AgGrid
open System
open Thoth.Fetch
open Thoth.Json
open Fable.Core


type CitColors =
    static member lightBlue = "#40a8b7"
    static member green = "#8cbf41"
    static member yellow = "#fec903"
    static member red = "#e1053a"
    static member orange = "#e97305"
    static member darkBlue = "#102035"

type Package = { Name: string; Link: string }

type StyledComponents =

    static member Container (children: ReactElement list) =
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

    static member Row (children: ReactElement list) =
        Html.div [
            prop.style [
                style.alignItems.center
                style.display.flex
            ]
            prop.children children
        ]

    static member NavbarLink (label: string) link=
        Html.a [
            prop.style [ style.margin(20, 0, 20, 20); style.color "white"; style.fontSize 20; style.fontWeight.bold]
            prop.href link
            prop.text label
        ]

    static member Navbar () =
        let logo: obj = importDefault "./cit-logo.png"
        Html.div [
            prop.style [
                style.padding (0, 20)
                style.backgroundColor CitColors.darkBlue
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
                        StyledComponents.Row [
                            Html.img [
                                prop.style [
                                    style.height 50
                                ]
                                prop.src (unbox<string>logo)
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

    static member SubHeading (label: string) =
        Bulma.subtitle [
            prop.style [
                style.borderBottom(2, borderStyle.solid, CitColors.darkBlue)
                style.marginTop 30
                style.paddingBottom 10
            ]
            prop.text label
        ]

    static member Link p =
        Html.a [
            prop.style [
                style.color CitColors.darkBlue
                style.fontWeight.bold
                style.borderBottom (2, borderStyle.solid, CitColors.darkBlue)
            ]
            prop.text p.Name
            prop.href p.Link
        ]

    static member Description (wrapperName: string) (wrappedComponent: string) nuget npm =
        Html.div [
            StyledComponents.SubHeading wrapperName
            Html.b $"Feliz style bindings for {wrappedComponent}"
            Bulma.content [
                Html.ul [
                    Html.li [
                        StyledComponents.Link nuget
                    ]
                    Html.li [
                        StyledComponents.Link npm
                    ]
                ]
            ]
        ]

    static member HeadingWithContent (title: string) (children: ReactElement) =
        Html.div [
            StyledComponents.SubHeading title
            children
        ]

    static member Checkbox updateProp =
        Bulma.input.checkbox [
            prop.style [
                style.height 30
                style.width 30
            ]
            prop.onClick updateProp
        ]

    static member CodeBlock (code: string) =
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

type Components =

    [<ReactComponent>]
    static member Demo () =


        let (olympicData, setOlympicData) = React.useState([||])
        let getData (): JS.Promise<Olympian []> =
            promise {
                let url = sprintf "https://www.ag-grid.com/example-assets/olympic-winners.json"
                return! Fetch.get(url, caseStrategy = CamelCase)
            }

        React.useEffectOnce (fun () ->
            let d = getData()
            d.``then``(fun data ->
                data
                |> setOlympicData)
                |> ignore)

        StyledComponents.Container [
            Html.div [
                prop.style [ style.display.flex; style.flexWrap.wrap; style.flexDirection.column ]
                prop.children [
                    Html.div [
                        StyledComponents.Description
                            "Feliz.AgGrid"
                            "ag-grid"
                            { Name = "nuget"; Link = "https://www.nuget.org/packages/Feliz.AgGrid/0.0.2" }
                            { Name = "npm"; Link = "https://www.npmjs.com/package/ag-grid-react" }

                        StyledComponents.HeadingWithContent
                            "Demo"
                            (Html.div [
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
                                                ColumnDef.valueGetter (fun d -> DateTime.Parse(d.Date))
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
                            ])
                        StyledComponents.HeadingWithContent
                            "Installation"
                            (StyledComponents.CodeBlock """
cd ./project
femto install Feliz.AgGrid""" )

                        StyledComponents.HeadingWithContent
                            "Sample Code"
                            (StyledComponents.CodeBlock """
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
                    ColumnDef.valueGetter (fun d -> DateTime.Parse(d.Date))
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
""" )
                    ]
                ]
            ]
        ]

    [<ReactComponent>]
    static member Documentation () =
        Html.div [
            StyledComponents.Navbar()
            Components.Demo()
        ]

