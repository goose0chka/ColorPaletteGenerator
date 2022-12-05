FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ColorPaletteGen.Bot/ColorPaletteGen.Bot.csproj", "ColorPaletteGen.Bot/"]
RUN dotnet restore "ColorPaletteGen.Bot/ColorPaletteGen.Bot.csproj"
COPY . .
WORKDIR "/src/ColorPaletteGen.Bot"
RUN dotnet build "ColorPaletteGen.Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ColorPaletteGen.Bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ColorPaletteGen.Bot.dll"]
