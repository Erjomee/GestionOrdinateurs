
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ../GestionOrdinateurs.Gateway/GestionOrdinateurs.Gateway.csproj ./GestionOrdinateurs.Gateway/

RUN dotnet restore GestionOrdinateurs.Gateway/GestionOrdinateurs.Gateway.csproj

COPY GestionOrdinateurs.Gateway/ GestionOrdinateurs.Gateway/

RUN dotnet publish GestionOrdinateurs.Gateway/GestionOrdinateurs.Gateway.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "GestionOrdinateurs.Gateway.dll"]
