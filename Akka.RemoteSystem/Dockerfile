FROM mcr.microsoft.com/dotnet/core/runtime:3.1
COPY publish/ publish/
RUN useradd -m myuser
USER myuser
CMD dotnet publish/Akka.RemoteSystem.dll -e PORT=$PORT