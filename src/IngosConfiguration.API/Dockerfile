FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/IngosConfiguration.API/IngosConfiguration.API.csproj", "IngosConfiguration.API/"]
RUN dotnet restore "src/IngosConfiguration.API/IngosConfiguration.API.csproj"
COPY . .
WORKDIR "/src/IngosConfiguration.API"
RUN dotnet build "IngosConfiguration.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IngosConfiguration.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IngosConfiguration.API.dll"]
