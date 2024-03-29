﻿# syntax=docker/dockerfile:1.4

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

COPY COMET.Web.Common COMET.Web.Common
COPY COMETwebapp COMETwebapp

RUN --mount=type=secret,id=DEVEXPRESS_NUGET_KEY export DEVEXPRESS_NUGET_KEY=$(cat /run/secrets/DEVEXPRESS_NUGET_KEY) \ 
 && dotnet nuget add source https://nuget.devexpress.com/api -n DXFeed -u DevExpress -p ${DEVEXPRESS_NUGET_KEY} --store-password-in-clear-text \
 && dotnet restore COMETwebapp

RUN dotnet build COMETwebapp -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish COMETwebapp -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0.3-alpine3.19 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user and give this user access to the working directory
RUN chown -R "$APP_UID" /app
USER $APP_UID 

ENTRYPOINT ["dotnet", "COMETwebapp.dll"]