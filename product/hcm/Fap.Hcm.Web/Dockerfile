FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["product/hcm/Fap.Hcm.Web/Fap.Hcm.Web.csproj", "product/hcm/Fap.Hcm.Web/"]
RUN dotnet restore "product/hcm/Fap.Hcm.Web/Fap.Hcm.Web.csproj"
COPY . .
WORKDIR "/src/product/hcm/Fap.Hcm.Web"
RUN dotnet build "Fap.Hcm.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fap.Hcm.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fap.Hcm.Web.dll"]