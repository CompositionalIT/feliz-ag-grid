# Feliz.AgGrid

Feliz-style bindings for [ag-grid-react](https://www.npmjs.com/package/ag-grid-react)

## Version Compatibility

### The table below gives the ranges of compatible versions of Feliz AgGrid with its dependent packages.

| Feliz.AgGrid  | ag-grid-react/community | React | Fable | Feliz |
|-              |-                        |-      |-      |-      |
| 0.0.6         | 25.x                    | 17.x  | 3.x   | 1.x   |
| 1.x           | 31.x                    | 18.x  | 4.x   | 2.x   |

## Installation

Run `femto install Feliz.AgGrid` from inside your project directory.

## Source code

Found in `./src`

## Demo

Demo code is in `./demo`. You can see it running live at [https://compositionalit.github.io/feliz-ag-grid/](https://compositionalit.github.io/feliz-ag-grid/)

## Publishing a new package

- Make changes in `./src`
- Change the version in `./src/Feliz.AgGrid.fsproj`
- Tag the Git commit
- Push to GitHub
- Wait for the GitHub action (configured in `./.github/workflows/deployment.yml`) to finish
