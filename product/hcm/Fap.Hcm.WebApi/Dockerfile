#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["product/hcm/Fap.Hcm.WebApi/Fap.Hcm.WebApi.csproj", "product/hcm/Fap.Hcm.WebApi/"]
RUN dotnet restore "product/hcm/Fap.Hcm.WebApi/Fap.Hcm.WebApi.csproj"
COPY . .
WORKDIR "/src/product/hcm/Fap.Hcm.WebApi"
RUN dotnet build "Fap.Hcm.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fap.Hcm.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fap.Hcm.WebApi.dll"]