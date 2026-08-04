FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
WORKDIR /src

# Package signature verification fetches CRLs from crl3.digicert.com over HTTP.
# When that host is unreachable, restore blocks on TCP timeouts instead of failing.
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false

COPY src/BragadoTripConnect.Api/BragadoTripConnect.Api.csproj src/BragadoTripConnect.Api/
RUN dotnet restore src/BragadoTripConnect.Api/BragadoTripConnect.Api.csproj

COPY src/ src/
RUN dotnet publish src/BragadoTripConnect.Api/BragadoTripConnect.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BragadoTripConnect.Api.dll"]
