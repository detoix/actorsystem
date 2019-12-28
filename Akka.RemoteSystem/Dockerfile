FROM mcr.microsoft.com/dotnet/core/runtime:3.0
COPY publish/ publish/
EXPOSE 8080
ENTRYPOINT ["dotnet", "publish/Akka.RemoteSystem.dll"]