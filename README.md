# Feliz.AgGrid

Feliz-style bindings for [ag-grid-react](https://www.npmjs.com/package/ag-grid-react)

## Versions

### Required versions of dependencies

Note: ag-grid-react/enterprise is only required if using enterprise features.

| Feliz.AgGrid | ag-grid-react/community | ag-grid-react/enterprise | React | Fable | Feliz |
|--------------|-------------------------|--------------------------|-------|-------|-------|
| 2.x          | 31.x                    | 31.x                     | 18.x  | 4.x   | 2.x   |
| 1.x          | 31.x                    | -                        | 18.x  | 4.x   | 2.x   |
| 0.0.6        | 25.x                    | -                        | 17.x  | 3.x   | 1.x   |

### Migration guides

#### v1 to v2

##### Import AG Grid style sheets

To give you better control of your bundle, Feliz.AgGrid no longer imports styles for you, so you will need to include
appropriate imports yourself. For example, add the following lines to your client code to import the necessary styles
and the Balham theme.

```fsharp
importAll "ag-grid-community/styles/ag-grid.css"
importAll "ag-grid-community/styles/ag-theme-balham.css"
```

## Installation

Run `femto install Feliz.AgGrid` from inside your project directory.

## Source code

Found in `./src`

## Demo

Demo code is in `./demo`. You can see it running live
at [https://compositionalit.github.io/feliz-ag-grid/](https://compositionalit.github.io/feliz-ag-grid/)

## Publishing a new package

- Make changes in `./src`
- Change the version in `./src/Feliz.AgGrid.fsproj`
- Tag the Git commit (to make finding the relevant commit easy in future)
- Push to GitHub
- Wait for the GitHub action (configured in `./.github/workflows/deployment.yml`) to finish
