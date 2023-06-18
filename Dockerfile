FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["STMLabs/STMLabs.csproj", "STMLabs/"]
RUN dotnet restore "STMLabs/STMLabs.csproj"
COPY . .
WORKDIR "/src/STMLabs"
RUN dotnet build "STMLabs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "STMLabs.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "STMLabs.dll"]
