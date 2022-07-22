# Feliz.AgGrid demo

## Run locally

- `npm install`
- `npm start`

## Deploy

- Change the reference to the src project into a reference to the appropriate NuGet package, e.g.:
    ``` diff
        </ItemGroup>
    -   <ItemGroup>
    -       <ProjectReference Include="..\..\src\Feliz.AgGrid.fsproj" />
    -   </ItemGroup>
        <ItemGroup>
            <PackageReference Include="Feliz" Version="1.65.0" />
    +       <PackageReference Include="Feliz.AgGrid" Version="0.0.4" />
    ```
- Run `npm run publish-docs`
