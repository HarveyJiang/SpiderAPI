FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY SpiderAPI/SpiderAPI.csproj SpiderAPI/
RUN dotnet restore SpiderAPI/SpiderAPI.csproj
COPY . .
WORKDIR /src/SpiderAPI
RUN dotnet build SpiderAPI.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish SpiderAPI.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SpiderAPI.dll"]
