FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
#”√µΩ¡ÀSystem.Drawing£¨Image
RUN apt-get update -y && apt-get install -y libgdiplus && apt-get clean && ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["product/hcm/Fap.Hcm.Web/Fap.Hcm.Web.csproj", "product/hcm/Fap.Hcm.Web/"]
COPY ["product/hcm/Fap.Hcm.Service/Fap.Hcm.Service.csproj", "product/hcm/Fap.Hcm.Service/"]
COPY ["src/Fap.AspNetCore/Fap.AspNetCore.csproj", "src/Fap.AspNetCore/"]
COPY ["src/Fap.Core/Fap.Core.csproj", "src/Fap.Core/"]
COPY ["src/Fap.ExcelReport/Fap.ExcelReport.csproj", "src/Fap.ExcelReport/"]
COPY ["src/Fap.Workflow/Fap.Workflow.csproj", "src/Fap.Workflow/"]
COPY ["thirdparty/SQLGeneration/SQLGeneration.csproj", "thirdparty/SQLGeneration/"]
COPY ["thirdparty/UEditorNetCore/UEditorNetCore.csproj", "thirdparty/UEditorNetCore/"]

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