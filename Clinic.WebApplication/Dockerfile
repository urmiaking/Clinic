#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Clinic.WebApplication/Clinic.WebApplication.csproj", "Clinic.WebApplication/"]
COPY ["Clinic.VIewModels/Clinic.ViewModels.csproj", "Clinic.VIewModels/"]
COPY ["Clinic.Models/Clinic.Models.csproj", "Clinic.Models/"]
COPY ["Clinic.Utilities/Clinic.Utilities.csproj", "Clinic.Utilities/"]
COPY ["Clinic.DataContext/Clinic.DataContext.csproj", "Clinic.DataContext/"]
COPY ["Clinic.Services/Clinic.Services.csproj", "Clinic.Services/"]
RUN dotnet restore "Clinic.WebApplication/Clinic.WebApplication.csproj"
COPY . .
WORKDIR "/src/Clinic.WebApplication"
RUN dotnet build "Clinic.WebApplication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Clinic.WebApplication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Clinic.WebApplication.dll"]