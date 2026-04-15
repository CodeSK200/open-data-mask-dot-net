FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/OpenDataMask.Console/OpenDataMask.Console.csproj ./src/OpenDataMask.Console/
RUN dotnet restore ./src/OpenDataMask.Console/OpenDataMask.Console.csproj
COPY . .
WORKDIR /src/src/OpenDataMask.Console
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "OpenDataMask.Console.dll"]
